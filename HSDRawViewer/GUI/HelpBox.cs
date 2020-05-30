using System.Windows.Forms;

namespace HSDRawViewer.GUI
{
    public partial class HelpBox : Form
    {
        public HelpBox(string helpText)
        {
            InitializeComponent();

            textBox1.Text = helpText;

            CenterToScreen();
        }
    }
}
