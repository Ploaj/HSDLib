using System.Windows.Forms;

namespace HSDRawViewer.GUI.Dialog
{
    public partial class HelpBox : Form
    {
        public HelpBox(string helpText)
        {
            InitializeComponent();

            helpText = helpText.Replace("\n", System.Environment.NewLine);

            textBox1.Text = helpText;

            CenterToScreen();
        }
    }
}
