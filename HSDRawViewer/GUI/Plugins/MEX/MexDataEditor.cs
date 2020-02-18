using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX;
using HSDRawViewer.Rendering;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public partial class MexDataEditor : DockContent, EditorBase, IDrawable
    {
        public MexDataEditor()
        {
            InitializeComponent();

            fighterList.DataSource = FighterEntries;

            JOBJManager.RenderBones = false;

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            groupBox2.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();
            //viewport.Visible = false;

            FormClosing += (sender, args) =>
            {
                JOBJManager.ClearRenderingCache();
                viewport.Dispose();
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
        private MEX_Data _data { get { if (_node.Accessor is MEX_Data data) return data; else return null; } }

        private BindingList<MEXEntry> FighterEntries = new BindingList<MEXEntry>();
        public ExpandedSSM[] SSMEntries { get; set; }
        public MEXEffectEntry[] Effects { get; set; }
        public MEX_CSSIcon[] Icons { get; set; }

        public DrawOrder DrawOrder => DrawOrder.Last;

        private JOBJManager JOBJManager = new JOBJManager();

        private ViewportControl viewport;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void LoadData()
        {
            FighterEntries.Clear();
            for(int i = 0; i < _data.MetaData.NumOfInternalIDs; i++)
            {
                FighterEntries.Add(new MEXEntry().LoadData(_data, i, MEXIdConverter.ToExternalID(i, _data.MetaData.NumOfInternalIDs)));
            }

            SSMEntries = new ExpandedSSM[_data.SSM_SSMFiles.Length];
            for (int i = 0; i < _data.SSM_SSMFiles.Length; i++)
            {
                ExpandedSSM ssm = new ExpandedSSM();
                ssm.Name = _data.SSM_SSMFiles[i].Value;
                ssm.Flag = _data.SSM_Flags[i].Flag;
                SSMEntries[i] = ssm;
            }
            ssmEditor.SetArrayFromProperty(this, "SSMEntries");
            
            Effects = new MEXEffectEntry[_data.Char_EffectFiles.Length];
            for(int i = 0; i < Effects.Length; i++)
            {
                Effects[i] = new MEXEffectEntry()
                {
                    FileName = _data.Char_EffectFiles[i].FileName,
                    Symbol = _data.Char_EffectFiles[i].Symbol,
                };
            }
            effectEditor.SetArrayFromProperty(this, "Effects");

            Icons = _data.MnSlChr_IconData.Icons;
            cssIconEditor.SetArrayFromProperty(this, "Icons");
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveFighterData()
        {
            var d = _data;
            int index = 0;
            d.MetaData.NumOfExternalIDs = FighterEntries.Count;
            d.MetaData.NumOfInternalIDs = FighterEntries.Count;
            d.MnSlChr_NameText = new HSDNullPointerArrayAccessor<HSD_String>();
            d.Char_CharFiles = new HSDArrayAccessor<MEX_CharFileStrings>();
            d.Char_CostumeIDs = new HSDArrayAccessor<MEX_CostumeIDs>();
            d.Char_CostumeFileSymbols = new HSDArrayAccessor<MEX_CostumeFileSymbolTable>();
            d.Char_AnimFiles = new HSDNullPointerArrayAccessor<HSD_String>();
            d.Char_AnimCount = new HSDArrayAccessor<MEX_AnimCount>();
            d.Char_InsigniaIDs = new HSDArrayAccessor<HSD_Byte>();
            d.GmRst_AnimFiles = new HSDNullPointerArrayAccessor<HSD_String>();
            d.GmRst_Scale = new HSDArrayAccessor<HSDRaw.MEX.HSD_Float>();
            d.GmRst_VictoryTheme = new HSDArrayAccessor<HSD_Int>();
            d.FtDemo_SymbolNames = new HSDNullPointerArrayAccessor<MEX_FtDemoSymbolNames>();
            d.SFX_NameDef = new HSDArrayAccessor<HSD_Int>();
            d.SSM_CharSSMFileIDs = new HSDArrayAccessor<MEX_CharSSMFileID>();
            d.Char_EffectIDs = new HSDArrayAccessor<HSD_Byte>();
            d.Char_CostumePointers = new HSDArrayAccessor<MEX_CostumeRuntimePointers>();

            d.OnLoad = new HSDArrayAccessor<HSD_UInt>();
            d.OnDeath = new HSDArrayAccessor<HSD_UInt>();
            d.OnUnknown = new HSDArrayAccessor<HSD_UInt>();
            d.MoveLogic = new HSDFixedLengthPointerArrayAccessor<HSDRaw.HSDArrayAccessor<MEX_MoveLogic>>();
            d.SpecialN = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialNAir = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialHi = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialHiAir = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialLw = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialLwAir = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialS = new HSDArrayAccessor<HSD_UInt>();
            d.SpecialSAir = new HSDArrayAccessor<HSD_UInt>();
            
            foreach (var v in FighterEntries)
            {
                v.SaveData(d, index, MEXIdConverter.ToExternalID(index, FighterEntries.Count));
                index++;
            }
        }

        private void SaveSSMData()
        {
            var numOfSSMs = SSMEntries.Length;

            _data.MetaData.NumOfSSMs = numOfSSMs;

            _data._s.GetCreateReference<HSDAccessor>(0x4C)._s.SetData(new byte[180 + ((numOfSSMs + 1) * 4 * 4) + ((numOfSSMs + 1) * 4)]);

            _data.SSM_SSMFiles = new HSDNullPointerArrayAccessor<HSD_String>();
            _data.SSM_Flags = new HSDArrayAccessor<SSMFlag>();
            for(int i = 0; i < SSMEntries.Length; i++)
            {
                _data.SSM_SSMFiles.Add(new HSD_String() { Value = SSMEntries[i].Name });
                _data.SSM_Flags.Add(new SSMFlag() { Flag = SSMEntries[i].Flag });
            }
        }

        private void SaveEffectData()
        {
            _data.Char_EffectFiles = new HSDArrayAccessor<MEX_EffectFiles>();
            foreach(var v in Effects)
            {
                _data.Char_EffectFiles.Add(new MEX_EffectFiles()
                {
                    FileName = v.FileName,
                    Symbol = v.Symbol
                });
            }
        }

        private void SaveIconData()
        {
            _data.MetaData.NumOfCSSIcons = Icons.Length;
            _data.MnSlChr_IconData.Icons = Icons;
        }

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fighterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = fighterList.SelectedItem;
            propertyGrid2.SelectedObject = (fighterList.SelectedItem as MEXEntry).Functions;
        }

        #endregion
        
        private void saveSSMButton_Click(object sender, EventArgs e)
        {
            SaveSSMData();
        }

        private void saveEffectButton_Click(object sender, EventArgs e)
        {
            SaveEffectData();
        }

        private void buttonSaveCSS_Click(object sender, EventArgs e)
        {
            SaveIconData();
        }

        private static Color IconColor = Color.FromArgb(128, 255, 0, 0);

        private static Color SelectedIconColor = Color.FromArgb(128, 255, 255, 0);

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            JOBJManager.Render(cam);

            foreach(var i in Icons)
            {
                if (cssIconEditor.SelectedObject == (object)i)
                    DrawShape.DrawRectangle(i.X1, i.Y1, i.X2, i.Y2, SelectedIconColor);
                else
                    DrawShape.DrawRectangle(i.X1, i.Y1, i.X2, i.Y2, IconColor);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void LoadModel(HSD_JOBJ jobj)
        {
            JOBJManager.ClearRenderingCache();
            JOBJManager.SetJOBJ(jobj);
            if (!viewport.Visible)
                viewport.Visible = true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadPlSl_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (f != null)
            {
                var hsd = new HSDRawFile(f);
                if (hsd.Roots[0].Data is SBM_SelectChrDataTable tab)
                {
                    LoadModel(tab.MenuModel);
                    JOBJManager.SetAnimJoint(tab.MenuAnimation);
                    JOBJManager.Frame = 600;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cloneButton_Click(object sender, EventArgs e)
        {
            if(fighterList.SelectedItem is MEXEntry me)
            {
                FighterEntries.Insert(FighterEntries.Count - 6, ObjectExtensions.Copy(me));
            }
        }

        private void saveFightersButton_Click(object sender, EventArgs e)
        {
            SaveFighterData();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FighterEntries.ResetBindings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(fighterList.SelectedItem is MEXEntry mex)
            {
                var f = Tools.FileIO.SaveFile("YAML (*.yaml)|*.yaml", mex.NameText + ".yaml");
                if (f != null)
                {
                    var builder = new SerializerBuilder();
                    builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
                    builder.WithTypeInspector(inspector => new MEXTypeInspector(inspector));

                    using (StreamWriter writer = File.CreateText(f))
                    {
                        builder.Build().Serialize(writer, fighterList.SelectedItem);
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("YAML (*.yaml)|*.yaml");
            if (f != null)
            {
                var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
                .Build();

                var me = deserializer.Deserialize<MEXEntry>(File.ReadAllText(f));

                FighterEntries.Insert(FighterEntries.Count - 6, ObjectExtensions.Copy(me));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFighter_Click(object sender, EventArgs e)
        {
            var selected = fighterList.SelectedIndex;
            if (IsClone(selected))
            {
                FighterEntries.RemoveAt(selected);
                return;
            }
            MessageBox.Show("Unable to delete base game fighters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Returns true if fighter at this index is an extended fighter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsClone(int index)
        {
            return (index >= 0x21 - 6 && index < FighterEntries.Count - 6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fighterList_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                Brush myBrush = Brushes.Black;
                
                if(IsClone(e.Index))
                {
                    myBrush = Brushes.DarkViolet;
                }

                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }
    }
}
