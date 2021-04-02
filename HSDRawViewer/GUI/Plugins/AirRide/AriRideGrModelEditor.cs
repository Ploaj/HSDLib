using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.AirRide
{
    public partial class AriRideGrModelEditor : DockContent, EditorBase, IDrawable
    {
        private JobjEditor _jointEditor;

        public AriRideGrModelEditor()
        {
            InitializeComponent();
            _jointEditor = new JobjEditor();
            _jointEditor.Dock = DockStyle.Fill;
            _jointEditor.AddDrawable(this);
            tabControl1.TabPages[0].Controls.Add(_jointEditor);

            FormClosing += (sender, args) =>
            {
                _jointEditor.RemoveDrawable(this);
            };
        }

        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(KAR_grMainModel) };

        public DataNode Node { get => _node; set
            {
                _node = value;

                if (_node.Accessor is KAR_grMainModel model)
                    LoadMainModel(model);
            }
        }

        public DrawOrder DrawOrder => DrawOrder.First;

        private DataNode _node;

        /// <summary>
        /// 
        /// </summary>
        private KAR_grMainModel mainModel;

        private KAR_grViewRegion SelectedRegion = null;

        public KAR_grViewRegion[] ViewRegions { get; set; }

        public KAR_grStaticBoundingBox[] StaticBoundingBoxes { get; set; }

        public KAR_grDynamicBoundingBoxes[] DynamicBoundingBoxes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void LoadMainModel(KAR_grMainModel model)
        {
            mainModel = model;
            _jointEditor.SetJOBJ(model.RootNode);
            RefreshBounding();
        }

        private void RefreshBounding()
        {
            Console.WriteLine(mainModel.ModelBounding.ViewRegionCount + " " + mainModel.ModelBounding.ViewRegions.Length);

            ViewRegions = mainModel.ModelBounding.ViewRegions;

            StaticBoundingBoxes = mainModel.ModelBounding.StaticBoundingBoxes;

            DynamicBoundingBoxes = mainModel.ModelBounding.DynamicBoundingBoxes;

            arrayMemberEditor1.SetArrayFromProperty(this, "ViewRegions");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (toolStripButton1.Checked)
                foreach (var v in ViewRegions)
                {
                    if (SelectedRegion == v)
                        DrawShape.DrawBox(Color.White, v.MinX, v.MinY, v.MinZ, v.MaxX, v.MaxY, v.MaxZ);
                    //else
                    //    DrawShape.DrawBox(Color.Red, v.MinX, v.MinY, v.MinZ, v.MaxX, v.MaxY, v.MaxZ);
                }

            if (StaticBoundingBoxes != null)
                foreach (var v in StaticBoundingBoxes)
                {
                    DrawShape.DrawBox(Color.Yellow, v.MinX, v.MinY, v.MinZ, v.MaxX, v.MaxY, v.MaxZ);
                }

            if (DynamicBoundingBoxes != null)
                foreach (var v in DynamicBoundingBoxes)
                {
                    if (v.BoneIndex >= 0 && v.BoneIndex < _jointEditor.JointManager.JointCount)
                        DrawShape.DrawBox(Color.Purple, _jointEditor.JointManager.GetWorldTransform(v.BoneIndex), v.MinX, v.MinY, v.MinZ, v.MaxX, v.MaxY, v.MaxZ);
                    else
                        DrawShape.DrawBox(Color.Purple, v.MinX, v.MinY, v.MinZ, v.MaxX, v.MaxY, v.MaxZ);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrayMemberEditor1_SelectedObjectChanged(object sender, EventArgs e)
        {
            if(arrayMemberEditor1.SelectedObject is KAR_grViewRegion bounding)
            {
                var jointManager = _jointEditor.JointManager;

                jointManager.HideAllDOBJs();

                foreach (var i in bounding.Indices)
                    jointManager.ShowDOBJ(i);

                SelectedRegion = bounding;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recalBoundingButton_Click(object sender, EventArgs e)
        {
            ModelExporter exp = new ModelExporter(_jointEditor.JointManager.GetJOBJ(0), new ModelExportSettings(), new Tools.JointMap());

            List<KAR_grStaticBoundingBox> staticBoxes = new List<KAR_grStaticBoundingBox>();
            List<KAR_grDynamicBoundingBoxes> dynamicBoxes = new List<KAR_grDynamicBoundingBoxes>();
            List<ushort> staticIndices = new List<ushort>();

            // calculate mesh bounding
            int meshIndex = 0;
            foreach (var m in exp.Scene.Models[0].Meshes)
            {
                float minx = float.MaxValue, miny = float.MaxValue, minz = float.MaxValue;
                float maxx = float.MinValue, maxy = float.MinValue, maxz = float.MinValue;
                foreach(var v in m.Vertices)
                {
                    minx = Math.Min(minx, v.Position.X);
                    miny = Math.Min(miny, v.Position.Y);
                    minz = Math.Min(minz, v.Position.Z);
                    maxx = Math.Max(maxx, v.Position.X);
                    maxy = Math.Max(maxy, v.Position.Y);
                    maxz = Math.Max(maxz, v.Position.Z);
                }

                if (m.Vertices.Count == 0)
                    minx = maxx = miny = maxy = minz = maxz = 0;

                staticBoxes.Add(new KAR_grStaticBoundingBox()
                {
                    MinX = minx,
                    MinY = miny,
                    MinZ = minz,
                    MaxX = maxx,
                    MaxY = maxy,
                    MaxZ = maxz
                });

                // TODO: dynamic (mesh with rigging)
                var boneIndex = -1;

                if (boneIndex == -1)
                {
                    staticIndices.Add((ushort)meshIndex);
                }
                else
                {
                    dynamicBoxes.Add(new KAR_grDynamicBoundingBoxes()
                    {
                        BoneIndex = boneIndex,
                        Indices = new ushort[] { (ushort)meshIndex },
                        MinX = minx,
                        MinY = miny,
                        MinZ = minz,
                        MaxX = maxx,
                        MaxY = maxy,
                        MaxZ = maxz
                    });
                }

                meshIndex++;
            }

            // calculate view regions
            // TODO: generate view regions
            KAR_grViewRegion region = new KAR_grViewRegion();
            region.MinX = -10000;
            region.MinY = -10000;
            region.MinZ = -10000;
            region.MaxX = 10000;
            region.MaxY = 10000;
            region.MaxZ = 10000;
            region.Indices = staticIndices.ToArray();


            // update bounding struct
            mainModel.ModelBounding.ViewRegions = new KAR_grViewRegion[]
            {
                region
            };
            mainModel.ModelBounding.StaticBoundingBoxes = staticBoxes.ToArray();
            mainModel.ModelBounding.DynamicBoundingBoxes = dynamicBoxes.ToArray();
            RefreshBounding();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ModelExporter exp = new ModelExporter(_jointEditor.JointManager.GetJOBJ(0), new ModelExportSettings(), new Tools.JointMap());

            List<GXVector3> points = new List<GXVector3>();

            List<KAR_CollisionTriangle> triangles = new List<KAR_CollisionTriangle>();

            // calculate mesh bounding
            int meshIndex = 0;
            foreach (var m in exp.Scene.Models[0].Meshes)
            {
                m.MakeTriangles();

                int pointStart = points.Count;

                foreach (var v in m.Vertices)
                {
                    points.Add(new GXVector3() { X = v.Position.X, Y = v.Position.Y, Z = v.Position.Z });
                }

                foreach(var p in m.Polygons)
                {
                    for(int i = 0; i < p.Indicies.Count; i += 3)
                    {
                        triangles.Add(new KAR_CollisionTriangle()
                        {
                            V1 = p.Indicies[i + 0] + pointStart,
                            V2 = p.Indicies[i + 1] + pointStart,
                            V3 = p.Indicies[i + 2] + pointStart,
                            Flags = 0x80 | 0x01 // ground and basic material?
                        });
                    }
                }

                meshIndex++;
            }

            KAR_grCollisionNode collision = new KAR_grCollisionNode();

            collision.Vertices = points.ToArray();
            collision.Triangles = triangles.ToArray();

            collision.Joints = new KAR_CollisionJoint[]{
            new KAR_CollisionJoint()
            {
                BoneID = 0,
                FaceStart = 0,
                FaceSize = triangles.Count,
                VertexStart = 0,
                VertexSize = points.Count
            }
            };

            var f = new HSDRawFile();
            f.Roots.Add(new HSDRootNode()
            {
                Name = "collision",
                Data = collision
            });
            f.Save(Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "collision.dat"));


            var f2 = new HSDRawFile();
            f2.Roots.Add(new HSDRootNode()
            {
                Name = "partition",
                Data = Converters.AirRide.BucketGen.GenerateBucketPartition(collision)
            });
            f2.Save(Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "partition.dat"));
        }
    }
}
