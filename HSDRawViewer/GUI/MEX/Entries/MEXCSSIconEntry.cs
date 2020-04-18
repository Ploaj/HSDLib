using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.MEX.Menus;
using HSDRawViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace HSDRawViewer.GUI.MEX
{

    /// <summary>
    /// use proxy class for make selecting character id easier
    /// </summary>
    public class MEX_CSSIconEntry
    {
        public MEX_CSSIcon icon;

        public List<HSD_TOBJ> CSPs = new List<HSD_TOBJ>();

        public HSD_JOBJ Joint;
        public HSD_AnimJoint AnimJoint;
        public HSD_MatAnimJoint MatAnimJoint;

        [Category("Fighter"), DisplayName("Fighter"), TypeConverter(typeof(FighterExternalIDConverter))]
        public int FighterExternalID { get => icon.ExternalCharID; set => icon.ExternalCharID = (byte)value; }

        [Category("Icon Position"), Description("X Position of Joint")]
        public float PositionX { get => Joint.TX; set => Joint.TX = value; }

        [Category("Icon Position"), Description("Y Position of Joint")]
        public float PositionY { get => Joint.TY; set => Joint.TY = value; }

        [Category("Icon Position"), Description("Z Position of Joint")]
        public float PositionZ { get => Joint.TZ; set => Joint.TZ = value; }

        [Category("Icon Position"), Description("X Scale of Joint")]
        public float ScaleX { get => Joint.SX; set => Joint.SX = value; }

        [Category("Icon Position"), Description("Y Scale of Joint")]
        public float ScaleY { get => Joint.SY; set => Joint.SY = value; }

        [Category("Icon Position"), Description("Z Scale of Joint")]
        public float ScaleZ { get => Joint.SZ; set => Joint.SZ = value; }

        [Category("Icon Collision"), Description("X Offset from Center of Icon")]
        public float OffsetX { get => icon.X1; set => icon.X1 = value; }

        [Category("Icon Collision"), Description("Y Offset from Center of Icon")]
        public float OffsetY { get => icon.Y1; set => icon.Y1 = value; }

        [Category("Icon Collision"), Description("Width of Collision")]
        public float Width { get => icon.X2; set => icon.X2 = value; }

        [Category("Icon Collision"), Description("Height of Collision")]
        public float Height { get => icon.Y2; set => icon.Y2 = value; }
        
        private Stack<Vector3> PositionStack = new Stack<Vector3>();

        public void PushPosition()
        {
            PositionStack.Push(new Vector3(PositionX, PositionY, PositionZ));
        }

        public void PopPosition()
        {
            if (PositionStack.Count == 0)
                return;

            var pos = PositionStack.Pop();
            PositionX = pos.X;
            PositionY = pos.Y;
            PositionZ = pos.Z;
        }

        public MEX_CSSIconEntry()
        {
            icon = new MEX_CSSIcon();
            Joint = new HSD_JOBJ() { Flags = JOBJ_FLAG.XLU | JOBJ_FLAG.CLASSICAL_SCALING, SX = 1, SY = 1, SZ = 1 };
            AnimJoint = new HSD_AnimJoint();
            MatAnimJoint = new HSD_MatAnimJoint();
        }

        public static MEX_CSSIconEntry FromIcon(MEX_CSSIcon icon)
        {
            return new MEX_CSSIconEntry()
            {
                icon = icon
            };
        }

        public MEX_CSSIcon ToIcon()
        {
            return icon;
        }

        private static Color IconColor = Color.FromArgb(128, 255, 0, 0);

        public static Color SelectedIconColor { get; } = Color.FromArgb(128, 255, 255, 0);

        public void Render(bool selected)
        {
            if (Joint == null || icon == null || icon.MEXICON == 0)
                return;

            var rect = ToRect();

            if (selected)
                DrawShape.DrawRectangle(rect, SelectedIconColor);
            else
                DrawShape.DrawRectangle(rect, IconColor);
        }

        public RectangleF ToRect()
        {
            return new RectangleF(Joint.TX + OffsetX, Joint.TY + OffsetY, Width, Height);
        }

        public override string ToString()
        {
            return String.Format("{0}",
                MEXConverter.externalIDValues[FighterExternalID + 1]);
        }
    }

}
