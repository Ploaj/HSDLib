using HSDLib.Common;
using HSDLib.Animation;
using HSDLib.MaterialAnimation;
using HSDLib.KAR;

namespace HSDLib
{
    /// <summary>
    /// A root of the hsd structure
    /// Contains a single HSDNode and a Name
    /// The type of node depends on the file type
    /// </summary>
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
            else if (Name.StartsWith("vcDataStar") || Name.StartsWith("vcDataWing"))
            {
                Node = Reader.ReadObject<KAR_VcStarVehicle>(Offset);
            }
            else if (Name.StartsWith("vcDataWheel"))
            {
                Node = Reader.ReadObject<KAR_WheelVehicle>(Offset);
            }
            else if (Name.StartsWith("grModelMotion"))
            {
                Node = Reader.ReadObject<KAR_GrModelMotion>(Offset);
            }
            else if (Name.StartsWith("grModel"))
            {
                Node = Reader.ReadObject<KAR_GrModel>(Offset);
            }
            else if (Name.StartsWith("grData"))
            {
                Node = Reader.ReadObject<KAR_GrData>(Offset);
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
