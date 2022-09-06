using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.GUI.Controls;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_AOBJ) })]
    public partial class AOBJEditor : PluginBase
    {
        public override DataNode Node { get => _node;
            set
            {
                GraphEditor.AnimType type = GraphEditor.AnimType.Joint;

                if (value.Parent is DataNode par)
                {
                    if (par.Accessor is HSD_MatAnim)
                        type = GraphEditor.AnimType.Material;
                    if (par.Accessor is HSD_TexAnim)
                        type = GraphEditor.AnimType.Texture;
                    if (par.Accessor is HSD_LightAnimPointer)
                        type = GraphEditor.AnimType.Light;
                }

                if (value.Accessor is HSD_AOBJ aobj)
                {
                    _node = value;
                    this.aobj = aobj;

                    graphEditor.LoadTracks(type, aobj);
                }
            }
        }
        private DataNode _node;
        private HSD_AOBJ aobj;

        private GraphEditor graphEditor;

        /// <summary>
        /// 
        /// </summary>
        public AOBJEditor()
        {
            InitializeComponent();

            graphEditor = new GraphEditor();
            graphEditor.Dock = DockStyle.Fill;
            Controls.Add(graphEditor);
            graphEditor.BringToFront();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            aobj.FObjDesc = graphEditor.ToFOBJs();
            MessageBox.Show("Saved Changes");
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnFileSave()
        {

        }
    }
}
