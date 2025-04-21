﻿using HSDRaw;
using HSDRawViewer.Sound;
using HSDRawViewer.Tools;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class SSMTool : Form
    {
        private string FilePath;

        private int StartIndex;

        public BindingList<DSP> Sounds = new();

        private readonly DSPViewer dspViewer = new();

        public ContextMenuStrip CMenu = new();

        public SSMTool()
        {
            InitializeComponent();

            listBox1.DataSource = Sounds;

            listBox1.ContextMenuStrip = CMenu;

            Text = "SSM Editor";

            dspViewer.Dock = DockStyle.Fill;
            groupBox1.Controls.Add(dspViewer);

            CenterToScreen();

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    dspViewer.Dispose();
                    args.Cancel = true;
                    Hide();
                }
            };
        }

        #region Controls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileIO.OpenFile("SSM (*.ssm, *.sdi)|*.ssm;*.sdi");

            if (file != null)
            {
                OpenFile(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is DSP dsp)
            {
                dspViewer.DSP = dsp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilePath == null)
                FilePath = FileIO.SaveFile("SSM (*.ssm)|*.ssm");

            if (FilePath != null)
                Save(FilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = FileIO.SaveFile("SSM (*.ssm)|*.ssm");

            if (filePath != null)
            {
                Save(filePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importDSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] files = FileIO.OpenFiles(DSP.SupportedImportFilter);

            if (files != null)
            {
                foreach (string file in files)
                {
                    DSP dsp = new();
                    dsp.FromFormat(File.ReadAllBytes(file), Path.GetExtension(file));
                    Sounds.Add(dsp);
                }
            }
        }

        #endregion

        #region Functions


        public void MoveUp()
        {
            MoveItem(-1);
        }

        public void MoveDown()
        {
            MoveItem(1);
        }

        public void MoveItem(int direction)
        {
            // Checking selected item
            if (listBox1.SelectedItem == null || listBox1.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = listBox1.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox1.Items.Count)
                return; // Index out of range - nothing to do

            object selected = listBox1.SelectedItem;

            // Removing removable element
            listBox1.Items.Remove(selected);
            // Insert it in new position
            listBox1.Items.Insert(newIndex, selected);
            // Restore selection
            listBox1.SetSelected(newIndex, true);
        }

        #endregion

        public void OpenFile(string filePath)
        {
            Text = "SSM Editor - " + filePath;

            Sounds.Clear();
            FilePath = filePath;

            if (Path.GetExtension(filePath).ToLower() == ".ssm")
                OpenSSM(filePath);

            if (Path.GetExtension(filePath).ToLower() == ".sdi")
                OpenSDI(filePath);

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Used in Eighting Engine Games
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenSDI(string filePath)
        {
            string sam = filePath.Replace(".sdi", ".sam");
            if (!File.Exists(sam))
                return;

            using BinaryReaderExt r = new(new FileStream(filePath, FileMode.Open));
            using BinaryReaderExt d = new(new FileStream(sam, FileMode.Open));
            r.BigEndian = true;

            while (true)
            {
                int id = r.ReadInt32();
                if (id == -1)
                    break;
                uint dataoffset = r.ReadUInt32();
                int padding = r.ReadInt32();
                short flags = r.ReadInt16();
                short frequency = r.ReadInt16();
                int value = r.ReadInt32();
                r.Skip(8); // unknown
                uint coefOffset = r.ReadUInt32();

                DSP dsp = new();
                dsp.Frequency = frequency;

                DSPChannel channel = new();
                channel.NibbleCount = value;

                uint temp = r.Position;
                uint end = (uint)d.Length;
                if (r.ReadInt32() != -1)
                    end = r.ReadUInt32();

                r.Seek(coefOffset);
                r.ReadInt32();
                r.ReadInt32();

                for (int i = 0; i < 0x10; i++)
                    channel.COEF[i] = r.ReadInt16();

                r.Seek(temp);

                d.Seek(dataoffset);
                byte[] data = d.ReadBytes((int)(end - dataoffset));

                channel.Data = data;
                channel.InitialPredictorScale = data[0];
                dsp.Channels.Add(channel);

                Sounds.Add(dsp);
            }
        }

        /// <summary>
        /// Melee's sound format
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenSSM(string filePath)
        {
            SSM ssm = new();
            StartIndex = ssm.StartIndex;
            ssm.Open(filePath);
            foreach (DSP s in ssm.Sounds)
                Sounds.Add(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void Save(string filePath)
        {
            FilePath = filePath;

            SSM ssm = new();
            ssm.StartIndex = StartIndex;
            ssm.Sounds = Sounds.ToArray();
            ssm.Save(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            dspViewer.PlaySound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileIO.SaveFile(DSP.SupportedExportFilter);

            if (file != null)
            {
                int sIndex = 0;
                foreach (DSP s in Sounds)
                {
                    string o = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + sIndex++.ToString("D2") + Path.GetExtension(file));
                    s.ExportFormat(o);
                    //s.ExportFormat(folder + "\\sound_" + sIndex++ + "_channels_" + s.Channels.Count + "_frequency_" + s.Frequency + ".wav");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is DSP dsp)
            {
                Sounds.Remove(dsp);
            }
        }
    }
}
