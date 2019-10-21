using System;
using System.IO;
using System.Windows.Forms;
using HSDRaw.Melee.Pl;
using System.Collections.Generic;

namespace HSDRawViewer.GUI
{
    public partial class AJSplitDialog : Form
    {
        private SBM_PlayerData PlayerData;

        public AJSplitDialog(SBM_PlayerData pd)
        {
            PlayerData = pd;

            InitializeComponent();

            labelAJ.Text = "";
            labelFolder.Text = "";
        }

        private void buttonAJ_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = "Pl**AJ.dat (*.dat)|*.dat";

                if(d.ShowDialog() == DialogResult.OK)
                {
                    labelAJ.Text = d.FileName;
                }
            }
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.SelectedPath = Directory.GetCurrentDirectory();

                if (d.ShowDialog() == DialogResult.OK)
                {
                    labelFolder.Text = d.SelectedPath;
                }
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if(File.Exists(labelAJ.Text) && Directory.Exists(labelFolder.Text))
            {
                using (BinaryReader r = new BinaryReader(new FileStream(labelAJ.Text, FileMode.Open)))
                {
                    var subs = PlayerData.SubActionTable.Subactions;
                    var winsubs = PlayerData.WinSubAction.Subactions;

                    HashSet<string> savedAnimations = new HashSet<string>();

                    foreach(var v in subs)
                    {
                        if(v.Name != null && !savedAnimations.Contains(v.Name))
                        {
                            savedAnimations.Add(v.Name);

                            r.BaseStream.Position = v.AnimationOffset;
                            File.WriteAllBytes(labelFolder.Text + "\\" + v.Name + ".dat", r.ReadBytes(v.AnimationSize));
                        }
                    }
                    /*foreach (var v in winsubs)
                    {
                        if (v.Name != null && !savedAnimations.Contains(v.Name))
                        {
                            savedAnimations.Add(v.Name);

                            r.BaseStream.Position = v.AnimationOffset;
                            File.WriteAllBytes(labelFolder.Text + "\\" + v.Name + ".dat", r.ReadBytes(v.AnimationSize));
                        }
                    }*/
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (File.Exists(labelAJ.Text) && Directory.Exists(labelFolder.Text))
            {
                Dictionary<string, Tuple<int, int>> animToOffset = new Dictionary<string, Tuple<int, int>>();

                using (BinaryWriter w = new BinaryWriter(new FileStream(labelAJ.Text, FileMode.Create)))
                {
                    var subs = PlayerData.SubActionTable.Subactions;
                    var winsubs = PlayerData.WinSubAction.Subactions;
                    
                    foreach (var f in Directory.GetFiles(labelFolder.Text))
                    {
                        if (f.EndsWith(".dat"))
                        {
                            var anim = File.ReadAllBytes(f);
                            animToOffset.Add(Path.GetFileNameWithoutExtension(f), new Tuple<int, int>((int)w.BaseStream.Position, anim.Length));
                            w.Write(anim);

                            while(w.BaseStream.Position % 0x20 != 0)
                                w.Write((byte)0xFF);
                        }
                    }

                    foreach (var v in subs)
                    {
                        if (v.Name != null && animToOffset.ContainsKey(v.Name))
                        {
                            var off = animToOffset[v.Name];

                            v.AnimationOffset = off.Item1;
                            v.AnimationSize = off.Item2;
                        }
                    }
                    /*foreach (var v in winsubs)
                    {
                        if (v.Name != null && animToOffset.ContainsKey(v.Name))
                        {
                            var off = animToOffset[v.Name];

                            v.AnimationOffset = off.Item1;
                            v.AnimationSize = off.Item2;
                        }
                    }*/

                    PlayerData.SubActionTable.Subactions = subs;
                    PlayerData.WinSubAction.Subactions = winsubs;
                }
            }
        }
    }
}
