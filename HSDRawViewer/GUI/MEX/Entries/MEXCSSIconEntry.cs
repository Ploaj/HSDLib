using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.MEX.Menus;
using HSDRawViewer.Converters;
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
        
        [Browsable(false)]
        public TOBJProxy[] CSPs { get; set; } = new TOBJProxy[0];
        
        public HSD_JOBJ Joint;
        public HSD_MatAnimJoint MatAnimJoint;
        
        [Browsable(false)]
        public HSD_AnimJoint AnimJoint { get => MexMenuAnimationGenerator.GenerateAnimJoint(Animation, Joint); }
        public MexMenuAnimation Animation;

        [Category("Fighter"), DisplayName("Fighter"), TypeConverter(typeof(FighterExternalIDConverter))]
        public int FighterExternalID { get => icon.ExternalCharID; set => icon.ExternalCharID = (byte)value; }

        [Category("Icon Position"), Description("X Position of Joint")]
        public float PositionX { get => Joint.TX; set { icon.X1 += value - Joint.TX; icon.X2 += value - Joint.TX; Joint.TX = value; } }

        [Category("Icon Position"), Description("Y Position of Joint")]
        public float PositionY { get => Joint.TY; set { icon.Y1 += value - Joint.TY; icon.Y2 += value - Joint.TY; Joint.TY = value; } }

        [Category("Icon Position"), Description("Z Position of Joint")]
        public float PositionZ { get => Joint.TZ; set => Joint.TZ = value; }

        [Category("Icon Position"), Description("X Scale of Joint")]
        public float ScaleX { get => Joint.SX; set => Joint.SX = value; }

        [Category("Icon Position"), Description("Y Scale of Joint")]
        public float ScaleY { get => Joint.SY; set => Joint.SY = value; }

        [Category("Icon Position"), Description("Z Scale of Joint")]
        public float ScaleZ { get => Joint.SZ; set => Joint.SZ = value; }

        [Category("Icon Collision"), Description("X Offset from Joint")]
        public float OffsetX { get => icon.X1 - PositionX; set { var w = Width; icon.X1 = value + PositionX; Width = w; } }

        [Category("Icon Collision"), Description("Y Offset from Joint")]
        public float OffsetY { get => icon.Y1 - PositionY + Height; set { var h = Height; icon.Y1 = value + PositionY - h; Height = h; } }

        [Category("Icon Collision"), Description("Width of Collision")]
        public float Width { get => icon.X2 - icon.X1; set => icon.X2 = icon.X1 + Math.Abs(value); }

        [Category("Icon Collision"), Description("Height of Collision")]
        public float Height { get => icon.Y2 - icon.Y1; set => icon.Y2 = icon.Y1 + Math.Abs(value); }
        
        private Stack<Vector3> PositionStack = new Stack<Vector3>();

        [Browsable(false)]
        public HSD_TOBJ IconTexture
        {
            get
            {
                if (Joint == null || Joint.Dobj == null || Joint.Dobj.Next == null || Joint.Dobj.Next.Mobj.Textures == null)
                    return null;

                return Joint.Dobj.Next.Mobj.Textures;
            }
            set
            {
                if (Joint == null || Joint.Dobj == null || Joint.Dobj.Next == null || Joint.Dobj.Next.Mobj.Textures == null)
                    return;

                Joint.Dobj.Next.Mobj.Textures = value;
            }
        }

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
            //AnimJoint = new HSD_AnimJoint();
            MatAnimJoint = new HSD_MatAnimJoint();
            Animation = new MexMenuAnimation();
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
            if (Joint == null || icon == null)
                return;

            var rect = ToRect();

            if (selected)
                DrawShape.DrawRectangle(rect, SelectedIconColor);
            else
                DrawShape.DrawRectangle(rect, IconColor);
        }

        public RectangleF ToRect()
        {
            return new RectangleF(icon.X1, icon.Y1, icon.X2 - icon.X1, icon.Y2 - icon.Y1);
        }

        public override string ToString()
        {
            return String.Format("{0}",
                MEXConverter.externalIDValues[FighterExternalID + 1]);
        }
    }

}
