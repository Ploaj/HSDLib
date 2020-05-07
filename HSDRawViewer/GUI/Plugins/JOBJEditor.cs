using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Converters;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using HSDRaw.Tools;
using HSDRaw.GX;
using System.Drawing;
using HSDRaw;
using HSDRawViewer.Rendering.Renderers;
using System.Drawing.Design;
using OpenTK;

namespace HSDRawViewer.GUI.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JOBJEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.DockLeft;

        public DrawOrder DrawOrder => DrawOrder.First;

        private bool SelectDOBJ { get => (toolStripComboBox2.SelectedIndex == 1); }

        private Dictionary<int, string> BoneLabelMap = new Dictionary<int, string>();

        private ViewportControl viewport;

        public JOBJEditor()
        {
            InitializeComponent();

            JOBJManager = new JOBJManager();

            renderModeBox.ComboBox.DataSource = Enum.GetValues(typeof(RenderMode));
            renderModeBox.SelectedIndex = 0;

            listDOBJ.DataSource = dobjList;

            toolStripComboBox2.SelectedIndex = 0;

            treeJOBJ.AfterSelect += (sender, args) =>
            {
                propertyGrid1.SelectedObject = treeJOBJ.SelectedNode.Tag;
                JOBJManager.SelectetedJOBJ = treeJOBJ.SelectedNode.Tag as HSD_JOBJ;
            };

            listDOBJ.SelectedIndexChanged += (sender, args) => {
                propertyGrid1.SelectedObject = listDOBJ.SelectedItem;
                JOBJManager.DOBJManager.SelectedDOBJ = ((listDOBJ.SelectedItem as DOBJContainer)?.DOBJ);
            };

            propertyGrid1.PropertyValueChanged += (sender, args) =>
            {
                // refresh
                listDOBJ.SelectedItem = listDOBJ.SelectedItem;
            };


            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            previewBox.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();

            FormClosing += (sender, args) =>
            {
                if (PluginManager.GetCommonViewport() != null)
                {
                    if (PluginManager.GetCommonViewport() != null)
                    {
                        PluginManager.GetCommonViewport().AnimationTrackEnabled = false;
                        PluginManager.GetCommonViewport().RemoveRenderer(this);
                    }
                    viewport.Dispose();
                    JOBJManager.CleanupRendering();
                }
            };
        }

        public Type[] SupportedTypes => new Type[] { typeof(HSD_JOBJ) };

        public DataNode Node { get => _node;
            set
            {
                _node = value;

                if (_node.Accessor is HSD_JOBJ jobj)
                {
                    root = jobj;
                    JOBJManager.SetJOBJ(root);
                    RefreshGUI();
                }
            }
        }

        private DataNode _node;
        private HSD_JOBJ root;
        private Dictionary<HSD_JOBJ, int> jobjToIndex = new Dictionary<HSD_JOBJ, int>();
        
        private class DOBJContainer
        {
            public int Index;
            public int JOBJIndex;
            public int DOBJIndex;
            public HSD_JOBJ ParentJOBJ;
            public HSD_DOBJ DOBJ;

            public int PolygonCount { get => DOBJ.Pobj != null ? DOBJ.Pobj.List.Count : 0; }

            public int TextureCount { get => DOBJ.Mobj.Textures != null ? DOBJ.Mobj.Textures.List.Count : 0; }

            public bool HasPixelProcessing { get => DOBJ.Mobj?.PEDesc != null; }

            public bool HasMaterialColor { get => DOBJ.Mobj?.Material != null; }

            public RENDER_MODE MOBJFlags { get => DOBJ.Mobj.RenderFlags; set => DOBJ.Mobj.RenderFlags = value; }

            public override string ToString()
            {
                return $"{Index}. JOBJ {JOBJIndex} : DOBJ {DOBJIndex} : POBJs {PolygonCount} : TOBJS {TextureCount} : PP {HasPixelProcessing}";
            }
        }

        private BindingList<DOBJContainer> dobjList = new BindingList<DOBJContainer>();

        private JOBJManager JOBJManager;

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

            JOBJManager.DOBJManager.HiddenDOBJs.Clear();
            CheckAll();
        }

        private int index = 0;

        private void LoadJOBJ(HSD_JOBJ jobj, TreeNode parent = null)
        {
            if (parent == null)
                index = 0;
            TreeNode tree = new TreeNode();

            if (BoneLabelMap.ContainsKey(index))
                tree.Text = BoneLabelMap[index];
            else
            if (!string.IsNullOrEmpty(jobj.ClassName))
                tree.Text = $"(JOBJ_{index})" + jobj.ClassName;
            else
                tree.Text = "JOBJ_" + index;
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

        private bool IsSelected(object ob)
        {
            foreach (var v in propertyGrid1.SelectedObjects)
                if (v == ob)
                    return true;
            return false;
        }

        public void LoadAnimation(JointAnimManager animation)
        {
            var vp = viewport;
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = animation.FrameCount;
            JOBJManager.Animation = animation;
        }

        public void LoadAnimation(HSD_FigaTree tree)
        {
            var vp = viewport; 
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = tree.FrameCount;
            JOBJManager.SetFigaTree(tree);
        }

        public void LoadAnimation(HSD_AnimJoint joint)
        {
            JOBJManager.SetAnimJoint(joint);
            var vp = viewport; 
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = JOBJManager.Animation.FrameCount;
        }

        public void LoadAnimation(HSD_MatAnimJoint joint)
        {
            JOBJManager.SetMatAnimJoint(joint);
            var vp = viewport;
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = JOBJManager.MatAnimation.FrameCount;
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
            JOBJManager.SetMOT(jointTable, motFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            JOBJManager.Frame = viewport.Frame;
            JOBJManager.DOBJManager.OutlineSelected = showSelectionOutlineToolStripMenuItem.Checked;
            JOBJManager.Render(cam);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(toolStripComboBox2.SelectedIndex == 0)
            {
                JOBJManager.RenderObjects = true;
                JOBJManager.DOBJManager.OnlyRenderSelected = false;
            }
            if (toolStripComboBox2.SelectedIndex == 1)
            {
                JOBJManager.RenderObjects = true;
                JOBJManager.DOBJManager.OnlyRenderSelected = true;
            }
            if (toolStripComboBox2.SelectedIndex == 2)
            {
                JOBJManager.RenderObjects = false;
                JOBJManager.DOBJManager.OnlyRenderSelected = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            JOBJManager.RenderBones = showBonesToolStrip.Checked;
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
            JOBJManager.RefreshRendering = true;
            BoneLabelMap.Clear();
            RefreshGUI();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportModelToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelExporter.ExportFile(root, BoneLabelMap);
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
            
            JOBJManager.DOBJManager.HiddenDOBJs.Clear();
            foreach(var i in checkedItems)
            {
                JOBJManager.DOBJManager.HiddenDOBJs.Add(i.DOBJ);
            }
        }


        public class DummyDOBJSetting
        {
            [DisplayName("Number to Generate"), Description("")]
            public int NumberToGenerate { get; set; } = 1;
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
                    for (int i = 0; i < setting.NumberToGenerate; i++)
                        root.Dobj.Add(new HSD_DOBJ()
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
                        });
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
            BoneLabelMap.Clear();

            var f = Tools.FileIO.OpenFile("Label INI (*.ini)|*.ini");

            if(f != null)
            {
                var text = File.ReadAllText(f);
                text = Regex.Replace(text, @"\#.*", "");

                var lines = text.Split('\n');

                foreach(var r in lines)
                {
                    var args = r.Split('=');

                    if(args.Length == 2)
                    {
                        var name = args[1].Trim();
                        var i = 0;
                        if(int.TryParse(new string(args[0].Where(c => char.IsDigit(c)).ToArray()), out i))
                        {
                            BoneLabelMap.Add(i, name);
                        }
                    }
                }
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
                JOBJManager.RefreshRendering = true;
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
            if (MessageBox.Show("Are you sure?\nThis cannot be undone", "Delete DOBJ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var dobj = listDOBJ.SelectedItem as DOBJContainer;
                if(dobj != null)
                {
                    var dobjs = dobj.ParentJOBJ.Dobj.List;

                    HSD_DOBJ prev = null;
                    foreach(var d in dobjs)
                    {
                        if(d == dobj.DOBJ)
                        {
                            if (prev == null)
                                dobj.ParentJOBJ.Dobj = d.Next;
                            else
                                prev.Next = d.Next;
                            break;
                        }
                        prev = d;
                    }

                    JOBJManager.RefreshRendering = true;
                    RefreshGUI();
                }
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
            JOBJManager.Animation = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("FigaTree/AnimJoint/MayaAnim/EightingMOT (*.dat*.anim*.mota*.gnta)|*.dat;*.anim;*.mota;*.gnta");

            if (f != null)
            {
                if (Path.GetExtension(f).ToLower().Equals(".mota") || Path.GetExtension(f).ToLower().Equals(".gnta"))
                {
                    var jointTable = Tools.FileIO.OpenFile("Joint Connector Value (*.jcv)|*.jcv");

                    if(jointTable != null)
                        LoadAnimation(MOTLoader.GetJointTable(jointTable), new MOT_FILE(f));
                }
                else
                if (Path.GetExtension(f).ToLower().Equals(".anim"))
                {
                    LoadAnimation(ConvMayaAnim.ImportFromMayaAnim(f));
                }
                else
                if (Path.GetExtension(f).ToLower().Equals(".dat"))
                {
                    var dat = new HSDRaw.HSDRawFile(f);

                    if (dat.Roots.Count > 0 && dat.Roots[0].Data is HSD_FigaTree tree)
                        LoadAnimation(tree);

                    if (dat.Roots.Count > 0 && dat.Roots[0].Data is HSD_AnimJoint joint)
                        LoadAnimation(joint);
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

        private void mayaANIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JOBJManager.Animation == null || JOBJManager.Animation.NodeCount == 0)
            {
                MessageBox.Show("No animation is loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var f = Tools.FileIO.SaveFile("Supported Formats (*.anim)|*.anim");

            if (f != null)
            {
                ConvMayaAnim.ExportToMayaAnim(f, JOBJManager.Animation);
            }
        }

        public class FigaTreeSettings
        {
            [DisplayName("Symbol Name"), Description("Name of animation used by the game")]
            public string Symbol { get; set; } = "_figatree";
        }

        private void figaTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JOBJManager.Animation == null || JOBJManager.Animation.NodeCount == 0)
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
                        Data = JOBJManager.Animation.ToFigaTree(),
                        Name = setting.Symbol
                    });
                    animFile.Save(f);
                }
        }


        public class AnimJointSettings
        {
            [DisplayName("Symbol Name"), Description("Should end in _animjoint")]
            public string Symbol { get; set; } = "_animjoint";

            [DisplayName("Flags"), Description("")]
            public AOBJ_Flags Flags { get; set; } = AOBJ_Flags.ANIM_LOOP;
        }
        private void animJointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (JOBJManager.Animation == null || JOBJManager.Animation.NodeCount == 0)
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
                        Data = JOBJManager.Animation.ToAnimJoint(JOBJManager.GetJOBJ(0), setting.Flags),
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
        private void renderModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(JOBJManager != null)
            {
                RenderMode mode = JOBJManager.RenderMode;
                Enum.TryParse<RenderMode>(renderModeBox.Text, out mode);
                JOBJManager.RenderMode = mode;
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
            if(JOBJManager.Animation == null)
                return;

            var settings = new EditAnimationSettings();
            settings.EndFrame = (int)JOBJManager.Animation.FrameCount;
            settings.ScaleRangeEndFrame = (int)JOBJManager.Animation.FrameCount;
            using (PropertyDialog d = new PropertyDialog("Animation Edit Options", settings))
            {
                if(d.ShowDialog() == DialogResult.OK)
                {
                    // trim animation
                    if(settings.TrimAnimation)
                        JOBJManager.Animation.Trim(settings.StartFrame, settings.EndFrame);

                    // Scale animation
                    if (settings.ScaleRangeFactor != 1)
                        JOBJManager.Animation.ScaleBy(settings.ScaleRangeFactor, settings.ScaleRangeStartFrame, settings.ScaleRangeEndFrame);
                    
                    
                    // reload edited animation
                    LoadAnimation(JOBJManager.Animation);
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
            JOBJManager.RecalculateInverseBinds();
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

                    if(temp.JointCount == JOBJManager.JointCount)
                    {
                        for(int i = 0; i < temp.JointCount; i++)
                        {
                            var old = JOBJManager.GetJOBJ(i);
                            var n = temp.GetJOBJ(i);
                            old.TX = n.TX; old.TY = n.TY; old.TZ = n.TZ;
                            old.RX = n.RX; old.RY = n.RY; old.RZ = n.RZ;
                            old.SX = n.SX; old.SY = n.SY; old.SZ = n.SZ;

                            if (old.InverseWorldTransform != null)
                            {
                                if(n.InverseWorldTransform == null)
                                    old.InverseWorldTransform = JOBJManager.TKMatixToHSDMatrix(temp.GetWorldTransform(n).Inverted());
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
                var f = Tools.FileIO.SaveFile("MOBJ (*.mobj)|*.mobj");

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
                var f = Tools.FileIO.OpenFile("MOBJ (*.mobj)|*.mobj");

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

                    JOBJManager.RefreshRendering = true;
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
                JOBJTools.ApplyMeleeFighterTransforms(root);
                JOBJManager.RecalculateInverseBinds();
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
            {
                using (FileStream stream = new FileStream(f, FileMode.Create))
                using (StreamWriter w = new StreamWriter(stream))
                    if (BoneLabelMap.Count > 0)
                    {
                        foreach (var b in BoneLabelMap)
                            w.WriteLine($"JOBJ_{b.Key}={b.Value}");
                    }
                    else
                    {
                        var bones = root.BreathFirstList;
                        var ji = 0;
                        foreach(var j in bones)
                        {
                            if (!string.IsNullOrEmpty(j.ClassName))
                                w.WriteLine($"JOBJ_{ji}={j.ClassName}");
                            ji++;
                        }
                    }
            }
        }
    }
}
