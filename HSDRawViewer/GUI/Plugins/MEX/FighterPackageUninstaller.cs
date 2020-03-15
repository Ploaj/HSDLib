using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee;
using HSDRaw.Melee.Mn;
using HSDRaw.Tools;
using HSDRawViewer.Sound;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class FighterPackageUninstaller : ProgressClass
    {
        private int internalID;
        private MEXFighterEntry fighter;
        private MexDataEditor editor;
        private List<Tuple<HSDRawFile, string, bool>> editedFiles = new List<Tuple<HSDRawFile, string, bool>>();
        private List<SEMEntry> SemEntries = new List<SEMEntry>();

        public FighterPackageUninstaller(int internalID, MEXFighterEntry fighter, MexDataEditor editor)
        {
            this.internalID = internalID;
            this.fighter = fighter;
            this.editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Work(BackgroundWorker w)
        {
            if (!editor.FighterEntries.Contains(fighter))
            {
                w.ReportProgress(100);
                return;
            }

            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var plco = Path.Combine(root, "PlCo.dat");
            var sem = Path.Combine(root, "audio\\us\\smash2.sem");

            if (!File.Exists(plco))
                throw new FileNotFoundException("PlCo.dat was not found");

            if (!File.Exists(sem))
                throw new FileNotFoundException("smash2.sem was not found");

            ProgressStatus = "Removing Bone Entry";
            w.ReportProgress(0);
            RemoveBoneTableEntry(plco);

            ProgressStatus = "Removing Fighter Entry";
            w.ReportProgress(20);
            editor.RemoveFighterEntry(internalID);

            ProgressStatus = "Removing Items";
            w.ReportProgress(40);
            RemoveMEXItems();

            ProgressStatus = "Removing Fighter Effects";
            w.ReportProgress(50);
            RemoveMEXEffects();

            ProgressStatus = "Removing Sounds";
            w.ReportProgress(60);
            RemoveSounds();

            ProgressStatus = "Removing UI";
            w.ReportProgress(70);
            RemoveUI(fighter, editor, internalID);

            // TODO: remove unused files

            ProgressStatus = "Saving Files";
            w.ReportProgress(90);
            SEM.SaveSEMFile(sem, SemEntries, editor._data);

            foreach (var v in editedFiles)
                v.Item1.Save(v.Item2, v.Item3);

            ProgressStatus = "Done";
            w.ReportProgress(100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="PlCoPath"></param>
        /// <param name="internalID"></param>
        /// <param name="editor"></param>
        /// <returns>true if bone table was injected</returns>
        private bool RemoveBoneTableEntry(string PlCoPath)
        {
            Console.WriteLine($"Removing Bone Table Entry...");

            var plco = new HSDRawFile(PlCoPath);

            if (plco.Roots[0].Data is ftLoadCommonData commonData)
            {
                var boneTables = commonData.BoneTables.Array.ToList();

                // This PlCo is invalid since it contains a different number of entries than expected
                if (boneTables.Count - 1 != editor.NumberOfEntries)
                {
                    throw new InvalidDataException("PlCo Table was invalid");
                    //return false;
                }

                boneTables.RemoveAt(internalID);
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
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        private void RemoveMEXItems()
        {
            // get items used by this fighter
            // remove them from editor

            // remove larger indices first
            var it = fighter.MEXItems.Select(e => e.Value).ToList();
            it.Sort();
            it.Reverse();
            
            foreach(var i in it)
                editor.RemoveMEXItem(i);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        private void RemoveMEXEffects()
        {
            if (fighter.EffectIndex >= 0)
                editor.SafeRemoveEffectFile(fighter.EffectIndex);
            if(fighter.KirbyEffectID >= 0)
                editor.SafeRemoveEffectFile(fighter.KirbyEffectID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        private void RemoveSounds()
        {
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var sem = Path.Combine(root, "audio\\us\\smash2.sem");

            if (!File.Exists(sem))
                return;

            // Load SEM File
            SemEntries = SEM.ReadSEMFile(sem, true, editor._data);

            // remove narrator call
            var inUse = editor.FighterEntries.Any(e=>e.AnnouncerCall == fighter.AnnouncerCall);
            if (!inUse)
            {
                var nameBank = SemEntries.Find(e => e.SoundBank?.Name == "nr_name.ssm");
                nameBank.RemoveSoundAt(fighter.AnnouncerCall % 10000);

                foreach(var v in editor.FighterEntries)
                {
                    if (v.AnnouncerCall > fighter.AnnouncerCall)
                        v.AnnouncerCall -= 1;
                }
            }

            // remove ssm
            var ssminUse = editor.FighterEntries.Any(e => e.SSMIndex == fighter.SSMIndex);
            if (!ssminUse)
            {
                SemEntries.RemoveAt(fighter.SSMIndex);

                foreach (var v in editor.FighterEntries)
                {
                    if (v.SSMIndex > fighter.SSMIndex)
                        v.SSMIndex -= 1;
                }
            }

            // remove victory theme
            var vicinUse = editor.FighterEntries.Any(e => e.VictoryThemeID == fighter.VictoryThemeID);
            if (!vicinUse)
            {
                editor.RemoveMusicAt(fighter.VictoryThemeID);

                foreach (var v in editor.FighterEntries)
                {
                    if (v.VictoryThemeID > fighter.VictoryThemeID)
                        v.VictoryThemeID -= 1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        /// <param name="internalID"></param>
        private void RemoveUI(MEXFighterEntry fighter, MexDataEditor editor, int internalID)
        {
            Console.WriteLine("Removing UI");
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            int GroupID = MEXIdConverter.ToExternalID(internalID, editor.FighterEntries.Count + 1) - 1;
            int stride = editor.FighterEntries.Count - 3 + 1;


            Console.WriteLine("Removing CSP");
            var chrSelPath = Path.Combine(root, "MnSlChr.usd");
            if (File.Exists(chrSelPath))
            {
                var cssFile = new HSDRawFile(chrSelPath);

                if (cssFile.Roots[0].Data is SBM_SelectChrDataTable cssTable)
                {
                    foreach (var n in cssTable.SingleMenuMaterialAnimation.Children[9].Children)
                        RemoveMatAnim(n.MaterialAnimation.TextureAnimation, GroupID, stride, FighterPackageInstaller.MAX_COSTUME_COUNT);

                    foreach (var n in cssTable.MenuMaterialAnimation.Children[6].Children)
                        RemoveMatAnim(n.MaterialAnimation.TextureAnimation, GroupID, stride, FighterPackageInstaller.MAX_COSTUME_COUNT);

                    foreach (var n in cssTable.SingleMenuMaterialAnimation.Children[6].Children)
                        RemoveMatAnim(n.MaterialAnimation.TextureAnimation, GroupID, stride, FighterPackageInstaller.MAX_COSTUME_COUNT);

                    foreach (var n in cssTable.PortraitMaterialAnimation.Children[2].Children)
                        RemoveMatAnim(n.MaterialAnimation.TextureAnimation, GroupID, stride, FighterPackageInstaller.MAX_COSTUME_COUNT);


                    foreach (var n in cssTable.MenuMaterialAnimation.Children[5].Children)
                        RemoveMatAnim(n.MaterialAnimation.Next.TextureAnimation, GroupID, stride, 1);

                    foreach (var n in cssTable.SingleMenuMaterialAnimation.Children[5].Children)
                        RemoveMatAnim(n.MaterialAnimation.Next.TextureAnimation, GroupID, stride, 1);

                    foreach (var n in cssTable.PortraitMaterialAnimation.Children[1].Children)
                        RemoveMatAnim(n.MaterialAnimation.Next.TextureAnimation, GroupID, stride, 1);

                }

                editedFiles.Add(new Tuple<HSDRawFile, string, bool>(cssFile, chrSelPath, true));
            }


            Console.WriteLine("Removing Icons");
            var ifallPath = Path.Combine(root, "IfAll.usd");
            if (File.Exists(ifallPath))
            {
                var datFile = new HSDRawFile(ifallPath);

                var mark = datFile.Roots.Find(e => e.Name.Equals("Stc_scemdls")).Data as HSDNullPointerArrayAccessor<HSD_JOBJDesc>;

                for (int i = 0; i < 7; i++) // first 7
                    RemoveMatAnim(mark[0].MaterialAnimations[0].Children[i].MaterialAnimation.TextureAnimation, GroupID, stride, FighterPackageInstaller.MAX_COSTUME_COUNT);


                var emblemGroup = datFile.Roots.Find(e => e.Name.Equals("DmgMrk_scene_models")).Data as HSDNullPointerArrayAccessor<HSD_JOBJDesc>;

                RemoveMatAnim(emblemGroup[0].MaterialAnimations[0].Child.MaterialAnimation.TextureAnimation, GroupID, stride, 1);


                editedFiles.Add(new Tuple<HSDRawFile, string, bool>(datFile, ifallPath, true));
            }


            Console.WriteLine("Removing Result UI");
            var gmrst = Path.Combine(root, "GmRst.usd");
            if (File.Exists(gmrst))
            {
                var datFile = new HSDRawFile(gmrst);

                var flmsce = datFile.Roots.Find(e => e.Name.Equals("flmsce")).Data as HSD_SOBJ;
                var pnlsce = datFile.Roots.Find(e => e.Name.Equals("pnlsce")).Data as HSD_SOBJ;

                // remove stock icons
                for (int i = 5; i <= 8; i++) // at 5-8, 2nd mat anim
                    RemoveMatAnim(pnlsce.JOBJDescs[0].MaterialAnimations[0].Children[i].MaterialAnimation.Next.TextureAnimation,
                        GroupID,
                        stride,
                        FighterPackageInstaller.MAX_COSTUME_COUNT);

                // remove emblem entries
                var matgroup = pnlsce.JOBJDescs[0].MaterialAnimations[0].Children[17];

                for (int i = 0; i < 4; i++)
                    RemoveMatAnim(matgroup.Children[i].MaterialAnimation.TextureAnimation, GroupID, stride, 1);


                // check if any other fighter is using this stock icon
                // if not, remove it and adjust ids
                if (!editor.FighterEntries.Any(e => e.InsigniaID == fighter.InsigniaID))
                {
                    var emblemGroup = flmsce.JOBJDescs[0];
                    
                    emblemGroup.JointAnimations[0].Children[4].RemoveChildAt(fighter.InsigniaID);
                    emblemGroup.MaterialAnimations[0].Children[4].RemoveChildAt(fighter.InsigniaID);
                    emblemGroup.RootJoint.Children[4].RemoveChildAt(fighter.InsigniaID);

                    foreach (var f in editor.FighterEntries)
                        if (f.InsigniaID > fighter.InsigniaID)
                            f.InsigniaID--;
                }


                // name textures
                var largenameGroup = pnlsce.JOBJDescs[0].MaterialAnimations[0].Children[0].Children[2].MaterialAnimation.Next.TextureAnimation;

                RemoveMatAnim(largenameGroup, GroupID, stride, 1);

                var smallnameGroup = pnlsce.JOBJDescs[0].MaterialAnimations[0];

                for (int i = 9; i < 13; i++)
                    RemoveMatAnim(smallnameGroup.Children[i].Children[1].MaterialAnimation.TextureAnimation, GroupID, stride, 1);

                editedFiles.Add(new Tuple<HSDRawFile, string, bool>(datFile, gmrst, true));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="externalID"></param>
        /// <param name="stride"></param>
        /// <param name="count"></param>
        private static void RemoveMatAnim(HSD_TexAnim texAnim, int externalID, int stride, int count)
        {
            var fobjs = texAnim.AnimationObject.FObjDesc.List;

            List<int> toRem = new List<int>();

            for (int i = count - 1; i >= 0; i--)
            {
                var frame = externalID + stride * i;

                foreach (var f in fobjs)
                {
                    var key = RemoveKey(f, frame);
                    if (key != null && key.Value != 0 && !toRem.Contains((int)key.Value))
                        toRem.Add((int)key.Value);
                }
            }

            toRem.Sort();
            toRem.Reverse();
            foreach (var r in toRem)
            {
                texAnim.RemoveImageAt(r);
            }
        }
        
        /// <summary>
        /// Removes key at frame and shifts other frames down
        /// Also shifts key values down from removed one
        /// </summary>
        /// <param name="frame"></param>
        /// <returns>removed key if it has a value at that frame</returns>
        private static FOBJKey RemoveKey(HSD_FOBJDesc fobj, int frame)
        {
            var keys = fobj.GetDecodedKeys();

            var key = keys.Find(e => e.Frame == frame);

            keys.Remove(key);

            foreach (var k in keys)
            {
                if(key != null && key.Value != 0)
                {
                    if (k.Value == key.Value)
                        k.Value = 0;
                    if (k.Value > key.Value)
                        k.Value--;
                }
                if (k.Frame > frame)
                    k.Frame--;
            }

            fobj.SetKeys(keys, fobj.TrackType);

            return key;
        }
    }
}
