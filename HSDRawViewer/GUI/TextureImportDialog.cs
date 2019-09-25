using HSDRaw.GX;
using HSDRaw.Tools;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI
{
    public partial class TextureImportDialog : Form
    {
        public GXTexFmt TextureFormat
        {
            get
            {
                GXTexFmt fmt;
                Enum.TryParse<GXTexFmt>(cbFormat.SelectedValue.ToString(), out fmt);
                return fmt;
            }
        }
        public GXTlutFmt PaletteFormat
        {
            get
            {
                GXTlutFmt fmt;
                Enum.TryParse<GXTlutFmt > (sbPalFormat.SelectedValue.ToString(), out fmt);
                return fmt;
            }
        }

        public TextureImportDialog()
        {
            InitializeComponent();

            cbFormat.DataSource = Enum.GetValues(typeof(GXTexFmt));
            sbPalFormat.DataSource = Enum.GetValues(typeof(GXTlutFmt));

            cbFormat.SelectedItem = GXTexFmt.CMP;
            sbPalFormat.SelectedItem = GXTlutFmt.RGB565;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            sbPalFormat.Enabled = TPLConv.IsPalettedFormat(TextureFormat);
        }
    }
}
