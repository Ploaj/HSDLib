using HSDRaw.Common;
using HSDRawViewer.Converters;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class MOBJEditor : Form
    {
        public class TextureContainer : ListViewItem
        {
            public HSD_TOBJ TOBJ;

            public TextureContainer(HSD_TOBJ t)
            {
                TOBJ = t;
                Text = t.Flags.ToString();
            }
        }

        private HSD_MOBJ _mobj;

        private ImageList TextureList = new ImageList();

        public MOBJEditor()
        {
            InitializeComponent();

            CenterToScreen();

            listTexture.SmallImageList = TextureList;
            listTexture.LargeImageList = TextureList;

            TextureList.ImageSize = new Size(64, 64);
            TextureList.ColorDepth = ColorDepth.Depth32Bit;

            FormClosed += (sender, args) =>
            {
                foreach(Image v in TextureList.Images)
                {
                    v.Dispose();
                }
            };
        }

        public void SetMOBJ(HSD_MOBJ mobj)
        {
            _mobj = mobj;

            // load textures
            if(mobj.Textures != null)
            {
                var tex = mobj.Textures.List;

                int index = 0;
                foreach(var t in tex)
                {
                    var bmp = TOBJConverter.RgbaToImage(t.GetDecodedImageData(), t.ImageData.Width, t.ImageData.Height);

                    TextureList.Images.Add(bmp);

                    listTexture.Items.Add(new TextureContainer(t)
                    {
                        ImageIndex = index++,
                    });
                }
                listTexture.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            // load material color
            var mc = mobj.MaterialColor;
            buttonAmbient.BackColor = Color.FromArgb(mc.AMB_A, mc.AMB_R, mc.AMB_G, mc.AMB_B);
            buttonDiffuse.BackColor = Color.FromArgb(mc.DIF_A, mc.DIF_R, mc.DIF_G, mc.DIF_B);
            buttonSpecular.BackColor = Color.FromArgb(mc.SPC_A, mc.SPC_R, mc.SPC_G, mc.SPC_B);
            tbShine.Text = mc.Shininess.ToString();
            tbAlpha.Text = mc.Alpha.ToString();

            // load pixel processing
            if(mobj.PixelProcessing != null)
            {
                cbEnablePP.Checked = true;
            }
        }

        private void cbEnablePP_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnablePP.Checked)
            {
                if(_mobj.PixelProcessing == null)
                {
                    _mobj.PixelProcessing = new HSD_PixelProcessing()
                    {
                        SrcFactor = HSDRaw.GX.GXBlendFactor.GX_BL_SRCALPHA,
                        DstFactor = HSDRaw.GX.GXBlendFactor.GX_BL_INVSRCCLR
                    };

                }
                propertyPixel.SelectedObject = _mobj.PixelProcessing;
                propertyPixel.Enabled = true;
            }
            else
            {
                _mobj.PixelProcessing = null;
                propertyPixel.SelectedObject = null;
                propertyPixel.Enabled = false;
            }
        }

        private void buttonAmbient_Click(object sender, EventArgs e)
        {
            var col = GetColor(buttonAmbient.BackColor);

            _mobj.MaterialColor.AmbientColorABGR = ColorToABGR(col);

            buttonAmbient.BackColor = col;
        }

        private void buttonDiffuse_Click(object sender, EventArgs e)
        {
            var col = GetColor(buttonDiffuse.BackColor);

            _mobj.MaterialColor.DiffuseColorABGR = ColorToABGR(col);

            buttonDiffuse.BackColor = col;
        }

        private void buttonSpecular_Click(object sender, EventArgs e)
        {
            var col = GetColor(buttonSpecular.BackColor);

            _mobj.MaterialColor.SpecularColorABGR = ColorToABGR(col);

            buttonSpecular.BackColor = col;
        }

        private uint ColorToABGR(Color col)
        {
            return (uint)((col.A << 24) | (col.B << 16) | (col.G << 8) | (col.R));
        }

        private Color GetColor(Color defaultColor)
        {
            using(ColorDialog d = new ColorDialog())
            {
                d.Color = defaultColor;

                if (d.ShowDialog() == DialogResult.OK)
                    return d.Color;
            }
            return defaultColor;
        }

        private void tbAlpha_TextChanged(object sender, EventArgs e)
        {
            float val;
            if(float.TryParse(tbAlpha.Text, out val))
            {
                _mobj.MaterialColor.Alpha = val;
            }
            else
                tbAlpha.Text = _mobj.MaterialColor.Alpha.ToString();
        }

        private void tbShine_TextChanged(object sender, EventArgs e)
        {
            float val;
            if (float.TryParse(tbShine.Text, out val))
            {
                _mobj.MaterialColor.Shininess = val;
            }
            else
                tbShine.Text = _mobj.MaterialColor.Shininess.ToString();
        }

        private void listTexture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                propertyTexture.SelectedObject = con.TOBJ;
            }
            else
                propertyTexture.SelectedObject = null;
        }

        /// <summary>
        /// Automatically updates texture flags
        /// </summary>
        private void UpdateTextureFlags()
        {
            TextureContainer prev = null;

            var index = 0;
            foreach(TextureContainer c in listTexture.Items)
            {
                if(prev == null)
                {
                    _mobj.Textures = c.TOBJ;
                }
                else
                {
                    prev.TOBJ.Next = c.TOBJ;
                }

                c.TOBJ.TexMapID = HSDRaw.GX.GXTexMapID.GX_TEXMAP0 + index++;

                prev = c;
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                var index = listTexture.SelectedIndices[0] - 1;

                if (index < 0)
                    index = listTexture.Items.Count - 2;

                listTexture.Items.Remove(con);

                listTexture.Items.Insert(index, con);

                UpdateTextureFlags();
            }
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                var index = listTexture.SelectedIndices[0] + 1;
                
                if (index > listTexture.Items.Count - 2)
                    index = 0;

                listTexture.Items.Remove(con);

                listTexture.Items.Insert(index, con);

                UpdateTextureFlags();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                listTexture.Items.Remove(con);

                UpdateTextureFlags();
            }
        }
    }
}
