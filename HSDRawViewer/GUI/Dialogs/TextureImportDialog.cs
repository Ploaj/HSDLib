using HSDRaw.GX;
using HSDRaw.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Dialog
{
    public enum ChannelType
    {
        RED,
        GREEN,
        BLUE,
        ALPHA,
        MIX
    }

    public partial class TextureImportDialog : Form
    {
        public class IAOptions
        {
            public ChannelType AlphaChannel { get; set; } = ChannelType.ALPHA;
            public ChannelType LumChannel { get; set; } = ChannelType.MIX;
        }
        public class IOptions
        {
            public ChannelType LumChannel { get; set; } = ChannelType.MIX;
        }
        public class AOptions
        {
            public ChannelType AlphaChannel { get; set; } = ChannelType.ALPHA;
        }

        public GXTexFmt TextureFormat
        {
            get
            {
                Enum.TryParse(cbFormat.SelectedValue.ToString(), out GXTexFmt fmt);
                return fmt;
            }
            set
            {
                cbFormat.SelectedItem = value;
            }
        }
        public GXTlutFmt PaletteFormat
        {
            get
            {
                Enum.TryParse(sbPalFormat.SelectedValue.ToString(), out GXTlutFmt fmt);
                return fmt;
            }
            set
            {
                sbPalFormat.SelectedItem = value;
            }
        }

        public int ImageCount
        {
            get
            {
                return (int)nudCount.Value;
            }
        }

        public bool ShowCount
        {
            set
            {
                if (value)
                {
                    countLabel.Visible = true;
                    nudCount.Visible = true;
                }
                else
                {
                    countLabel.Visible = false;
                    nudCount.Visible = false;
                }
            }
        }

        public IAOptions IASettings { get; internal set; } = new IAOptions();
        public IOptions ASettings { get; internal set; } = new IOptions();
        public AOptions ISettings { get; internal set; } = new AOptions();

        public TextureImportDialog()
        {
            InitializeComponent();

            cbFormat.DataSource = Enum.GetValues(typeof(GXTexFmt));
            sbPalFormat.DataSource = Enum.GetValues(typeof(GXTlutFmt));

            cbFormat.SelectedItem = GXTexFmt.CMP;
            sbPalFormat.SelectedItem = GXTlutFmt.RGB565;

            countLabel.Visible = false;
            nudCount.Visible = false;

            CenterToScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            sbPalFormat.Enabled = GXImageConverter.IsPalettedFormat(TextureFormat);

            propertyGrid1.SelectedObject = null;

            if (TextureFormat == GXTexFmt.IA4 || TextureFormat == GXTexFmt.IA8)
                propertyGrid1.SelectedObject = IASettings;

            if (TextureFormat == GXTexFmt.I8 || TextureFormat == GXTexFmt.I4)
                propertyGrid1.SelectedObject = ISettings;
        }

        public void ApplySettings(Image<Bgra32> image)
        {
            if (TextureFormat == GXTexFmt.IA4 || TextureFormat == GXTexFmt.IA8)
            {
                for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                    {
                        image[x, y] = ApplyChannels(image[x, y], IASettings.LumChannel, IASettings.AlphaChannel);
                    }
            }
            else
            if (TextureFormat == GXTexFmt.I8 || TextureFormat == GXTexFmt.I4)
            {
                for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                    {
                        image[x, y] = ApplyChannels(image[x, y], IASettings.AlphaChannel, IASettings.AlphaChannel);
                    }
            }
        }

        private Bgra32 ApplyChannels(Bgra32 input, ChannelType color, ChannelType alpha)
        {
            return new Bgra32(
                GetChannel(input, ChannelType.RED, color),
                GetChannel(input, ChannelType.GREEN, color),
                GetChannel(input, ChannelType.BLUE, color),
                GetChannel(input, ChannelType.ALPHA, alpha));
        }

        private byte GetChannel(Bgra32 input, ChannelType inputChannel, ChannelType channel)
        {
            byte r = input.R;
            byte g = input.G;
            byte b = input.B;
            byte a = input.A;

            switch (channel)
            {
                case ChannelType.ALPHA:
                    return a;
                case ChannelType.RED:
                    return r;
                case ChannelType.GREEN:
                    return g;
                case ChannelType.BLUE:
                    return b;
                default:
                    return GetChannel(input, inputChannel, inputChannel);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
