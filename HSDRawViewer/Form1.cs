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

        private Dictionary<int, byte[]> offsetToData = new Dictionary<int, byte[]>();
        private Dictionary<int, List<int>> offsetToOffsets = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> offsetToInnerOffsets = new Dictionary<int, List<int>>();

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
                var offsets = offsetToOffsets[((DataNode)args.Node).Offset];
                var innerOffsets = offsetToInnerOffsets[((DataNode)args.Node).Offset];
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
                    args.Node.Nodes.Add(new DataNode(name, offsets[i], offsetToData[offsets[i]]));
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
            using (HSDLib.HSDReader r = new HSDLib.HSDReader(new FileStream(FilePath, FileMode.Open)))
            {
                r.ReadInt32(); // dat size
                int relocOffset = r.ReadInt32() + 0x20;
                int relocCount = r.ReadInt32();
                int rootCount = r.ReadInt32();

                List<int> Offsets = new List<int>();
                HashSet<int> OffsetContain = new HashSet<int>();
                Offsets.Add(relocOffset);

                Dictionary<int, int> relocOffsets = new Dictionary <int, int>();

                r.BaseStream.Position = relocOffset;
                for (int i = 0; i < relocCount; i++)
                {
                    int offset = r.ReadInt32() + 0x20;

                    var temp = r.BaseStream.Position;
                    r.BaseStream.Position = offset;
                    var objectOff = r.ReadInt32() + 0x20;

                    relocOffsets.Add(offset, objectOff);

                    if (!OffsetContain.Contains(objectOff))
                    {
                        OffsetContain.Add(objectOff);
                        Offsets.Add(objectOff);
                    }

                    r.BaseStream.Position = temp;
                }

                List<int> rootOffsets = new List<int>();
                List<string> rootStrings = new List<string>();
                var stringStart = r.BaseStream.Position + rootCount * 8;
                for(int i = 0; i < rootCount; i++)
                {
                    rootOffsets.Add(r.ReadInt32() + 0x20);
                    rootStrings.Add(r.ReadString((int)stringStart + r.ReadInt32()));
                }
                Offsets.AddRange(rootOffsets);

                Offsets.Sort();

                for (int i = 0; i < Offsets.Count - 1; i++)
                {
                    r.BaseStream.Position = Offsets[i];
                    byte[] data = r.ReadBytes(Offsets[i + 1] - Offsets[i]);

                    if (!offsetToOffsets.ContainsKey(Offsets[i]))
                    {
                        var relocKets = relocOffsets.Keys.ToList().FindAll(e => e >= Offsets[i] && e < Offsets[i + 1]);
                        var list = new List<int>();
                        foreach (var k in relocKets)
                            list.Add(relocOffsets[k]);
                        offsetToOffsets.Add(Offsets[i], list);
                        offsetToInnerOffsets.Add(Offsets[i], relocKets);
                    }

                    if (!offsetToData.ContainsKey(Offsets[i]))
                        offsetToData.Add(Offsets[i], data);
                }
                
                for(int i = 0; i < rootOffsets.Count; i++)
                {
                    var name = rootOffsets[i].ToString("X8");
                    if (rootStrings[i].Contains("grData"))
                        name = "GrData";
                    
                    treeView1.Nodes.Add(new DataNode(name, rootOffsets[i], offsetToData[rootOffsets[i]]));
                }
            }
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
