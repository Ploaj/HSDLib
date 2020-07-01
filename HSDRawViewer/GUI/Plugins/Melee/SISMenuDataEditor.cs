using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee;
using HSDRawViewer.Converters;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    /// <summary>
    /// big TODO: op codes and editing
    /// </summary>
    public partial class SISMenuDataEditor : DockContent, EditorBase
    {
        public enum TEXT_OP_CODE
        {
            END = 0x00,
            RESET = 0x01,
            UNKNOWN_02 = 0x02,
            LINE_BREAK = 0x03,
            UNKNOWN_04 = 0x04,
            UNKNOWN_05 = 0x05,
            UNKNOWN_06 = 0x06,
            OFFSET = 0x07,
            UNKNOWN_08 = 0x08,
            UNKNOWN_09 = 0x09,
            SCALING = 0x0A,
            RESET_SCALING = 0x0B,
            COLOR = 0x0C,
            CLEAR_COLOR = 0x0D,
            SET_TEXTBOX = 0x0E,
            RESET_TEXTBOX = 0x0F,
            CENTERED = 0x10,
            RESET_CENTERED = 0x11,
            LEFT_ALIGNED = 0x12,
            RESET_LEFT_ALIGN = 0x13,
            RIGHT_ALIGNED = 0x14,
            RESET_RIGHT_ALIGN = 0x15,
            KERNING = 0x16,
            NO_KERNING = 0x17,
            FITTING = 0x18,
            NO_FITTING = 0x19,
            SPACE = 0x1A,
            COMMON_CHARACTER = 0x20,
            SPECIAL_CHARACTER = 0x40
        }

        /// <summary>
        /// 
        /// </summary>
        public class MenuText
        {
            public byte[] Data
            {
                get
                {
                    return _data;
                }
                set
                {
                    _data = value;
                }
            }
            private byte[] _data = new byte[] { 0 };

            public List<Tuple<TEXT_OP_CODE, ushort[]>> OPCodes { get => DeserializeCodes(_data); }

            public static Dictionary<TEXT_OP_CODE, Tuple<string, string>> CODES = new Dictionary<TEXT_OP_CODE, Tuple<string, string>>()
            {
                { TEXT_OP_CODE.CENTERED, new Tuple<string, string>("CENTER", "") },
                { TEXT_OP_CODE.RESET_CENTERED, new Tuple<string, string>("/CENTER", "") },
                { TEXT_OP_CODE.CLEAR_COLOR, new Tuple<string, string>("/COLOR", "") },
                { TEXT_OP_CODE.COLOR, new Tuple<string, string>("COLOR", "bbb") },
                { TEXT_OP_CODE.END, new Tuple<string, string>("END", "") },
                { TEXT_OP_CODE.FITTING, new Tuple<string, string>("FIT", "") },
                { TEXT_OP_CODE.KERNING, new Tuple<string, string>("KERN", "") },
                { TEXT_OP_CODE.LEFT_ALIGNED, new Tuple<string, string>("LEFT", "") },
                { TEXT_OP_CODE.LINE_BREAK, new Tuple<string, string>("BR", "") },
                { TEXT_OP_CODE.NO_FITTING, new Tuple<string, string>("/FIT", "") },
                { TEXT_OP_CODE.NO_KERNING, new Tuple<string, string>("/KERN", "") },
                { TEXT_OP_CODE.OFFSET, new Tuple<string, string>("OFFSET", "ss") },
                { TEXT_OP_CODE.RESET, new Tuple<string, string>("RESET", "") },
                { TEXT_OP_CODE.RESET_LEFT_ALIGN, new Tuple<string, string>("/LEFT", "") },
                { TEXT_OP_CODE.RESET_RIGHT_ALIGN, new Tuple<string, string>("/RIGHT", "") },
                { TEXT_OP_CODE.RESET_SCALING, new Tuple<string, string>("/SCALE", "") },
                { TEXT_OP_CODE.RESET_TEXTBOX, new Tuple<string, string>("/TEXTBOX", "") },
                { TEXT_OP_CODE.RIGHT_ALIGNED, new Tuple<string, string>("/RIGHT", "") },
                { TEXT_OP_CODE.SCALING, new Tuple<string, string>("SCALE", "bbbb") },
                { TEXT_OP_CODE.SET_TEXTBOX, new Tuple<string, string>("TEXTBOX", "ss") },
                { TEXT_OP_CODE.UNKNOWN_02, new Tuple<string, string>("UNK02", "") },
                { TEXT_OP_CODE.UNKNOWN_04, new Tuple<string, string>("UNK04", "") },
                { TEXT_OP_CODE.UNKNOWN_05, new Tuple<string, string>("UNK05", "s") },
                { TEXT_OP_CODE.UNKNOWN_06, new Tuple<string, string>("UNK06", "ss") },
                { TEXT_OP_CODE.UNKNOWN_08, new Tuple<string, string>("UNK08", "") },
                { TEXT_OP_CODE.UNKNOWN_09, new Tuple<string, string>("UNK09", "") },
                { TEXT_OP_CODE.SPACE, new Tuple<string, string>("S", "") },
            };

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static List<Tuple<TEXT_OP_CODE, ushort[]>> DeserializeCodes(byte[] data)
            {
                List<Tuple<TEXT_OP_CODE, ushort[]>> d = new List<Tuple<TEXT_OP_CODE, ushort[]>>();

                for (int i = 0; i < data.Length;)
                {
                    var opcode = (TEXT_OP_CODE)data[i++];
                    ushort[] param = new ushort[0];

                    int textCode = (byte)opcode;

                    if ((textCode >> 4) == 2)
                        param = new ushort[] { (ushort)(((textCode << 8) | (data[i++] & 0xFF)) & 0xFFF) };
                    else
                    if ((textCode >> 4) == 4)
                        param = new ushort[] { (ushort)(((textCode << 8) | (data[i++] & 0xFF)) & 0xFFF) };
                    else
                    if (!CODES.ContainsKey(opcode))
                        throw new Exception("Opcode Not Supported " + opcode.ToString("X"));
                    else
                    {
                        var code = CODES[opcode];
                        var p = code.Item2.ToCharArray();
                        param = new ushort[p.Length];
                        for (int j = 0; j < param.Length; j++)
                        {
                            switch (p[j])
                            {
                                case 'b':
                                    param[j] = (ushort)(data[i++] & 0xFF);
                                    break;
                                case 's':
                                    param[j] = (ushort)(((data[i++] & 0xFF) << 8) | (data[i++] & 0xFF));
                                    break;
                            }
                        }
                    }

                    Tuple<TEXT_OP_CODE, ushort[]> c = new Tuple<TEXT_OP_CODE, ushort[]>(opcode, param);
                    d.Add(c);
                }

                return d;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static string DeserializeString(byte[] data)
            {
                StringBuilder sb = new StringBuilder();

                var codes = DeserializeCodes(data);

                foreach (var d in codes)
                {
                    if (CODES.ContainsKey(d.Item1))
                    {
                        var code = CODES[d.Item1];
                        sb.Append("<" + code.Item1 + (d.Item2.Length > 0 ? ", " : "") + string.Join(", ", d.Item2) + ">");
                    }
                    else
                    {
                        if (d.Item1 == TEXT_OP_CODE.SPECIAL_CHARACTER)
                            sb.Append("<" + "CHR" + ", " + d.Item2[0] + ">");

                        if (d.Item1 == TEXT_OP_CODE.COMMON_CHARACTER)
                            sb.Append(CharMAP[d.Item2[0]]);
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="str"></param>
            public bool FromString(string str)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(str, @"((?<=<).+?(?=>))|((?<=>*)([^>]+?)(?=<))");

                List<byte> newData = new List<byte>();
                
                foreach(var m in matches)
                {
                    var p = m.ToString().Split(',');

                    if (p.Length == 0)
                        continue;

                    // check if p is a code
                    var key = CODES.FirstOrDefault(x => x.Value.Item1 == p[0]);

                    if (key.Value != null)
                    {
                        if (p.Length - 1 != key.Value.Item2.Length)
                            return false;

                        newData.Add((byte)key.Key);

                        for (int j = 0; j < key.Value.Item2.Length; j++)
                        {
                            switch (key.Value.Item2[j])
                            {
                                case 'b':
                                    if (byte.TryParse(p[j + 1].Trim(), out byte res))
                                        newData.Add(res);
                                    else
                                        newData.Add(0);
                                    break;
                                case 's':
                                    if (ushort.TryParse(p[j + 1].Trim(), out ushort sht))
                                    {
                                        newData.Add((byte)(sht >> 8));
                                        newData.Add((byte)(sht & 0xFF));
                                    }
                                    else
                                    {
                                        newData.Add(0);
                                        newData.Add(0);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // process string otherwise

                        if (p.Length >= 2 && p[0] == "CHR")
                        {
                            if (ushort.TryParse(p[1].Trim(), out ushort ch))
                            {
                                ushort sht = (ushort)(((ushort)TEXT_OP_CODE.SPECIAL_CHARACTER << 8) | ch);
                                newData.Add((byte)(sht >> 8));
                                newData.Add((byte)(sht & 0xFF));
                            }
                        }
                        else
                            foreach (var chr in p[0])
                            {
                                var index = CharMAP.IndexOf(chr);
                                if (index != -1)
                                {
                                    ushort sht = (ushort)(((ushort)TEXT_OP_CODE.COMMON_CHARACTER << 8) | index);
                                    newData.Add((byte)(sht >> 8));
                                    newData.Add((byte)(sht & 0xFF));
                                }
                                else
                                    return false;
                            }
                    }
                    

                }

                _data = newData.ToArray();

                return true;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return DeserializeString(_data);
            }
        }

        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(SBM_SISData) };

        public MenuText[] MenuTexts { get; set; }

        public DataNode Node
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
                    MenuTexts = new MenuText[(sisData._s.Length / 4) - 2];
                    for (int i = 0; i < MenuTexts.Length; i++)
                    {
                        MenuTexts[i] = new MenuText();
                        MenuTexts[i].Data = sisData._s.GetReference<HSDAccessor>(i * 4 + 8)._s.GetData();
                    }

                    var image = sisData.ImageData._s.GetData();

                    var tobj = new HSD_TOBJ();
                    tobj.ImageData = new HSD_Image();
                    tobj.ImageData.Width = 32;
                    tobj.ImageData.Height = (short)(image.Length / 32);
                    tobj.ImageData.ImageData = image;
                    tobj.ImageData.Format = HSDRaw.GX.GXTexFmt.I4;

                    if (FileFont != null)
                        FileFont.Dispose();

                    FileFont = TOBJConverter.ToBitmap(tobj);
                    pictureBox1.Image = FileFont;
                    FileSpacing = sisData.CharacterSpacingParams._s.GetData();
                }

                arrayMemberEditor1.SetArrayFromProperty(this, "MenuTexts");
            }
        }
        private DataNode _node;

        private Bitmap FileFont;
        private byte[] FileSpacing;

        public static int SpaceConstant = 0;

        private Brush BackBrush = new SolidBrush(Color.Black);

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

            var xoff = 0;
            var yoff = 0;
            var font = Properties.Resources.melee_font;

            bool kern = false;
            Color colr = Color.White;

            if (arrayMemberEditor1.SelectedObject is MenuText text)
            {
                foreach (var o in text.OPCodes)
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
                        var before = 0;
                        var after = 0;

                        if (kern && o.Item2[0] * 2 + 1 < FileSpacing.Length)
                        {
                            before = Spacing[o.Item2[0] * 2];
                            after = Spacing[o.Item2[0] * 2 + 1];
                        }

                        var dest = new Rectangle(xoff, yoff, 32 - after - before, 32);
                        var src = new Rectangle(o.Item2[0] * 32 + before, 0, 32 - after - before, 32);

                        using (var c = GetColoredCharacter(font, src, colr))
                            if(c != null)
                                e.Graphics.DrawImage(c, dest);

                        xoff += 32 - before - after + SpaceConstant;
                    }

                    if (o.Item1 == TEXT_OP_CODE.SPECIAL_CHARACTER)
                    {
                        var before = 0;
                        var after = 0;
                        if (kern && o.Item2[0] * 2 + 1 < FileSpacing.Length)
                        {
                            before = FileSpacing[o.Item2[0] * 2];
                            after = FileSpacing[o.Item2[0] * 2 + 1];
                        }

                        var dest = new Rectangle(xoff, yoff, 32 - after - before, 32);
                        var src = new Rectangle(0, o.Item2[0] * 32 + before, 32, 32 - after - before);

                        using (var c = GetColoredCharacter(FileFont, src, colr))
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
                    return BitmapTools.Multiply(croppedImage, colr.R, colr.G, colr.B);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrayMemberEditor1_SelectedObjectChanged(object sender, EventArgs e)
        {
            if(arrayMemberEditor1.SelectedObject is MenuText text)
                textBox1.Text = text.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(arrayMemberEditor1.SelectedObject is MenuText text)
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
                for(int i = 0; i < MenuTexts.Length; i++)
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
            using(HelpBox b = new HelpBox(HelpText))
                b.ShowDialog();
        }

        private static string HelpText = @"Text Editing Help

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
        private static int[] Spacing = new int[] { 09, 0x08, 0x09, 0x0C, 0x09, 0x08, 0x08, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x09, 0x08, 0x04, 0x03, 0x06, 0x05, 0x04, 0x04, 0x05, 0x03, 0x08, 0x06, 0x08, 0x06, 0x04, 0x03, 0x05, 0x03, 0x0D, 0x0B, 0x07, 0x06, 0x06, 0x04, 0x08, 0x06, 0x03, 0x01, 0x04, 0x03, 0x04, 0x03, 0x06, 0x04, 0x04, 0x02, 0x06, 0x04, 0x05, 0x04, 0x06, 0x05, 0x05, 0x03, 0x04, 0x03, 0x01, 0x00, 0x05, 0x04, 0x04, 0x03, 0x05, 0x04, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x08, 0x09, 0x07, 0x06, 0x08, 0x07, 0x0A, 0x0A, 0x09, 0x0C, 0x08, 0x06, 0x0C, 0x0C, 0x01, 0x00, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x07, 0x06, 0x0A, 0x09, 0x08, 0x07, 0x08, 0x09, 0x07, 0x05, 0x07, 0x06, 0x02, 0x01, 0x07, 0x06, 0x06, 0x06, 0x07, 0x06, 0x05, 0x05, 0x03, 0x02, 0x05, 0x05, 0x03, 0x02, 0x06, 0x06, 0x04, 0x04, 0x06, 0x05, 0x03, 0x02, 0x05, 0x03, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x04, 0x04, 0x00, 0x04, 0x05, 0x04, 0x03, 0x02, 0x02, 0x02, 0x00, 0x04, 0x04, 0x03, 0x00, 0x04, 0x03, 0x03, 0x00, 0x05, 0x03, 0x05, 0x03, 0x02, 0x02, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x02, 0x02, 0x01, 0x00, 0x02, 0x02, 0x02, 0x00, 0x03, 0x03, 0x03, 0x00, 0x04, 0x05, 0x01, 0x02, 0x01, 0x00, 0x02, 0x02, 0x02, 0x00, 0x04, 0x04, 0x03, 0x00, 0x01, 0x01, 0x03, 0x02, 0x01, 0x00, 0x00, 0x00, 0x01, 0x01, 0x02, 0x01, 0x02, 0x00, 0x02, 0x00, 0x02, 0x01, 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x01, 0x01, 0x00, 0x01, 0x00, 0x03, 0x03, 0x01, 0x01, 0x02, 0x01, 0x02, 0x02, 0x03, 0x02, 0x04, 0x04, 0x01, 0x01, 0x05, 0x05, 0x02, 0x02, 0x05, 0x05, 0x02, 0x02, 0x03, 0x03, 0x05, 0x05, 0x02, 0x02, 0x01, 0x00, 0x02, 0x03, 0x04, 0x04, 0x01, 0x01, 0x02, 0x03, 0x01, 0x01, 0x05, 0x04, 0x03, 0x01, 0x04, 0x06, 0x01, 0x03, 0x06, 0x05, 0x03, 0x02, 0x05, 0x04, 0x02, 0x02, 0x04, 0x05, 0x02, 0x01, 0x02, 0x02, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x03, 0x02, 0x02, 0x00, 0x01, 0x01, 0x01, 0x00, 0x03, 0x04, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x02, 0x01, 0x02, 0x01, 0x02, 0x01, 0x02, 0x00, 0x02, 0x02, 0x02, 0x00, 0x02, 0x03, 0x01, 0x00, 0x03, 0x02, 0x02, 0x00, 0x02, 0x02, 0x02, 0x00, 0x05, 0x05, 0x02, 0x02, 0x01, 0x00, 0x02, 0x02, 0x01, 0x00, 0x09, 0x03, 0x09, 0x02, 0x02, 0x02, 0x01, 0x01, 0x02, 0x02, 0x03, 0x02, 0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x04, 0x02, 0x04, 0x03, 0x03, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x04, 0x04, 0x01, 0x01, 0x02, 0x03, 0x02, 0x02, 0x04, 0x04, 0x02, 0x01, 0x05, 0x05, 0x02, 0x02, 0x06, 0x06, 0x03, 0x04, 0x03, 0x03, 0x05, 0x05, 0x01, 0x01, 0x05, 0x03, 0x03, 0x03, 0x06, 0x06, 0x03, 0x03, 0x04, 0x03, 0x02, 0x02, 0x03, 0x00, 0x04, 0x05, 0x04, 0x00, 0x08, 0x08, 0x02, 0x12, 0x02, 0x13, 0x0C, 0x0D, 0x0C, 0x0D, 0x0B, 0x0A, 0x0D, 0x0C, 0x0D, 0x0C, 0x07, 0x06, 0x0D, 0x0C, 0x0A, 0x08, 0x00, 0x00, 0x02, 0x01, 0x01, 0x00, 0x01, 0x01, 0x0F, 0x0F, 0x02, 0x17, 0x01, 0x10, 0x15, 0x00, 0x01, 0x14, 0x15, 0x00, 0x01, 0x13, 0x13, 0x00, 0x01, 0x12, 0x04, 0x03, 0x04, 0x03, 0x05, 0x04, 0x04, 0x03, 0x01, 0x01, 0x01, 0x00, 0x04, 0x02, 0x05, 0x05, 0x01, 0x00, 0x04, 0x03, 0x03, 0x02, 0x08, 0x06, 0x03, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00 };

        /// <summary>
        /// 
        /// </summary>
        public static List<char> CharMAP = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ぁ', 'あ', 'ぃ', 'い', 'ぅ', 'う', 'ぇ', 'え', 'ぉ', 'お', 'か', 'が', 'き', 'ぎ', 'く', 'ぐ', 'け', 'げ', 'こ', 'ご', 'さ', 'ざ', 'し', 'じ', 'す', 'ず', 'せ', 'ぜ', 'そ', 'ぞ', 'た', 'だ', 'ち', 'ぢ', 'っ', 'つ', 'づ', 'て', 'で', 'と', 'ど', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ば', 'ぱ', 'ひ', 'び', 'ぴ', 'ふ', 'ぶ', 'ぷ', 'へ', 'べ', 'ぺ', 'ほ', 'ぼ', 'ぽ', 'ま', 'み', 'む', 'め', 'も', 'ゃ', 'や', 'ゅ', 'ゆ', 'ょ', 'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'ゎ', 'わ', 'を', 'ん', 'ァ', 'ア', 'ィ', 'イ', 'ゥ', 'ウ', 'ェ', 'エ', 'ォ', 'オ', 'カ', 'ガ', 'キ', 'ギ', 'ク', 'グ', 'ケ', 'ゲ', 'コ', 'ゴ', 'サ', 'ザ', 'シ', 'ジ', 'ス', 'ズ', 'セ', 'ゼ', 'ソ', 'ゾ', 'タ', 'ダ', 'チ', 'ヂ', 'ッ', 'ツ', 'ヅ', 'テ', 'デ', 'ト', 'ド', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'バ', 'パ', 'ヒ', 'ビ', 'ピ', 'フ', 'ブ', 'プ', 'ヘ', 'ベ', 'ペ', 'ホ', 'ボ', 'ポ', 'マ', 'ミ', 'ム', 'メ', 'モ', 'ャ', 'ヤ', 'ュ', 'ユ', 'ョ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ヮ', 'ワ', 'ヲ', 'ン', 'ヴ', 'ヵ', 'ヶ', '　', '、', '。', ',', '.', '•', ',', ';', '?', '!', '^', '_', '—', '/', '~', '|', '\'', '"', '(', ')', '[', ']', '{', '}', '+', '-', '×', '=', '<', '>', '¥', '$', '%', '#', '&', '*', '@', '扱', '押', '軍', '源', '個', '込', '指', '示', '取', '書', '詳', '人', '生', '説', '体', '団', '電', '読', '発', '抜', '閑', '本', '明' };

    }
}
