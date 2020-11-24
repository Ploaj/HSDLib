using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HSDRaw.Common;
using HSDRaw.MEX;
using HSDRaw.MEX.Sounds;
using System.IO;
using HSDRawViewer.Sound;
using HSDRawViewer.Tools;
using System.ComponentModel;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXMusicControl : UserControl, IMEXControl
    {
        /// <summary>
        /// 
        /// </summary>
        public class MusicEntry
        {
            [DisplayName("File Name")]
            public string FileName { get => _fileName.Value; set => _fileName.Value = value; }

            public HSD_String _fileName = new HSD_String() { Value = "" };

            public override string ToString()
            {
                return FileName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class MusicEntryLabel : MusicEntry
        {
            [DisplayName("Label")]
            public string Label { get => _label.Value; set => _label.Value = value; }

            public HSD_ShiftJIS_String _label = new HSD_ShiftJIS_String() { Value = "" };

            public override string ToString()
            {
                return Label;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MEX_Data MexData
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e._data;
                return null;
            }
        }

        public MexDataEditor MexDataEditor
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MusicEntry[] Music { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MEXPlaylistEntry[] MenuPlaylist { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MEXMusicControl()
        {
            InitializeComponent();

            //musicDSPPlayer.ReplaceButtonVisible = false;

            musicListEditor.EnablePropertyViewDescription = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Music";
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckEnable(MexDataEditor editor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            var filenames = data.MusicTable.BGMFileNames.Array;
            var labels = data.MusicTable.BGMLabels?.Array;
            
            // load labels if they exist
            if (labels != null)
            {
                Music = new MusicEntryLabel[filenames.Length];
                for (int i = 0; i < filenames.Length; i++)
                {
                    Music[i] = new MusicEntryLabel()
                    {
                        _fileName = filenames[i],
                        _label = labels[i]
                    };
                }
            }
            else
                Music = filenames.Select(e => new MusicEntry() { _fileName = e }).ToArray();

            // bind array
            musicListEditor.SetArrayFromProperty(this, "Music");

            // menu playlist
            MenuPlaylist = new MEXPlaylistEntry[data.MusicTable.MenuPlayListCount];
            for (int i = 0; i < MenuPlaylist.Length; i++)
            {
                var e = data.MusicTable.MenuPlaylist[i];
                MenuPlaylist[i] = new MEXPlaylistEntry()
                {
                    MusicID = e.HPSID,
                    PlayChance = e.ChanceToPlay.ToString() + "%"
                };
            }

            // bind playlist
            menuPlaylistEditor.SetArrayFromProperty(this, "MenuPlaylist");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            data.MetaData.NumOfMusic = Music.Length;
            data.MusicTable.BGMFileNames.Array = Music.Select(e => e._fileName).ToArray();
            data.MusicTable.BGMLabels.Array = new HSD_ShiftJIS_String[Music.Length];
            for(int i = 0; i < Music.Length; i++)
            {
                if (Music[i] is MusicEntryLabel l)
                    data.MusicTable.BGMLabels.Set(i, new HSD_ShiftJIS_String() { Value = l.Label});
                else
                    data.MusicTable.BGMLabels.Set(i, new HSD_ShiftJIS_String() { Value = "" });
            }

            // playlist
            data.MusicTable.MenuPlaylist.Array = new MEX_PlaylistItem[0];
            data.MusicTable.MenuPlayListCount = MenuPlaylist.Length;
            for (int i = 0; i < MenuPlaylist.Length; i++)
            {
                var v = MenuPlaylist[i];
                data.MusicTable.MenuPlaylist.Set(i, new MEX_PlaylistItem()
                {
                    HPSID = (ushort)v.MusicID,
                    ChanceToPlay = v.PlayChanceValue
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {
            musicListEditor.ResetBindings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        private void PlayMusicFromName(string str)
        {
            var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{str}");

            musicDSPPlayer.DSP = null;
            if (File.Exists(path))
            {
                musicDSPPlayer.LoadFromFile(path);
                musicDSPPlayer.PlaySound();
            }
            else
            {
                MessageBox.Show("Could not find sound \"" + str + "\"", "Sound Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AddMusic(HSD_String musicName, string label = null)
        {
            if(string.IsNullOrEmpty(label))
                musicListEditor.AddItem(new MusicEntry() { _fileName = musicName });
            else
                musicListEditor.AddItem(new MusicEntryLabel() { _fileName = musicName, _label = new HSD_ShiftJIS_String() { Value = label} });

            return Music.Length - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="musicName"></param>
        public void RemoveMusicAt(int index)
        {
            musicListEditor.RemoveAt(index);
        }

        #region Events

        public class HPSImportOptions
        {
            [DisplayName("File Name"), Description("File name to use in file system")]
            public string FileName { get; set; }

            [DisplayName("Music Name"), Description("Name shown in game")]
            public string MusicName { get; set; }

            [DisplayName("Loop Point"), Description("Time stamp to loop to when music ends")]
            public string LoopPoint
            {
                get => _loopPoint.ToString();
                set => TimeSpan.TryParse(value, out _loopPoint);
            }
            public TimeSpan _loopPoint = TimeSpan.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createHPSButton_Click(object sender, EventArgs e)
        {
            var fs = FileIO.OpenFiles(DSP.SupportedImportFilter);

            if (fs != null)
            {
                var audioPath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "audio\\");

                if (!Directory.Exists(audioPath))
                    Directory.CreateDirectory(audioPath);

                foreach (var f in fs)
                {
                    var options = new HPSImportOptions();
                    options.FileName = Path.GetFileNameWithoutExtension(f) + ".hps";
                    options.MusicName = Path.GetFileNameWithoutExtension(f);

                    using (PropertyDialog d = new PropertyDialog("HPS Creation Options", options))
                    {
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            var dsp = new DSP();
                            dsp.FromFile(f);
                            dsp.SetLoopFromTimeSpan(options._loopPoint);
                            HPS.SaveDSPAsHPS(dsp, Path.Combine(audioPath, options.FileName));

                            foreach (var v in Music)
                                if (v.FileName.Equals(options.FileName))
                                    continue;

                            AddMusic(new HSD_String() { Value = options.FileName }, options.MusicName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string[] GetFileNameStrings()
        {
            return Music.Select(e => e.FileName).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicListEditor_ArrayUpdated(object sender, EventArgs e)
        {
            var oldValues = MEXConverter.musicIDValues.ToArray();

            var music = GetFileNameStrings();

            MEXConverter.musicIDValues.Clear();
            MEXConverter.musicIDValues.AddRange(music);

            if (oldValues != null && oldValues.Length == music.Length)
            {
                for (int i = 0; i < oldValues.Length; i++)
                {
                    if (oldValues[i] != music[i])
                    {
                        var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{oldValues[i]}");
                        var newpath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{music[i]}");

                        if (File.Exists(path) && !File.Exists(newpath))
                        {
                            if (MessageBox.Show($"Rename {oldValues[i]} to {music[i]}?", "Rename File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                File.Move(path, newpath);
                            }
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
        private void musicListEditor_DoubleClickedNode(object sender, EventArgs e)
        {
            if (musicListEditor.SelectedObject is MusicEntry str)
            {
                PlayMusicFromName(str.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuPlaylistEditor_DoubleClickedNode(object sender, EventArgs e)
        {
            if (menuPlaylistEditor.SelectedObject is MEXPlaylistEntry str)
            {
                PlayMusicFromName(Music[str.MusicID].FileName);
            }
        }

        #endregion

    }
}
