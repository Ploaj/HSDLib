using HSDRaw.MEX;
using HSDRawViewer.Converters;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class MEXStageIconEntry
    {
        public MexMapSpace MapSpace = new MexMapSpace();
        public MEX_StageIconData IconData = new MEX_StageIconData();
        
        [Description("PreviewID for name texture and mini-model")]
        public byte PreviewID { get => IconData.PreviewModelID; set => IconData.PreviewModelID = value; }

        [DisplayName("External ID")]
        public int ExternalID { get => IconData.ExternalID; set => IconData.ExternalID = value; }

        [Description("Selection Width")]
        public float Width { get => IconData.CursorWidth; set => IconData.CursorWidth = value; }

        [Description("Selection Height")]
        public float Height { get => IconData.CursorHeight; set => IconData.CursorHeight = value; }

        [Description("Icon Width")]
        public float OutlineWidth
        {
            get => IconData.OutlineWidth;
            set
            {
                IconData.OutlineWidth = value;
                if (CheckMapSpace())
                    MapSpace.SX = value;
            }
        }

        [Description("Icon Height")]
        public float OutlineHeight
        {
            get => IconData.OutlineHeight;
            set
            {
                IconData.OutlineHeight = value;
                if (CheckMapSpace())
                    MapSpace.SY = value;
            }
        }

        [Description("X")]
        public float X { get => CheckMapSpace() ? MapSpace.X : 0; set { if(CheckMapSpace()) MapSpace.X = value; } }

        [Description("Y")]
        public float Y { get => CheckMapSpace() ? MapSpace.Y : 0; set { if (CheckMapSpace()) MapSpace.Y = value; } }

        [Description("Motion")]
        public MexMapAnimType Motion { get => CheckMapSpace() ? MapSpace.AnimType : 0; set { if (CheckMapSpace()) MapSpace.AnimType = value; } }

        [Description("Enter Frame")]
        public int StartFrame { get => CheckMapSpace() ? MapSpace.StartFrame : 0; set { if (CheckMapSpace()) MapSpace.StartFrame = value; } }

        [Description("End Frame")]
        public int EndFrame { get => CheckMapSpace() ? MapSpace.EndFrame : 0; set { if (CheckMapSpace()) MapSpace.EndFrame = value; } }

        private bool CheckMapSpace()
        {
            if (MapSpace == null)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return "Stage ID: " + ExternalID;
        }
    }
}
