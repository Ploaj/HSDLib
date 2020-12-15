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
                    var bmp = Tools.BitmapTools.BGRAToBitmap(t.GetDecodedImageData(), t.ImageData.Width, t.ImageData.Height);

                    TextureList.Images.Add(bmp);

                    listTexture.Items.Add(new TextureContainer(t)
                    {
                        ImageIndex = index++,
                    });
                }
                listTexture.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            // load material color
            var mc = mobj.Material;
            buttonAmbient.BackColor = mc.AmbientColor;
            buttonDiffuse.BackColor = mc.DiffuseColor;
            buttonSpecular.BackColor = mc.SpecularColor;
            tbShine.Text = mc.Shininess.ToString();
            tbAlpha.Text = mc.Alpha.ToString();

            // load pixel processing
            if(mobj.PEDesc != null)
            {
                cbEnablePP.Checked = true;
            }
        }

        private void cbEnablePP_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnablePP.Checked)
            {
                if(_mobj.PEDesc == null)
                {
                    _mobj.PEDesc = new HSD_PEDesc()
                    {
                        AlphaComp0 = HSDRaw.GX.GXCompareType.Always,
                        AlphaComp1 = HSDRaw.GX.GXCompareType.Always,
                        BlendMode = HSDRaw.GX.GXBlendMode.GX_BLEND,
                        BlendOp = HSDRaw.GX.GXLogicOp.GX_LO_SET,
                        DepthFunction = HSDRaw.GX.GXCompareType.LEqual,
                        SrcFactor = HSDRaw.GX.GXBlendFactor.GX_BL_SRCALPHA,
                        DstFactor = HSDRaw.GX.GXBlendFactor.GX_BL_INVSRCALPHA,
                        Flags = (PIXEL_PROCESS_ENABLE)25
                    };

                }
                propertyPixel.SelectedObject = _mobj.PEDesc;
                propertyPixel.Enabled = true;
            }
            else
            {
                _mobj.PEDesc = null;
                propertyPixel.SelectedObject = null;
                propertyPixel.Enabled = false;
            }
        }

        private void buttonAmbient_Click(object sender, EventArgs e)
        {
            var col = GetColor(buttonAmbient.BackColor);

            _mobj.Material.AmbientColor = col;

            buttonAmbient.BackColor = col;
        }

        private void buttonDiffuse_Click(object sender, EventArgs e)
        {
            var col = GetColor(buttonDiffuse.BackColor);

            _mobj.Material.DiffuseColor = col;

            buttonDiffuse.BackColor = col;
        }

        private void buttonSpecular_Click(object sender, EventArgs e)
        {
            var col = GetColor(buttonSpecular.BackColor);

            _mobj.Material.SpecularColor = col;

            buttonSpecular.BackColor = col;
        }

        private uint ColorToRGBA(Color col)
        {
            return (uint)((col.R << 24) | (col.G << 16) | (col.B << 8) | (col.A));
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
                _mobj.Material.Alpha = val;
            }
            else
                tbAlpha.Text = _mobj.Material.Alpha.ToString();
        }

        private void tbShine_TextChanged(object sender, EventArgs e)
        {
            float val;
            if (float.TryParse(tbShine.Text, out val))
            {
                _mobj.Material.Shininess = val;
            }
            else
                tbShine.Text = _mobj.Material.Shininess.ToString();
        }

        private void listTexture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                propertyTexture.SelectedObject = con.TOBJ;

                enableTEVCB.Checked = (con.TOBJ.TEV != null);
                tevPropertyGrid.SelectedObject = con.TOBJ.TEV;
            }
            else
            {
                propertyTexture.SelectedObject = null;
                enableTEVCB.Checked = false;
                tevPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>
        /// Automatically updates texture flags
        /// </summary>
        private void UpdateTextureFlags()
        {
            TextureContainer prev = null;
            _mobj.Textures = null;

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
                
                c.TOBJ.GXTexGenSrc = 4 + index;
                c.TOBJ.TexMapID = HSDRaw.GX.GXTexMapID.GX_TEXMAP0 + index++;

                prev = c;
            }

            if(prev != null)
                prev.TOBJ.Next = null;
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                var index = listTexture.SelectedIndices[0] - 1;

                listTexture.Items.Remove(con);

                if (index < 0)
                    listTexture.Items.Add(con);
                else
                    listTexture.Items.Insert(index, con);

                UpdateTextureFlags();
            }
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                var index = listTexture.SelectedIndices[0] + 1;

                listTexture.Items.Remove(con);

                if (index > listTexture.Items.Count)
                    listTexture.Items.Insert(0, con);
                else
                    listTexture.Items.Insert(index, con);


                UpdateTextureFlags();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                // remove texture
                listTexture.Items.Remove(con);

                // fix images
                for (int i = 0; i < listTexture.Items.Count; i++)
                {
                    if (listTexture.Items[i].ImageIndex > con.ImageIndex)
                        listTexture.Items[i].ImageIndex -= 1;
                }

                // remove image from texture list
                var index = TextureList.Images[con.ImageIndex];

                TextureList.Images.RemoveAt(con.ImageIndex);

                index.Dispose();

                // update flags
                UpdateTextureFlags();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importTexture_Click(object sender, EventArgs e)
        {
            var tobj = TOBJConverter.ImportTOBJFromFile();

            if(tobj != null)
            {
                var bmp = Tools.BitmapTools.BGRAToBitmap(tobj.GetDecodedImageData(), tobj.ImageData.Width, tobj.ImageData.Height);

                listTexture.Items.Add(new TextureContainer(tobj)
                {
                    ImageIndex = TextureList.Images.Count,
                });

                TextureList.Images.Add(bmp);

                // update flags
                UpdateTextureFlags();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                var tobj = TOBJConverter.ImportTOBJFromFile();

                if (tobj != null)
                {
                    // replace tobj
                    con.TOBJ.ImageData = tobj.ImageData;
                    con.TOBJ.TlutData = tobj.TlutData;
                    //con.TOBJ._s.SetFromStruct(tobj._s);
                    
                    // replace image in list
                    var image = TextureList.Images[con.ImageIndex];
                    image.Dispose();

                    var newImage = Tools.BitmapTools.BGRAToBitmap(tobj.GetDecodedImageData(), tobj.ImageData.Width, tobj.ImageData.Height);

                    TextureList.Images[con.ImageIndex] = newImage;
                    
                    // update flags
                    UpdateTextureFlags();
                    
                    // refresh
                    listTexture.Items[listTexture.SelectedIndices[0]] = listTexture.Items[listTexture.SelectedIndices[0]];
                    listTexture.Invalidate();
                }
            }
        }

        /// <summary>
        /// Update label
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyTexture_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // refresh
            if(listTexture.SelectedIndices.Count > 0)
            {
                var tc = (listTexture.SelectedItems[0] as TextureContainer);
                tc.Text = tc.TOBJ.Flags.ToString();
                listTexture.Items[listTexture.SelectedIndices[0]] = listTexture.Items[listTexture.SelectedIndices[0]];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveTexture_Click(object sender, EventArgs e)
        {
            if (listTexture.SelectedItems.Count > 0 && listTexture.SelectedItems[0] is TextureContainer con)
            {
                var f = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);
                if (f != null)
                {
                    var bmp = TOBJConverter.ToBitmap(con.TOBJ);
                    bmp.Save(f);
                    bmp.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enableTEVCB_CheckedChanged(object sender, EventArgs e)
        {
            if (propertyTexture.SelectedObject is HSD_TOBJ tobj)
            {
                if (enableTEVCB.Checked)
                {
                    if(tobj.TEV == null)
                        tobj.TEV = new HSD_TOBJ_TEV();

                    tevPropertyGrid.SelectedObject = tobj.TEV;
                    tevPropertyGrid.Visible = true;
                }
                else
                {
                    tobj.TEV = null;
                    tevPropertyGrid.Visible = false;
                }
            }
        }
    }
}
