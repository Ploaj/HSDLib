using GCILib;
using HSDRawViewer.GUI.Controls;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class ISOFileTool : Form
    {
        private ISOEditor _editor;

        public string FilePath
        {
            get => _editor.OpenFilePath;
        }

        public byte[] FileData
        {
            get => _editor.OpenFile;
        }

        public ISOFileTool(GCISO iso)
        {
            InitializeComponent();

            _editor = new ISOEditor(iso);
            _editor.Dock = DockStyle.Fill;
            Controls.Add(_editor);

            CenterToScreen();
        }
    }
}
