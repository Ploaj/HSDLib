using HSDRaw.MEX;
using HSDRawViewer.GUI.MEX.Controls;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(MEX_Stock) })]
    public partial class MEXStockIconEditor : DockContent, EditorBase
    {
        public DockState DefaultDockState => DockState.Document;

        public DataNode Node { get => node;
            set
            {
                node = value;

                if(node.Accessor is MEX_Stock stock)
                    stockControl.LoadStockNode(stock);
            }
        }

        private DataNode node;

        private MEXStockIconControl stockControl;

        public MEXStockIconEditor()
        {
            InitializeComponent();

            stockControl = new MEXStockIconControl();
            stockControl.Dock = DockStyle.Fill;
            Controls.Add(stockControl);
        }
    }
}
