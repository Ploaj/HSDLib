using HSDRaw.MEX;
using HSDRaw.MEX.Stages;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class MEXStageEntry
    {
        public MEX_Stage Stage = new MEX_Stage();
        public MEX_StageReverb Reverb = new MEX_StageReverb();

        [Category("0 - General"), DisplayName("Internal ID"), Description("")]
        public int InternalID { get => Stage.StageInternalID; set => Stage.StageInternalID = value; }

        [Category("0 - General"), DisplayName("File Path"), Description("")]
        public string FileName { get => Stage.StageFileName; set => Stage.StageFileName = value; }

        [Category("0 - General"), DisplayName("Unknown Stage Value"), Description("")]
        public int UnknownValue { get => Stage.UnknownValue; set => Stage.UnknownValue = value; }

        [Category("0 - General"), DisplayName("SSM"), Description(""), TypeConverter(typeof(SSMIDConverter))]
        public int SSM { get => Reverb.SSMID; set => Reverb.SSMID = (byte)value; }

        [Category("0 - General"), DisplayName("Reverb"), Description("")]
        public int ReverbValue { get => Reverb.Reverb; set => Reverb.Reverb = (byte)value; }

        [Category("0 - General"), DisplayName("Unknown Sound Data"), Description("")]
        public int Unknown { get => Reverb.Unknown; set => Reverb.Unknown = (byte)value; }

        [Category("1 - Extra"), DisplayName("Moving Collision Points"), Description("")]
        public MEX_MovingCollisionPoint[] MovingCollisions
        {
            get => Stage.MovingCollisionPoint?.Array; set
            {
                Stage.MovingCollisionPointCount = value.Length;
                if (value == null || value.Length == 0)
                {
                    Stage.MovingCollisionPoint = null;
                }
                else
                {
                    Stage.MovingCollisionPoint = new HSDRaw.HSDArrayAccessor<MEX_MovingCollisionPoint>();
                    Stage.MovingCollisionPoint.Array = value;
                }
            }
        }

        [Category("1 - Extra"), DisplayName("GOBJ Functions"), Description("")]
        public MEX_MapGOBJFunctions[] GOBJFunctions
        {
            get => Stage.GOBJFunctions?.Array; set
            {
                if (value == null || value.Length == 0)
                {
                    Stage.GOBJFunctions = null;
                }
                else
                {
                    Stage.GOBJFunctions = new HSDRaw.HSDArrayAccessor<MEX_MapGOBJFunctions>();
                    Stage.GOBJFunctions.Array = value;
                }
            }
        }

        [Category("2 - Functions"), DisplayName("OnStageInit"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnStageInit { get => Stage.OnStageInit; set => Stage.OnStageInit = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown1"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown1 { get => Stage.OnUnknown1; set => Stage.OnUnknown1 = value; }

        [Category("2 - Functions"), DisplayName("OnStageLoad"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnStageLoad { get => Stage.OnStageLoad; set => Stage.OnStageLoad = value; }

        [Category("2 - Functions"), DisplayName("OnStageGo"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnStageGo { get => Stage.OnStageGo; set => Stage.OnStageGo = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown2"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown2 { get => Stage.OnUnknown2; set => Stage.OnUnknown2 = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown3"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown3 { get => Stage.OnUnknown3; set => Stage.OnUnknown3 = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown4"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown4 { get => Stage.OnUnknown4; set => Stage.OnUnknown4 = value; }

        public override string ToString()
        {
            return FileName == null ? "" : FileName;
        }
    }
}
