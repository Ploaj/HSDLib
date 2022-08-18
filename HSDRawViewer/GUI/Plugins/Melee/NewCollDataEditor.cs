using HSDRaw.Melee.Gr;
using HSDRawViewer.Rendering;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    //[SupportedTypes(new Type[] { typeof(SBM_Coll_Data) })]
    public partial class NewCollDataEditor : PluginBase, IDrawableInterface
    {
        public DrawOrder DrawOrder => DrawOrder.Last;

        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;

                CollData = _node.Accessor as SBM_Coll_Data;

                map_head = MainForm.Instance.GetSymbol("map_head") as SBM_Map_Head;

                Groups.Clear();
                CollDataBuilder.LoadCollData(CollData, Groups, Lines);
                cbGroupList.SelectedIndex = -1;
                cbGroupList.SelectedIndex = 0;
            }
        }
        private DataNode _node;

        private SBM_Coll_Data CollData;

        private SBM_Map_Head map_head;

        private BindingList<CollLineGroup> Groups = new BindingList<CollLineGroup>();

        private BindingList<CollLine> Lines = new BindingList<CollLine>();

        public CollLine[] SelectedLines { get; set; }


        public NewCollDataEditor()
        {
            InitializeComponent();
            cbGroupList.ComboBox.DataSource = Groups;

            cbGroupList.SelectedIndexChanged += (sender, args) =>
            {
                if (cbGroupList.SelectedItem is CollLineGroup group)
                    SelectGroup(group);
            };

            lineArrayEditor.OnItemAdded += (args) =>
            {
                if (args.Item is CollLine line)
                {
                    if (lineArrayEditor.SelectedObject is CollLine selectedLine)
                    {
                        line.v1 = selectedLine.v2;
                        line.v2 = new CollVertex(line.v1.X + 10, line.v1.Y);
                    }
                    else
                    {
                        line.v1 = new CollVertex(-10, 0);
                        line.v2 = new CollVertex(10, 0);
                    }
                }
            };
        }

        private void SelectGroup(CollLineGroup group)
        {
            SelectedLines = Lines.Where(e => e.Group == group).ToArray();
            lineArrayEditor.SetArrayFromProperty(this, "SelectedLines");
        }
        
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
        }

        #region Interaction

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
        }

        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        public void ViewportKeyPress(KeyEventArgs kbState)
        {
        }

        #endregion
    }
}
