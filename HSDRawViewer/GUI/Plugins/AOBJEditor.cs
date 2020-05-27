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
                if (value.Parent is DataNode par)
                {
                    if(par.Accessor is HSD_AnimJoint)
                        comboBoxAnimType.SelectedIndex = 0;
                    if (par.Accessor is HSD_MatAnim)
                        comboBoxAnimType.SelectedIndex = 1;
                    if (par.Accessor is HSD_TexAnim)
                        comboBoxAnimType.SelectedIndex = 2;
                }

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

        private HSD_FOBJDesc SelectedFOBJ;

        /// <summary>
        /// 
        /// </summary>
        public AOBJEditor()
        {
            InitializeComponent();

            keyEditor = new KeyEditor();
            keyEditor.Dock = DockStyle.Fill;

            comboBoxAnimType.SelectedIndex = 0;

            groupBox2.Controls.Add(keyEditor);
            keyEditor.BringToFront();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshList()
        {
            if (aobj == null || aobj.FObjDesc == null)
                return;
            
            treeView1.Nodes.Clear();
            foreach(var fobjDesc in aobj.FObjDesc?.List)
            {
                treeView1.Nodes.Add(new TreeNode()
                {
                    Text =  GetTrackName(fobjDesc),
                    Tag = fobjDesc
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        private string GetTrackName(HSD_FOBJDesc desc)
        {
            switch (comboBoxAnimType.SelectedIndex)
            {
                case 0: return desc.JointTrackType.ToString();
                case 1: return desc.MatTrackType.ToString();
                case 2: return desc.TexTrackType.ToString();
                default: return desc.TrackType.ToString("X2");
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
                SelectedFOBJ = desc;
                if(desc.TrackType - 1 < cbAnimationType.Items.Count)
                    cbAnimationType.SelectedIndex = desc.TrackType - 1;
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
            if (SelectedFOBJ != null)
            {
                SelectedFOBJ.SetKeys(keyEditor.GetFOBJKeys(), (byte)(cbAnimationType.SelectedIndex + 1));
                RefreshList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var FOBJDesc = new HSD_FOBJDesc();
            FOBJDesc.SetKeys(new System.Collections.Generic.List<HSDRaw.Tools.FOBJKey>()
            {
                new HSDRaw.Tools.FOBJKey()
                {
                    Frame = 0,
                    InterpolationType = GXInterpolationType.HSD_A_OP_CON
                },
                new HSDRaw.Tools.FOBJKey()
                {
                    Frame = aobj.EndFrame,
                    InterpolationType = GXInterpolationType.HSD_A_OP_CON
                }
            }, 1);

            if (aobj.FObjDesc == null)
                aobj.FObjDesc = FOBJDesc;
            else
                aobj.FObjDesc.Add(FOBJDesc);

            RefreshList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;

            var selected = treeView1.SelectedNode.Tag as HSD_FOBJDesc;

            if (aobj.FObjDesc == selected)
                aobj.FObjDesc = selected.Next;
            else
            {
                foreach (var v in aobj.FObjDesc.List)
                {
                    if (v.Next == selected)
                    {
                        v.Next = selected.Next;
                        break;
                    }
                }
            }

            RefreshList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAnimType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAnimType.SelectedIndex)
            {
                case 0:
                    cbAnimationType.DataSource = Enum.GetValues(typeof(JointTrackType));
                    break;
                case 1:
                    cbAnimationType.DataSource = Enum.GetValues(typeof(MatTrackType));
                    break;
                case 2:
                    cbAnimationType.DataSource = Enum.GetValues(typeof(TexTrackType));
                    break;
            }

            if (treeView1.SelectedNode?.Tag is HSD_FOBJDesc desc && desc.TrackType - 1 < cbAnimationType.Items.Count)
                cbAnimationType.SelectedIndex = desc.TrackType - 1;

            RefreshList();
        }
    }
}
