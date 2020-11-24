using HSDRaw;
using HSDRaw.MEX;
using HSDRawViewer.GUI.MEX.Controls;
using HSDRawViewer.GUI.Plugins;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.MEX
{
    public partial class MexDataEditor : DockContent, EditorBase
    {
        public MEXExternalFileControl FileControl;
        public MEXFighterControl FighterControl;
        public MEXStageControl StageControl;
        public MEXItemControl ItemControl;
        public MEXEffectControl EffectControl;
        public MEXMusicControl MusicControl;
        public MEXMenuControl MenuControl;
        public MEXSemControl SoundControl;
        
        /// <summary>
        /// 
        /// </summary>
        public MexDataEditor()
        {
            InitializeComponent();

            //GUITools.SetIconFromBitmap(this, Properties.Resources.doc_kabii);

            var page = 0;

            FileControl = new MEXExternalFileControl();
            FileControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(FileControl);

            FighterControl = new MEXFighterControl();
            FighterControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(FighterControl);

            StageControl = new MEXStageControl();
            StageControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(StageControl);

            ItemControl = new MEXItemControl();
            ItemControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(ItemControl);

            EffectControl = new MEXEffectControl();
            EffectControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(EffectControl);

            MenuControl = new MEXMenuControl();
            MenuControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(MenuControl);

            MusicControl = new MEXMusicControl();
            MusicControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(MusicControl);

            SoundControl = new MEXSemControl();
            SoundControl.Dock = DockStyle.Fill;
            mainTabControl.TabPages[page++].Controls.Add(SoundControl);

            FileControl.AddExternal("MnSlMap.usd", new string[] { "MnSelectStageDataTable", "mexMapData" }, "Stage Select Screen Data (US)");
            FileControl.AddExternal("MnSlChr.usd", new string[] { "MnSelectChrDataTable", "mexSelectChr" }, "Character Select Screen Data (US)");
            FileControl.AddExternal("PlCo.dat", new string[] { "ftLoadCommonData" }, "Contains Fighter Bone Tables");
            FileControl.AddExternal("IfAll.usd", new string[] { "Stc_icns" }, "Contains stock icons");

            MEXExternalFileControl.OnFileLoaded += (sender, args) =>
            {
                FighterControl.CheckEnable(this);
                MenuControl.CheckEnable(this);
            };
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
        /// <param name="filePath"></param>
        /// <returns></returns>
        public HSDRawFile GetFile(string file)
        {
            return FileControl.GetFile(file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public HSDAccessor GetSymbol(string symbol)
        {
            return FileControl.GetSymbol(symbol);
        }

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

            // Stages------------------------------------
            StageControl.LoadData(_data);

            // Sound------------------------------------
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

            FileControl.SaveFiles();

            if (MessageBox.Show("Save File", "Save All Changes to Disk", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                MainForm.Instance.SaveDAT();
        }

    }
}
