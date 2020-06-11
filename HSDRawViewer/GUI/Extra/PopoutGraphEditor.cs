using HSDRawViewer.Rendering;
using System.Linq;
using System.Windows.Forms;
using static HSDRawViewer.GUI.Controls.GraphEditor;

namespace HSDRawViewer.GUI.Extra
{
    public partial class PopoutGraphEditor : Form
    {
        private AnimNode _node;

        public PopoutGraphEditor(string boneName, AnimNode node)
        {
            InitializeComponent();

            Text = boneName;

            _node = node;
            graphEditor.LoadTracks(AnimType.Joint, node.Tracks);

            graphEditor.OnTrackListUpdate += (sender, args) =>
            {
                _node.Tracks = graphEditor.TrackPlayers.ToList();
            };
        }
    }
}
