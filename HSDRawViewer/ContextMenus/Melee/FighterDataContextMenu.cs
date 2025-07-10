using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.Animation;
using HSDRawViewer.Tools.Melee;
using System;
using System.IO;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace HSDRawViewer.ContextMenus.Melee
{
    /// <summary>
    /// 
    /// </summary>
    public class FighterDataContextMenu : CommonContextMenu
    {
#if DEBUG

        public class RenameProperty
        {
            public string OldName { get; set; }
            public string NewName { get; set; }
        }

#endif
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_FighterData) };

        public FighterDataContextMenu() : base()
        {
            ToolStripMenuItem addFromFile = new("Add Article Folder");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterData root)
                {
                    if (root.Articles == null)
                        root.Articles = new SBM_ArticlePointer();
                }
            };
            Items.Add(addFromFile);

            ToolStripMenuItem remap = new("Remap All Bones");
            remap.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterData root)
                {
                    // load source bone map
                    JointMap source = null;
                    {
                        var f = FileIO.OpenFile(ApplicationSettings.BoneFileFilter, "source.ini");
                        if (f != null)
                            source = new JointMap(f);
                    }
                    if (source == null)
                        return;

                    // load target bone map
                    JointMap target = null;
                    {
                        var f = FileIO.OpenFile(ApplicationSettings.BoneFileFilter, "target.ini");
                        if (f != null)
                            target = new JointMap(f);
                    }
                    if (target == null)
                        return;

                    // load animation file
                    string dir = Path.GetDirectoryName(MainForm.Instance.FilePath);
                    string charCode = Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath).Substring(2);
                    string charName = MainForm.SelectedDataNode.Text.Substring(6);

                    string ajFile = null;
                    string introFile = null;
                    string endingFile = null;
                    string resultFile = null;
                    string waitFile = null;
                    if (MessageBox.Show("Remap Animation", "Would you like to also remap animations?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        // ajFile = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath) + "AJ.dat");

                        ajFile = Path.Combine(dir, $"Pl{charCode}AJ.dat");
                        introFile = Path.Combine(dir, $"ftDemoIntro{charName}.dat");
                        endingFile = Path.Combine(dir, $"ftDemoEnding{charName}.dat");
                        resultFile = Path.Combine(dir, $"GmRstM{charCode}.dat");
                        waitFile = Path.Combine(dir, $"Pl{charCode}DViWaitAJ.dat");

                        if (!System.IO.File.Exists(ajFile))
                            ajFile = null;
                        if (!System.IO.File.Exists(introFile))
                            introFile = null;
                        if (!System.IO.File.Exists(endingFile))
                            endingFile = null;
                        if (!System.IO.File.Exists(resultFile))
                            resultFile = null;
                        if (!System.IO.File.Exists(waitFile))
                            waitFile = null;

                        MessageBox.Show($"Main: {Path.GetFileName(ajFile)}\n" +
                            $"Intro: {Path.GetFileName(introFile)}\n" +
                            $"Ending: {Path.GetFileName(endingFile)}\n" +
                            $"Result: {Path.GetFileName(resultFile)}\n" +
                            $"Wait: {Path.GetFileName(waitFile)}",
                            "Loaded Animations", 
                            MessageBoxButtons.OK);
                    }

                    FighterBoneRemapper r = new FighterBoneRemapper(
                        source, 
                        target, 
                        ajFile,
                        resultFile,
                        waitFile,
                        introFile,
                        endingFile);
                    r.RemapAll(root);
                }
            };
            Items.Add(remap);

#if DEBUG
            ToolStripMenuItem renameAnimSymbol = new ToolStripMenuItem("Rename Anim Symbol");
            renameAnimSymbol.Click += (sender, args) =>
            {
                var rn = new RenameProperty();

                using (PropertyDialog d = new PropertyDialog("Rename Symbol", rn))
                {
                    bool inlcudeVictoryAnim = MessageBox.Show("Include Victory Anim Symbols?", "Victory Symbols", MessageBoxButtons.YesNoCancel) == DialogResult.Yes;

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        if (MainForm.SelectedDataNode.Accessor is SBM_FighterData root)
                        {
                            var sa = root.FighterActionTable.Commands;
                            foreach (var s in sa)
                                if (s.SymbolName != null)
                                    s.SymbolName.Value = s.SymbolName.Value.Replace(rn.OldName, rn.NewName);

                            if (inlcudeVictoryAnim)
                            {
                                var vc = root.DemoActionTable.Commands;

                                foreach (var s in vc)
                                    if (s.SymbolName != null)
                                        s.SymbolName.Value = s.SymbolName.Value.Replace(rn.OldName, rn.NewName);
                            }
                        }
                    }
                }
            };
            Items.Add(renameAnimSymbol);
#endif
        }
    }
}
