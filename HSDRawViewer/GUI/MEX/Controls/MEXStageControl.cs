using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HSDRaw.MEX;
using HSDRaw;
using HSDRaw.MEX.Sounds;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXStageControl : UserControl, IMEXControl
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

        /// <summary>
        /// 
        /// </summary>
        public MEXStageEntry[] StageEntries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MEXStageExternalEntry[] StageIDs { get; set; }

        public MEXStageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Stages";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            StageEntries = new MEXStageEntry[data.StageFunctions.Length];

            if (data.StageData._s.Length < data.StageData.TrimmedSize)
                data.StageData._s.Resize(data.StageData.TrimmedSize);

            if (data.StageData.StagePlaylists == null)
            {
                data.StageData.StagePlaylists = new HSDArrayAccessor<MEX_Playlist>();
                data.StageData.StagePlaylists.Array = new MEX_Playlist[data.StageData.StageIDTable.Length].Select(e => new MEX_Playlist()).ToArray();
            }

            for (int i = 0; i < StageEntries.Length; i++)
            {
                StageEntries[i] = new MEXStageEntry()
                {
                    Stage = data.StageFunctions[i],
                    Reverb = data.StageData.ReverbTable[i],
                    Collision = data.StageData.CollisionTable[i],
                    ItemLookup = data.StageData.StageItemLookup[i],
                    EffectLookup = data.StageData.StageEffectLookup[i],
                };
                if (data.StageData.StagePlaylists[i] != null && data.StageData.StagePlaylists[i].MenuPlayListCount > 0)
                {
                    StageEntries[i].Playlist = data.StageData.StagePlaylists[i].MenuPlaylist.Array.Select(e => new MEXPlaylistEntry() { MusicID = e.HPSID, PlayChance = e.ChanceToPlay.ToString() }).ToArray();
                }
            }
            stageEditor.SetArrayFromProperty(this, "StageEntries");

            StageIDs = data.StageData.StageIDTable.Array.Select(e => new MEXStageExternalEntry() { IDTable = e }).ToArray();
            stageIDEditor.SetArrayFromProperty(this, "StageIDs");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            for (int i = 0; i < StageEntries.Length; i++)
                StageEntries[i].InternalID = i;

            data.StageFunctions.Array = StageEntries.Select(e => e.Stage).ToArray();
            data.StageData.ReverbTable.Array = StageEntries.Select(e => e.Reverb).ToArray();
            data.StageData.CollisionTable.Array = StageEntries.Select(e => e.Collision).ToArray();
            data.StageData.StageItemLookup.Array = StageEntries.Select(e => e.ItemLookup).ToArray();
            data.StageData.StageEffectLookup.Array = StageEntries.Select(e => e.EffectLookup).ToArray();
            data.StageData.StagePlaylists.Array = StageEntries.Select(e => e.GetPlaylist()).ToArray();

            data.StageData.StageIDTable.Array = StageIDs.Select(e => e.IDTable).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {

        }
        
        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveStageButton_Click(object sender, EventArgs e)
        {
            SaveData(MexData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stageEditor_ArrayUpdated(object sender, EventArgs e)
        {
            MEXConverter.stageIDValues.Clear();
            MEXConverter.stageIDValues.AddRange(StageEntries.Select(s => s.InternalID + " - " + s.FileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapGOBJCopyButton_Click(object sender, EventArgs e)
        {
            if (stageEditor.SelectedObject is MEXStageEntry entry)
            {
                var functions = entry.GOBJFunctions;

                StringBuilder table = new StringBuilder();
                int index = 0;
                foreach (var m in functions)
                {
                    table.AppendLine("\t// map gobj " + index++);

                    table.AppendLine(string.Format(
                        "\t{{" +
                        "\n\t\t0x{0, -10}// OnCreation" +
                        "\n\t\t0x{1, -10}// OnDeletion" +
                        "\n\t\t0x{2, -10}// OnFrame" +
                        "\n\t\t0x{3, -10}// OnUnk" +
                        "\n\t\t0x{4, -10}// Bitflags" +
                        "\n\t}},",
                m.OnCreation.ToString("X") + ",",
                m.OnDeletion.ToString("X") + ",",
                m.OnFrame.ToString("X") + ",",
                m.OnUnk.ToString("X") + ",",
                m.Bitflags.ToString("X") + ","
                ));
                }

                Clipboard.SetText(
                    @"__attribute__((used))
static struct map_GOBJDesc map_gobjs[] = {
" + table.ToString() + @"}; ");

                MessageBox.Show("Map GOBJ Functions Copied to Clipboard");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stageTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            mapGOBJCopyButton.Visible = stageTabControl.SelectedIndex == 0;
        }

        #endregion

    }
}
