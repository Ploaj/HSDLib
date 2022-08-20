using HSDRaw.Tools;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class DockableTrackEditor : DockContent
    {
        private List<FOBJ_Player> list;

        public delegate void TrackUpdate();
        public TrackUpdate TracksUpdated;

        /// <summary>
        /// 
        /// </summary>
        public DockableTrackEditor()
        {
            InitializeComponent();

            Text = "Animation Editor";

            graphEditor1.TrackListUpdated += (s, a) =>
            {
                if (list != null)
                {
                    list.Clear();
                    list.AddRange(graphEditor1.TrackPlayers);
                }
                TracksUpdated?.Invoke();
            };

            // prevent user closing
            CloseButtonVisible = false;
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aobj"></param>
        public void SetKeys(string objectName, GraphEditor.AnimType type, List<FOBJ_Player> tracks)
        {
            Text = $"Animation Editor: {objectName}";

            list = null;

            graphEditor1.ClearTracks();
            if (tracks != null)
                graphEditor1.LoadTracks(type, tracks);

            list = tracks;
        }
    }
}
