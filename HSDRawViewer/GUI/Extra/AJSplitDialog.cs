using System;
using System.IO;
using System.Windows.Forms;
using HSDRaw.Melee.Pl;
using HSDRaw;
using System.ComponentModel;
using System.Collections.Generic;

namespace HSDRawViewer.GUI
{
    public partial class AJSplitDialog : Form
    {
        private class Animation
        {
            public string Name;

            public byte[] Data;

            public override string ToString()
            {
                return Name;
            }
        }

        private string FighterName;

        private HSDRawFile PlayerFile;

        private bool AJLoaded = false;

        private bool ResultLoaded = false;

        private BindingList<Animation> Animations = new BindingList<Animation>();

        private SBM_PlayerData PlayerData;

        public AJSplitDialog()
        {
            InitializeComponent();

            listBox1.DataSource = Animations;

            CenterToScreen();
        }

        private bool AnimationLoaded(string name)
        {
            foreach (var a in Animations)
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
                Animations.Clear();
                PlayerFile = null;
                PlayerData = null;
                AJLoaded = false;
                ResultLoaded = false;

                var file = new HSDRawFile(f);

                foreach(var root in file.Roots)
                {
                    if(root.Data is SBM_PlayerData plData)
                    {
                        PlayerFile = file;
                        PlayerData = plData;
                        FighterName = root.Name.Replace("ftData", "");
                        Text = "AJ Split - " + FighterName;
                        openPlAJdatToolStripMenuItem.Enabled = true;
                        openGmRstMdatToolStripMenuItem.Enabled = true;
                        exportToolStripMenuItem.Enabled = true;
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
            var f = Tools.FileIO.OpenFile("Player Animation DAT (Pl**AJ.dat)|*.dat");

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

            listBox1.BeginUpdate();
            using (BinaryReader r = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                var subs = PlayerData.SubActionTable.Subactions;
                var winsubs = PlayerData.WinSubAction.Subactions;

                foreach (var v in subs)
                {
                    if (v.Name != null && !AnimationLoaded(v.Name))
                    {
                        r.BaseStream.Position = v.AnimationOffset;

                        Animations.Add(new Animation() { Name = v.Name, Data = r.ReadBytes(v.AnimationSize) });
                    }
                }
            }
            listBox1.EndUpdate();

            AJLoaded = true;
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

            listBox1.BeginUpdate();
            using (BinaryReader r = new BinaryReader(new MemoryStream(gmDat.Roots[0].Data._s.GetData())))
            {
                var winsubs = PlayerData.WinSubAction.Subactions;

                foreach (var v in winsubs)
                {
                    if (v.Name != null && !AnimationLoaded(v.Name))
                    {
                        r.BaseStream.Position = v.AnimationOffset;

                        Animations.Add(new Animation() { Name = v.Name, Data = r.ReadBytes(v.AnimationSize) });
                    }
                }
            }
            listBox1.EndUpdate();

            ResultLoaded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSelected_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem is Animation a)
            {
                var f = Tools.FileIO.SaveFile("FigaTree DAT (*.dat)|*.dat", a.Name + ".dat");

                if (f != null)
                    File.WriteAllBytes(f, a.Data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is Animation a)
            {
                var f = Tools.FileIO.OpenFile("FigaTree DAT (*_figatree.dat)|*.dat");

                if (f != null)
                    a.Data = File.ReadAllBytes(f);
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
                    
                    foreach (var a in Animations)
                    {
                        if (a.Name == fname)
                            a.Data = File.ReadAllBytes(file);
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
            // no animations loaded
            if(Animations.Count == 0)
            {
                MessageBox.Show("Please load an Animation file Pl**AJ.dat or GmRstM**.dat");
                return;
            }

            var f = Tools.FileIO.SaveFile("Player Animation DAT (Pl**AJ.dat)|*.dat");

            if (f != null)
            {
                ExportFiles(f);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Animation GetAnimationByName(string Name)
        {
            foreach (var a in Animations)
                if (a.Name == Name)
                    return a;

            return Animations[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void ExportFiles(string filePath)
        {
            var AJPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_AJ.dat");
            var resultPath = Path.Combine(Path.GetDirectoryName(filePath), "GrRstM" + Path.GetFileNameWithoutExtension(filePath) + ".dat");
           

            if (AJLoaded)
                using (BinaryWriter w = new BinaryWriter(new FileStream(AJPath, FileMode.Create)))
                {
                    PlayerData.SubActionTable.Subactions = CreateAnimationFile(w, PlayerData.SubActionTable.Subactions);
                }

            if (ResultLoaded)
            {
                HSDRawFile res = new HSDRawFile();
                res.Roots.Add(new HSDRootNode() { Name = "ftDemoResultMotionFile" + FighterName, Data = new HSDAccessor() });
                using (BinaryWriter w = new BinaryWriter(new MemoryStream()))
                {
                    PlayerData.WinSubAction.Subactions = CreateAnimationFile(w, PlayerData.WinSubAction.Subactions);

                    res.Roots[0].Data._s.SetData(((MemoryStream)w.BaseStream).ToArray());
                }
                res.Save(resultPath);
            }

            PlayerFile.Save(filePath);
        }

        private SBM_FighterSubAction[] CreateAnimationFile(BinaryWriter w, SBM_FighterSubAction[] subs)
        {
            Dictionary<string, Tuple<int, int>> animToOffset = new Dictionary<string, Tuple<int, int>>();

            foreach (var v in subs)
            {
                if (v.Name != null)
                {
                    if (!animToOffset.ContainsKey(v.Name))
                    {
                        var a = GetAnimationByName(v.Name);

                        animToOffset.Add(a.Name, new Tuple<int, int>((int)w.BaseStream.Position, a.Data.Length));
                        w.Write(a.Data);

                        while (w.BaseStream.Position % 0x20 != 0)
                            w.Write((byte)0xFF);
                    }

                    var off = animToOffset[v.Name];

                    v.AnimationOffset = off.Item1;
                    v.AnimationSize = off.Item2;
                }
            }

            return subs;
        }
    }
}
