using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class AOBJEditor : DockContent, EditorBase
    {
        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(HSD_AOBJ) };

        public DataNode Node { get => _node;
            set
            {
                if (value.Accessor is HSD_AOBJ aobj)
                {
                    _node = value;
                    this.aobj = aobj;
                    RefreshList();
                }
            }
        }
        private DataNode _node;
        private HSD_AOBJ aobj;

        private KeyEditor keyEditor;

        public AOBJEditor()
        {
            InitializeComponent();

            keyEditor = new KeyEditor();
            keyEditor.Dock = DockStyle.Fill;

            cbAnimationType.DataSource = Enum.GetValues(typeof(JointTrackType));

            groupBox2.Controls.Add(keyEditor);
            keyEditor.BringToFront();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshList()
        {
            treeView1.Nodes.Clear();
            foreach(var fobjDesc in aobj.FObjDesc.List)
            {
                treeView1.Nodes.Add(new TreeNode()
                {
                    Text = fobjDesc.AnimationType.ToString(),
                    Tag = fobjDesc
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node?.Tag is HSD_FOBJDesc desc)
            {
                cbAnimationType.SelectedItem = desc.AnimationType;
                keyEditor.SetKeys(desc.GetDecodedKeys());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag is HSD_FOBJDesc desc)
            {
                desc.SetKeys(keyEditor.GetFOBJKeys(), (JointTrackType)(cbAnimationType.SelectedItem));
                treeView1.SelectedNode.Text = desc.AnimationType.ToString();
            }
        }
    }
}
