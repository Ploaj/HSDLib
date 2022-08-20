using HSDRawViewer.Rendering;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class DockableTrackEditor : DockContent
    {
        private AnimNode _node;

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
                _node.Tracks = graphEditor1.TrackPlayers.ToList();
                TracksUpdated?.Invoke();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aobj"></param>
        public void SetKeys(GraphEditor.AnimType type, AnimNode node)
        {
            _node = node;
            graphEditor1.ClearTracks();

            if (node != null)
            {
                graphEditor1.LoadTracks(type, node.Tracks);
            }
        }
    }
}
