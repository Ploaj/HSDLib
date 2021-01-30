using System;
using System.IO;
using System.Windows.Forms;
using HSDRaw.Melee.Pl;
using HSDRaw;
using System.ComponentModel;
using System.Collections.Generic;
using HSDRaw.Common.Animation;

namespace HSDRawViewer.GUI
{
    public partial class AJSplitDialog : Form
    {
        private class Animation
        {
            public string Name;

            public byte[] Data;

            /// <summary>
            /// 
            /// </summary>
            public void SetFromFile(string filePath)
            {
                try
                {
                    var figaFile = new HSDRawFile(filePath);
                    if(figaFile.Roots.Count > 0 && figaFile.Roots[0].Data is HSD_FigaTree tree)
                    {
                        if(figaFile.Roots[0].Name.Equals(Name))
                            Data = File.ReadAllBytes(filePath);
                        else
                        {
                            // rename symbol if necessary
                            //if(MessageBox.Show($"The animation symbol does not match./nRename It?\n{Name}\n{figaFile.Roots[0].Name}", "Symbol Mismatch", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                figaFile.Roots[0].Name = Name;
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    figaFile.Save(stream);
                                    Data = stream.ToArray();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error replacing animation", "Animation Replace Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Name;
            }
        }

        private string FighterName;

        private HSDRawFile PlayerFile;

        private bool AJLoaded = false;

        private bool ResultLoaded = false;

        private BindingList<Animation> FightingAnimations = new BindingList<Animation>();
        private BindingList<Animation> ResultAnimations = new BindingList<Animation>();

        private SBM_FighterData PlayerData;

        private string ftDataPath;
        private string ftAJPath;

        public AJSplitDialog()
        {
            InitializeComponent();

            lbFighting.DataSource = FightingAnimations;
            lbResult.DataSource = ResultAnimations;

            lbFighting.AllowDrop = true;
            lbFighting.DragDrop += listBox_DragDrop;
            lbFighting.DragEnter += listBox_DragEnter;
            lbResult.AllowDrop = true;
            lbResult.DragDrop += listBox_DragDrop;
            lbResult.DragEnter += listBox_DragEnter;

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int i;
            bool replaceAll = s.Length == 1 ? true : (MessageBox.Show($"Replace All?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes);
            for (i = 0; i < s.Length; i++)
            {
                if (s[i].ToLower().EndsWith(".dat"))
                {
                    var name = Path.GetFileNameWithoutExtension(s[i]);
                    var d = File.ReadAllBytes(s[i]);

                    foreach (Animation v in lbFighting.Items)
                        if (v.Name.Equals(name))
                        {
                            if (replaceAll || MessageBox.Show($"Replace {v.Name}?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                v.Data = d;
                        }
                    foreach (Animation v in lbResult.Items)
                        if (v.Name.Equals(name))
                        {
                            if (replaceAll || MessageBox.Show($"Replace {v.Name}?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                v.Data = d;
                        }
                }
            }
        }

        /// <summary>
        /// Returns true if animation of given name is already loaded
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool AnimationLoaded(string name)
        {
            foreach (var a in FightingAnimations)
                if (a.Name == name)
                    return true;
            foreach (var a in ResultAnimations)
                if (a.Name == name)
                    return true;
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openPldatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Player DAT (Pl**.dat)|*.dat");

            if (f != null)
            {
                ResultAnimations.Clear();
                FightingAnimations.Clear();
                PlayerFile = null;
                PlayerData = null;
                AJLoaded = false;
                ResultLoaded = false;

                var file = new HSDRawFile(f);

                foreach(var root in file.Roots)
                {
                    if(root.Data is SBM_FighterData plData)
                    {
                        ftDataPath = f;
                        PlayerFile = file;
                        PlayerData = plData;
                        FighterName = root.Name.Replace("ftData", "");
                        Text = "AJ Split - " + FighterName;
                        buttonLoadAnims.Enabled = true;
                        buttonLoadResult.Enabled = true;
                        exportDATsToolStripMenuItem.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openPlAJdatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Player Animations (Pl**AJ.dat)|*.dat");

            if (f != null)
                LoadAJ(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openGmRstMdatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Player Victory Animation DAT (GmRstM**.dat)|*.dat");

            if (f != null)
                LoadGm(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadAJ(string filePath)
        {
            // validate
            var idle = new HSDRawFile(filePath);
            if (idle.Roots.Count == 0)
                return;
            var match = System.Text.RegularExpressions.Regex.Match(idle.Roots[0].Name, @"(?![Ply])(.*)(?=\dK)");

            if (!match.Success || match.Groups[0].Value != FighterName)
            {
                if (MessageBox.Show($"Import animation {match.Groups[0].Value} instead of {FighterName}?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }

            lbFighting.BeginUpdate();
            using (BinaryReader r = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                var subs = PlayerData.FighterCommandTable.Commands;
                var winsubs = PlayerData.WinCommandTable.Commands;

                foreach (var v in subs)
                {
                    if (v.Name != null && !AnimationLoaded(v.Name))
                    {
                        r.BaseStream.Position = v.AnimationOffset;

                        FightingAnimations.Add(new Animation() { Name = v.Name, Data = r.ReadBytes(v.AnimationSize) });
                    }
                }
            }
            lbFighting.EndUpdate();

            AJLoaded = true;
            ftAJPath = filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        private void LoadGm(string FilePath)
        {
            var gmDat = new HSDRawFile(FilePath);

            // validate
            if (gmDat.Roots.Count == 0)
                return;
            var name = gmDat.Roots[0].Name.Replace("ftDemoResultMotionFile", "");

            if (name != FighterName)
            {
                if (MessageBox.Show($"Import animation for {name} instead of {FighterName}?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }

            lbResult.BeginUpdate();
            using (BinaryReader r = new BinaryReader(new MemoryStream(gmDat.Roots[0].Data._s.GetData())))
            {
                var winsubs = PlayerData.WinCommandTable.Commands;

                foreach (var v in winsubs)
                {
                    if (v.Name != null && !AnimationLoaded(v.Name))
                    {
                        r.BaseStream.Position = v.AnimationOffset;
                        var data = r.ReadBytes(v.AnimationSize);

                        try
                        {
                            HSDRawFile file = new HSDRawFile(data);

                            //if (file.Roots[0].Name != v.Name)
                            //    continue;

                            ResultAnimations.Add(new Animation() { Name = v.Name, Data = data });
                        } catch(Exception)
                        {

                        }

                    }
                }
            }
            lbResult.EndUpdate();

            ResultLoaded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSelected_Click(object sender, EventArgs e)
        {
            ListBox box = lbFighting;
            if (tabControl1.SelectedIndex == 1)
                box = lbResult;

            if (box.SelectedItems.Count > 1)
            {
                var f = Tools.FileIO.OpenFolder();
                if (f != null)
                    foreach (var v in box.SelectedItems)
                        if (v is Animation a)
                            File.WriteAllBytes(f + "\\" + a.Name + ".dat", a.Data);
            }
            else
            {
                if (box.SelectedItem is Animation a)
                {
                    var f = Tools.FileIO.SaveFile("FigaTree DAT (*.dat)|*.dat", a.Name + ".dat");

                    if (f != null)
                        File.WriteAllBytes(f, a.Data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            ListBox box = lbFighting;
            if (tabControl1.SelectedIndex == 1)
                box = lbResult;
            if (box.SelectedItems.Count > 0)
            {
                var f = Tools.FileIO.OpenFile("FigaTree DAT (*_figatree.dat)|*.dat");

                foreach(var v in box.SelectedItems)
                    if(v is Animation anim)
                    {
                        if (f != null)
                        {
                            anim.SetFromFile(f);
                        }
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFolder();

            if(f != null)
            {
                var Animations = FightingAnimations;
                if (tabControl1.SelectedIndex == 1)
                    Animations = ResultAnimations;

                foreach(var a in Animations)
                {
                    File.WriteAllBytes(Path.Combine(f, a.Name + ".dat"), a.Data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFolder();

            if (f != null)
            {
                foreach (var file in Directory.GetFiles(f))
                {
                    var fname = Path.GetFileNameWithoutExtension(file);

                    var Animations = FightingAnimations;
                    if (tabControl1.SelectedIndex == 1)
                        Animations = ResultAnimations;

                    foreach (var a in Animations)
                    {
                        if (a.Name == fname)
                        {
                            a.SetFromFile(file);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportFiles();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void ExportFiles()
        {
            var filePath = Tools.FileIO.SaveFile("Pl**.dat (*.dat)|*.dat", Path.GetFileName(ftDataPath));
            if (filePath != null)
            {
                if (AJLoaded)
                {
                    var AJPath = Tools.FileIO.SaveFile("Pl**AJ.dat (*.dat)|*.dat", Path.GetFileName(ftAJPath));
                    if (AJPath != null)
                        using (BinaryWriter w = new BinaryWriter(new FileStream(AJPath, FileMode.Create)))
                        {
                            PlayerData.FighterCommandTable.Commands = CreateAnimationFile(w, PlayerData.FighterCommandTable.Commands, FightingAnimations);
                        }
                }

                if (ResultLoaded)
                {
                    var resultPath = Tools.FileIO.SaveFile("GrRstM**.dat (*.dat)|*.dat", "GrRstM" + FighterName + ".dat");
                    if (resultPath != null)
                    {
                        HSDRawFile res = new HSDRawFile();
                        res.Roots.Add(new HSDRootNode() { Name = "ftDemoResultMotionFile" + FighterName, Data = new HSDAccessor() });
                        using (BinaryWriter w = new BinaryWriter(new MemoryStream()))
                        {
                            PlayerData.WinCommandTable.Commands = CreateAnimationFile(w, PlayerData.WinCommandTable.Commands, ResultAnimations);

                            res.Roots[0].Data._s.SetData(((MemoryStream)w.BaseStream).ToArray());
                        }
                        res.Save(resultPath);
                    }
                }

                PlayerFile.Save(filePath, false, false);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Animation GetAnimationByName(string Name, BindingList<Animation> anims)
        {
            foreach (var a in anims)
                if (a.Name == Name)
                    return a;

            return null;
        }
        
        /// <summary>
        /// Creates a new animation file and returns the adjusted subactions
        /// </summary>
        /// <param name="w"></param>
        /// <param name="subs"></param>
        /// <returns></returns>
        private SBM_FighterCommand[] CreateAnimationFile(BinaryWriter w, SBM_FighterCommand[] subs, BindingList<Animation> anims)
        {
            Dictionary<string, Tuple<int, int>> animToOffset = new Dictionary<string, Tuple<int, int>>();

            foreach (var v in subs)
            {
                if (v.Name != null)
                {
                    if (!animToOffset.ContainsKey(v.Name))
                    {
                        var a = GetAnimationByName(v.Name, anims);

                        if (a != null)
                        {
                            animToOffset.Add(a.Name, new Tuple<int, int>((int)w.BaseStream.Position, a.Data.Length));
                            w.Write(a.Data);

                            while (w.BaseStream.Position % 0x20 != 0)
                                w.Write((byte)0xFF);
                        }
                    }

                    if (animToOffset.ContainsKey(v.Name))
                    {
                        var off = animToOffset[v.Name];

                        v.AnimationOffset = off.Item1;
                        v.AnimationSize = off.Item2;
                    }
                }
            }

            return subs;
        }
    }
}
