using HSDRaw.MEX;
using HSDRaw.MEX.Menus;
using HSDRawViewer.Rendering;
using System;
using System.ComponentModel;
using System.Drawing;

namespace HSDRawViewer.GUI.MEX
{

    /// <summary>
    /// use proxy class for make selecting character id easier
    /// </summary>
    public class MEX_CSSIconEntry
    {
        [Description("Joint ID on the CSS to use for Icon Flash Animation")]
        public int JointID { get; set; }

        [DisplayName("Fighter"), TypeConverter(typeof(FighterExternalIDConverter))]
        public int FighterExternalID { get; set; }

        [Description("Starting X Coord")]
        public float X { get; set; }

        [Description("Ending X Coord")]
        public float Y { get; set; }

        [Description("Starting Y Coord")]
        public float Width { get; set; }

        [Description("Ending Y Coord")]
        public float Height { get; set; }

        public static MEX_CSSIconEntry FromIcon(MEX_CSSIcon icon)
        {
            return new MEX_CSSIconEntry()
            {
                JointID = icon.JointID,
                FighterExternalID = icon.ExternalCharID,
                X = icon.X1,
                Y = icon.Y1,
                Width = icon.X2 - icon.X1,
                Height = icon.Y1 - icon.Y2
            };
        }

        public MEX_CSSIcon ToIcon()
        {
            return new MEX_CSSIcon()
            {
                JointID = (byte)JointID,
                UnkID = (byte)JointID,
                ExternalCharID = (byte)FighterExternalID,
                CharUNKID = (byte)(FighterExternalID - (FighterExternalID > 18 ? 1 : 0)),
                X1 = X,
                Y1 = Y,
                X2 = X + Width,
                Y2 = Y - Height
            };
        }

        private static Color IconColor = Color.FromArgb(128, 255, 0, 0);

        public static Color SelectedIconColor { get; } = Color.FromArgb(128, 255, 255, 0);

        public void Render(bool selected)
        {
            if (selected)
                DrawShape.DrawRectangle(X, Y, X + Width, Y - Height, SelectedIconColor);
            else
                DrawShape.DrawRectangle(X, Y, X + Width, Y - Height, IconColor);
        }

        public RectangleF ToRect()
        {
            return new RectangleF(X, Y - Height, Width, Height);
        }

        public override string ToString()
        {
            return String.Format("{0}\t X:{1} Y:{2} W:{3} H:{4}",
                MEXConverter.externalIDValues[FighterExternalID + 1],
                X,
                Y,
                Width,
                Height);
        }
    }

}
