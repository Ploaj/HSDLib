using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee;
using HSDRaw.Tools.Melee;
using HSDRawViewer.GUI.Dialog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    /// <summary>
    /// big TODO: op codes and editing
    /// </summary>
    [SupportedTypes(new Type[] { typeof(SBM_SISData) })]
    public partial class SISMenuDataEditor : PluginBase
    {
        public MeleeMenuText[] MenuTexts { get; set; }

        public override DataNode Node
        {
            get
            {
                return _node;
            }
            set
            {
                _node = value;

                if (value.Accessor is SBM_SISData sisData)
                {
                    MenuTexts = new MeleeMenuText[(sisData._s.Length / 4) - 2];
                    for (int i = 0; i < MenuTexts.Length; i++)
                    {
                        MenuTexts[i] = new MeleeMenuText();
                        HSDAccessor r = sisData._s.GetReference<HSDAccessor>(i * 4 + 8);
                        if (r != null)
                            MenuTexts[i].Data = r._s.GetData();
                    }

                    if (sisData.ImageData != null)
                    {
                        byte[] image = sisData.ImageData._s.GetData();

                        HSD_TOBJ tobj = new();
                        tobj.ImageData = new HSD_Image();
                        tobj.ImageData.Width = 32;
                        tobj.ImageData.Height = (short)(image.Length / 32);
                        tobj.ImageData.ImageData = image;
                        tobj.ImageData.Format = HSDRaw.GX.GXTexFmt.I4;

                        if (FileFont != null)
                            FileFont.Dispose();

                        FileFont = tobj.ToImage().ToBitmap();
                        fontTable.Image = FileFont;
                        FileSpacing = sisData.CharacterSpacingParams._s.GetData();
                    }
                    else
                    {
                        FileSpacing = new byte[0];
                    }
                }

                arrayMemberEditor1.SetArrayFromProperty(this, nameof(MenuTexts));
            }
        }
        private DataNode _node;

        private Bitmap FileFont;
        private byte[] FileSpacing;

        public static int SpaceConstant = 0;

        private readonly Brush BackBrush = new SolidBrush(Color.Black);

        /// <summary>
        /// 
        /// </summary>
        public SISMenuDataEditor()
        {
            InitializeComponent();

            FormClosed += (sender, args) =>
            {
                if (FileFont != null)
                    FileFont.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(BackBrush, e.ClipRectangle);

            int xoff = 0;
            int yoff = 0;
            Bitmap font = Properties.Resources.melee_font;

            bool kern = false;
            Color colr = Color.White;

            if (arrayMemberEditor1.SelectedObject is MeleeMenuText text)
            {
                foreach (Tuple<TEXT_OP_CODE, short[]> o in text.OPCodes)
                {
                    Console.WriteLine(o.Item1);
                    if (o.Item1 == TEXT_OP_CODE.LINE_BREAK)
                    {
                        xoff = 0;
                        yoff += 32;
                    }

                    if (o.Item1 == TEXT_OP_CODE.CLEAR_COLOR)
                        colr = Color.White;

                    if (o.Item1 == TEXT_OP_CODE.COLOR)
                        colr = Color.FromArgb(o.Item2[0], o.Item2[1], o.Item2[2]);

                    if (o.Item1 == TEXT_OP_CODE.SPACE)
                        xoff += 16;

                    if (o.Item1 == TEXT_OP_CODE.KERNING)
                        kern = true;

                    if (o.Item1 == TEXT_OP_CODE.NO_KERNING)
                        kern = false;

                    if (o.Item1 == TEXT_OP_CODE.COMMON_CHARACTER)
                    {
                        int before = 0;
                        int after = 0;

                        if (kern && o.Item2[0] * 2 + 1 < Spacing.Length)
                        {
                            before = Spacing[o.Item2[0] * 2];
                            after = Spacing[o.Item2[0] * 2 + 1];
                        }

                        Rectangle dest = new(xoff, yoff, 32 - after - before, 32);
                        Rectangle src = new(o.Item2[0] * 32 + before, 0, 32 - after - before, 32);

                        using (Bitmap c = GetColoredCharacter(font, src, colr))
                            if (c != null)
                                e.Graphics.DrawImage(c, dest);

                        xoff += 32 - before - after + SpaceConstant;
                    }

                    if (o.Item1 == TEXT_OP_CODE.SPECIAL_CHARACTER)
                    {
                        int before = 0;
                        int after = 0;
                        if (kern && o.Item2[0] * 2 + 1 < FileSpacing.Length)
                        {
                            before = FileSpacing[o.Item2[0] * 2];
                            after = FileSpacing[o.Item2[0] * 2 + 1];
                        }

                        Rectangle dest = new(xoff, yoff, 32 - after - before, 32);
                        Rectangle src = new(0, o.Item2[0] * 32 + before, 32, 32 - after - before);

                        using (Bitmap c = GetColoredCharacter(FileFont, src, colr))
                            if (c != null)
                                e.Graphics.DrawImage(c, dest);

                        xoff += 32 - before - after + SpaceConstant;
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="src"></param>
        /// <param name="colr"></param>
        /// <returns></returns>
        private static Bitmap GetColoredCharacter(Bitmap src, Rectangle rect, Color colr)
        {
            if (rect.X < src.Width && rect.Y < src.Height && rect.X + rect.Width <= src.Width && rect.Y + rect.Height <= src.Height)
                using (Bitmap croppedImage = src.Clone(rect, PixelFormat.Format32bppArgb))
                    return Multiply(croppedImage, colr.R, colr.G, colr.B);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Bitmap Multiply(Bitmap bitmap, byte r, byte g, byte b, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            Rectangle size = new(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(size, ImageLockMode.ReadOnly, format);

            byte[] buffer = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, buffer.Length);
            bitmap.UnlockBits(bitmapData);

            byte Calc(byte c1, byte c2)
            {
                double cr = c1 / 255d * c2 / 255d * 255d;
                return (byte)(cr > 255 ? 255 : cr);
            }

            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = Calc(buffer[i], b);
                buffer[i + 1] = Calc(buffer[i + 1], g);
                buffer[i + 2] = Calc(buffer[i + 2], r);
            }

            Bitmap result = new(bitmap.Width, bitmap.Height);
            BitmapData resultData = result.LockBits(size, ImageLockMode.WriteOnly, format);

            Marshal.Copy(buffer, 0, resultData.Scan0, buffer.Length);
            result.UnlockBits(resultData);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrayMemberEditor1_SelectedObjectChanged(object sender, EventArgs e)
        {
            if (arrayMemberEditor1.SelectedObject is MeleeMenuText text)
                textBox1.Text = text.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (arrayMemberEditor1.SelectedObject is MeleeMenuText text)
                if (text.FromString(textBox1.Text))
                {
                    textBox1.BackColor = System.Drawing.SystemColors.Control;
                    panel1.Invalidate();
                }
                else
                {
                    textBox1.BackColor = Color.Red;
                }
        }

        /// <summary>
        /// Save Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (_node.Accessor is SBM_SISData sisData)
            {
                sisData._s.Resize(8 + MenuTexts.Length * 4);
                for (int i = 0; i < MenuTexts.Length; i++)
                    sisData._s.SetReferenceStruct(8 + i * 4, new HSDStruct(MenuTexts[i].Data));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            using HelpBox b = new(HelpText);
            b.ShowDialog();
        }

        private static readonly string HelpText = @"Text Editing Help

Common letters can be used like normal
e.x: <KERN>Hello World<END>

Special characters can be used by
<CHR, short> The short is the id of the text graphic in this file
See the Extra Characters tab for more info

<S> Space

<BR> Line break

<CENTER> Following Text is Centered
<RIGHT>  Following Text is Right Aligned
<LEFT>   Following Text is Left Aligned

All of these commands do the same thing
Resets text alignment:
</CENTER></RIGHT></LEFT>

<COLOR, R, G, B> Sets the text color ex. <COLOR, 255, 0, 0> is red
</COLOR> Resets color to white

<TEXTBOX, xoff, width, yscale, height> Sets up text box area
</TEXTBOX> Resets textbox area

<KERN> Enables kerning
</KERN> Disabled kerning

<SCALE, byte, byte, byte, byte> Unknown scaling params e.x: <SCALE, 0, 1, 0, 1> 
</SCALE> Resets scaling

<FIT>  Enables fitting so that text is to be within <TEXTBOX>
</FIT> Disabled Fitting

<OFFSET, short, short> Something to do with text offseting e.x: <OFFSET, 120, 120>

<RESET> Resets all parameters

<END> Ends text rendering

The following are unknown. If you uncover their functions please let me know

<UNK02>
<UNK04>
<UNK05, short>
<UNK06, short, short>
<UNK08>
<UNK09>";



        /// <summary>
        /// 
        /// </summary>
        private static readonly int[] Spacing = new int[] { 09, 0x08, 0x09, 0x0C, 0x09, 0x08, 0x08, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x04, 0x03, 0x06, 0x05, 0x04, 0x04, 0x05, 0x03, 0x08, 0x06, 0x08, 0x06, 0x04, 0x03, 0x05, 0x03, 0x0D, 0x0B, 0x07, 0x06, 0x06, 0x04, 0x08, 0x06, 0x03, 0x01, 0x04, 0x03, 0x04, 0x03, 0x06, 0x04, 0x04, 0x02, 0x06, 0x04, 0x05, 0x04, 0x06, 0x05, 0x05, 0x03, 0x04, 0x03, 0x01, 0x00, 0x05, 0x04, 0x04, 0x03, 0x05, 0x04, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x08, 0x09, 0x07, 0x06, 0x08, 0x07, 0x0A, 0x0A, 0x09, 0x0C, 0x08, 0x06, 0x0C, 0x0C, 0x01, 0x00, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x0A, 0x09, 0x08, 0x07, 0x08, 0x09, 0x07, 0x05, 0x07, 0x06, 0x02, 0x01, 0x07, 0x06, 0x06, 0x06, 0x07, 0x06, 0x05, 0x05, 0x03, 0x02, 0x05, 0x05, 0x03, 0x02, 0x06, 0x06, 0x04, 0x04, 0x06, 0x05, 0x03, 0x02, 0x05, 0x03, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x04, 0x04, 0x00, 0x04, 0x05, 0x04, 0x03, 0x02, 0x02, 0x02, 0x00, 0x04, 0x04, 0x03, 0x00, 0x04, 0x03, 0x03, 0x00, 0x05, 0x03, 0x05, 0x03, 0x02, 0x02, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x02, 0x02, 0x01, 0x00, 0x02, 0x02, 0x02, 0x00, 0x03, 0x03, 0x03, 0x00, 0x04, 0x05, 0x01, 0x02, 0x01, 0x00, 0x02, 0x02, 0x02, 0x00, 0x04, 0x04, 0x03, 0x00, 0x01, 0x01, 0x03, 0x02, 0x01, 0x00, 0x00, 0x00, 0x01, 0x01, 0x02, 0x01, 0x02, 0x00, 0x02, 0x00, 0x02, 0x01, 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x01, 0x01, 0x00, 0x01, 0x00, 0x03, 0x03, 0x01, 0x01, 0x02, 0x01, 0x02, 0x02, 0x03, 0x02, 0x04, 0x04, 0x01, 0x01, 0x05, 0x05, 0x02, 0x02, 0x05, 0x05, 0x02, 0x02, 0x03, 0x03, 0x05, 0x05, 0x02, 0x02, 0x01, 0x00, 0x02, 0x03, 0x04, 0x04, 0x01, 0x01, 0x02, 0x03, 0x01, 0x01, 0x05, 0x04, 0x03, 0x01, 0x04, 0x06, 0x01, 0x03, 0x06, 0x05, 0x03, 0x02, 0x05, 0x04, 0x02, 0x02, 0x04, 0x05, 0x02, 0x01, 0x02, 0x02, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x03, 0x02, 0x02, 0x00, 0x01, 0x01, 0x01, 0x00, 0x03, 0x04, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x02, 0x01, 0x02, 0x01, 0x02, 0x01, 0x02, 0x00, 0x02, 0x02, 0x02, 0x00, 0x02, 0x03, 0x01, 0x00, 0x03, 0x02, 0x02, 0x00, 0x02, 0x02, 0x02, 0x00, 0x05, 0x05, 0x02, 0x02, 0x01, 0x00, 0x02, 0x02, 0x01, 0x00, 0x09, 0x03, 0x09, 0x02, 0x02, 0x02, 0x01, 0x01, 0x02, 0x02, 0x03, 0x02, 0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x04, 0x02, 0x04, 0x03, 0x03, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x04, 0x04, 0x01, 0x01, 0x02, 0x03, 0x02, 0x02, 0x04, 0x04, 0x02, 0x01, 0x05, 0x05, 0x02, 0x02, 0x06, 0x06, 0x03, 0x04, 0x03, 0x03, 0x05, 0x05, 0x01, 0x01, 0x05, 0x03, 0x03, 0x03, 0x06, 0x06, 0x03, 0x03, 0x04, 0x03, 0x02, 0x02, 0x03, 0x00, 0x04, 0x05, 0x04, 0x00, 0x08, 0x08, 0x02, 0x12, 0x02, 0x13, 0x0C, 0x0D, 0x0C, 0x0D, 0x0B, 0x0A, 0x0D, 0x0C, 0x0D, 0x0C, 0x07, 0x06, 0x0D, 0x0C, 0x0A, 0x08, 0x00, 0x00, 0x02, 0x01, 0x01, 0x00, 0x01, 0x01, 0x0F, 0x0F, 0x02, 0x17, 0x01, 0x10, 0x15, 0x00, 0x01, 0x14, 0x15, 0x00, 0x01, 0x13, 0x13, 0x00, 0x01, 0x12, 0x04, 0x03, 0x04, 0x03, 0x05, 0x04, 0x04, 0x03, 0x01, 0x01, 0x01, 0x00, 0x04, 0x02, 0x05, 0x05, 0x01, 0x00, 0x04, 0x03, 0x03, 0x02, 0x08, 0x06, 0x03, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00 };

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
