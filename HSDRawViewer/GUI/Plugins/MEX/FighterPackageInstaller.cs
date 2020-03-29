using Ionic.Zip;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using HSDRaw;
using HSDRaw.Melee;
using System.Text.RegularExpressions;
using HSDRaw.Common;
using HSDRaw.Melee.Ef;
using HSDRawViewer.Sound;
using HSDRawViewer.Converters;
using HSDRaw.Melee.Mn;
using HSDRaw.Common.Animation;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    /// <summary>
    /// 
    /// </summary>
    public class FighterPackageInstaller : ProgressClass
    {
        public static int MAX_COSTUME_COUNT { get; } = 7;

        // Prerequisite:
        // Mex Editor confirms existence of
        // MnSlChr, IfAll, GmRst, GmTou1-4
        // audio/smash2.sem and ssm files
        // PlCo.dat
        // Create interfaces with these files for easier manipulation


        /*
         * Fighter Package
         * Contents of fighter package to automate installation
         * 
         * TODO: Tourney stuff
         * 
         * ! - done
         * ? - blocked until more info
         * 
         *! /Costume/Pl****.dat
         * /CSS/icon.png
         *! /CSS/csp**.png
         *! /Effect/effect.ptcl
         *! /Effect/effect.texg
         *! /Effect/effect**.dat
         *! /Icon/ico**.png
         *! /Item/item**.dat
         *! /Kirby/PlKbCp**.dat
         * /Kirby/EfKb**.dat
         * /Kirby/sfx**.dsp
         *! /Sound/narrator.dsp
         *! /Sound/sem.yaml (wait until personal sound id is added)
         *! /Sound/sound.ssm
         *! /Sound/victory.hps
         *! /UI/result_victory_name.png
         *! /UI/result_name.png
         *! /UI/emblem.obj
         *! /bone_table.dat
         *! /fighter.yaml            -Required
         *! /GmRst**.dat
         *! /Pl**.dat
         *! /Pl**AJ.dat
         *! /Pl**DViWaitAJ.dat
         */
         

        private string packagePath;
        private MexDataEditor editor;
        private List<Tuple<HSDRawFile, string, bool>> editedFiles = new List<Tuple<HSDRawFile, string, bool>>();
        private List<SEMEntry> SemEntries = new List<SEMEntry>();

        public FighterPackageInstaller(string packagePage, MexDataEditor editor)
        {
            this.packagePath = packagePage;
            this.editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packagePath"></param>
        public override void Work(BackgroundWorker w)
        {
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var plco = Path.Combine(root, "PlCo.dat");
            var sem = Path.Combine(root, "audio\\us\\smash2.sem");
            
            if (!File.Exists(plco))
                throw new FileNotFoundException("PlCo.dat was not found");

            if (!File.Exists(sem))
                throw new FileNotFoundException("smash2.sem was not found");

            using (ZipFile pack = new ZipFile(packagePath))
            {
                string fighterYAMLPath = null;

                foreach (var e in pack)
                {
                    Console.WriteLine(e.FileName);
                    if (Path.GetFileName(e.FileName).ToLower() == "fighter.yaml")
                        fighterYAMLPath = e.FileName;
                }

                if (string.IsNullOrEmpty(fighterYAMLPath))
                    return;

                MEXFighterEntry mexEntry = null;

                using (MemoryStream zos = new MemoryStream())
                {
                    pack[fighterYAMLPath].Extract(zos);
                    zos.Position = 0;
                    using (StreamReader r = new StreamReader(zos))
                        mexEntry = MEXFighterEntry.Deserialize(r.ReadToEnd());
                }

                if (mexEntry == null)
                    return;
                
                ProgressStatus = $"Importing  {mexEntry.NameText}...";
                w.ReportProgress(0);

                var internalID = editor.AddEntry(mexEntry);

                ProgressStatus = "Installing Item Data"; w.ReportProgress(10);
                ImportItemData(pack, mexEntry);

                ProgressStatus = "Installing Item Data"; w.ReportProgress(20);
                GenerateEffectFile(pack, mexEntry);

                ProgressStatus = "Installing Item Data"; w.ReportProgress(30);
                ImportSoundData(pack, mexEntry, sem);

                ProgressStatus = "Installing UI"; w.ReportProgress(50);
                InstallUI(pack, mexEntry);

                ProgressStatus = "Installing Item Data"; w.ReportProgress(60);
                InjectBoneTable(pack, plco, internalID);

                //...
                ProgressStatus = "Extracting Files";
                w.ReportProgress(70);
                ExtractFiles(pack, mexEntry, editor);

                ProgressStatus = "Building new SEM";
                w.ReportProgress(80);
                if (SemEntries.Count != 0)
                    SEM.SaveSEMFile(sem, SemEntries, editor._data);

                ProgressStatus = "Saving Files";
                w.ReportProgress(90);
                foreach (var d in editedFiles)
                    d.Item1.Save(d.Item2, d.Item3);

                ProgressStatus = "Done";
                w.ReportProgress(100);
                // done
                Console.WriteLine($"Done!");
            }
        }

        /// <summary>
        /// Gets the file bytes from a zip entry
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static byte[] GetBytes(ZipEntry e)
        {
            using (MemoryStream zos = new MemoryStream())
            {
                e.Extract(zos);
                return zos.ToArray();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="PlCoPath"></param>
        /// <param name="internalID"></param>
        /// <param name="editor"></param>
        /// <returns>true if bone table was injected</returns>
        private bool InjectBoneTable(ZipFile pack, string PlCoPath, int internalID)
        {
            var bone = pack["bone_table.dat"];
            if (bone == null)
            {
                Console.WriteLine($"Could not find bone table...");
                return false;
            }

            Console.WriteLine($"Injecting Bone Table...");
            
            var plco = new HSDRawFile(PlCoPath);
            var boneTable = new HSDRawFile(GetBytes(bone));

            if (plco.Roots[0].Data is ftLoadCommonData commonData)
            {
                var boneTables = commonData.BoneTables.Array.ToList();

                // This PlCo is invalid since it contains a different number of entries than expected
                if (boneTables.Count != editor.NumberOfEntries)
                {
                    throw new InvalidDataException("PlCo Table was invalid");
                }

                // this is technically externalID, but it should be fine?
                boneTables.Insert(internalID - 1, new SBM_BoneTables() { _s = boneTable.Roots[0].Data._s });

                commonData.BoneTables.Array = boneTables.ToArray();
            }
            else
                return false;

            editedFiles.Add(new Tuple<HSDRawFile, string, bool>(plco, PlCoPath, false));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        private void ImportItemData(ZipFile pack, MEXFighterEntry fighter)
        {
            Console.WriteLine($"Importing Item Data...");

            var itemEntries = pack.Where(e => Regex.IsMatch(e.FileName, @"Item/item\d\d.*.yaml"));

            HSD_UShort[] itemIndices = new HSD_UShort[0];
            foreach(var item in itemEntries)
            {
                int val;
                if(int.TryParse(Regex.Match(item.FileName, @"(?<=item)\d\d").Value, out val))
                {
                    using (MemoryStream s = new MemoryStream(GetBytes(item)))
                    using (StreamReader r = new StreamReader(s))
                    {
                        // add item entry to mex items
                        var id = editor.AddMEXItem(r.ReadToEnd());

                        // add it to itemIndices
                        Array.Resize(ref itemIndices, val + 1);
                        itemIndices[val] = new HSD_UShort() { Value = (ushort)id };
                    }
                }
            }
            fighter.MEXItems = itemIndices.ToArray();
        }

        /// <summary>
        /// Generates an Eff file for Fighter & Kirby
        /// Adds entries for effects to mexData
        /// Updates ids for Fighter Mex Entry
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        /// <returns>Dictionary to maps old id values to new values</returns>
        private void GenerateEffectFile(ZipFile pack, MEXFighterEntry fighter)
        {
            Console.WriteLine($"Generating Effect File...");

            // Generate information---------------------------------------------
            string charID = Regex.Match(fighter.FighterDataPath, @"(?<=Pl)..").Value;
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var effectFileName = $"Ef{charID}Data.dat";
            var effectOutputFilePath = Path.Combine(root, effectFileName);
            var symbol = $"eff{fighter.NameText}DataTable";
            

            // Generate Effect File-------------------------------------------
            var models = pack.Where(e => Regex.IsMatch(e.FileName, @"Effect/effect\d\d.*.dat")).ToArray();
            var ptcl = pack["Effect/effect.ptcl"];
            var texg = pack["Effect/effect.texg"];
            
            SBM_EffectTable effTable = new SBM_EffectTable();
            
            if (ptcl != null && texg != null)
            {
                effTable._s.SetReferenceStruct(0x00, new HSDStruct(GetBytes(ptcl)));
                effTable._s.SetReferenceStruct(0x04, new HSDStruct(GetBytes(texg)));
            }

            SBM_EffectModel[] effModels = new SBM_EffectModel[models.Length];
            for(int i = 0; i < models.Length; i++)
            {
                effModels[i] = new SBM_EffectModel();
                effModels[i]._s = new HSDRawFile(GetBytes(models[i])).Roots[0].Data._s;
            }
            effTable.Models = effModels;

            // Save File---------------------------------------------

            var effFile = new HSDRawFile();

            effFile.Roots.Add(new HSDRootNode()
            {
                Name = symbol,
                Data = effTable
            });

            editedFiles.Add(new Tuple<HSDRawFile, string, bool>(effFile, effectOutputFilePath, true));


            // Add Effect Entries-------------------------------------------
            MEX_EffectEntry effectFiles = new MEX_EffectEntry();
            effectFiles.FileName = effectFileName;
            effectFiles.Symbol = symbol;
            var effectID = editor.AddMEXEffectFile(effectFiles);

            fighter.EffectIndex = effectID;
            Console.WriteLine("Effect ID:" + effectID);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        /// <returns>Dictionary to maps old sound id values to new values</returns>
        private void ImportSoundData(ZipFile pack, MEXFighterEntry fighter, string semFile)
        {
            Console.WriteLine($"Importing Sound Data...");

            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);

            // Load SEM File
            SemEntries = SEM.ReadSEMFile(semFile, true, editor._data);

            // narrator call-----------------------------------------------
            var narratorScript = @".SFXID : (id)
.REVERB : 48
.PRIORITY : 15
.UNKNOWN06 : 229
.END : 0";
            var narr = pack["Sound/narrator.dsp"];

            var nameBank = SemEntries.Find(e => e.SoundBank?.Name == "nr_name.ssm");
            if(narr != null && nameBank != null)
            {
                var narsound = new DSP();
                narsound.FromFormat(GetBytes(narr), "dsp");
                var index = nameBank.SoundBank.Sounds.Count;
                nameBank.SoundBank.Sounds.Add(narsound);

                var script = new SEMSound();
                SEM.CompileSEMScript(narratorScript.Replace("(id)", index.ToString()), out script.CommandData);
                var scriptIndex = nameBank.Sounds.Count;
                nameBank.Sounds.Add(script);

                fighter.AnnouncerCall = scriptIndex + SemEntries.IndexOf(nameBank) * 10000;

                Console.WriteLine("Imported Announcer Call");
            }

            // Create and import SSM-----------------------------------------------

            var semYAML = pack["Sound/sem.yaml"];
            var ssmFile = pack["Sound/sound.ssm"];
            if (semYAML != null)
                using (MemoryStream zos = new MemoryStream())
                {
                    semYAML.Extract(zos);
                    zos.Position = 0;
                    using (StreamReader r = new StreamReader(zos))
                    {
                        var semEntry = SEMEntry.Deserialize(r.ReadToEnd());

                        if (ssmFile != null)
                        {
                            var ssmName = fighter.NameText.ToLower() + ".ssm";
                            semEntry.SoundBank = new SSM();
                            using (MemoryStream ssmStream = new MemoryStream())
                            {
                                ssmFile.Extract(ssmStream);
                                ssmStream.Position = 0;
                                semEntry.SoundBank.Open(ssmName, ssmStream);
                            }
                            var ssmFilePath = Path.Combine(root, "audio\\us\\" + ssmName);
                            File.WriteAllBytes(ssmFilePath, GetBytes(ssmFile));
                        }

                        fighter.SSMIndex = SemEntries.Count;
                        SemEntries.Add(semEntry);
                    }
                }

            
            // Import Victory Theme
            var victory = pack["Sound/victory.hps"];
            if (victory != null)
            {
                var ffname = $"ff_{fighter.NameText.ToLower()}.hps";
                fighter.VictoryThemeID = editor.AddMusic(new HSD_String() { Value = ffname });

                var fffilePath = Path.Combine(root, "audio\\" + ffname);
                File.WriteAllBytes(fffilePath, GetBytes(victory));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InstallUI(ZipFile pack, MEXFighterEntry fighter)
        {
            Console.WriteLine($"Installing UI data...");

            // LOAD Stock Icons
            var icons = pack.Where(e => Regex.IsMatch(e.FileName, "Icon/ico..\\.png")).ToArray();
            HSD_TOBJ[] iconTOBJs = new HSD_TOBJ[icons.Length];
            foreach (var c in icons)
            {
                var index = int.Parse(Regex.Match(c.FileName, @"\d\d").Value);
                using (MemoryStream stream = new MemoryStream(GetBytes(c)))
                using (var bmp = new System.Drawing.Bitmap(stream))
                    iconTOBJs[index] = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI4, HSDRaw.GX.GXTlutFmt.RGB5A3);
            }

            // Load Menu Icon

            // Load Emblem
            var emblemPack = pack["UI/emblem.obj"];
            HSD_TOBJ emblemTexture = null;
            HSD_JOBJ emblemModel = null;
            if(emblemPack != null)
            {
                EmblemModel model;
                using (MemoryStream stream = new MemoryStream())
                {
                    emblemPack.Extract(stream);
                    stream.Position = 0;
                    model = Converters.EmblemConverter.GenerateEmblemModelFromOBJ(stream);
                }
                emblemModel = Converters.EmblemConverter.GenerateEmblemModel(model);
                emblemTexture = Converters.EmblemConverter.GenerateEmblemIconImage(model);
            }

            // Load Misc Name Tags and icons

            var largeName = pack["UI/result_victory_name.png"];
            var smallName = pack["UI/result_name.png"];
            HSD_TOBJ largeNameTexture = null;
            HSD_TOBJ smallNameTexture = null;
            if (largeName != null)
                using (MemoryStream stream = new MemoryStream(GetBytes(largeName)))
                using (var bmp = new System.Drawing.Bitmap(stream))
                    largeNameTexture = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);
            if (smallName != null)
                using (MemoryStream stream = new MemoryStream(GetBytes(smallName)))
                using (var bmp = new System.Drawing.Bitmap(stream))
                    smallNameTexture = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);

            // --------------------------------------------------------------------------

            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            
            int stride = editor.FighterEntries.Count - 3;
            int internalID = editor.FighterEntries.IndexOf(fighter);
            var externalId = MEXIdConverter.ToExternalID(internalID, editor.FighterEntries.Count);
            int GroupID = externalId - (externalId > 18 ? 1 : 0);

            // Inject CSPs and Stock Icons into Character Select
            var chrSelPath = Path.Combine(root, "MnSlChr.usd");
            if (File.Exists(chrSelPath))
                InjectCharSelectImages(pack, chrSelPath, iconTOBJs, emblemTexture, fighter, stride, GroupID);

            // Inject Stock Icons into IfAll, GmRst

            var ifallPath = Path.Combine(root, "IfAll.usd");
            if (File.Exists(ifallPath))
            {
                var datFile = new HSDRawFile(ifallPath);

                var mark = datFile.Roots.Find(e => e.Name.Equals("Stc_scemdls")).Data as HSDNullPointerArrayAccessor<HSD_JOBJDesc>;

                for(int i = 0; i < 7; i++) // first 7
                    InjectIntoMatTexAnim(mark[0].MaterialAnimations[0].Children[i].MaterialAnimation.TextureAnimation, iconTOBJs, GroupID, stride, MAX_COSTUME_COUNT);


                var emblemGroup = datFile.Roots.Find(e => e.Name.Equals("DmgMrk_scene_models")).Data as HSDNullPointerArrayAccessor<HSD_JOBJDesc>;

                InjectIntoMatTexAnim(emblemGroup[0].MaterialAnimations[0].Child.MaterialAnimation.TextureAnimation, new HSDRaw.Common.HSD_TOBJ[] { emblemTexture }, GroupID, stride, 1, fighter.InsigniaID);


                editedFiles.Add(new Tuple<HSDRawFile, string, bool>(datFile, ifallPath, true));
            }


            var gmRst = Path.Combine(root, "GmRst.usd");
            if (File.Exists(gmRst))
            {
                var datFile = new HSDRawFile(gmRst);

                var flmsce = datFile.Roots.Find(e => e.Name.Equals("flmsce")).Data as HSD_SOBJ;
                var pnlsce = datFile.Roots.Find(e => e.Name.Equals("pnlsce")).Data as HSD_SOBJ;

                // Stock Icons-------------------------------------
                for (int i = 5; i <= 8; i++) // at 5-8, 2nd mat anim
                    InjectIntoMatTexAnim(
                        pnlsce.JOBJDescs[0].MaterialAnimations[0].Children[i].MaterialAnimation.Next.TextureAnimation,
                        iconTOBJs, 
                        GroupID, 
                        stride,
                        MAX_COSTUME_COUNT);

                // Emblem Textures--------------------------------------
                var matgroup = pnlsce.JOBJDescs[0].MaterialAnimations[0].Children[17];

                for (int i = 0; i < 4; i++)
                    InjectIntoMatTexAnim(matgroup.Children[i].MaterialAnimation.TextureAnimation, new HSDRaw.Common.HSD_TOBJ[] { emblemTexture }, GroupID, stride, 1, fighter.InsigniaID);

                // Emblem Model--------------------------------------
                var emblemGroup = flmsce.JOBJDescs[0];

                if(emblemModel != null)
                {
                    var modelIndex = emblemGroup.RootJoint.Children[4].Children.Length;

                    fighter.InsigniaID = (byte)modelIndex;

                    var jointClone = HSDAccessor.DeepClone<HSDRaw.Common.Animation.HSD_AnimJoint>(emblemGroup.JointAnimations[0].Children[4].Child);
                    jointClone.Next = null;

                    var matjointClone = HSDAccessor.DeepClone<HSDRaw.Common.Animation.HSD_MatAnimJoint>(emblemGroup.MaterialAnimations[0].Children[4].Child);
                    matjointClone.Next = null;

                    emblemGroup.JointAnimations[0].Children[4].AddChild(jointClone);
                    emblemGroup.MaterialAnimations[0].Children[4].AddChild(matjointClone);
                    emblemGroup.RootJoint.Children[4].AddChild(emblemModel);
                }


                // name textures
                var largenameGroup = pnlsce.JOBJDescs[0].MaterialAnimations[0].Children[0].Children[2].MaterialAnimation.Next.TextureAnimation;

                InjectIntoMatTexAnim(largenameGroup, new HSD_TOBJ[] { largeNameTexture }, GroupID, stride, 1);

                var smallnameGroup = pnlsce.JOBJDescs[0].MaterialAnimations[0];

                for (int i = 9; i < 13; i++)
                    InjectIntoMatTexAnim(smallnameGroup.Children[i].Children[1].MaterialAnimation.TextureAnimation, new HSD_TOBJ[] { smallNameTexture }, GroupID, stride, 1);


                editedFiles.Add(new Tuple<HSDRawFile, string, bool>(datFile, gmRst, true));
            }

            // Inject Emblem

            // Generate Emblem Models and Inject

            // Inject Misc Name Tags and icons
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datPath"></param>
        /// <param name="pack"></param>
        private void InjectCharSelectImages(ZipFile pack, string datPath, HSD_TOBJ[] icons, HSD_TOBJ emblemTexture, MEXFighterEntry fighter, int stride, int groupID)
        {
            var cssFile = new HSDRawFile(datPath);

            if (cssFile.Roots[0].Data is SBM_SelectChrDataTable cssTable)
            {
                // get base width and height
                int width = 132;
                int height = 188; 
                if (cssTable.PortraitMaterialAnimation.Children[2].Child.MaterialAnimation.TextureAnimation.ImageCount > 0)
                {
                    width = cssTable.PortraitMaterialAnimation.Children[2].Child.MaterialAnimation.TextureAnimation.ImageBuffers[0].Data.Width;
                    height = cssTable.PortraitMaterialAnimation.Children[2].Child.MaterialAnimation.TextureAnimation.ImageBuffers[0].Data.Height;
                }
                // LOAD CSPs
                var csps = pack.Where(e => Regex.IsMatch(e.FileName, "CSS/css..\\.png")).ToArray();
                HSD_TOBJ[] cspTOBJs = new HSD_TOBJ[csps.Length];
                foreach (var c in csps)
                {
                    var index = int.Parse(Regex.Match(c.FileName, @"\d\d").Value);
                    using (MemoryStream stream = new MemoryStream(GetBytes(c)))
                    using (var bmp = new System.Drawing.Bitmap(stream))
                    {
                        // shrink but don't grow
                        if(bmp.Width > width || bmp.Height > height)
                        {
                            using (System.Drawing.Bitmap resized = new System.Drawing.Bitmap(width, height))
                            {
                                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(resized))
                                    g.DrawImage(bmp, 0, 0, width, height);

                                cspTOBJs[index] = TOBJConverter.BitmapToTOBJ(resized, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);
                            }
                        }
                        else
                        {
                            cspTOBJs[index] = TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);
                        }
                    }
                }

                // Inject Icons
                foreach (var n in cssTable.SingleMenuMaterialAnimation.Children[9].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.TextureAnimation, icons, groupID, stride, MAX_COSTUME_COUNT);

                // Inject CSPs
                foreach (var n in cssTable.MenuMaterialAnimation.Children[6].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.TextureAnimation, cspTOBJs, groupID, stride, MAX_COSTUME_COUNT);

                foreach (var n in cssTable.SingleMenuMaterialAnimation.Children[6].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.TextureAnimation, cspTOBJs, groupID, stride, MAX_COSTUME_COUNT);

                foreach (var n in cssTable.PortraitMaterialAnimation.Children[2].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.TextureAnimation, cspTOBJs, groupID, stride, MAX_COSTUME_COUNT);


                // Emblem 

                foreach (var n in cssTable.MenuMaterialAnimation.Children[5].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.Next.TextureAnimation, new HSDRaw.Common.HSD_TOBJ[] { emblemTexture }, groupID, stride, 1, fighter.InsigniaID);

                foreach (var n in cssTable.SingleMenuMaterialAnimation.Children[5].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.Next.TextureAnimation, new HSDRaw.Common.HSD_TOBJ[] { emblemTexture }, groupID, stride, 1, fighter.InsigniaID);

                foreach (var n in cssTable.PortraitMaterialAnimation.Children[1].Children)
                    InjectIntoMatTexAnim(n.MaterialAnimation.Next.TextureAnimation, new HSDRaw.Common.HSD_TOBJ[] { emblemTexture }, groupID, stride, 1, fighter.InsigniaID);

            }

            editedFiles.Add(new Tuple<HSDRawFile, string, bool>(cssFile, datPath, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="tobjs"></param>
        /// <param name="externalID"></param>
        /// <param name="stride"></param>
        /// <param name="count"></param>
        public static void InjectIntoMatTexAnim(HSD_TexAnim texAnim, HSD_TOBJ[] tobjs, int externalID, int stride, int count, int defaultValue = 0)
        {
            var fobjs = texAnim.AnimationObject.FObjDesc.List;

            int cspIndex = 0;
            for (int i = 0; i < count; i++)
            {
                var tindex = defaultValue;
                if (i < tobjs.Length && tobjs[i] != null)
                    tindex = texAnim.AddImage(tobjs[i]);
                foreach (var f in fobjs)
                {
                    f.InsertKey(new HSDRaw.Tools.FOBJKey()
                    {
                        Frame = externalID + stride * cspIndex,
                        Value = tindex,
                        InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_CON
                    });
                }
                cspIndex++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="fighter"></param>
        /// <param name="mexData"></param>
        private static void ExtractFiles(ZipFile pack, MEXFighterEntry fighter, MexDataEditor mexData)
        {
            Console.WriteLine($"Copying files to directory...");

            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);

            foreach(var e in pack)
            {
                var fileName = Path.GetFileName(e.FileName);

                if (Regex.IsMatch(fileName, @"Pl...dat") ||
                    Regex.IsMatch(fileName, @"Pl..AJ.dat") ||
                    Regex.IsMatch(fileName, @"GmRstM...dat") ||
                    Regex.IsMatch(fileName, @"Pl..DViWaitAJ.dat") ||
                    Regex.IsMatch(fileName, @"Kirby/PlKbCp...dat") ||
                    Regex.IsMatch(fileName, @"Costume/.*"))
                    File.WriteAllBytes(Path.Combine(root, Path.GetFileName(fileName)), GetBytes(e));

            }
        }


        /// <summary>
        /// Makes group of TOBJs use the same Image Data but different palettes
        /// </summary>
        /// <param name="tobjs"></param>
        public static void OptimizeImageData(HSD_TOBJ[] tobjs)
        {
            if (tobjs.Length == 0)
                return;

            var max = tobjs.Max(e => e.TlutData.ColorCount);
            int maxColorIndex = tobjs.Select((car, index) => new { car, index }).First(e => e.car.TlutData.ColorCount == max).index;
            maxColorIndex = 1;

            var baseTOBJ = tobjs[maxColorIndex];

            var simg = baseTOBJ.ImageData.ImageData.ToList();
            Dictionary<byte, int> palToSamp = new Dictionary<byte, int>();
            for (byte j = 0; j < baseTOBJ.TlutData.ColorCount; j ++)
            {
                palToSamp.Add(j, simg.IndexOf(j));
            }

            for (int i = 0; i < tobjs.Length; i++)
            {
                if (tobjs[i] == null || i == maxColorIndex)
                    continue;

                var dimg = tobjs[i].ImageData.ImageData;
                var dpal = tobjs[i].TlutData.TlutData;
                var npal = new byte[256 * 2];

                for (byte j = 0; j < baseTOBJ.TlutData.ColorCount; j++)
                {
                    if (palToSamp[j] == -1)
                        continue;
                    var samp = dimg[palToSamp[j]];
                    npal[j * 2] = dpal[samp * 2];
                    npal[j * 2 + 1] = dpal[samp * 2 + 1];
                }

                tobjs[i].ImageData.ImageData = simg.ToArray();
                tobjs[i].TlutData.TlutData = npal;
            }
        }
    }
}
