using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee;
using HSDRaw.Melee.Mn;
using HSDRaw.Tools;
using HSDRawViewer.Sound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class FighterPackageUninstaller
    {
        
        
        /// <summary>
        /// 
        /// </summary>
        public static void UninstallerFighter(int internalID, MEXEntry fighter, MexDataEditor editor)
        {
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var plco = Path.Combine(root, "PlCo.dat");
            var sem = Path.Combine(root, "audio\\us\\smash2.sem");

            if (!File.Exists(plco))
                throw new FileNotFoundException("PlCo.dat was not found");

            if (!File.Exists(sem))
                throw new FileNotFoundException("smash2.sem was not found");
            
            RemoveBoneTableEntry(plco, internalID, editor);

            editor.RemoveFighterEntry(internalID);

            RemoveMEXItems(fighter, editor);

            RemoveMEXEffects(fighter, editor);

            RemoveSounds(fighter, editor);
            
            RemoveUI(fighter, editor, internalID);

            // TODO: remove unused files
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="PlCoPath"></param>
        /// <param name="internalID"></param>
        /// <param name="editor"></param>
        /// <returns>true if bone table was injected</returns>
        private static bool RemoveBoneTableEntry(string PlCoPath, int internalID, MexDataEditor editor)
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

            plco.Save(PlCoPath, false);

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        private static void RemoveMEXItems(MEXEntry fighter, MexDataEditor editor)
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
        private static void RemoveMEXEffects(MEXEntry fighter, MexDataEditor editor)
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
        private static void RemoveSounds(MEXEntry fighter, MexDataEditor editor)
        {
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            var sem = Path.Combine(root, "audio\\us\\smash2.sem");

            if (!File.Exists(sem))
                return;

            // Load SEM File
            var semEntries = SEM.ReadSEMFile(sem, true, editor._data);

            // remove narrator call
            var inUse = editor.FighterEntries.Any(e=>e.AnnouncerCall == fighter.AnnouncerCall);
            if (!inUse)
            {
                var nameBank = semEntries.Find(e => e.SoundBank?.Name == "nr_name.ssm");
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
                semEntries.RemoveAt(fighter.SSMIndex);

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

            // save SEM File
            SEM.SaveSEMFile(sem, semEntries, editor._data);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="editor"></param>
        /// <param name="internalID"></param>
        private static void RemoveUI(MEXEntry fighter, MexDataEditor editor, int internalID)
        {
            var root = Path.GetDirectoryName(MainForm.Instance.FilePath);
            int GroupID = MEXIdConverter.ToExternalID(internalID, editor.FighterEntries.Count + 1) - 1;
            int stride = editor.FighterEntries.Count - 3 + 1;


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
                }
                cssFile.Save(chrSelPath);
            }

            
            var ifallPath = Path.Combine(root, "IfAll.usd");
            if (File.Exists(ifallPath))
            {
                var datFile = new HSDRawFile(ifallPath);

                var mark = datFile.Roots.Find(e => e.Name.Equals("Stc_scemdls")).Data as HSDNullPointerArrayAccessor<HSD_JOBJDesc>;

                for (int i = 0; i < 7; i++) // first 7
                    RemoveMatAnim(mark[0].MaterialAnimations[0].Children[i].MaterialAnimation.TextureAnimation, GroupID, stride, FighterPackageInstaller.MAX_COSTUME_COUNT);

                datFile.Save(ifallPath);
            }


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

                datFile.Save(gmrst);
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

            fobj.SetKeys(keys, fobj.AnimationType);

            return key;
        }
    }
}
