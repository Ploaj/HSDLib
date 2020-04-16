using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HSDRaw.Common;
using HSDRaw.MEX;
using HSDRaw.MEX.Sounds;
using System.IO;
using HSDRawViewer.Sound;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXMusicControl : UserControl, IMEXControl
    {
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
        public HSD_String[] Music { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MEXPlaylistEntry[] MenuPlaylist { get; set; }

        public MEXMusicControl()
        {
            InitializeComponent();

            musicDSPPlayer.ReplaceButtonVisible = false;

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
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            MEXConverter.ssmValues.Clear();
            MEXConverter.ssmValues.AddRange(data.SSMTable.SSM_SSMFiles.Array.Select(e => e.Value));

            Music = data.MusicTable.BackgroundMusicStrings.Array;
            musicListEditor.SetArrayFromProperty(this, "Music");

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
            menuPlaylistEditor.SetArrayFromProperty(this, "MenuPlaylist");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            data.MetaData.NumOfMusic = Music.Length;
            data.MusicTable.BackgroundMusicStrings.Array = new HSD_String[0];
            foreach (var v in Music)
                data.MusicTable.BackgroundMusicStrings.Add(v);

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
                musicDSPPlayer.SoundName = str;
                var dsp = new Sound.DSP();
                dsp.FromFile(path);
                musicDSPPlayer.DSP = dsp;
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
        public int AddMusic(HSD_String musicName)
        {
            musicListEditor.AddItem(musicName);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveMusicButton_Click(object sender, EventArgs e)
        {
            SaveData(MexData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createHPSButton_Click(object sender, EventArgs e)
        {
            var fs = Tools.FileIO.OpenFiles(DSP.SupportedImportFilter);

            if (fs != null)
            {
                var audioPath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "audio\\");

                if (!Directory.Exists(audioPath))
                    Directory.CreateDirectory(audioPath);

                foreach (var f in fs)
                {
                    var dsp = new DSP();
                    dsp.FromFile(f);
                    HPS.SaveDSPAsHPS(dsp, Path.Combine(audioPath, Path.GetFileNameWithoutExtension(f) + ".hps"));
                    var newHPSName = Path.GetFileNameWithoutExtension(f) + ".hps";
                    foreach (var v in Music)
                        if (v.Value.Equals(newHPSName))
                            return;
                    musicListEditor.AddItem(new HSD_String() { Value = newHPSName });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicListEditor_ArrayUpdated(object sender, EventArgs e)
        {
            var oldValues = MEXConverter.musicIDValues.ToArray();
            MEXConverter.musicIDValues.Clear();
            MEXConverter.musicIDValues.AddRange(Music.Select(r => r.Value));

            if (oldValues != null && oldValues.Length == Music.Length)
            {
                for (int i = 0; i < oldValues.Length; i++)
                {
                    if (oldValues[i] != Music[i].Value)
                    {
                        var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{oldValues[i]}");
                        var newpath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{Music[i].Value}");

                        if (File.Exists(path) && !File.Exists(newpath))
                        {
                            if (MessageBox.Show($"Rename {oldValues[i]} to {Music[i].Value}?", "Rename File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
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
            if (musicListEditor.SelectedObject is HSD_String str)
            {
                PlayMusicFromName(str.Value);
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
                PlayMusicFromName(Music[str.MusicID].Value);
            }
        }

        #endregion

    }
}
