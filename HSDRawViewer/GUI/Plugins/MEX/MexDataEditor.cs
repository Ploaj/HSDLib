using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX;
using HSDRaw.MEX.Characters;
using HSDRawViewer.Rendering;
using HSDRawViewer.Sound;
using OpenTK;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public partial class MexDataEditor : DockContent, EditorBase, IDrawableInterface
    {
        public int NumberOfEntries
        {
            get => FighterEntries.Count;
        }

        public MexDataEditor()
        {
            InitializeComponent();

            //GUITools.SetIconFromBitmap(this, Properties.Resources.doc_kabii);

            fighterList.DataSource = FighterEntries;

            JOBJManager.RenderBones = false;

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = false;
            groupBox2.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();
            //viewport.Visible = false;

            musicDSPPlayer.ReplaceButtonVisible = false;

            menuPlaylistEditor.DoubleClickedNode += menuPlaylistEditor_DoubleClick;

            musicListEditor.DoubleClickedNode += musicListEditor_DoubleClicked;
            musicListEditor.ArrayUpdated += musicListEditor_ArrayUpdated;
            musicListEditor.EnablePropertyViewDescription = false;

            commonItemEditor.DisableAllControls();
            fighterItemEditor.DisableAllControls();
            pokemonItemEditor.DisableAllControls();
            stageItemEditor.DisableAllControls();

            mexItemEditor.OnItemRemove += ( args) =>
            {
                foreach (var v in FighterEntries)
                {
                    foreach (var s in v.MEXItems)
                    {
                        if (s.Value == MEXItemOffset + args.Index)
                            s.Value = 0;
                        if (s.Value > MEXItemOffset + args.Index)
                            s.Value -= 1;
                    }
                }
            };

            effectEditor.OnItemRemove += (args) =>
            {
                foreach (var v in FighterEntries)
                {
                    if (v.EffectIndex == args.Index)
                        v.EffectIndex = 0;
                    if (v.EffectIndex > args.Index)
                        v.EffectIndex--;
                    if (v.KirbyEffectID == args.Index)
                        v.KirbyEffectID = 0;
                    if (v.KirbyEffectID > args.Index)
                        v.KirbyEffectID--;
                }
            };

            commonItemEditor.TextOverrides.AddRange(DefaultItemNames.CommonItemNames);
            fighterItemEditor.TextOverrides.AddRange(DefaultItemNames.FighterItemNames);
            pokemonItemEditor.TextOverrides.AddRange(DefaultItemNames.PokemonItemNames);
            stageItemEditor.TextOverrides.AddRange(DefaultItemNames.StageItemNames);

            FighterEntries.ListChanged += (sender, args) =>
            {
                MEXConverter.internalIDValues.Clear();
                MEXConverter.internalIDValues.Add("None");
                MEXConverter.internalIDValues.AddRange(FighterEntries.Select(e=>e.NameText));
            };

            effectEditor.ArrayUpdated += (sender, args) =>
            {
                MEXConverter.effectValues.Clear();
                MEXConverter.effectValues.AddRange(Effects.Select(e=>e.FileName));
            };

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
        public MEX_Data _data { get { if (_node.Accessor is MEX_Data data) return data; else return null; } }

        public BindingList<MEXEntry> FighterEntries = new BindingList<MEXEntry>();

        public MEX_EffectEntry[] Effects { get; set; }
        public MEX_FighterEffect[] MEX_Effects { get; set; }

        public MEX_CSSIconEntry[] Icons { get; set; }
        
        public HSD_String[] Music { get; set; }
        public MEXPlaylistEntry[] MenuPlaylist { get; set; }

        public MEX_Item[] ItemCommon { get; set; }
        public MEX_Item[] ItemFighter { get; set; }
        public MEX_Item[] ItemPokemon { get; set; }
        public MEX_Item[] ItemStage { get; set; }
        public MEX_Item[] ItemMEX { get; set; }

        private int MEXItemOffset = 0;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private JOBJManager JOBJManager = new JOBJManager();

        private ViewportControl viewport;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void LoadData()
        {
            // Effects------------------------------------
            Effects = new MEX_EffectEntry[_data.EffectFiles.Length];
            for (int i = 0; i < Effects.Length; i++)
            {
                Effects[i] = new MEX_EffectEntry()
                {
                    FileName = _data.EffectFiles[i].FileName,
                    Symbol = _data.EffectFiles[i].Symbol,
                };
            }
            effectEditor.SetArrayFromProperty(this, "Effects");


            // Fighters------------------------------------
            FighterEntries.Clear();
            for(int i = 0; i < _data.MetaData.NumOfInternalIDs; i++)
            {
                FighterEntries.Add(new MEXEntry().LoadData(_data, i, MEXIdConverter.ToExternalID(i, _data.MetaData.NumOfInternalIDs)));
            }


            // CSS------------------------------------
            Icons = new MEX_CSSIconEntry[_data.CSSIconData.Icons.Length];
            for(int i = 0; i < Icons.Length; i++)
            {
                Icons[i] = MEX_CSSIconEntry.FromIcon(_data.CSSIconData.Icons[i]);
            }
            cssIconEditor.SetArrayFromProperty(this, "Icons");


            // Music------------------------------------
            MEXConverter.ssmValues.Clear();
            MEXConverter.ssmValues.AddRange(_data.SSMTable.SSM_SSMFiles.Array.Select(e=>e.Value));

            Music = _data.MusicTable.BackgroundMusicStrings.Array;
            musicListEditor.SetArrayFromProperty(this, "Music");

            MenuPlaylist = new MEXPlaylistEntry[_data.MusicTable.MenuPlayListCount];
            for (int i = 0; i < MenuPlaylist.Length; i++)
            {
                var e = _data.MusicTable.MenuPlaylist[i];
                MenuPlaylist[i] = new MEXPlaylistEntry()
                {
                    MusicID = e.HPSID,
                    PlayChance = e.ChanceToPlay.ToString() + "%"
                };
            }
            menuPlaylistEditor.SetArrayFromProperty(this, "MenuPlaylist");


            // Items------------------------------------
            MEXItemOffset = 0;

            ItemCommon = _data.ItemTable.CommonItems.Array;
            commonItemEditor.SetArrayFromProperty(this, "ItemCommon");
            MEXItemOffset += ItemCommon.Length;

            ItemFighter = _data.ItemTable.FighterItems.Array;
            fighterItemEditor.SetArrayFromProperty(this, "ItemFighter");
            fighterItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemFighter.Length;

            ItemPokemon = _data.ItemTable.Pokemon.Array;
            pokemonItemEditor.SetArrayFromProperty(this, "ItemPokemon");
            pokemonItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemPokemon.Length;

            ItemStage = _data.ItemTable.Stages.Array;
            stageItemEditor.SetArrayFromProperty(this, "ItemStage");
            stageItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemStage.Length;

            ItemMEX = _data.ItemTable.MEXItems.Array;
            mexItemEditor.SetArrayFromProperty(this, "ItemMEX");
            mexItemEditor.ItemIndexOffset = MEXItemOffset;
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
            d.FighterData.NameText.Array = new HSD_String[0];
            d.FighterData.CharFiles.Array = new MEX_CharFileStrings[0];
            d.FighterData.CostumeIDs.Array = new MEX_CostumeIDs[0];
            d.FighterData.CostumeFileSymbols.Array = new MEX_CostumeFileSymbolTable[0];
            d.FighterData.AnimFiles.Array = new HSD_String[0];
            d.FighterData.AnimCount.Array = new MEX_AnimCount[0];
            d.FighterData.InsigniaIDs.Array = new HSD_Byte[0];
            d.FighterData.ResultAnimFiles.Array = new HSD_String[0];
            d.FighterData.ResultScale.Array = new HSD_Float[0];
            d.FighterData.VictoryThemeIDs.Array = new HSD_Int[0];
            d.FighterData.FtDemo_SymbolNames.Array = new MEX_FtDemoSymbolNames[0];
            d.FighterData.AnnouncerCalls.Array = new HSD_Int[0];
            d.FighterData.SSMFileIDs.Array = new MEX_CharSSMFileID[0];
            d.FighterData.EffectIDs.Array = new HSD_Byte[0];
            d.FighterData.CostumePointers.Array = new MEX_CostumeRuntimePointers[0];
            d.FighterData.DefineIDs.Array = new MEX_CharDefineIDs[0];
            d.FighterData.WallJump.Array = new HSD_Byte[0];
            d.FighterData.RstRuntime.Array = new MEX_RstRuntime[0];
            d.FighterData.FighterItemLookup.Array = new MEX_FighterItem[0];
            d.FighterData.FighterEffectLookup.Array = new MEX_FighterEffect[0];

            d.FighterFunctions.OnLoad.Array = new HSD_UInt[0];
            d.FighterFunctions.OnDeath.Array = new HSD_UInt[0];
            d.FighterFunctions.OnUnknown.Array = new HSD_UInt[0];
            d.FighterFunctions.MoveLogic.Array = new HSDArrayAccessor<MEX_MoveLogic>[0];
            d.FighterFunctions.SpecialN.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialNAir.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialHi.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialHiAir.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialLw.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialLwAir.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialS.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialSAir.Array = new HSD_UInt[0];
            d.FighterFunctions.OnAbsorb.Array = new HSD_UInt[0];
            d.FighterFunctions.onItemPickup.Array = new HSD_UInt[0];
            d.FighterFunctions.onMakeItemInvisible.Array = new HSD_UInt[0];
            d.FighterFunctions.onMakeItemVisible.Array = new HSD_UInt[0];
            d.FighterFunctions.onItemDrop.Array = new HSD_UInt[0];
            d.FighterFunctions.onItemCatch.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownItemRelated.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownCharacterModelFlags1.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownCharacterModelFlags2.Array = new HSD_UInt[0];
            d.FighterFunctions.onHit.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownEyeTextureRelated.Array = new HSD_UInt[0];
            d.FighterFunctions.onFrame.Array = new HSD_UInt[0];
            d.FighterFunctions.onActionStateChange.Array = new HSD_UInt[0];
            d.FighterFunctions.onRespawn.Array = new HSD_UInt[0];
            d.FighterFunctions.onModelRender.Array = new HSD_UInt[0];
            d.FighterFunctions.onShadowRender.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownMultijump.Array = new HSD_UInt[0];
            d.FighterFunctions.onActionStateChangeWhileEyeTextureIsChanged.Array = new HSD_UInt[0];
            d.FighterFunctions.onTwoEntryTable.Array = new HSD_UInt[0];
            d.FighterFunctions.onLand.Array = new HSD_UInt[0];

            d.FighterFunctions.onSmashDown.Array = new HSD_UInt[0];
            d.FighterFunctions.onSmashUp.Array = new HSD_UInt[0];
            d.FighterFunctions.onSmashForward.Array = new HSD_UInt[0];

            d.FighterFunctions.enterFloat.Array = new HSD_UInt[0];
            d.FighterFunctions.enterSpecialDoubleJump.Array = new HSD_UInt[0];
            d.FighterFunctions.enterTether.Array = new HSD_UInt[0];

            d.KirbyData.CapFiles.Array = new MEX_KirbyCapFiles[0];
            d.KirbyData.KirbyCostumes.Array = new MEX_KirbyCostume[0];
            d.KirbyData.EffectIDs.Array = new HSD_Byte[0];
            d.KirbyFunctions.OnAbilityGain.Array = new HSD_UInt[0];
            d.KirbyFunctions.OnAbilityLose.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbySpecialN.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbySpecialNAir.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbyOnHit.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbyOnItemInit.Array = new HSD_UInt[0];

            // runtime fighter pointer struct
            d.FighterData._s.GetReference<HSDAccessor>(0x40)._s.Resize(FighterEntries.Count * 8);

            // kirby runtimes
            d.KirbyData.CapFileRuntime._s = new HSDStruct(4 * FighterEntries.Count);
            d.KirbyData.CostumeRuntime._s = new HSDStruct(4);
            
            // dump data
            foreach (var v in FighterEntries)
            {
                v.SaveData(d, index, MEXIdConverter.ToExternalID(index, FighterEntries.Count));
                index++;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void SaveEffectData()
        {
            _data.MetaData.NumOfEffects = Effects.Length;
            _data.EffectFiles = new HSDArrayAccessor<MEX_EffectFiles>();
            foreach(var v in Effects)
            {
                _data.EffectFiles.Add(new MEX_EffectFiles()
                {
                    FileName = v.FileName,
                    Symbol = v.Symbol
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveMusicData()
        {
            _data.MetaData.NumOfMusic = Music.Length;
            _data.MusicTable.BackgroundMusicStrings.Array = new HSD_String[0];
            foreach (var v in Music)
                _data.MusicTable.BackgroundMusicStrings.Add(v);

            _data.MusicTable.MenuPlaylist.Array = new MEX_MenuPlaylistItem[0];
            _data.MusicTable.MenuPlayListCount = MenuPlaylist.Length;
            for(int i = 0; i < MenuPlaylist.Length; i++)
            {
                var v = MenuPlaylist[i];
                _data.MusicTable.MenuPlaylist.Set(i, new MEX_MenuPlaylistItem()
                {
                    HPSID = (ushort)v.MusicID,
                    ChanceToPlay = v.PlayChanceValue
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveIconData()
        {
            _data.MetaData.NumOfCSSIcons = Icons.Length;
            MEX_CSSIcon[] ico = new MEX_CSSIcon[Icons.Length];
            for (int i = 0; i < ico.Length; i++)
                ico[i] = Icons[i].ToIcon();
            _data.CSSIconData.Icons = ico;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveItemData()
        {
            _data.ItemTable.CommonItems.Array = ItemCommon;
            _data.ItemTable.FighterItems.Array = ItemFighter;
            _data.ItemTable.Pokemon.Array = ItemPokemon;
            _data.ItemTable.Stages.Array = ItemStage;
            _data.ItemTable.MEXItems.Array = ItemMEX;
            _data.ItemTable._s.GetCreateReference<HSDAccessor>(0x18)._s.Resize(Math.Max(4, ItemMEX.Length * 4));
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

        #region Save Button GUI
        private void saveAllChangesButton_Click(object sender, EventArgs e)
        {
            SaveFighterData();
            SaveItemData();
            SaveEffectData();
            SaveIconData();
            SaveMusicData();
        }
        
        private void saveMusicButton_Click(object sender, EventArgs e)
        {
            SaveMusicData();
        }

        private void saveEffectButton_Click(object sender, EventArgs e)
        {
            SaveEffectData();
        }

        private void buttonSaveCSS_Click(object sender, EventArgs e)
        {
            SaveIconData();
        }

        private void saveItemButton_Click(object sender, EventArgs e)
        {
            SaveItemData();
        }

        #endregion


        #region IconRendering

        private HSDRawFile MenuFile;
        private string MenuFilePath;

        public AddedIcon[] AddedIcons;

        public class AddedIcon
        {
            public HSD_JOBJ jobj;

            public float X { get => jobj.TX; set => jobj.TX = value; }
            public float Y { get => jobj.TY; set => jobj.TY = value; }
            public float Z { get => jobj.TZ; set => jobj.TZ = value; }

            public AddedIcon(HSD_JOBJ jobj)
            {
                this.jobj = jobj;
            }

            public override string ToString()
            {
                return $"{X} {Y} {Z}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (!viewport.Visible)
                return;

            JOBJManager.Render(cam);

            foreach(var i in Icons)
            {
                i.Render(cssIconEditor.SelectedObject == (object)i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveMenuFile()
        {
            if (MenuFile != null)
                MenuFile.Save(MenuFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModel(HSD_JOBJ jobj)
        {
            JOBJManager.ClearRenderingCache();
            JOBJManager.SetJOBJ(jobj);
            mnslchrToolStrip.Visible = true;
            addedIconEditor.Visible = true;
            if (!viewport.Visible)
                viewport.Visible = true;
            
            addedIconEditor.ItemIndexOffset = jobj.BreathFirstList.Count + 1;

            if (jobj.Children.Length < 17)
                AddedIcons = new AddedIcon[0];
            else
            {
                addedIconEditor.ItemIndexOffset = JOBJManager.IndexOf(jobj.Children[16]) + 1;
                AddedIcons = new AddedIcon[jobj.Children[16].Children.Length];
                for (int i = 0; i < AddedIcons.Length; i++)
                    AddedIcons[i] = new AddedIcon(jobj.Children[16].Children[i]);
            }

            Console.WriteLine(AddedIcons.Length);
            addedIconEditor.SetArrayFromProperty(this, "AddedIcons");
            Console.WriteLine(AddedIcons.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UnloadMenuFile()
        {
            JOBJManager.ClearRenderingCache();
            mnslchrToolStrip.Visible = false;
            viewport.Visible = false;
            addedIconEditor.Visible = false;
            MenuFile = null;
            addedIconEditor.SetArrayFromProperty(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveMnSlChrButton_Click(object sender, EventArgs e)
        {
            SaveMenuFile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importIconButton_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("PNG (*.png)|*.png");

            if(f != null)
            {
                HSD_TOBJ tobj = null;
                using (Bitmap bmp = new Bitmap(f))
                    tobj = Converters.TOBJConverter.BitmapToTOBJ(bmp, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB5A3);

                var chrsel = MenuFile.Roots[0].Data as SBM_SelectChrDataTable;

                InjectCSSIconTexture(chrsel.MenuModel, chrsel.MenuAnimation, chrsel.MenuMaterialAnimation, tobj, 17, true);
                InjectCSSIconTexture(chrsel.SingleMenuModel, chrsel.SingleMenuAnimation, chrsel.SingleMenuMaterialAnimation, tobj, 13, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="tobj"></param>
        /// <param name="jobjSection"></param>
        /// <param name="setSelectedIcon"></param>
        private void InjectCSSIconTexture(HSD_JOBJ menu, HSD_AnimJoint anm, HSD_MatAnimJoint matAnm, HSD_TOBJ tobj, int jobjSection, bool setSelectedIcon)
        {
            var iconAnimClone = HSDAccessor.DeepClone<HSD_AnimJoint>(anm.Children[2].Children[0]);

            var iconMatAnimClone = HSDAccessor.DeepClone<HSD_MatAnimJoint>(matAnm.Children[2].Children[0]);

            var iconClone = HSDAccessor.DeepClone<HSD_JOBJ>(menu.Children[2].Children[0]);
            iconClone.Next = null;
            iconClone.Child = null;
            iconClone.TX = 0;
            iconClone.TY = 0;
            iconClone.TZ = 0;
            iconClone.Dobj.List[1].Mobj.Textures = tobj;

            if (menu.Children.Length < jobjSection)
                menu.AddChild(new HSD_JOBJ() { SX = 1, SY = 1, SZ = 1, Flags = JOBJ_FLAG.CLASSICAL_SCALING | JOBJ_FLAG.ROOT_XLU });
            menu.Children[jobjSection - 1].AddChild(iconClone);

            if (anm.Children.Length < jobjSection)
                anm.AddChild(new HSD_AnimJoint());
            anm.Children[jobjSection - 1].AddChild(iconAnimClone);

            if (matAnm.Children.Length < jobjSection)
                matAnm.AddChild(new HSD_MatAnimJoint());
            matAnm.Children[jobjSection - 1].AddChild(iconMatAnimClone);

            if (setSelectedIcon && cssIconEditor.SelectedObject is MEX_CSSIconEntry ico)
            {
                JOBJManager.ClearRenderingCache();
                JOBJManager.UpdateNoRender();
                addedIconEditor.AddItem(new AddedIcon(iconClone));
                iconClone.TX = ico.X;
                iconClone.TY = ico.Y;
                ico.JointID = JOBJManager.IndexOf(iconClone);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeIconButton_Click(object sender, EventArgs e)
        {
            if(cssIconTabControl.SelectedIndex == 1 && addedIconEditor.SelectedObject is AddedIcon ico)
            {
                var index = addedIconEditor.IndexOf(ico);

                addedIconEditor.RemoveAt(index);

                foreach (var v in Icons)
                    if (v.JointID > addedIconEditor.ItemIndexOffset + index)
                        v.JointID--;

                var chrsel = MenuFile.Roots[0].Data as SBM_SelectChrDataTable;

                chrsel.MenuModel.Children[17 - 1].RemoveChildAt(index);
                chrsel.MenuAnimation.Children[17 - 1].RemoveChildAt(index);
                chrsel.MenuMaterialAnimation.Children[17 - 1].RemoveChildAt(index);

                chrsel.SingleMenuModel.Children[13 - 1].RemoveChildAt(index);
                chrsel.SingleMenuAnimation.Children[13 - 1].RemoveChildAt(index);
                chrsel.SingleMenuMaterialAnimation.Children[13 - 1].RemoveChildAt(index);

                JOBJManager.ClearRenderingCache();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadMnSlChr(string filePath)
        {
            HSDRawFile hsd = new HSDRawFile(filePath);

            if (hsd.Roots[0].Data is SBM_SelectChrDataTable tab)
            {
                MenuFilePath = filePath;
                MenuFile = hsd;

                LoadModel(tab.MenuModel);
                JOBJManager.SetAnimJoint(tab.MenuAnimation);
                JOBJManager.Frame = 600;
            }
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
                LoadMnSlChr(f);
            }
        }

        #endregion

        #region Fighter Controls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cloneButton_Click(object sender, EventArgs e)
        {
            if(fighterList.SelectedItem is MEXEntry me)
            {
                var clone = ObjectExtensions.Copy(me);
                // give unique name
                int clnIndex = 0;
                if (NameExists(clone.NameText))
                {
                    while (NameExists(clone.NameText + " " + clnIndex.ToString())) clnIndex++;
                    clone.NameText = clone.NameText + " " + clnIndex;
                }
                AddEntry(clone);
            }
        }

        /// <summary>
        /// Returns true if any fighter already uses given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NameExists(string name)
        {
            foreach(var v in FighterEntries)
            {
                if (v.NameText.Equals(name))
                    return true;
            }
            return false;
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
                    mex.Serialize(f);
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
                FighterEntries.Insert(FighterEntries.Count - 6, MEXEntry.DeserializeFile(f));
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
            if (IsExtendedFighter(selected))
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
        private bool IsExtendedFighter(int index)
        {
            return (index >= 0x21 - 6 && index < FighterEntries.Count - 6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsSpecialFighter(int index)
        {
            return (index >= FighterEntries.Count - 6);
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

                var brush = ApplicationSettings.SystemWindowTextColorBrush;
                
                if (IsExtendedFighter(e.Index))
                    brush = Brushes.DarkViolet;

                if (IsSpecialFighter(e.Index))
                    brush = ApplicationSettings.SystemWindowTextRedColorBrush;

                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, brush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }

        #endregion

        #region Music

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        private void PlayMusicFromName(string str)
        {
            var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{str}");
            
            musicDSPPlayer.DSP = null;
            if (File.Exists(path))
            {
                musicDSPPlayer.SoundName = str;
                var dsp = new Sound.DSP();
                dsp.FromFile(path);
                musicDSPPlayer.DSP = dsp;
                musicDSPPlayer.PlaySound();
            }
            else
            {
                MessageBox.Show("Could not find sound \"" + str + "\"", "Sound Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicListEditor_DoubleClicked(object sender, EventArgs e)
        {
            if(musicListEditor.SelectedObject is HSD_String str)
            {
                PlayMusicFromName(str.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuPlaylistEditor_DoubleClick(object sender, EventArgs e)
        {
            if (menuPlaylistEditor.SelectedObject is MEXPlaylistEntry str)
            {
                PlayMusicFromName(Music[str.MusicID].Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicListEditor_ArrayUpdated(object sender, EventArgs e)
        {
            var oldValues = MEXConverter.musicIDValues.ToArray();
            MEXConverter.musicIDValues.Clear();
            MEXConverter.musicIDValues.AddRange(Music.Select(r => r.Value));

            if(oldValues != null && oldValues.Length == Music.Length)
            {
                for(int i = 0; i < oldValues.Length; i++)
                {
                    if(oldValues[i] != Music[i].Value)
                    {
                        var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{oldValues[i]}");
                        var newpath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{Music[i].Value}");
                        
                        if (File.Exists(path) && !File.Exists(newpath))
                        {
                            if(MessageBox.Show($"Rename {oldValues[i]} to {Music[i].Value}?", "Rename File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                File.Move(path, newpath);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createHPSButton_Click(object sender, EventArgs e)
        {
            var fs = Tools.FileIO.OpenFiles(DSP.SupportedImportFilter);

            if(fs != null)
            {
                var audioPath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "audio\\");

                if (!Directory.Exists(audioPath))
                    Directory.CreateDirectory(audioPath);

                foreach(var f in fs)
                {
                    var dsp = new DSP();
                    dsp.FromFile(f);
                    HPS.SaveDSPAsHPS(dsp, Path.Combine(audioPath, Path.GetFileNameWithoutExtension(f) + ".hps"));
                    var newHPSName = Path.GetFileNameWithoutExtension(f) + ".hps";
                    foreach (var v in Music)
                        if (v.Value.Equals(newHPSName))
                            return;
                    musicListEditor.AddItem(new HSD_String() { Value = newHPSName });
                }
            }
        }

        #endregion

        #region Items

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mexItemCloneButton_Click(object sender, EventArgs e)
        {
            if(itemTabs.SelectedIndex == 0)
                mexItemEditor.AddItem(commonItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 1)
                mexItemEditor.AddItem(fighterItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 2)
                mexItemEditor.AddItem(pokemonItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 3)
                mexItemEditor.AddItem(stageItemEditor.SelectedObject);
        }




        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
            if (cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                if (button == MouseButtons.Right && MousePrevDown)
                {
                    icon.X = oldPosition.X;
                    icon.Y = oldPosition.Y;
                    MousePrevDown = false;
                }
            }
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);
            foreach (var i in Icons)
            {
                if(i.ToRect().Contains(planePoint.X, planePoint.Y))
                {
                    cssIconEditor.SelectObject(i);
                    break;
                }
            }
        }

        private bool MousePrevDown = false;
        private Vector3 prevPlanePoint = Vector3.Zero;
        private Vector2 oldPosition = Vector2.Zero;
        public void ScreenDrag(PickInformation pick, float deltaX, float deltaY)
        {
            var mouseDown = OpenTK.Input.Mouse.GetState().IsButtonDown(OpenTK.Input.MouseButton.Left);
            
            if (cssIconEditor.SelectedObject is MEX_CSSIconEntry icon)
            {
                if (mouseDown &&
                    viewport.IsAltAction)
                {
                    var planePoint = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);
                    if (!MousePrevDown)
                    {
                        oldPosition = new Vector2(icon.X, icon.Y);
                        prevPlanePoint = planePoint;
                    }
                    if (icon.ToRect().Contains(prevPlanePoint.X, prevPlanePoint.Y))
                    {
                        icon.X -= prevPlanePoint.X - planePoint.X;
                        icon.Y -= prevPlanePoint.Y - planePoint.Y;
                        SnapAlignIcon(icon);
                    }
                    prevPlanePoint = planePoint;
                }
            }

            MousePrevDown = mouseDown;
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {

        }

        private float SnapDelta = 0.15f;

        private void SnapAlignIcon(MEX_CSSIconEntry icon)
        {
            foreach(var i in Icons)
            {
                if (i == icon)
                    continue;

                // if distance between part of rect is less than threshold, snap to it
                if (Math.Abs(icon.X - (i.X + i.Width)) < SnapDelta) icon.X = i.X + i.Width;
                if (Math.Abs(icon.X - i.X) < SnapDelta) icon.X = i.X;

                if (Math.Abs((icon.X + icon.Width) - (i.X + i.Width)) < SnapDelta) icon.X = i.X + i.Width - icon.Width;
                if (Math.Abs((icon.X + icon.Width) - i.X) < SnapDelta) icon.X = i.X - icon.Width;

                if (Math.Abs(icon.Y - (i.Y - i.Height)) < SnapDelta) icon.Y = i.Y - i.Height;
                if (Math.Abs(icon.Y - i.Y) < SnapDelta) icon.Y = i.Y;

                if (Math.Abs((icon.Y - icon.Height) - (i.Y - i.Height)) < SnapDelta) icon.Y = i.Y - i.Height + icon.Height;
                if (Math.Abs((icon.Y - icon.Height) - i.Y) < SnapDelta) icon.Y = i.Y + icon.Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemExportButton_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("Item (*.yaml)|*.yaml");
            if(f != null)
            {
                if (itemTabs.SelectedIndex == 0 && commonItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, commonItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 1 && fighterItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, fighterItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 2 && pokemonItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, pokemonItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 3 && stageItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, stageItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 4 && mexItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, mexItemEditor.SelectedObject as MEX_Item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MEX_Item Deserialize(string data)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .Build();

            return deserializer.Deserialize<MEX_Item>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Serialize(string filepath, MEX_Item item)
        {
            var builder = new SerializerBuilder()
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .WithNamingConvention(CamelCaseNamingConvention.Instance);

            using (StreamWriter writer = File.CreateText(filepath))
            {
                builder.Build().Serialize(writer, item);
            }
        }

        #endregion

        private void installFighterButton_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Fighter Package (*.zip)|*.zip");
            if(f != null)
            {
                bool openMenuFile = MenuFile != null;
                UnloadMenuFile(); // for editing

                using (ProgressBarDisplay d = new ProgressBarDisplay (new FighterPackageInstaller(f, this)))
                {
                    d.DoWork();
                    d.ShowDialog();
                }

                FighterEntries.ResetBindings();
                mexItemEditor.ResetBindings();
                effectEditor.ResetBindings();
                musicListEditor.ResetBindings();

                MEXConverter.ssmValues.Clear();
                MEXConverter.ssmValues.AddRange(_data.SSMTable.SSM_SSMFiles.Array.Select(s => s.Value));
                MessageBox.Show("Fighter installed");
                saveAllChangesButton_Click(null, null);
                MainForm.Instance.SaveDAT();
            }
        }

        private void uninstallFighterButton_Click(object sender, EventArgs e)
        {
            if(IsExtendedFighter(fighterList.SelectedIndex) && fighterList.SelectedItem is MEXEntry en)
            {
                using (ProgressBarDisplay d = new ProgressBarDisplay(new FighterPackageUninstaller(fighterList.SelectedIndex, en, this)))
                {
                    d.DoWork();
                    d.ShowDialog();
                }
                
                FighterEntries.ResetBindings();
                mexItemEditor.ResetBindings();
                effectEditor.ResetBindings();
                musicListEditor.ResetBindings();

                MEXConverter.ssmValues.Clear();
                MEXConverter.ssmValues.AddRange(_data.SSMTable.SSM_SSMFiles.Array.Select(s => s.Value));
                MessageBox.Show("Fighter uninstalled");
                saveAllChangesButton_Click(null, null);
                MainForm.Instance.SaveDAT();
            }
        }

        /// <summary>
        /// Adds new entry to fighter list
        /// </summary>
        /// <param name="e"></param>
        /// <returns>internal ID</returns>
        public int AddEntry(MEXEntry e)
        {
            FighterEntries.Insert(FighterEntries.Count - 6, e);
            return FighterEntries.Count - 6;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="internalID"></param>
        public void RemoveFighterEntry(int internalID)
        {
            FighterEntries.RemoveAt(internalID);
        }

        /// <summary>
        /// Adds a new MEX item to table
        /// </summary>
        /// <param name="item"></param>
        /// <returns>added mex item id</returns>
        public int AddMEXItem(string yaml)
        {
            return AddMEXItem(Deserialize(yaml));
        }

        /// <summary>
        /// Adds a new MEX item to table
        /// </summary>
        /// <param name="item"></param>
        /// <returns>added mex item id</returns>
        public int AddMEXItem(MEX_Item item)
        {
            mexItemEditor.AddItem(item);

            return MEXItemOffset + ItemMEX.Length - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void RemoveMEXItem(int index)
        {
            // only remove if index is in range of mex item
            if (index < MEXItemOffset)
                return;
            
            mexItemEditor.RemoveAt(index - MEXItemOffset);
        }

        /// <summary>
        /// Adds a new MEX effect file to table
        /// </summary>
        /// <param name="item"></param>
        /// <returns>added mex file id</returns>
        public int AddMEXEffectFile(MEX_EffectEntry item)
        {
            effectEditor.AddItem(item);
            return Effects.Length - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SafeRemoveEffectFile(int index)
        {
            bool inUse = FighterEntries.Any(e => e.EffectIndex == index || e.KirbyEffectID == index);
            if(!inUse)
                effectEditor.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        public int AddMusic(HSD_String musicName)
        {
            musicListEditor.AddItem(musicName);
            return Music.Length - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="musicName"></param>
        public void RemoveMusicAt(int index)
        {
            musicListEditor.RemoveAt(index);
        }
    }
}
