using HSDRaw.MEX;
using HSDRawViewer.GUI.MEX.Controls;
using HSDRawViewer.GUI.Plugins;
using HSDRawViewer.Tools;
using System;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.MEX
{
    public partial class MexDataEditor : DockContent, EditorBase
    {
        public MEXFighterControl FighterControl;
        public MEXStageControl StageControl;
        public MEXItemControl ItemControl;
        public MEXEffectControl EffectControl;
        public MEXMusicControl MusicControl;
        public MEXMenuControl MenuControl;
        public MEXSemControl SoundControl;

        public MexDataEditor()
        {
            InitializeComponent();

            //GUITools.SetIconFromBitmap(this, Properties.Resources.doc_kabii);
            FighterControl = new MEXFighterControl();
            FighterControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[0].Controls.Add(FighterControl);

            StageControl = new MEXStageControl();
            StageControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[1].Controls.Add(StageControl);

            ItemControl = new MEXItemControl();
            ItemControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[2].Controls.Add(ItemControl);

            EffectControl = new MEXEffectControl();
            EffectControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[3].Controls.Add(EffectControl);

            MenuControl = new MEXMenuControl();
            MenuControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[4].Controls.Add(MenuControl);

            MusicControl = new MEXMusicControl();
            MusicControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[5].Controls.Add(MusicControl);

            SoundControl = new MEXSemControl();
            SoundControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[6].Controls.Add(SoundControl);

        }

        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(MEX_Data) };

        public DataNode Node
        {
            get => _node; set
            {
                _node = value;
                LoadData();
            }
        }
        private DataNode _node;
        public MEX_Data _data { get { if (_node.Accessor is MEX_Data data) return data; else return null; } }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void LoadData()
        {
            // Effects------------------------------------
            EffectControl.LoadData(_data);
            
            // Fighters------------------------------------
            FighterControl.LoadData(_data);
            
            // CSS------------------------------------
            MenuControl.LoadData(_data);
            
            // Music------------------------------------
            MusicControl.LoadData(_data);

            // Items------------------------------------
            ItemControl.LoadData(_data);

            // Stages
            StageControl.LoadData(_data);

            // Stages
            SoundControl.LoadData(_data);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAllChangesButton_Click(object sender, EventArgs e)
        {
            FighterControl.SaveData(_data);
            StageControl.SaveData(_data);
            ItemControl.SaveData(_data);
            EffectControl.SaveData(_data);
            MenuControl.SaveData(_data);
            MusicControl.SaveData(_data);
            SoundControl.SaveData(_data);

            if (MessageBox.Show("Save File", "Save All Changes to Disk", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                MainForm.Instance.SaveDAT();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void installFighterButton_Click(object sender, EventArgs e)
        {
        }

        private void OldInstall()
        {
            var f = FileIO.OpenFile("Fighter Package (*.zip)|*.zip");
            if (f != null)
            {
                MenuControl.CloseMenuFiles();

                using (ProgressBarDisplay d = new ProgressBarDisplay(new FighterPackageInstaller(f, this)))
                {
                    d.DoWork();
                    d.ShowDialog();
                }

                FighterControl.ResetDataBindings();
                ItemControl.ResetDataBindings();
                EffectControl.ResetDataBindings();
                MusicControl.ResetDataBindings();

                MEXConverter.ssmValues.Clear();
                MEXConverter.ssmValues.AddRange(_data.SSMTable.SSM_SSMFiles.Array.Select(s => s.Value));
                MessageBox.Show("Fighter installed");
                saveAllChangesButton_Click(null, null);
                MainForm.Instance.SaveDAT();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uninstallFighterButton_Click(object sender, EventArgs e)
        {
        }

        private void OldUninstall()
        {
            if (FighterControl.IsExtendedFighter(FighterControl.SelectedIndex) && FighterControl.SelectedEntry != null)
            {
                using (ProgressBarDisplay d = new ProgressBarDisplay(new FighterPackageUninstaller(FighterControl.SelectedIndex, FighterControl.SelectedEntry, this)))
                {
                    d.DoWork();
                    d.ShowDialog();
                }

                FighterControl.ResetDataBindings();
                ItemControl.ResetDataBindings();
                EffectControl.ResetDataBindings();
                MusicControl.ResetDataBindings();

                MEXConverter.ssmValues.Clear();
                MEXConverter.ssmValues.AddRange(_data.SSMTable.SSM_SSMFiles.Array.Select(s => s.Value));
                MessageBox.Show("Fighter uninstalled");
                saveAllChangesButton_Click(null, null);
                MainForm.Instance.SaveDAT();
            }
        }
        
        
    }
}
