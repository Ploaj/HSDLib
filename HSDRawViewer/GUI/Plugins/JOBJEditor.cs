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

namespace HSDRawViewer.GUI.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JOBJEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.DockLeft;

        public DrawOrder DrawOrder => DrawOrder.First;

        private bool SelectDOBJ { get => (toolStripComboBox1.SelectedIndex == 1); }

        private Dictionary<int, string> BoneLabelMap = new Dictionary<int, string>();

        private ViewportControl viewport;

        public JOBJEditor()
        {
            InitializeComponent();

            JOBJManager = new JOBJManager();

            listDOBJ.DataSource = dobjList;

            toolStripComboBox1.SelectedIndex = 0;

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
                    JOBJManager.ClearRenderingCache();
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
            public HSD_JOBJ ParentJOBJ;
            public HSD_DOBJ DOBJ;

            public int PolygonCount { get => DOBJ.Pobj != null ? DOBJ.Pobj.List.Count : 0; }

            public int TextureCount { get => DOBJ.Mobj.Textures != null ? DOBJ.Mobj.Textures.List.Count : 0; }

            public bool HasPixelProcessing { get => DOBJ.Mobj?.PEDesc != null; }

            public bool HasMaterialColor { get => DOBJ.Mobj?.Material != null; }

            public RENDER_MODE MOBJFlags { get => DOBJ.Mobj.RenderFlags; set => DOBJ.Mobj.RenderFlags = value; }

            public override string ToString()
            {
                return $"JOBJ {JOBJIndex} : DOBJ {Index} : POBJs {PolygonCount} : TOBJS {TextureCount} : PP {HasPixelProcessing}" +
                    $"";
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
                    dobjList.Add(new DOBJContainer() { DOBJ = dobj, ParentJOBJ = jobj, Index = dobjIndex++, JOBJIndex = jobjToIndex[jobj] } );
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

        public void LoadAnimation(HSD_FigaTree tree)
        {
            var vp = viewport; //PluginManager.GetCommonViewport();
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = tree.FrameCount;
            JOBJManager.SetFigaTree(tree);
        }

        public void LoadAnimation(HSD_AnimJoint joint)
        {
            var vp = viewport; //PluginManager.GetCommonViewport();
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = JOBJManager.SetAnimJoint(joint);
        }

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            JOBJManager.Frame = viewport.Frame;// PluginManager.GetCommonViewport().Frame;
            JOBJManager.Render(cam);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(toolStripComboBox1.SelectedIndex == 0)
            {
                JOBJManager.RenderObjects = true;
                JOBJManager.DOBJManager.OnlyRenderSelected = false;
            }
            if (toolStripComboBox1.SelectedIndex == 1)
            {
                JOBJManager.RenderObjects = true;
                JOBJManager.DOBJManager.OnlyRenderSelected = true;
            }
            if (toolStripComboBox1.SelectedIndex == 2)
            {
                JOBJManager.RenderObjects = false;
                JOBJManager.DOBJManager.OnlyRenderSelected = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            JOBJManager.RenderBones = toolStripButton1.Checked;
        }

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
            JOBJManager.DOBJManager.RenderVertexColor = renderVertexColorsToolStripMenuItem.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importModelFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelImporter.ReplaceModelFromFile(root);
            JOBJManager.ClearRenderingCache();
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
            if (mainRender.Checked)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDummyDOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            root.Dobj.Add(new HSD_DOBJ()
            {
                Mobj = new HSD_MOBJ()
                {
                    RenderFlags = RENDER_MODE.ALPHA_COMPAT | RENDER_MODE.DIFFUSE_MAT,
                    Material = new HSD_Material()
                    {
                        DiffuseColor = Color.White,
                        SpecularColor = Color.White,
                        AmbientColor = Color.White,
                        Shininess = 50
                    }
                }
            });

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

        private class OutlineSettings
        {
            [DisplayName("Outline Thickness"), Description("Thickness of the outline")]
            public float Size { get; set; } = 0.0375f;

            [DisplayName("Use Triangle Strips"), Description("Slower to generate, but better optimized for in-game")]
            public bool UseStrips { get; set; } = true;
            
            [DisplayName("Outline Color"), Description("Color of Outline")]
            public Color Color { get; set; } = Color.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createOutlineMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new OutlineSettings();

            using (PropertyDialog d = new PropertyDialog("Outline Settings", settings))
            {
                if (d.ShowDialog() != DialogResult.OK)
                    return;
            }

            if (propertyGrid1.SelectedObject is DOBJContainer con)
            {
                var pobjGen = new POBJ_Generator();
                pobjGen.UseTriangleStrips = settings.UseStrips;

                var newDOBJ = new HSD_DOBJ();
                newDOBJ.Mobj = new HSD_MOBJ()
                {
                    Material = new HSD_Material()
                    {
                        AmbientColor = Color.Black,
                        SpecularColor = Color.Black,
                        DiffuseColor = Color.Black,
                        DIF_A = 255,
                        SPC_A = 255,
                        AMB_A = 255,
                        Shininess = 50,
                        Alpha = 1
                    },
                    RenderFlags = RENDER_MODE.ALPHA_COMPAT | RENDER_MODE.DIFFUSE_MAT
                };

                foreach (var pobj in con.DOBJ.Pobj.List)
                {
                    var dl = pobj.ToDisplayList();

                    var vertices = dl.Vertices;

                    GXAttribName[] attrs = new GXAttribName[]
                    {
                        GXAttribName.GX_VA_PNMTXIDX,
                        GXAttribName.GX_VA_POS,
                        GXAttribName.GX_VA_CLR0,
                        GXAttribName.GX_VA_NULL
                    };

                    List<GX_Vertex> newVerties = new List<GX_Vertex>();

                    var offset = 0;
                    foreach (var prim in dl.Primitives)
                    {
                        var verts = vertices.GetRange(offset, prim.Count);
                        offset += prim.Count;

                        switch (prim.PrimitiveType)
                        {
                            case GXPrimitiveType.Quads:
                                verts = TriangleConverter.QuadToList(verts);
                                break;
                            case GXPrimitiveType.TriangleStrip:
                                verts = TriangleConverter.StripToList(verts);
                                break;
                            case GXPrimitiveType.Triangles:
                                break;
                            default:
                                Console.WriteLine(prim.PrimitiveType);
                                break;
                        }

                        newVerties.AddRange(verts);
                    }

                    // extrude
                    for (int i = 0; i < newVerties.Count; i++)
                    {
                        var v = newVerties[i];
                        v.POS.X += v.NRM.X * settings.Size;
                        v.POS.Y += v.NRM.Y * settings.Size;
                        v.POS.Z += v.NRM.Z * settings.Size;
                        v.CLR0.R = settings.Color.R / 255f;
                        v.CLR0.G = settings.Color.G / 255f;
                        v.CLR0.B = settings.Color.B / 255f;
                        v.CLR0.A = settings.Color.A / 255f;
                        newVerties[i] = v;
                    }

                    // invert faces
                    for (int i = 0; i < newVerties.Count; i += 3)
                    {
                        var temp = newVerties[i];
                        newVerties[i] = newVerties[i + 2];
                        newVerties[i + 2] = temp;
                    }

                    var newpobj = pobjGen.CreatePOBJsFromTriangleList(newVerties, attrs, dl.Envelopes);
                    foreach(var p in newpobj.List)
                        p.Flags |= POBJ_FLAG.CULLBACK | POBJ_FLAG.UNKNOWN;
                    if (newDOBJ.Pobj == null)
                        newDOBJ.Pobj = newpobj;
                    else
                        newDOBJ.Pobj.Add(newpobj);
                }

                pobjGen.SaveChanges();

                con.ParentJOBJ.Dobj.Add(newDOBJ);

                RefreshGUI();
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
                JOBJManager.ClearRenderingCache();
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

                    JOBJManager.ClearRenderingCache();
                    RefreshGUI();
                }
            }
        }
    }
}
