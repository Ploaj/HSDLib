using System.Windows.Forms;

namespace HSDRawViewer
{
    public class DataNode : TreeNode
    {
        public int Offset { get; set; }
        public byte[] Data { get; set; }

        public DataNode(string Text, int offset, byte[] data)
        {
            this.Text = Text;
            this.Offset = offset;
            this.Data = data;
            Nodes.Add(new TreeNode()); // dummy
        }
    }
}
