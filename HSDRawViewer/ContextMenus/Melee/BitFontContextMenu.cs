using HSDRaw.MEX.Akaneia;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HSDRawViewer.ContextMenus.Melee
{
    internal class BitFontContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(AK_BitFont) };

        public BitFontContextMenu() : base()
        {
            ToolStripMenuItem import = new ToolStripMenuItem("Import From PNG");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_BitFont bitfont)
                {
                    var f = FileIO.OpenFile("PNG (*.png)|*.png", "");

                    if (f != null)
                    {
                        bitfont.Clear();
                        using (Image<Rgba32> bmp = SixLabors.ImageSharp.Image.Load<Rgba32>(f))
                        {
                            char[] chars = new char[]
                            {
                                'a',
                                'b',
                                'c',
                                'd',
                                'e',
                                'f',
                                'g',
                                'h',
                                'i',
                                'j',
                                'k',
                                'l',
                                'm',
                                'n',
                                'o',

                                'p',
                                'q',
                                'r',
                                's',
                                't',
                                'u',
                                'v',
                                'w',
                                'x',
                                'y',
                                'z',
                                ' ',
                                (char)0,
                                (char)0,
                                (char)0,

                                '0',
                                '1',
                                '2',
                                '3',
                                '4',
                                '5',
                                '6',
                                '7',
                                '8',
                                '9',
                                ':',
                                '!',
                            };

                            int char_index = 0;
                            for (int y = 0; y < bmp.Height; y += 7)
                            {
                                for (int x = 0; x < bmp.Width; x += 6)
                                {
                                    if (char_index >= chars.Length)
                                        break;

                                    AK_BitFontChar c = new AK_BitFontChar();
                                    c.Character = chars[char_index];
                                    c.Spacing = 1;
                                    c.Kerning = 0;

                                    byte[] pixel_data = new byte[8 * 8];

                                    System.Diagnostics.Debug.WriteLine(chars[char_index]);
                                    for (int y1 = 0; y1 < 7; y1 ++)
                                    {
                                        System.Diagnostics.Debug.WriteLine("");
                                        for (int x1 = 0; x1 < 6; x1++)
                                        {
                                            var pix = bmp[x + x1, y + y1];

                                            int index = x1 + y1 * 8;

                                            if (pix.R == 0 && pix.G == 0 && pix.B == 0)
                                            {
                                                pixel_data[index] = 1;
                                                System.Diagnostics.Debug.Write("0");
                                                c.Kerning = (byte)Math.Max(c.Kerning, x1 + 1);
                                            }
                                            else
                                            {
                                                System.Diagnostics.Debug.Write(" ");
                                            }
                                        }
                                    }

                                    c.Data = pixel_data;

                                    bitfont.Add(c);

                                    char_index++;
                                }
                            }
                        }
                    }
                }
            };
            Items.Add(import);
        }
    }
}
