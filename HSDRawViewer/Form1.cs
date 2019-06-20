using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.IO;

namespace HSDRawViewer
{
    public partial class Form1 : Form
    {
        private ByteViewer
           _myByteViewer;

        private RawHSD RawHSDFile = new RawHSD();

        private Dictionary<string, StructData> stringToStruct = new Dictionary<string, StructData>();

        public Form1()
        {
            _myByteViewer = new ByteViewer();
            _myByteViewer.Dock = DockStyle.Fill;
            Controls.Add(_myByteViewer);

            InitializeComponent();

            treeView1.AfterExpand += (sender, args) =>
            {
                args.Node.Nodes.Clear();
                var offsets = RawHSDFile.offsetToOffsets[((DataNode)args.Node).Offset];
                var innerOffsets = RawHSDFile.offsetToInnerOffsets[((DataNode)args.Node).Offset];
                StructData scheme = null;
                if (((DataNode)args.Node).Text != null && ((DataNode)args.Node).Text != "" && stringToStruct.ContainsKey(((DataNode)args.Node).Text))
                {
                    scheme = stringToStruct[((DataNode)args.Node).Text];
                }
                for (int i =0; i < offsets.Count; i++)
                {
                    int relativeOffset = (innerOffsets[i] - ((DataNode)args.Node).Offset);
                    string name = relativeOffset.ToString("X8");
                    if (scheme != null && scheme.Map.ContainsKey(relativeOffset))
                        name = scheme.Map[relativeOffset];
                    args.Node.Nodes.Add(new DataNode(name, offsets[i], RawHSDFile.offsetToData[offsets[i]]));
                }
            };

            treeView1.AfterSelect += (sender, args) =>
            {
                if(treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode n)
                {
                    _myByteViewer.SetBytes(n.Data);
                }
            };

            InitializeStructs();
            
        }

        private void InitializeStructs()
        {
            using (StreamReader r = new StreamReader(new FileStream("Structs.txt", FileMode.Open)))
            {
                while (!r.EndOfStream)
                {
                    StructData d = new StructData();
                    d.Read(r);
                    stringToStruct.Add(d.Name, d);
                }
            }
        }

        private void OpenFile(string FilePath)
        {
            treeView1.Nodes.Clear();
            RawHSDFile.Open(FilePath);
            treeView1.Nodes.AddRange(RawHSDFile.RootNodes.ToArray());
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = "HSD (*.dat)|*.dat";

                if(d.ShowDialog() == DialogResult.OK)
                    OpenFile(d.FileName);
            }
        }
    }
}
