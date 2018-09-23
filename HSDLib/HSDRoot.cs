using HSDLib.Common;
using HSDLib.Animation;
using HSDLib.MaterialAnimation;
using HSDLib.KAR;

namespace HSDLib
{
    public class HSDRoot : IHSDNode
    {
        public string Name;
        public IHSDNode Node;

        public uint Offset;
        public uint NameOffset;

        public override void Open(HSDReader Reader)
        {
            // Assuming Node Type from the root name
            if (Name.EndsWith("matanim_joint"))
            {
                Node = Reader.ReadObject<HSD_MatAnimJoint>(Offset);
            }
            else if (Name.EndsWith("_joint"))
            {
                Node = Reader.ReadObject<HSD_JOBJ>(Offset);
            }
            else if (Name.EndsWith("_figatree"))
            {
                Node = Reader.ReadObject<HSD_FigaTree>(Offset);
            }
            else if (Name.StartsWith("vcDataStar"))
            {
                Node = Reader.ReadObject<KAR_Vehicle>(Offset);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            // a little wonky to have to go through the saving process 3 times
            // but this does result in smaller filesizes due to data alignment
            Writer.Mode = WriterWriteMode.BUFFER;
            Node.Save(Writer);
            Writer.Mode = WriterWriteMode.NORMAL;
            Node.Save(Writer);
            Writer.Mode = WriterWriteMode.TEXTURE;
            Node.Save(Writer);
            Writer.Mode = WriterWriteMode.NORMAL;
        }
    }
}
