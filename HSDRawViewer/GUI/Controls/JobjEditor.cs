using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Converters;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;
using HSDRaw;
using HSDRawViewer.Tools;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using HSDRaw.GX;
using HSDRawViewer.Rendering.Animation;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.GX;
using HSDRawViewer.Converters.Animation;
using HSDRaw.Tools;

namespace HSDRawViewer.GUI.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JobjEditor : UserControl, IDrawable
    {
        public DrawOrder DrawOrder => DrawOrder.First;

        private JointMap _jointMap = new JointMap();

        private ViewportControl viewport;

        private PopoutJointAnimationEditor jointAnimEditor = new PopoutJointAnimationEditor(false);

        /// <summary>
        /// 
        /// </summary>
        public JobjEditor()
        {
            InitializeComponent();

            JointManager = new JOBJManager();

            showBonesToolStripMenuItem.Checked = JointManager._settings.RenderBones;
            showMeshToolStripMenuItem.Checked = JointManager._settings.RenderObjects;
            showBoneOrientationToolStripMenuItem.Checked = JointManager._settings.RenderOrientation;
            showSelectionOutlineToolStripMenuItem.Checked = JointManager._settings.OutlineSelected;

            renderModeBox.ComboBox.DataSource = Enum.GetValues(typeof(RenderMode));
            renderModeBox.SelectedIndex = 0;

            listDOBJ.DataSource = dobjList;

            toolStripComboBox2.SelectedIndex = 0;

            treeJOBJ.AfterSelect += (sender, args) =>
            {
                propertyGrid1.SelectedObject = treeJOBJ.SelectedNode.Tag;
                JointManager.SelectedJOBJ = treeJOBJ.SelectedNode.Tag as HSD_JOBJ;
            };

            listDOBJ.SelectedIndexChanged += (sender, args) => {
                propertyGrid1.SelectedObject = listDOBJ.SelectedItem;
                JointManager.DOBJManager.SelectedDOBJ = ((listDOBJ.SelectedItem as DOBJContainer)?.DOBJ);

                materialDropDownButton1.Enabled = listDOBJ.SelectedItems.Count == 1;
                buttonMoveDown.Enabled = materialDropDownButton1.Enabled;
                buttonMoveUp.Enabled = materialDropDownButton1.Enabled;
            };

            propertyGrid1.PropertyValueChanged += (sender, args) =>
            {
                // refresh
                listDOBJ.SelectedItem = listDOBJ.SelectedItem;
            };

            //listDOBJ.SelectionMode = SelectionMode.MultiExtended;

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            viewport.EnableCSPMode = true;
            previewBox.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();

            Disposed += (sender, args) =>
            {
                jointAnimEditor.Dispose();
                if (PluginManager.GetCommonViewport() != null)
                {
                    if (PluginManager.GetCommonViewport() != null)
                    {
                        PluginManager.GetCommonViewport().AnimationTrackEnabled = false;
                        PluginManager.GetCommonViewport().RemoveRenderer(this);
                    }
                }
                viewport.Dispose();
                JointManager.CleanupRendering();
            };
        }

        private List<IDrawable> _drawables = new List<IDrawable>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="draw"></param>
        public void AddDrawable(IDrawable draw)
        {
            _drawables.Add(draw);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="draw"></param>
        public void RemoveDrawable(IDrawable draw)
        {
            _drawables.Remove(draw);
        }

        private Dictionary<HSD_JOBJ, int> jobjToIndex = new Dictionary<HSD_JOBJ, int>();

        private class DOBJContainer
        {
            public enum CullMode
            {
                None,
                Front,
                Back,
                FrontAndBack
            }

            public int Index;
            public int JOBJIndex;
            public int DOBJIndex;
            public HSD_JOBJ ParentJOBJ;
            public HSD_DOBJ DOBJ;

            public string Name { get => DOBJ.ClassName; set => DOBJ.ClassName = value; }

            [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public RENDER_MODE MOBJFlags { get => DOBJ.Mobj.RenderFlags; set => DOBJ.Mobj.RenderFlags = value; }

            public Color Ambient { get => DOBJ.Mobj.Material.AmbientColor; set => DOBJ.Mobj.Material.AmbientColor = value; }

            public Color Diffuse { get => DOBJ.Mobj.Material.DiffuseColor; set => DOBJ.Mobj.Material.DiffuseColor = value; }

            public Color Specular { get => DOBJ.Mobj.Material.SpecularColor; set => DOBJ.Mobj.Material.SpecularColor = value; }

            public float Shinniness { get => DOBJ.Mobj.Material.Shininess; set => DOBJ.Mobj.Material.Shininess = value; }

            public float Alpha { get => DOBJ.Mobj.Material.Alpha; set => DOBJ.Mobj.Material.Alpha = value; }
            
            public CullMode Culling
            {
                get
                {
                    if (DOBJ.Pobj == null)
                        return CullMode.None;
                    else
                    {
                        if (DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLBACK) && DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLFRONT))
                            return CullMode.FrontAndBack;

                        if (DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLBACK))
                            return CullMode.Back;

                        if (DOBJ.Pobj.Flags.HasFlag(POBJ_FLAG.CULLFRONT))
                            return CullMode.Front;

                        return CullMode.None;
                    }
                }
                set
                {
                    if(DOBJ.Pobj != null)
                        foreach (var p in DOBJ.Pobj.List)
                        {
                            p.Flags &= ~(POBJ_FLAG.CULLBACK | POBJ_FLAG.CULLFRONT);
                            switch (value)
                            {
                                case CullMode.FrontAndBack:
                                    p.Flags |= POBJ_FLAG.CULLBACK | POBJ_FLAG.CULLFRONT;
                                    break;
                                case CullMode.Front:
                                    p.Flags |= POBJ_FLAG.CULLFRONT;
                                    break;
                                case CullMode.Back:
                                    p.Flags |= POBJ_FLAG.CULLBACK;
                                    break;
                            }
                        }
                }
            }

            [Browsable(false)]
            public int PolygonCount { get => DOBJ.Pobj != null ? DOBJ.Pobj.List.Count : 0; }

            [Browsable(false)]
            public int TextureCount { get => DOBJ.Mobj == null ? -1 : (DOBJ.Mobj.Textures != null ? DOBJ.Mobj.Textures.List.Count : 0); }

            [Browsable(false)]
            public bool HasPixelProcessing { get => DOBJ.Mobj?.PEDesc != null; }

            [Browsable(false)]
            public bool HasTEV
            {
                get
                {
                    if(DOBJ.Mobj != null && DOBJ.Mobj.Textures != null)
                    {
                        if (DOBJ.Mobj.Textures.List.Any(e => e.TEV != null))
                            return true;
                    }
                    return false;
                }
            }
            
            [Browsable(false)]
            public bool HasMaterialColor { get => DOBJ.Mobj?.Material != null; }

            public override string ToString()
            {
                return $"{Index}. Joint {JOBJIndex} : Object {DOBJIndex} : Polygons {PolygonCount} : Textures {TextureCount} {Name}";
            }
        }

        private BindingList<DOBJContainer> dobjList = new BindingList<DOBJContainer>();

        public JOBJManager JointManager { get; internal set; }

        private HSD_JOBJ root;

        public void SetJOBJ(HSD_JOBJ jobj)
        {
            root = jobj;
            JointManager.SetJOBJ(root);
            RefreshGUI();
        }

        /// <summary>
        /// Refreshes and reloads the jobj data
        /// </summary>
        private void RefreshGUI()
        {
            treeJOBJ.Nodes.Clear();
            dobjList.Clear();
            jobjToIndex.Clear();

            listDOBJ.BeginUpdate();
            treeJOBJ.BeginUpdate();
            LoadJOBJ(root);
            listDOBJ.EndUpdate();
            treeJOBJ.EndUpdate();

            treeJOBJ.ExpandAll();

            JointManager.DOBJManager.HiddenDOBJs.Clear();
            CheckAll();
        }

        private int index = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="parent"></param>
        private void LoadJOBJ(HSD_JOBJ jobj, TreeNode parent = null)
        {
            if (parent == null)
                index = 0;
            TreeNode tree = new TreeNode();

            if (_jointMap[index] != null)
                tree.Text = _jointMap[index];
            else
            if (!string.IsNullOrEmpty(jobj.ClassName))
                tree.Text = $"(Joint_{index})" + jobj.ClassName;
            else
                tree.Text = "Joint_" + index;
            index++;
            tree.Tag = jobj;

            jobjToIndex.Add(jobj, jobjToIndex.Count);

            if(jobj.Dobj != null)
            {
                int dobjIndex = 0;
                foreach (var dobj in jobj.Dobj.List)
                {
                    dobjList.Add(new DOBJContainer() {Index = dobjList.Count, DOBJ = dobj, ParentJOBJ = jobj, DOBJIndex = dobjIndex++, JOBJIndex = jobjToIndex[jobj] } );
                }
            }

            if (parent == null)
                treeJOBJ.Nodes.Add(tree);
            else
                parent.Nodes.Add(tree);

            foreach(var c in jobj.Children)
            {
                LoadJOBJ(c, tree);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        private bool IsSelected(object ob)
        {
            foreach (var v in propertyGrid1.SelectedObjects)
                if (v == ob)
                    return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fog"></param>
        public void SetFog(HSD_FogDesc fog)
        {
            JointManager._fogParam.LoadFromHSD(fog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(HSD_Camera camera)
        {
            // TODO: load camera
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        public void LoadAnimation(JointAnimManager animation)
        {
            var vp = viewport;
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = animation.FrameCount;
            JointManager.Animation = animation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void LoadAnimation(HSD_MatAnimJoint joint)
        {
            JointManager.SetMatAnimJoint(joint);
            var vp = viewport;
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = JointManager.MatAnimation.FrameCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        public void LoadAnimation(HSD_ShapeAnimJoint animation)
        {
            JointManager.SetShapeAnimJoint(animation);
            var vp = viewport;
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = JointManager.ShapeAnimation.FrameCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motFiles"></param>
        public void LoadAnimation(short[] jointTable, MOT_FILE motFile)
        {
            var vp = viewport; 
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = motFile.EndTime * 60;
            JointManager.SetMOT(jointTable, motFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            JointManager.Frame = viewport.Frame;
            JointManager._settings.RenderBones = showBonesToolStripMenuItem.Checked;
            JointManager._settings.RenderOrientation = showBoneOrientationToolStripMenuItem.Checked;
            JointManager._settings.RenderObjects = showMeshToolStripMenuItem.Checked;
            JointManager._settings.OutlineSelected = showSelectionOutlineToolStripMenuItem.Checked;
            JointManager.Render(cam);

            foreach (var d in _drawables)
                d.Draw(cam, windowWidth, windowHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = null;

            if(tabControl1.SelectedIndex == 2)
                LoadTextureList();
            else
                UnloadTextureList();
        }

        public class TextureListProxy : ImageArrayItem
        {
            private List<HSD_TOBJ> tobjs = new List<HSD_TOBJ>();

            //private Bitmap PreviewImage;

            public int _hash;

            public string Hash { get => _hash.ToString("X"); }

            public GXTexFmt ImageFormat { get => tobjs[0].ImageData.Format; }

            public GXTlutFmt PaletteFormat
            {
                get
                {
                    if (tobjs[0].TlutData == null)
                        return GXTlutFmt.IA8;

                    return tobjs[0].TlutData.Format;
                }
            }

            public int Width
            {
                get
                {
                    if (tobjs[0].ImageData == null)
                        return 0;

                    return tobjs[0].ImageData.Width;
                }
            }

            public int Height
            {
                get
                {
                    if (tobjs[0].ImageData == null)
                        return 0;

                    return tobjs[0].ImageData.Height;
                }
            }

            public TextureListProxy(int hash)
            {
                _hash = hash;
            }

            public void Dispose()
            {
                //PreviewImage.Dispose();
            }

            public override string ToString()
            {
                return $"References: {tobjs.Count}";
            }

            public void AddTOBJ(HSD_TOBJ tobj)
            {
                //if (PreviewImage == null)
                //    PreviewImage = TOBJConverter.ToBitmap(tobj);
                tobjs.Add(tobj);
            }

            public void Replace(HSD_TOBJ newTOBJ)
            {
                foreach (var t in tobjs)
                {
                    if (newTOBJ.ImageData != null)
                    {
                        if (t.ImageData == null)
                            t.ImageData = new HSD_Image();
                        t.ImageData.ImageData = newTOBJ.ImageData.ImageData;
                        t.ImageData.Format = newTOBJ.ImageData.Format;
                        t.ImageData.Width = newTOBJ.ImageData.Width;
                        t.ImageData.Height = newTOBJ.ImageData.Height;
                    }
                    else
                        t.ImageData = null;

                    if (newTOBJ.TlutData != null)
                    {
                        if (t.TlutData == null)
                            t.TlutData = new HSD_Tlut();
                        t.TlutData.TlutData = newTOBJ.TlutData.TlutData;
                        t.TlutData.Format = newTOBJ.TlutData.Format;
                    }
                    else
                        t.TlutData = null;

                    //if(PreviewImage != null)
                    //    PreviewImage.Dispose();
                    //PreviewImage = TOBJConverter.ToBitmap(newTOBJ);
                }
            }

            public Image ToImage()
            {
                if (tobjs.Count > 0)
                    return TOBJConverter.ToBitmap(tobjs[0]);
                return null;
            }
        }

        public TextureListProxy[] TextureLists { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private void LoadTextureList()
        {
            UnloadTextureList();

            var tex = new List<TextureListProxy>();
            
            foreach(var jobj in root.BreathFirstList)
            {
                if (jobj.Dobj == null)
                    continue;

                foreach(var dobj in jobj.Dobj.List)
                {
                    if (dobj.Mobj == null || dobj.Mobj.Textures == null)
                        continue;

                    foreach(var tobj in dobj.Mobj.Textures.List)
                    {
                        var hash = HSDRawFile.ComputeHash(tobj.GetDecodedImageData());

                        var proxy = tex.Find(e => e._hash == hash);

                        if(proxy == null)
                        {
                            proxy = new TextureListProxy(hash);
                            tex.Add(proxy);
                        }

                        proxy.AddTOBJ(tobj);
                    }
                }
            }

            TextureLists = tex.ToArray();
            textureArrayEditor.SetArrayFromProperty(this, "TextureLists");
        }

        /// <summary>
        /// 
        /// </summary>
        private void UnloadTextureList()
        {
            if(TextureLists != null)
            {
                foreach (var t in TextureLists)
                    t.Dispose();
                TextureLists = new TextureListProxy[0];
                textureArrayEditor.SetArrayFromProperty(this, "TextureLists");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void TakeScreenShot()
        {
            Show();
            viewport.TakeScreenShot = true;
            viewport.ForceDraw();
        }

        /// <summary>
        /// Render Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(toolStripComboBox2.SelectedIndex == 0)
            {
                JointManager._settings.RenderObjects = true;
                JointManager.DOBJManager.OnlyRenderSelected = false;
            }
            if (toolStripComboBox2.SelectedIndex == 1)
            {
                JointManager._settings.RenderObjects = true;
                JointManager.DOBJManager.OnlyRenderSelected = true;
            }
            if (toolStripComboBox2.SelectedIndex == 2)
            {
                JointManager._settings.RenderObjects = false;
                JointManager.DOBJManager.OnlyRenderSelected = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            using (MOBJEditor m = new MOBJEditor())
            {
                m.SetMOBJ((listDOBJ.SelectedItem as DOBJContainer).DOBJ.Mobj);

                if (m.ShowDialog() != DialogResult.OK)
                {
                    dobjList[listDOBJ.SelectedIndex] = dobjList[listDOBJ.SelectedIndex];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vertexColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //JOBJManager.DOBJManager.RenderVertexColor = renderVertexColorsToolStripMenuItem.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importModelFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelImporter.ReplaceModelFromFile(root);
            JointManager.RefreshRendering = true;
            _jointMap.Clear();
            RefreshGUI();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportModelToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelExporter.ExportFile(root, _jointMap);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainRender_CheckStateChanged(object sender, EventArgs e)
        {
            if (showInViewportToolStripMenuItem.Checked)
            {
                PluginManager.GetCommonViewport().AddRenderer(this);
            }
            else
            {
                PluginManager.GetCommonViewport().RemoveRenderer(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listDOBJ_BindingContextChanged(object sender, EventArgs e)
        {
            CheckAll();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckAll()
        {
            for (int i = 0; i < listDOBJ.Items.Count; i++)
            {
                listDOBJ.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListDOBJ_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            List<DOBJContainer> checkedItems = new List<DOBJContainer>();

            foreach (object item in listDOBJ.Items)
            {
                if (!listDOBJ.CheckedItems.Contains(item))
                {
                    checkedItems.Add(item as DOBJContainer);
                }
            }

            if (e.NewValue != CheckState.Checked)
                checkedItems.Add(listDOBJ.Items[e.Index] as DOBJContainer);
            else
                checkedItems.Remove(listDOBJ.Items[e.Index] as DOBJContainer);
            
            JointManager.DOBJManager.HiddenDOBJs.Clear();
            foreach(var i in checkedItems)
            {
                JointManager.DOBJManager.HiddenDOBJs.Add(i.DOBJ);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DummyDOBJSetting
        {
            [DisplayName("Number to Generate"), Description("")]
            public int NumberToGenerate { get; set; } = 1;

            [DisplayName("Add Dummy Texture"), Description("")]
            public bool AddDummyTexture { get; set; } = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDummyDOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var setting = new DummyDOBJSetting();

            using (PropertyDialog d = new PropertyDialog("Add Dummy DOBJs", setting))
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    Bitmap dummy = null;

                    if (setting.AddDummyTexture)
                        dummy = new Bitmap(8, 8);

                    for (int i = 0; i < setting.NumberToGenerate; i++)
                    {
                        var dobj = new HSD_DOBJ()
                        {
                            Mobj = new HSD_MOBJ()
                            {
                                RenderFlags = RENDER_MODE.CONSTANT,
                                Material = new HSD_Material()
                                {
                                    DiffuseColor = Color.White,
                                    SpecularColor = Color.White,
                                    AmbientColor = Color.White,
                                    Shininess = 50
                                }
                            }
                        };

                        if (setting.AddDummyTexture)
                        {
                            dobj.Mobj.RenderFlags |= RENDER_MODE.TEX0;
                            dobj.Mobj.Textures = TOBJConverter.BitmapToTOBJ(dummy, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);
                            dobj.Mobj.Textures.Optimize();
                        }

                        root.Dobj.Add(dobj);
                    }

                    if (dummy != null)
                        dummy.Dispose();
                }
            }

            RefreshGUI();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is DOBJContainer con)
            {
                if (con.DOBJ.Next == null)
                    return;

                var newIndex = listDOBJ.SelectedIndex + 1;

                HSD_DOBJ prev = null;
                foreach(var dobj in con.ParentJOBJ.Dobj?.List)
                {
                    if(dobj._s == con.DOBJ._s)
                    {
                        // totally not confusing
                        if (prev == null)
                            con.ParentJOBJ.Dobj = con.DOBJ.Next;
                        else
                            prev.Next = con.DOBJ.Next;
                        var newNext = con.DOBJ.Next.Next;
                        con.DOBJ.Next.Next = con.DOBJ;
                        con.DOBJ.Next = newNext;
                        break;
                    }
                    prev = dobj;
                }

                RefreshGUI();
                listDOBJ.SelectedIndex = newIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is DOBJContainer con)
            {
                var newIndex = listDOBJ.SelectedIndex - 1;

                HSD_DOBJ prev = null;
                HSD_DOBJ prevprev = null;
                foreach (var dobj in con.ParentJOBJ.Dobj?.List)
                {
                    if (dobj._s == con.DOBJ._s)
                    {
                        if (prev == null)
                            break;
                        
                        // totally not confusing
                        if (prevprev == null)
                            con.ParentJOBJ.Dobj = con.DOBJ;
                        else
                            prevprev.Next = con.DOBJ;
                        prev.Next = con.DOBJ.Next;
                        con.DOBJ.Next = prev;
                        break;
                    }
                    prevprev = prev;
                    prev = dobj;
                }

                RefreshGUI();
                listDOBJ.SelectedIndex = newIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importBoneLabelINIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Label INI (*.ini)|*.ini");

            if(f != null)
            {
                _jointMap.Load(f);
            }

            RefreshGUI();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createOutlineMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is DOBJContainer con)
            {
                var newDOBJ = OutlineGenerator.GenerateOutlineMesh(con.DOBJ);
                
                if(newDOBJ != null)
                {
                    con.ParentJOBJ.Dobj.Add(newDOBJ);

                    RefreshGUI();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAllPOBJsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure?\nThis will clear all polygons in the model\n and cannot be undone", "Clear POBJs", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DOBJContainer v in listDOBJ.Items)
                {
                    v.DOBJ.Pobj = null;
                }
                JointManager.RefreshRendering = true;
                RefreshGUI();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDOBJDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedDOBJs();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteSelectedDOBJs()
        {
            if (listDOBJ.SelectedItems.Count > 0 && MessageBox.Show("Are you sure?\nThis cannot be undone", "Delete Object?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DOBJContainer dobj in listDOBJ.SelectedItems)
                {
                    /*if (listDOBJ.CheckedItems.Contains(dobj))
                        continue;*/

                    var dobjs = dobj.ParentJOBJ.Dobj.List;

                    HSD_DOBJ prev = null;
                    foreach (var d in dobjs)
                    {
                        if (d == dobj.DOBJ)
                        {
                            if (prev == null)
                                dobj.ParentJOBJ.Dobj = d.Next;
                            else
                                prev.Next = d.Next;
                            break;
                        }
                        prev = d;
                    }
                }

                JointManager.RefreshRendering = true;
                RefreshGUI();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAnimButton_Click(object sender, EventArgs e)
        {
            viewport.AnimationTrackEnabled = false;
            JointManager.Animation = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("FigaTree/AnimJoint/MayaAnim/EightingMOT (*.dat*.anim*.mota*.gnta*.xml)|*.dat;*.anim;*.mota;*.gnta;*.chr0;*.xml");

            if (f != null)
            {
                if (Path.GetExtension(f).ToLower().Equals(".mota") || Path.GetExtension(f).ToLower().Equals(".gnta") ||
                    (Path.GetExtension(f).ToLower().Equals(".xml") && MOT_FILE.IsMotXML(f)))
                {
                    var jointTable = Tools.FileIO.OpenFile("Joint Connector Value (*.jcv)|*.jcv");

                    if(jointTable != null)
                        LoadAnimation(MOTLoader.GetJointTable(jointTable), new MOT_FILE(f));
                }
                else
                {
                    LoadAnimation(JointAnimationLoader.LoadJointAnimFromFile(_jointMap, f));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportAsANIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mayaANIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JointManager.Animation == null || JointManager.Animation.NodeCount == 0)
            {
                MessageBox.Show("No animation is loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var f = Tools.FileIO.SaveFile("Supported Formats (*.anim)|*.anim");

            if (f != null)
            {
                ConvMayaAnim.ExportToMayaAnim(f, JointManager.Animation, _jointMap);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FigaTreeSettings
        {
            [DisplayName("Symbol Name"), Description("Name of animation used by the game")]
            public string Symbol { get; set; } = "_figatree";

            [DisplayName("Compression Error"), Description("A larger value will make a smaller file but with loss of accuracy")]
            public float CompressionError { get; set; } = 0.0001f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void figaTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JointManager.Animation == null || JointManager.Animation.NodeCount == 0)
            {
                MessageBox.Show("No animation is loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);
            var setting = new FigaTreeSettings();

            using (PropertyDialog d = new PropertyDialog("Figatree Settings", setting))
                if (f != null && d.ShowDialog() == DialogResult.OK)
                {
                    HSDRawFile animFile = new HSDRawFile();
                    animFile.Roots.Add(new HSDRootNode()
                    {
                        Data = JointManager.Animation.ToFigaTree(setting.CompressionError),
                        Name = setting.Symbol
                    });
                    animFile.Save(f);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        public class AnimJointSettings
        {
            [DisplayName("Symbol Name"), Description("Should end in _animjoint")]
            public string Symbol { get; set; } = "_animjoint";

            [DisplayName("Flags"), Description("")]
            public AOBJ_Flags Flags { get; set; } = AOBJ_Flags.ANIM_LOOP;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animJointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JointManager.Animation == null || JointManager.Animation.NodeCount == 0)
            {
                MessageBox.Show("No animation is loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);

            var setting = new AnimJointSettings();

            using (PropertyDialog d = new PropertyDialog("AnimJoint Settings", setting))
                if (f != null && d.ShowDialog() == DialogResult.OK)
                {
                    HSDRawFile animFile = new HSDRawFile();
                    animFile.Roots.Add(new HSDRootNode()
                    {
                        Data = JointManager.Animation.ToAnimJoint(JointManager.GetJOBJ(0), setting.Flags),
                        Name = setting.Symbol
                    });
                    animFile.Save(f);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void motToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JointManager.Animation == null || !(JointManager.Animation is MotAnimManager))
            {
                MessageBox.Show("No mot animation is loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MotAnimManager anim = (MotAnimManager)(JointManager.Animation);

            var f = Tools.FileIO.SaveFile("EightingMOT|*.mota;*.gnta");

            MOT_FILE file = anim.GetMOT();
            file.Save(f);
        }

        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JointManager.Animation == null || !(JointManager.Animation is MotAnimManager))
            {
                MessageBox.Show("No mot animation is loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MotAnimManager anim = (MotAnimManager)(JointManager.Animation);
            var f = Tools.FileIO.SaveFile("XML|*.xml;");

            MOT_FILE file = anim.GetMOT();
            file.ExportXML(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renderModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(JointManager != null)
            {
                RenderMode mode = JointManager.RenderMode;
                Enum.TryParse<RenderMode>(renderModeBox.Text, out mode);
                JointManager.RenderMode = mode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class EditAnimationSettings
        {
            [Category("Trim Options"), DisplayName("Trim Animation"), Description("Trimming is done before scaling")]
            public bool TrimAnimation { get; set; }

            [Category("Trim Options"), DisplayName("Start Trim Frame"), Description("")]
            public int StartFrame { get; set; }

            [Category("Trim Options"), DisplayName("End  Trim Frame"), Description("")]
            public int EndFrame { get; set; }


            [Category("Scale Range"), DisplayName("Scale Factor"), Description("")]
            public float ScaleRangeFactor { get; set; } = 1;

            [Category("Scale Range"), DisplayName("Start Scale Frame"), Description("")]
            public int ScaleRangeStartFrame { get; set; }

            [Category("Scale Range"), DisplayName("End Scale Frame"), Description("")]
            public int ScaleRangeEndFrame { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(JointManager.Animation == null)
                return;

            var settings = new EditAnimationSettings();
            settings.EndFrame = (int)JointManager.Animation.FrameCount;
            settings.ScaleRangeEndFrame = (int)JointManager.Animation.FrameCount;
            using (PropertyDialog d = new PropertyDialog("Animation Edit Options", settings))
            {
                if(d.ShowDialog() == DialogResult.OK)
                {
                    // trim animation
                    if (settings.TrimAnimation)
                        JointManager.Animation.Trim(settings.StartFrame, settings.EndFrame);

                    // Scale animation
                    if (settings.ScaleRangeFactor != 1)
                        JointManager.Animation.ScaleBy(settings.ScaleRangeFactor, settings.ScaleRangeStartFrame, settings.ScaleRangeEndFrame);
                    
                    
                    // reload edited animation
                    LoadAnimation(JointManager.Animation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recalculateInverseBindsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JointManager.RecalculateInverseBinds();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceBonesFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if(f != null)
            {
                var file = new HSDRawFile(f);
                if(file.Roots.Count > 0 && file.Roots[0].Data is HSD_JOBJ jobj)
                {
                    JOBJManager temp = new JOBJManager();
                    temp.SetJOBJ(jobj);
                    temp.UpdateNoRender();

                    if(temp.JointCount == JointManager.JointCount)
                    {
                        for(int i = 0; i < temp.JointCount; i++)
                        {
                            var old = JointManager.GetJOBJ(i);
                            var n = temp.GetJOBJ(i);
                            old.TX = n.TX; old.TY = n.TY; old.TZ = n.TZ;
                            old.RX = n.RX; old.RY = n.RY; old.RZ = n.RZ;
                            old.SX = n.SX; old.SY = n.SY; old.SZ = n.SZ;

                            if (old.InverseWorldTransform != null)
                            {
                                if(n.InverseWorldTransform == null)
                                    old.InverseWorldTransform = temp.GetWorldTransform(n).Inverted().ToHsdMatrix();
                                else
                                    old.InverseWorldTransform = n.InverseWorldTransform;
                            }
                        }
                        MessageBox.Show("Skeleton Replaced");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is DOBJContainer con)
            {
                var f = Tools.FileIO.SaveFile("Material (*.mobj)|*.mobj");

                if (f != null)
                {
                    HSDRawFile file = new HSDRawFile();
                    file.Roots.Add(new HSDRootNode() { Name = "mobj", Data = con.DOBJ.Mobj });
                    file.Save(f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is DOBJContainer con)
            {
                var f = Tools.FileIO.OpenFile("Material (*.mobj)|*.mobj");

                if (f != null)
                {
                    HSDRawFile file = new HSDRawFile(f);

                    if (file.Roots.Count > 0)
                    {
                        var mobj = new HSD_MOBJ();
                        if(file.Roots[0].Data._s.Length >= mobj.TrimmedSize)
                        {
                            mobj._s = file.Roots[0].Data._s;
                            con.DOBJ.Mobj = mobj;
                        }
                    }

                    JointManager.RefreshRendering = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reorientSkeletonForFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Reorient Bones", "This makes changes to bones and cannot be undone", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                JOBJExtensions.ApplyMeleeFighterTransforms(root);
                JointManager.RecalculateInverseBinds();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportBoneLabelINIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("Label INI (*.ini)|*.ini");

            if (f != null)
                _jointMap.Save(f, root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoUpdateFlagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JOBJExtensions.UpdateJOBJFlags(root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewAnimationGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jointAnimEditor.SetJoint(root, JointManager.Animation);
            jointAnimEditor.Show();
            jointAnimEditor.WindowState = FormWindowState.Maximized;

            /*if (JOBJManager.Animation != null && treeJOBJ.SelectedNode.Tag is HSD_JOBJ joint)
            {
                var boneIndex = JOBJManager.IndexOf(joint);

                if (boneIndex < JOBJManager.Animation.NodeCount && boneIndex >= 0)
                {
                    var node = JOBJManager.Animation.Nodes[boneIndex];

                    using (PopoutGraphEditor editor = new PopoutGraphEditor(treeJOBJ.SelectedNode.Text, node))
                    {
                        editor.ShowDialog(this);
                    }
                }
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSelectedPOBJsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?\nThis will clear all polygons in the selected object\n and cannot be undone", "Clear Polygons", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (propertyGrid1.SelectedObject is DOBJContainer con)
                    con.DOBJ.Pobj = null;

                JointManager.RefreshRendering = true;

                RefreshGUI();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class AnimationCreationSettings
        {
            public int FrameCount { get; set; } = 60;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new AnimationCreationSettings();
            using (PropertyDialog d = new PropertyDialog("Create Animation", settings))
            {
                if (d.ShowDialog() == DialogResult.OK && settings.FrameCount > 0)
                {
                    JointManager.Animation = new JointAnimManager()
                    {
                        FrameCount = settings.FrameCount
                    };

                    for (int i = 0; i < JointManager.JointCount; i++)
                        JointManager.Animation.Nodes.Add(
                            new AnimNode()
                        {
                            Tracks = new List<HSDRaw.Tools.FOBJ_Player>()
                        }
                        );

                    var vp = viewport;
                    vp.AnimationTrackEnabled = true;
                    vp.Frame = 0;
                    vp.MaxFrame = settings.FrameCount;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JOBJExtensions.ImportTextures(root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsExportTextures_Click(object sender, EventArgs e)
        {
            JOBJExtensions.ExportTextures(root);
        }

        /// <summary>
        /// 
        /// </summary>
        public class SceneSettings
        {
            public float Frame { get; set; } = 0;

            public bool CSPMode { get; set; } = false;

            public bool ShowGrid { get; set; } = true;

            public bool ShowBackdrop { get; set; } = true;

            public Camera Camera { get; set; }

            public JobjDisplaySettings Settings { get; set; }

            public GXLightParam Lighting { get; set; }

            public JointAnimManager Animation { get; set; }

            public int[] HiddenNodes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public static SceneSettings Deserialize(string filePath)
            {
                var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

                return deserializer.Deserialize<SceneSettings>(File.ReadAllText(filePath));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filepath"></param>
            public void Serialize(string filepath)
            {
                var builder = new SerializerBuilder();
                builder.WithNamingConvention(CamelCaseNamingConvention.Instance);

                using (StreamWriter writer = File.CreateText(filepath))
                {
                    builder.Build().Serialize(writer, this);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSceneSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.SaveFile("Scene (*.yaml)|*.yml");

            if(f != null)
            {
                SceneSettings settings = new SceneSettings()
                {
                    Frame = viewport.Frame,
                    CSPMode = viewport.CSPMode,
                    ShowGrid = viewport.EnableFloor,
                    ShowBackdrop = viewport.EnableBack,
                    Camera = viewport.Camera,
                    Lighting = JointManager._lightParam,
                    Settings = JointManager._settings,
                    Animation = JointManager.Animation,
                    HiddenNodes = JointManager.GetHiddenDOBJIndices().ToArray()
                };
                settings.Serialize(f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importSceneSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile("Scene (*.yaml)|*.yml");

            if (f != null)
                LoadSceneYAML(f);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadSceneYAML(string filePath)
        {
            var settings = SceneSettings.Deserialize(filePath);
            
            viewport.CSPMode = settings.CSPMode;

            if (settings.CSPMode && showSelectionOutlineToolStripMenuItem.Checked)
                showSelectionOutlineToolStripMenuItem.PerformClick();

            viewport.EnableBack = settings.ShowBackdrop;
            viewport.EnableFloor = settings.ShowGrid;

            if (settings.Camera != null)
                viewport.Camera = settings.Camera;

            if (settings.Settings != null)
                JointManager._settings = settings.Settings;

            if (settings.Lighting != null)
                JointManager._lightParam = settings.Lighting;

            if (settings.Animation != null)
            {
                // load animations
                LoadAnimation(settings.Animation);

                // load material animation if exists
                var symbol = MainForm.SelectedDataNode.Text.Replace("_joint", "_matanim_joint");
                var matAnim = MainForm.Instance.GetSymbol(symbol);
                if (matAnim != null && matAnim is HSD_MatAnimJoint maj)
                    LoadAnimation(maj);

                // set frames
                //JOBJManager.Frame = settings.Frame;
                //JOBJManager.MaterialFrame = settings.Frame;
                viewport.Frame = settings.Frame;
            }

            if (settings.HiddenNodes != null)
                for (int i = 0; i < listDOBJ.Items.Count; i++)
                    listDOBJ.SetItemChecked(i, !settings.HiddenNodes.Contains(i));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = textureArrayEditor.SelectedObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceTextureButton_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            if (f != null)
                ReplaceTexture(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (s != null && s.Length > 0)
                ReplaceTexture(s[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        private void ReplaceTexture(string f)
        {
            if (textureArrayEditor.SelectedObject is TextureListProxy proxy)
            {
                if (!TOBJConverter.FormatFromString(f, out GXTexFmt imgFormat, out GXTlutFmt palFormat))
                {
                    using (var teximport = new TextureImportDialog())
                        if (teximport.ShowDialog() == DialogResult.OK)
                        {
                            imgFormat = teximport.TextureFormat;
                            palFormat = teximport.PaletteFormat;
                        }
                        else
                            return;
                }

                proxy.Replace(TOBJConverter.ImportTOBJFromFile(f, imgFormat, palFormat));
                JointManager.RefreshRendering = true;
                textureArrayEditor.Invalidate();
                textureArrayEditor.Update();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeJOBJ_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listDOBJ_KeyPress(object sender, KeyPressEventArgs e)
        {
            DeleteSelectedDOBJs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displaySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Light Settings", JointManager._lightParam))
                d.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fogSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Fog Settings", JointManager._fogParam))
                d.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        public class FrameSpeedModifierSettings
        {
            public List<FrameSpeedMultiplier> Modifiers { get; set; } = new List<FrameSpeedMultiplier>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fSMApplyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mult = new FrameSpeedModifierSettings();

            using (PropertyDialog d = new PropertyDialog("Frame Speed Multipler Settings", mult))
                if (d.ShowDialog() == DialogResult.OK)
                {
                    JointManager.Animation.ApplyFSMs(mult.Modifiers);
                    viewport.MaxFrame = JointManager.Animation.FrameCount;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void makeParticleJointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JointManager.SelectedJOBJ != null)
            {
                if (MessageBox.Show(
                    "Are you sure you want to make this joint a particle joint?\n" +
                    "This will remove all objects on this joint", 
                    "Make Particle Joint",
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    JointManager.SelectedJOBJ.ParticleJoint = new HSD_ParticleJoint();
                }
            }
        }
    }
}
