using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.MEX.Menus;
using HSDRawViewer.Converters;
using OpenTK;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace HSDRawViewer.GUI.MEX
{
    public class MEXStageIconEntry
    {
        public HSD_TOBJ IconTOBJ;

        public HSD_TOBJ NameTOBJ;

        public HSD_JOBJ Joint = new HSD_JOBJ() { SX = 1, SY = 1, SZ = 1, Flags = JOBJ_FLAG.CLASSICAL_SCALING };

        [Browsable(false)]
        public HSD_AnimJoint AnimJoint { get => MexMenuAnimationGenerator.GenerateAnimJoint(Animation, Joint); }
        public MexMenuAnimation Animation;

        public MEX_StageIconData IconData = new MEX_StageIconData();
        
        [Category("Data"), Description("PreviewID for name texture and mini-model")]
        public byte PreviewID { get => IconData.PreviewModelID; set => IconData.PreviewModelID = value; }

        [Category("Data"), DisplayName("External ID")]
        public int ExternalID { get => IconData.ExternalID; set => IconData.ExternalID = value; }

        [Category("Collision"), Description("Selection Width")]
        public float Width { get => IconData.CursorWidth; set => IconData.CursorWidth = value; }

        [Category("Collision"), Description("Selection Height")]
        public float Height { get => IconData.CursorHeight; set => IconData.CursorHeight = value; }

        [Category("Position"), Description("Position of Joint on X Axis")]
        public float X { get => Joint.TX; set => Joint.TX = value; }

        [Category("Position"), Description("Position of Joint on Y Axis")]
        public float Y { get => Joint.TY; set => Joint.TY = value; }

        [Category("Position"), Description("Position of Joint on Z Axis")]
        public float Z { get => Joint.TZ; set => Joint.TZ = value; }

        [Category("Data"), Description("Icon Width")]
        public float OutlineWidth
        {
            get => IconData.OutlineWidth;
            set
            {
                IconData.OutlineWidth = value;
                Joint.SX = value;
            }
        }

        [Category("Data"), Description("Icon Height")]
        public float OutlineHeight
        {
            get => IconData.OutlineHeight;
            set
            {
                IconData.OutlineHeight = value;
                Joint.SY = value;
            }
        }

        private Stack<Vector3> PositionStack = new Stack<Vector3>();

        public void PushPosition()
        {
            PositionStack.Push(new Vector3(X, Y, Z));
        }

        public void PopPosition()
        {
            if (PositionStack.Count == 0)
                return;

            var pos = PositionStack.Pop();
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }

        public RectangleF ToRectangle()
        {
            return new RectangleF(Joint.TX - Width, Joint.TY - Height, Width * 2, Height * 2);
        }

        public override string ToString()
        {
            return "Stage ID: " + ExternalID;
        }
    }
}
