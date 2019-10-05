using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.IO;
using HSDRaw;
using HSDRawViewer.Rendering;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Converters;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI;

namespace HSDRawViewer
{
    public partial class Form1 : Form
    {
        private ByteViewer _myByteViewer;
        private Viewport _Viewport;
        private SubactionEditor _ScriptEditor;

        private HSDRawFile RawHSDFile = new HSDRawFile();

        private Dictionary<string, StructData> stringToStruct = new Dictionary<string, StructData>();

        public static DataNode SelectedDataNode { get; internal set; } = null;

        private Dictionary<Type, ContextMenu> typeToContextMenu = new Dictionary<Type, ContextMenu>();
        private ContextMenu commonContextMenu = new ContextMenu();

        public static bool RefreshNode = false;

        public Form1()
        {
            InitializeComponent();

            _myByteViewer = new ByteViewer();
            _myByteViewer.Dock = DockStyle.Fill;

            _Viewport = new Viewport();
            _Viewport.Dock = DockStyle.Fill;

            _ScriptEditor = new SubactionEditor();
            _ScriptEditor.Dock = DockStyle.Fill;

            tabControl1.TabPages[0].Controls.Add(_myByteViewer);
            tabControl1.TabPages[2].Controls.Add(_Viewport);

            ImageList myImageList = new ImageList();
            myImageList.ImageSize = new System.Drawing.Size(24, 24);
            myImageList.Images.Add("unknown", Properties.Resources.ico_unknown);
            myImageList.Images.Add("known", Properties.Resources.ico_known);
            myImageList.Images.Add("jobj", Properties.Resources.ico_jobj);
            myImageList.Images.Add("dobj", Properties.Resources.ico_dobj);
            myImageList.Images.Add("pobj", Properties.Resources.ico_pobj);
            myImageList.Images.Add("mobj", Properties.Resources.ico_mobj);
            myImageList.Images.Add("tobj", Properties.Resources.ico_tobj);
            myImageList.Images.Add("aobj", Properties.Resources.ico_aobj);
            myImageList.Images.Add("cobj", Properties.Resources.ico_cobj);
            myImageList.Images.Add("fobj", Properties.Resources.ico_fobj);
            myImageList.Images.Add("iobj", Properties.Resources.ico_iobj);
            myImageList.Images.Add("lobj", Properties.Resources.ico_lobj);
            myImageList.Images.Add("sobj", Properties.Resources.ico_sobj);

            treeView1.ImageList = myImageList;

            treeView1.AfterExpand += (sender, args) =>
            {
                args.Node.Nodes.Clear();
                if(args.Node is DataNode node)
                {
                    node.ExpandData();
                }

            };

            treeView1.AfterCollapse += (sender, args) =>
            {
                args.Node.Nodes.Clear();
                args.Node.Nodes.Add(new TreeNode());
            };

            treeView1.AfterSelect += (sender, args) =>
            {
                SelectNode(null);
            };

            treeView1.NodeMouseClick += (sender, args) =>
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(args.Location);
                if(args.Button == MouseButtons.Right && args.Node != null && args.Node is DataNode node)
                {
                    if (typeToContextMenu.ContainsKey(node.Accessor.GetType()))
                    {
                        var cm = typeToContextMenu[node.Accessor.GetType()];
                        cm.Show(this, args.Location);
                    }
                    else
                    {
                        commonContextMenu.Show(this, args.Location);
                    }
                }
            };

            propertyGrid1.PropertyValueChanged += (sneder, args) =>
            {
                if(SelectedDataNode != null)
                _myByteViewer.SetBytes(SelectedDataNode.Accessor._s.GetData());
            };

            InitializeStructs();

            GenerateContextMenus();
        }

        public void SelectNode(HSDAccessor cast)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode n)
            {
                _myByteViewer.SetBytes(n.Accessor._s.GetData());
                if(cast == null)
                {
                    propertyGrid1.SelectedObject = n.Accessor;
                    _Viewport.SelectedAccessor = n.Accessor;
                }
                else
                {
                    cast._s = n.Accessor._s;
                    propertyGrid1.SelectedObject = cast;
                    _Viewport.SelectedAccessor = cast;
                }
                SelectedDataNode = n;

                tabControl1.TabPages[2].Controls.Clear();

                if (n.Accessor is SBM_SubActionTable)
                {
                    tabControl1.TabPages[2].Controls.Add(_ScriptEditor);
                    _ScriptEditor.SetSubactionAccessor(n);
                }
                else
                {
                    tabControl1.TabPages[2].Controls.Add(_Viewport);
                }

                LocationLabel.Text = "Location: " + n.FullPath;
            }
        }

        private void InitializeStructs()
        {
            if(File.Exists("Structs.txt"))
            using (StreamReader r = new StreamReader(new FileStream("Structs.txt", FileMode.Open)))
            {
                while (!r.EndOfStream)
                {
                    StructData d = new StructData();
                    d.Read(r);
                    stringToStruct.Add(d.Name, d);
                }
            }
        }

        private void OpenFile(string FilePath)
        {
            treeView1.Nodes.Clear();
            RawHSDFile = new HSDRawFile();
            RawHSDFile.Open(FilePath);
            foreach(var r in RawHSDFile.Roots)
            {
                treeView1.Nodes.Add(new DataNode(r.Name, r.Data));
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = "HSD (*.dat)|*.dat;*.usd";

                if(d.ShowDialog() == DialogResult.OK)
                    OpenFile(d.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = "HSD (*.dat)|*.dat;*.usd";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    RawHSDFile.Save(d.FileName);
                }
            }
        }

        public void GenerateContextMenus()
        {
            {
                var cm = new ContextMenu();
                AttachCommonMenus(cm);

                MenuItem OpenAsJOBJ = new MenuItem("Open As JOBJ");
                OpenAsJOBJ.Click += (sender, args) => SelectNode(new HSD_JOBJ());
                cm.MenuItems.Add(OpenAsJOBJ);

                MenuItem OpenAsAJ = new MenuItem("Open As AnimJoint");
                OpenAsAJ.Click += (sender, args) => SelectNode(new HSD_AnimJoint());
                cm.MenuItems.Add(OpenAsAJ);

                //cm.MenuItems.Add(delete);

                typeToContextMenu.Add(typeof(HSDAccessor), cm);
            }

            {
                var cm = new ContextMenu();
                AttachCommonMenus(cm);

                MenuItem OpenAsAJ = new MenuItem("Export As SVG");
                OpenAsAJ.Click += (sender, args) =>
                {
                    Converters.ConvSVG.CollDataToSVG("test.svg", SelectedDataNode.Accessor as SBM_Coll_Data);
                };
                cm.MenuItems.Add(OpenAsAJ);

                typeToContextMenu.Add(typeof(SBM_Coll_Data), cm);
            }

            {
                var cm = new ContextMenu();
                AttachCommonMenus(cm);

                MenuItem OpenAsAJ = new MenuItem("Import Model Group");
                OpenAsAJ.Click += (sender, args) =>
                {
                    if(SelectedDataNode.Accessor is SBM_Map_Head)
                    {
                        SelectedDataNode.ImportModelGroup();
                    }
                };
                cm.MenuItems.Add(OpenAsAJ);

                typeToContextMenu.Add(typeof(SBM_Map_Head), cm);
            }


            {
                var cm = new ContextMenu();
                AttachCommonMenus(cm);

                MenuItem OpenAsAJ = new MenuItem("Add Texture Anim");
                OpenAsAJ.Click += (sender, args) =>
                {
                    if (SelectedDataNode.Accessor is HSD_MatAnim matanim)
                    {
                        matanim.TextureAnimation = new HSD_TexAnim();
                        matanim.TextureAnimation.AnimationObject = new HSD_AOBJ();
                        matanim.TextureAnimation.AnimationObject.FObjDesc = new HSD_FOBJDesc();
                    }
                };
                cm.MenuItems.Add(OpenAsAJ);

                typeToContextMenu.Add(typeof(HSD_MatAnim), cm);
            }

            {
                var cm = new ContextMenu();
                AttachCommonMenus(cm);

                MenuItem Export = new MenuItem("Export Frames TXT");
                Export.Click += (sender, args) =>
                {
                    using (SaveFileDialog d = new SaveFileDialog())
                    {
                        d.Filter = "TXT (*.txt)|*.txt";

                        if(d.ShowDialog() == DialogResult.OK)
                        {
                            if (SelectedDataNode.Accessor is HSD_FOBJ fobj)
                                File.WriteAllText(d.FileName, ConvFOBJ.ToString(fobj));

                            if (SelectedDataNode.Accessor is HSD_FOBJDesc fobjdesc)
                                File.WriteAllText(d.FileName, ConvFOBJ.ToString(fobjdesc));
                        }
                    }
                };
                cm.MenuItems.Add(Export);

                MenuItem Import = new MenuItem("Import Frames TXT");
                Import.Click += (sender, args) =>
                {
                    using (OpenFileDialog d = new OpenFileDialog())
                    {
                        d.Filter = "TXT (*.txt)|*.txt";

                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            if (SelectedDataNode.Accessor is HSD_FOBJ fobj)
                                ConvFOBJ.ImportKeys(fobj, File.ReadAllLines(d.FileName));

                            if (SelectedDataNode.Accessor is HSD_FOBJDesc fobjdesc)
                                ConvFOBJ.ImportKeys(fobjdesc, File.ReadAllLines(d.FileName));
                        }
                    }
                };
                cm.MenuItems.Add(Import);

                typeToContextMenu.Add(typeof(HSD_FOBJ), cm);
                typeToContextMenu.Add(typeof(HSD_FOBJDesc), cm);
            }

            AttachCommonMenus(commonContextMenu);

        }

        private void AttachCommonMenus(ContextMenu menu)
        {
            MenuItem delete = new MenuItem("Delete");
            delete.Click += (sender, args) =>
            {
                if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode node)
                {
                    node.Delete();
                }
            };
            MenuItem export = new MenuItem("Export");
            export.Click += (sender, args) =>
            {
                if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode node)
                {
                    node.Export();
                }
            };
            MenuItem import = new MenuItem("Import");
            import.Click += (sender, args) =>
            {
                if (treeView1.SelectedNode != null && treeView1.SelectedNode is DataNode node)
                {
                    node.Import();
                }
            };

            menu.MenuItems.Add(delete);
            menu.MenuItems.Add(export);
            menu.MenuItems.Add(import);
        }

        private void addRootFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog f = new OpenFileDialog())
            {
                f.Filter = "HSD (*.dat)|*.dat";
                f.FileName = Text;

                if (f.ShowDialog() == DialogResult.OK)
                {
                    var file = new HSDRawFile(f.FileName);

                    RawHSDFile.Roots.Add(file.Roots[0]);
                }
            }
        }
    }
}
