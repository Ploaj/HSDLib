using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Extensions;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.IO.Model;
using HSDRawViewer.Rendering.GX;
using HSDRawViewer.Rendering.Models;
using IONET;
using IONET.Collada.Kinematics.Joints;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using IONET.Fbx.APITest;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.Converters
{
    public class ModelImporter : ProgressClass
    {
        // location to look for external files like textures
        public string FolderPath;

        // main model settings
        private ModelImportSettings Settings;

        // lookup mesh settings from iomesh
        private readonly Dictionary<IOMesh, MeshImportSettings> meshSettings = new();

        // lookup material settings from iomaterial
        private readonly Dictionary<IOMaterial, MaterialImportSettings> materialSettings = new();

        // cache the jobj names to their respective jobj
        private readonly Dictionary<string, HSD_JOBJ> NameToJOBJ = new();

        // Indicates jobjs that need the SKELETON flag set along with inverted transform
        private readonly List<HSD_JOBJ> EnvelopedJOBJs = new();

        // for cleaning root
        private Matrix4 CleanRotMatrix = Matrix4.Identity;

        // 
        private readonly Dictionary<HSD_JOBJ, Matrix4> jobjToWorldTransform = new();

        // cache textures
        private readonly Dictionary<string, HSD_TOBJ> pathToTObj = new();

        /// <summary>
        /// 
        /// </summary>
        private HSD_JOBJ NewModel;

        /// <summary>
        /// 
        /// </summary>
        private readonly IOScene scene;

        /// <summary>
        /// 
        /// </summary>
        private readonly IOModel model;

        private static readonly string ModelFileFilter = @"Support Formats|*.dae;*.smd;*.obj;*.fbx;*.hsdm;";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static HSD_JOBJ ImportModelFromFile(string f, HSD_JOBJ original = null)
        {
            if (f != null)
            {
                IOScene scene = IOManager.LoadScene(f, new ImportSettings()
                {
                    Triangulate = true,
                });

                if (scene.Models.Count == 0)
                    return null;

                // remove blender's dumb root bone
                for (int i = 0; i < scene.Models[0].Skeleton.RootBones.Count; i++)
                {
                    if (scene.Models[0].Skeleton.RootBones[i].Name.Equals("Armature"))
                    {
                        scene.Models[0].Skeleton.RootBones[i] = scene.Models[0].Skeleton.RootBones[i].Child;

                        var joint = scene.Models[0].Skeleton.RootBones[i];
                        while (joint != null)
                        {
                            while (joint.Sibling != null &&
                                    joint.Sibling.Type != BoneType.JOINT)
                            {
                                joint.Sibling.Parent = null;
                            }

                            joint = joint.Sibling;
                        }

                        scene.Models[0].Skeleton.RootBones[i].Parent = null;
                    }
                }

                // check to replace skeleton
                bool replace = false;
                if (original != null)
                {
                    if (JointTreeIsSimilar(original, scene.Models[0].Skeleton))
                    {
                        if (MessageBox.Show("The imported model shares the same skeletal structure of the current model.\n\n" +
                            "Preserve the current model's skeleton?\n" +
                            "(Recommended for online play)",
                            "Preserve Skeleton", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ReplaceWithBonesFromFile(original, scene.Models[0].Skeleton.RootBones[0]);
                            replace = true;
                        }
                    }
                }

                // import model
                using ModelImportDialog d = new(scene, scene.Models[0]);
                if (d.ShowDialog() == DialogResult.OK)
                {
                    ModelImporter imp = new(Path.GetDirectoryName(f), scene, scene.Models[0], d.settings, d.GetMeshSettings(), d.GetMaterialSettings());

                    using (ProgressBarDisplay pb = new(imp))
                    {
                        pb.DoWork();
                        pb.ShowDialog();
                    }

                    if (replace)
                        ReplaceWithBonesFromFile(original, imp.NewModel);

                    return imp.NewModel;
                }
            }

            return null;
        }

        private static HSD_JOBJ ImportHSDM(string filePath, HSD_JOBJ original)
        {
            var path = Path.GetDirectoryName(filePath);
            var jobj = HsdImportHelper.ImportSklToJObj(filePath);

            // check to replace skeleton
            if (original != null)
            {
                if (JointTreeIsSimilar(original, jobj))
                {
                    if (MessageBox.Show("The imported model shares the same skeletal structure of the current model.\n\n" +
                        "Preserve the current model's skeleton?\n" +
                        "(Recommended for online play)",
                        "Preserve Skeleton", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ReplaceWithBonesFromFile(original, jobj);
                    }
                }
            }

            return jobj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toReplace"></param>
        public static HSD_JOBJ ImportModelFromFile(HSD_JOBJ original)
        {
            var f = Tools.FileIO.OpenFile(ModelFileFilter);

            if (f == null)
                return null;

            if (Path.GetExtension(f).ToLower() == ".hsdm")
            {
                return ImportHSDM(f, original);
            }

            return ImportModelFromFile(f, original);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toReplace"></param>
        public static void ReplaceModelFromFile(HSD_JOBJ original)
        {
            HSD_JOBJ model = ImportModelFromFile(original);
            if (model != null)
                original._s.SetFromStruct(model._s);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool JointTreeIsSimilar(HSD_JOBJ from, HSD_JOBJ to)
        {
            float eps = 0.01f;

            if (Math.Abs(from.TX - to.TX) > eps) return false;
            if (Math.Abs(from.TY - to.TY) > eps) return false;
            if (Math.Abs(from.TZ - to.TZ) > eps) return false;
            if (Math.Abs(from.RX - to.RX) > eps) return false;
            if (Math.Abs(from.RY - to.RY) > eps) return false;
            if (Math.Abs(from.RZ - to.RZ) > eps) return false;
            if (Math.Abs(from.SX - to.SX) > eps) return false;
            if (Math.Abs(from.SY - to.SY) > eps) return false;
            if (Math.Abs(from.SZ - to.SZ) > eps) return false;

            if ((from.Child != null) != (to.Child != null))
                return false;

            if ((from.Next != null) != (to.Next != null))
                return false;
            
            if (from.Child != null)
            {
                if (!JointTreeIsSimilar(from.Child, to.Child))
                    return false;
            }

            if (from.Next != null)
            {
                if (!JointTreeIsSimilar(from.Next, to.Next))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Returns true if both jobjs have same structure and transform values
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool JointTreeIsSimilar(HSD_JOBJ from, IOSkeleton to)
        {
            //List<HSD_JOBJ> fromList = from.TreeList;
            //List<IOBone> toList = to.BreathFirstOrder();

            //// check if they have save joint count
            //if (fromList.Count != toList.Count)
            //    return false;

            // check if structure is the same
            if (to.RootBones.Count == 0 || 
                !JointTreeMatchesStructure(from, to.RootBones[0]))
                return false;

            // check each joint transforms
            //float epsilon = 0.001f;
            //for (int i = 0; i < fromList.Count; i++)
            //{
            //    if (Math.Abs(from.TX - to.TX) > epsilon) return false;
            //    if (Math.Abs(from.TY - to.TY) > epsilon) return false;
            //    if (Math.Abs(from.TZ - to.TZ) > epsilon) return false;
            //    if (Math.Abs(from.RX - to.RX) > epsilon) return false;
            //    if (Math.Abs(from.RY - to.RY) > epsilon) return false;
            //    if (Math.Abs(from.RZ - to.RZ) > epsilon) return false;
            //    if (Math.Abs(from.SX - to.SX) > epsilon) return false;
            //    if (Math.Abs(from.SY - to.SY) > epsilon) return false;
            //    if (Math.Abs(from.SZ - to.SZ) > epsilon) return false;
            //}

            // both joint trees are similar enough!
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <returns></returns>
        private static bool JointTreeMatchesStructure(HSD_JOBJ j1, IOBone j2)
        {
            if ((j1.Next == null) != (j2.Sibling == null))
                return false;

            if ((j1.Child == null) != (j2.Child == null))
                return false;

            if (j1.Child != null)
                if (!JointTreeMatchesStructure(j1.Child, j2.Child))
                    return false;

            if (j1.Next != null)
                if (!JointTreeMatchesStructure(j1.Next, j2.Sibling))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <returns></returns>
        private static void ReplaceWithBonesFromFile(HSD_JOBJ j1, IOBone j2)
        {
            j2.Translation = new System.Numerics.Vector3(j1.TX, j1.TY, j1.TZ);
            j2.RotationEuler = new System.Numerics.Vector3(j1.RX, j1.RY, j1.RZ);
            j2.Scale = new System.Numerics.Vector3(j1.SX, j1.SY, j1.SZ);

            //System.Diagnostics.Debug.WriteLine($"{j2.Name} {j1.RX} {j1.RY} {j1.RZ} {j2.RotationEuler.ToString()}");

            if (j1.Child != null)
                ReplaceWithBonesFromFile(j1.Child, j2.Child);

            if (j1.Next != null)
                ReplaceWithBonesFromFile(j1.Next, j2.Sibling);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void ReplaceWithBonesFromFile(HSD_JOBJ from, HSD_JOBJ to)
        {
            LiveJObj oldlist = new(to);
            LiveJObj newlist = new(from);

            if (newlist.JointCount == oldlist.JointCount)
            {
                for (int i = 0; i < newlist.JointCount; i++)
                {
                    HSD_JOBJ old = oldlist.GetJObjAtIndex(i).Desc;
                    HSD_JOBJ n = newlist.GetJObjAtIndex(i).Desc;

                    // copy compensate flag if it exists
                    if (n.Flags.HasFlag(JOBJ_FLAG.MTX_SCALE_COMPENSATE))
                        old.Flags |= JOBJ_FLAG.MTX_SCALE_COMPENSATE;

                    old.TX = n.TX; old.TY = n.TY; old.TZ = n.TZ;
                    old.RX = n.RX; old.RY = n.RY; old.RZ = n.RZ;
                    old.SX = n.SX; old.SY = n.SY; old.SZ = n.SZ;

                    if (old.InverseWorldTransform == null)
                    {
                        if (n.InverseWorldTransform == null)
                            old.InverseWorldTransform = newlist.GetJObjAtIndex(i).WorldTransform.Inverted().ToHsdMatrix();
                        else
                            old.InverseWorldTransform = n.InverseWorldTransform;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="scene"></param>
        /// <param name="settings"></param>
        /// <param name="meshSettings"></param>
        /// <param name="materialSettings"></param>
        public ModelImporter(
            string folderPath,
            IOScene scene,
            IOModel model,
            ModelImportSettings settings,
            IEnumerable<MeshImportSettings> meshSettings,
            IEnumerable<MaterialImportSettings> materialSettings)
        {
            this.scene = scene;
            this.model = model;
            this.FolderPath = folderPath;
            this.Settings = settings;

            // generate mesh lookup
            foreach (MeshImportSettings m in meshSettings)
                this.meshSettings.Add(m._poly, m);

            // generate material lookup
            foreach (MaterialImportSettings m in materialSettings)
                this.materialSettings.Add(m._material, m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        public override void Work(BackgroundWorker w)
        {
            try
            {
                Import(w);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to import model: {e.ToString()}");
                w.ReportProgress(100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        private void Import(BackgroundWorker w)
        {
            // settings
            Settings ??= new ModelImportSettings();

            // create pobj generate
            POBJ_Generator POBJGen = new()
            {
                UseTriangleStrips = Settings.UseStrips,
                VertexColorFormat = (GXCompType)Settings.VertexColorFormat,
            };

            // apply settings to model
            if (Settings.SmoothNormals)
                model.SmoothNormals();

            // process nodes
            ProgressStatus = "Processing Joints...";
            w.ReportProgress(30);
            HSD_JOBJ root = null;
            foreach (IOBone r in model.Skeleton.RootBones)
            {
                if (r.Name == "Armature")
                    continue;

                if (root == null)
                    root = IOBoneToJOBJ(r);
                else
                    root.Add(IOBoneToJOBJ(r));
            }
            if (root == null)
            {
                root = IOBoneToJOBJ(
                    new IOBone()
                    {
                        Name = "Root"
                    });
            }


            // process mesh
            ProgressStatus = "Processing Mesh...";
            foreach (IOMesh mesh in model.Meshes)
            {
                ProcessMesh(scene, mesh, root, ref POBJGen);

                if (root.Dobj != null)
                {
                    ProgressStatus = $"Processing Mesh {root.Dobj.List.Count} {model.Meshes.Count + 1}...";
                    w.ReportProgress((int)(30 + 60 * (root.Dobj.List.Count / (float)model.Meshes.Count)));
                }
            }

            // calculate inverse binds
            if (Settings.EnvelopeAll)
            {
                foreach (KeyValuePair<HSD_JOBJ, Matrix4> v in jobjToWorldTransform)
                    v.Key.InverseWorldTransform = v.Value.Inverted().ToHsdMatrix();
            }
            else
            {
                foreach (HSD_JOBJ jobj in EnvelopedJOBJs)
                {
                    ProgressStatus = "Generating Inverse Transforms...";
                    if (jobjToWorldTransform.ContainsKey(jobj))
                        jobj.InverseWorldTransform = jobjToWorldTransform[jobj].Inverted().ToHsdMatrix();
                }
            }


            // SAVE POBJ buffers
            ProgressStatus = "Generating and compressing vertex buffers...";
            w.ReportProgress(90);
            POBJGen.SaveChanges();

            // done
            NewModel = root;

            // auto update flags
            NewModel.UpdateFlags();

            // apply classical scale flag
            if (Settings.ClassicalScaling)
                NewModel.Flags |= JOBJ_FLAG.CLASSICAL_SCALING;

#if DEBUG
            //
            if (Settings.Merge)
                JOBJExtensions.MergeIntoOneObject(NewModel);
#endif

            ProgressStatus = "Done!";
            w.ReportProgress(100);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        private HSD_JOBJ IOBoneToJOBJ(IOBone bone)
        {
#if DEBUG
            //
            if (Settings.CleanRoot)
            {
                // if this is root bone calculate the clean matrix
                if (bone.Parent == null)
                {
                    CleanRotMatrix = Matrix4.CreateFromQuaternion(new Quaternion(bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W));
                    bone.Rotation = System.Numerics.Quaternion.Identity;
                }
                else // otherwise apply the cleaning to current bone
                {
                    var pos = Vector3.TransformNormal(new Vector3(bone.TranslationX, bone.TranslationY, bone.TranslationZ), CleanRotMatrix);
                    bone.Translation = new System.Numerics.Vector3(pos.X, pos.Y, pos.Z);
                }

                // clean scale
                bone.Scale = new System.Numerics.Vector3(1, 1, 1);
            }

            // melee-ify model if flag is set
            if (bone.Parent == null && Settings.Meleeify)
            {
                bone.Meleeify();
            }
#endif
            // create jobj
            HSD_JOBJ jobj = new()
            {
                TX = bone.TranslationX,
                TY = bone.TranslationY,
                TZ = bone.TranslationZ,
                RX = bone.RotationEuler.X,
                RY = bone.RotationEuler.Y,
                RZ = bone.RotationEuler.Z,
                SX = bone.ScaleX,
                SY = bone.ScaleY,
                SZ = bone.ScaleZ,
            };

            // optional import bone name
            if (Settings.ImportBoneNames)
                jobj.ClassName = bone.Name;

            // add to lookup
            NameToJOBJ.Add(bone.Name, jobj);

            // add world transform lookup
            jobjToWorldTransform.Add(jobj, bone.WorldTransform.ToTKMatrix());

            // recursivly add children
            foreach (IOBone child in bone.Children)
            {
                jobj.AddChild(IOBoneToJOBJ(child));
            }

            return jobj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ProcessMesh(IOScene scene, IOMesh mesh, HSD_JOBJ rootnode, ref POBJ_Generator POBJGen)
        {
            // get settings if they exist
            MeshImportSettings settings;
            if (meshSettings.ContainsKey(mesh))
                settings = meshSettings[mesh];
            else
                settings = new MeshImportSettings(mesh);

            // apply settings
            if (settings.FlipUVs)
                mesh.FlipUVs();

            if (!settings.FlipFaces)
                mesh.FlipWindingOrder();

            // determine parent
            HSD_JOBJ parent = rootnode;
            if (mesh.ParentBone != null && NameToJOBJ.ContainsKey(mesh.ParentBone.Name))
                parent = NameToJOBJ[mesh.ParentBone.Name];
            if (settings.SingleBindJoint != null && 
                NameToJOBJ.ContainsKey(settings.SingleBindJoint))
                parent = NameToJOBJ[settings.SingleBindJoint];

            Console.WriteLine("Processing " + mesh.Name);

            if (mesh.Name.Contains("BUMP"))
            {
                mesh.CalculateTangentBitangent();

                foreach (IOVertex v in mesh.Vertices)
                {
                    v.Tangent /= 100;
                    v.Binormal /= 100;
                }
            }

            // begin processing polygons
            HSD_DOBJ root = null;
            HSD_DOBJ prev = null;
            foreach (IOPolygon poly in mesh.Polygons)
            {
                // Skip Empty Polygon
                if (poly.Indicies.Count == 0)
                    continue;

                // convert to triangles
                poly.ToTriangles(mesh);
                if (poly.PrimitiveType != IOPrimitive.TRIANGLE)
                    continue;

                // Generate DOBJ
                HSD_DOBJ dobj = new();

                if (Settings.ImportMeshNames)
                    dobj.ClassName = mesh.Name;

                if (root == null)
                    root = dobj;
                else
                    prev.Next = dobj;
                prev = dobj;


                // generate material
                IOMaterial material = scene.Materials.Find(e => e.Name == poly.MaterialName);
                dobj.Mobj = GenerateMaterial(settings, material);


                Console.WriteLine(mesh.Name + " " + material?.Name);


                // reflective mobjs do not use uvs
                bool hasReflection = settings.IsReflective;
#if DEBUG
                if (Settings.MetalModel)
                    hasReflection = true;
#endif

                // bump maps need tangents and bitangents
                bool hasBump = settings.GenerateTB;

                // Assess additional attributes based on the material MOBJ
                if (dobj.Mobj !=null && dobj.Mobj.Textures != null)
                {
                    foreach (HSD_TOBJ t in dobj.Mobj.Textures.List)
                    {
                        if (t.Flags.HasFlag(TOBJ_FLAGS.COORD_REFLECTION))
                            hasReflection = true;
                        if (t.Flags.HasFlag(TOBJ_FLAGS.BUMP))
                            hasBump = true;
                    }
                }

                // assess attributes
                List<GXAttribName> Attributes = new();

                // determine if position normal matrix is needed
                if (mesh.HasEnvelopes() && Settings.ImportSkinning && !settings.SingleBind)
                {
                    Attributes.Add(GXAttribName.GX_VA_PNMTXIDX);

                    if (hasReflection)
                    {
                        Attributes.Add(GXAttribName.GX_VA_TEX0MTXIDX);
#if DEBUG
                        if (Settings.MetalModel && 
                            !Attributes.Contains(GXAttribName.GX_VA_TEX1MTXIDX))
                        {
                            Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);
                        }
                        else
#endif
                        if (mesh.Name.Contains("REFLECTIVE2"))
                            Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);
                        else
                        if (dobj.Mobj.Textures != null)
                        {
                            if (dobj.Mobj.Textures.List.Count > 1)
                                Attributes.Add(GXAttribName.GX_VA_TEX1MTXIDX);

                            if (dobj.Mobj.Textures.List.Count > 2)
                                Attributes.Add(GXAttribName.GX_VA_TEX2MTXIDX);
                        }
                    }
                }

                // always add position attribute
                Attributes.Add(GXAttribName.GX_VA_POS);

                // add normal attribute if needed
                if (hasBump)
                    Attributes.Add(GXAttribName.GX_VA_NBT);
                else
                if (mesh.HasNormals)
                {
                    if (settings.ImportNormals == ImportNormalSettings.Yes ||
                        (settings.ImportNormals == ImportNormalSettings.YesIfNoVertexColor &&
                        !mesh.HasColorSet(0)))
                        Attributes.Add(GXAttribName.GX_VA_NRM);
                }

                // add vertex color attribute if needed
                if (mesh.HasColorSet(0) && settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR0);

                if (mesh.HasColorSet(1) && settings.ImportVertexColor)
                    Attributes.Add(GXAttribName.GX_VA_CLR1);

                // add texture coordinate uvs
#if DEBUG
                if (!Settings.MetalModel)
                {
#endif
                    if (settings.ImportUVs != ImportYesNo.No)
                    {

                        if (mesh.HasUVSet(0))
                            Attributes.Add(GXAttribName.GX_VA_TEX0);

                        if ((mesh.HasUVSet(1) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 1)))
                            Attributes.Add(GXAttribName.GX_VA_TEX1);

                        if ((mesh.HasUVSet(2) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 2)))
                            Attributes.Add(GXAttribName.GX_VA_TEX2);

                        if ((mesh.HasUVSet(3) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 3)))
                            Attributes.Add(GXAttribName.GX_VA_TEX3);

                        if ((mesh.HasUVSet(4) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 4)))
                            Attributes.Add(GXAttribName.GX_VA_TEX4);

                        if ((mesh.HasUVSet(5) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 5)))
                            Attributes.Add(GXAttribName.GX_VA_TEX5);

                        if ((mesh.HasUVSet(6) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 6)))
                            Attributes.Add(GXAttribName.GX_VA_TEX6);

                        if ((mesh.HasUVSet(7) || (dobj.Mobj.Textures != null && dobj.Mobj.Textures.List.Count > 7)))
                            Attributes.Add(GXAttribName.GX_VA_TEX7);
                    }
#if DEBUG
                }
#endif

                // being processing mesh
                List<GX_Vertex> vertices = new();
                List<HSD_JOBJ[]> jobjList = new();
                List<float[]> weightList = new();

                // generarte vertex list
                foreach (int face in poly.Indicies)
                {
                    IOVertex v = mesh.Vertices[face];

                    GX_Vertex vertex = new();

                    Vector3 tkvert = new(v.Position.X, v.Position.Y, v.Position.Z);
                    Vector3 tknrm = new(v.Normal.X, v.Normal.Y, v.Normal.Z);
                    Vector3 tktan = new(v.Tangent.X, v.Tangent.Y, v.Tangent.Z);
                    Vector3 tkbitan = new(v.Binormal.X, v.Binormal.Y, v.Binormal.Z);

                    // transform by inverse of parent
                    if (jobjToWorldTransform[parent] != Matrix4.Identity)
                    {
                        Matrix4 parentTransform = jobjToWorldTransform[parent].Inverted();
                        tkvert = Vector3.TransformPosition(tkvert, parentTransform);
                        tknrm = Vector3.TransformNormal(tknrm, parentTransform).Normalized();
                        tktan = Vector3.TransformNormal(tktan, parentTransform);
                        tkbitan = Vector3.TransformNormal(tkbitan, parentTransform);
                    }

                    // additional processing for skin
                    if (mesh.HasEnvelopes() && Settings.ImportSkinning)
                    {
                        // create weighting lists
                        List<float> weight = new();
                        List<HSD_JOBJ> bones = new();

                        // single bind if there are no weights
                        if (v.Envelope.Weights.Count == 0)
                        {
                            weight.Add(1);
                            bones.Add(rootnode);
                        }

                        // check for too many weights
                        if (v.Envelope.Weights.Count > 6)
                            throw new Exception($"Too many weights! {v.Envelope.Weights.Count} in {mesh.Name}");

                        // process weights
                        foreach (IOBoneWeight bw in v.Envelope.Weights)
                        {
                            // check if skeleton actually contains bone
                            if (NameToJOBJ.ContainsKey(bw.BoneName))
                            {
                                // add envelope
                                bones.Add(NameToJOBJ[bw.BoneName]);
                                weight.Add(bw.Weight);

                                // indicate enveloped jobjs
                                if (!EnvelopedJOBJs.Contains(NameToJOBJ[bw.BoneName]))
                                    EnvelopedJOBJs.Add(NameToJOBJ[bw.BoneName]);
                            }
                            else
                            {
                                throw new Exception($"Bone not found \"{bw.BoneName}\" Weight: {bw.Weight} in {mesh.Name}");
                            }
                        }

                        // add to jobj and weight list
                        jobjList.Add(bones.ToArray());
                        weightList.Add(weight.ToArray());

                        // invert single binds
                        if (v.Envelope.Weights.Count == 1 && parent == rootnode)
                        {
                            Matrix4 inv = jobjToWorldTransform[NameToJOBJ[v.Envelope.Weights[0].BoneName]].Inverted();
                            tkvert = Vector3.TransformPosition(tkvert, inv);
                            tknrm = Vector3.TransformNormal(tknrm, inv).Normalized();
                            tktan = Vector3.TransformNormal(tktan, inv);
                            tkbitan = Vector3.TransformNormal(tkbitan, inv);
                        }
                    }

#if DEBUG
                    if (Settings.OutlineSize != 0.0)
                    {
                        tkvert += tknrm.Normalized() * Settings.OutlineSize;
                    }
#endif

                    // set final vertex data
                    vertex.POS = GXTranslator.fromVector3(tkvert);
                    vertex.NRM = GXTranslator.fromVector3(tknrm.Normalized());
                    vertex.TAN = GXTranslator.fromVector3(tktan);
                    vertex.BITAN = GXTranslator.fromVector3(tkbitan);


                    if (settings.InvertNormals)
                    {
                        vertex.NRM.X *= -1;
                        vertex.NRM.Y *= -1;
                        vertex.NRM.Z *= -1;
                        vertex.TAN.X *= -1;
                        vertex.TAN.Y *= -1;
                        vertex.TAN.Z *= -1;
                        vertex.BITAN.X *= -1;
                        vertex.BITAN.Y *= -1;
                        vertex.BITAN.Z *= -1;
                    }

                    if (mesh.HasUVSet(0))
                        vertex.TEX0 = new GXVector2(v.UVs[0].X, v.UVs[0].Y);

                    if (mesh.HasUVSet(1))
                        vertex.TEX1 = new GXVector2(v.UVs[1].X, v.UVs[1].Y);

                    if (mesh.HasUVSet(2))
                        vertex.TEX2 = new GXVector2(v.UVs[2].X, v.UVs[2].Y);

                    if (mesh.HasUVSet(3))
                        vertex.TEX3 = new GXVector2(v.UVs[3].X, v.UVs[3].Y);

                    if (mesh.HasUVSet(4))
                        vertex.TEX4 = new GXVector2(v.UVs[4].X, v.UVs[4].Y);

                    if (mesh.HasUVSet(5))
                        vertex.TEX5 = new GXVector2(v.UVs[5].X, v.UVs[5].Y);

                    if (mesh.HasUVSet(6))
                        vertex.TEX6 = new GXVector2(v.UVs[6].X, v.UVs[6].Y);

                    if (mesh.HasUVSet(7))
                        vertex.TEX7 = new GXVector2(v.UVs[7].X, v.UVs[7].Y);

                    if (mesh.HasColorSet(0))
                        vertex.CLR0 = new GXColor4(
                        v.Colors[0].X,
                        v.Colors[0].Y,
                        v.Colors[0].Z,
                        v.Colors[0].W);

                    if (mesh.HasColorSet(1))
                        vertex.CLR1 = new GXColor4(
                        v.Colors[1].X,
                        v.Colors[1].Y,
                        v.Colors[1].Z,
                        v.Colors[1].W);

                    vertices.Add(vertex);
                }


                // generate pobjs
                HSD_POBJ pobj = null;

                if (mesh.HasEnvelopes() && Settings.ImportSkinning && !settings.SingleBind)
                    pobj = POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), jobjList, weightList);
                else
                    pobj = POBJGen.CreatePOBJsFromTriangleList(vertices, Attributes.ToArray(), null);

                if (settings.SingleBind && jobjList.Count > 0 && jobjList[0].Length > 0)
                    parent = jobjList[0][0];

                if (pobj != null)
                {
                    if (dobj.Pobj == null)
                        dobj.Pobj = pobj;
                    else
                        dobj.Pobj.Add(pobj);
                }

            }

            // generate dummy 
            if (root == null && settings.IsDummy)
                root = CreateDummyDObj();

            // force texture count
            if (root != null && settings.ForceTextureCount >= 0)
                ForceTextureCount(root, settings.ForceTextureCount);

            // add to list
            if (parent.Dobj == null)
                parent.Dobj = root;
            else
                parent.Dobj.Add(root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HSD_DOBJ CreateDummyDObj()
        {
            HSD_DOBJ dobj = new()
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

            return dobj;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ForceTextureCount(HSD_DOBJ dobj, int forceTextureCount)
        {
            // get current texture count
            int texCount = dobj.Mobj.Textures != null ? dobj.Mobj.Textures.List.Count : 0;

            // texture count is perfect
            if (texCount == forceTextureCount)
                return;

            // too many textures
            if (texCount > forceTextureCount)
            {
                for (int i = 0; i < texCount - forceTextureCount; i++)
                {
                    if (dobj.Mobj.Textures.Next == null)
                        dobj.Mobj.Textures = null;
                    else
                        dobj.Mobj.Textures.RemoveLast();
                }
            }
            else
            // not enough textures
            {
                for (int i = 0; i < forceTextureCount - texCount; i++)
                {
                    HSD_TOBJ tobj = CreateDummyTObj();
                    if (dobj.Mobj.Textures == null)
                        dobj.Mobj.Textures = tobj;
                    else
                        dobj.Mobj.Textures.Add(tobj);
                }
            }

            // update flags
            dobj.Mobj.SetFlag(RENDER_MODE.TEX0, forceTextureCount >= 0);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX1, forceTextureCount >= 1);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX2, forceTextureCount >= 2);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX3, forceTextureCount >= 3);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX4, forceTextureCount >= 4);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX5, forceTextureCount >= 5);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX6, forceTextureCount >= 6);
            dobj.Mobj.SetFlag(RENDER_MODE.TEX7, forceTextureCount >= 7);

            // optimize textures
            dobj.Mobj.Textures.Optimize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HSD_TOBJ CreateDummyTObj()
        {
            return new HSD_TOBJ()
            {
                MagFilter = GXTexFilter.GX_LINEAR,
                Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                RepeatT = 1,
                RepeatS = 1,
                WrapS = GXWrapMode.CLAMP,
                WrapT = GXWrapMode.CLAMP,
                ColorOperation = COLORMAP.PASS,
                AlphaOperation = ALPHAMAP.PASS,
                SX = 1,
                SY = 1,
                SZ = 1,
                GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0,
                Blending = 1,
                ImageData = new HSD_Image()
                {
                    Format = GXTexFmt.I4,
                    Width = 8,
                    Height = 8,
                    ImageData = new byte[32]
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetFullTexturePath(string path)
        {
            string texturePath = path;

            if (texturePath.Contains("file://"))
                texturePath = texturePath.Replace("file://", "");

            if (File.Exists(Path.Combine(FolderPath, texturePath)))
                texturePath = Path.Combine(FolderPath, texturePath);
            else
            if (File.Exists(path))
                texturePath = path;
            else
            if (File.Exists(texturePath + ".png"))
                texturePath = texturePath + ".png";

            return texturePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        private HSD_MOBJ GenerateMaterial(MeshImportSettings mesh, IOMaterial material)
        {
            // create blank mobj
            HSD_MOBJ Mobj = new();
            Mobj.Material = new HSD_Material()
            {
                AMB_A = 0xFF,
                AMB_R = 0x7F,
                AMB_G = 0x7F,
                AMB_B = 0x7F,
                DiffuseColor = System.Drawing.Color.White,
                SpecularColor = System.Drawing.Color.White,
                Shininess = 50,
                Alpha = 1
            };

            // no material data
            if (material == null)
                return Mobj;

            // get settings if they exist
            MaterialImportSettings settings;
            if (materialSettings.ContainsKey(material))
                settings = materialSettings[material];
            else
                settings = new MaterialImportSettings(material);

            // optional mobj loading
            if (settings.ImportMOBJ)
            {
                string mobjPath = Path.Combine(FolderPath, settings.MobjPath);

                if (File.Exists(mobjPath))
                {
                    Mobj._s = new HSDRaw.HSDRawFile(mobjPath).Roots[0].Data._s;
                    return Mobj;
                }
                else if (settings.ImportTexture &&
                    material.DiffuseMap != null &&
                    !string.IsNullOrEmpty(material.DiffuseMap.FilePath))
                {
                    string textPath = GetFullTexturePath(material.DiffuseMap.FilePath);
                    mobjPath = Path.Combine(Path.GetDirectoryName(textPath), Path.GetFileNameWithoutExtension(textPath) + ".mobj");

                    if (File.Exists(mobjPath))
                    {
                        Mobj._s = new HSDRaw.HSDRawFile(mobjPath).Roots[0].Data._s;
                        return Mobj;
                    }
                }
            }

            // detect and set flags
            if (mesh.ImportVertexColor)
                Mobj.RenderFlags |= RENDER_MODE.VERTEX;

            if (settings.EnableDiffuse && mesh.ImportVertexColor == false)
                Mobj.RenderFlags |= RENDER_MODE.DIFFUSE;

            if (settings.EnableConstant)
                Mobj.RenderFlags |= RENDER_MODE.CONSTANT;

            // Properties
            if (settings.ImportMaterialInfo)
            {
                Mobj.Material.Shininess = settings.Shininess;
                Mobj.Material.Alpha = settings.Alpha;
                Mobj.Material.AmbientColor = settings.AmbientColor;
                Mobj.Material.DiffuseColor = settings.DiffuseColor;
                Mobj.Material.SpecularColor = settings.SpecularColor;
            }

            // Textures
            if (settings.ImportTexture)
            {
                if (material.DiffuseMap != null && !string.IsNullOrEmpty(material.DiffuseMap.FilePath))
                {
                    string texturePath = GetFullTexturePath(material.DiffuseMap.FilePath);

                    if (File.Exists(texturePath) && (texturePath.ToLower().EndsWith(".png") || texturePath.ToLower().EndsWith(".bmp")))
                    {
                        Mobj.RenderFlags |= RENDER_MODE.TEX0;

                        if (pathToTObj.ContainsKey(texturePath))
                        {
                            Mobj.Textures = HSDRaw.HSDAccessor.DeepClone<HSD_TOBJ>(pathToTObj[texturePath]);
                        }
                        else
                        {
                            HSD_TOBJ tobj = new();
                            tobj.ImportImage(texturePath, settings.TextureFormat, settings.PaletteFormat);
                            tobj.Flags = TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.COLORMAP_MODULATE;

                            tobj.GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0;
                            tobj.TexMapID = GXTexMapID.GX_TEXMAP0;

                            tobj.WrapS = material.DiffuseMap.WrapS.ToGXWrapMode();
                            tobj.WrapT = material.DiffuseMap.WrapT.ToGXWrapMode();

                            if (tobj.IsTransparent())
                            {
                                Mobj.RenderFlags |= RENDER_MODE.XLU;
                                tobj.Flags |= TOBJ_FLAGS.ALPHAMAP_MODULATE;
                            }

                            Mobj.Textures = tobj;

                            pathToTObj.Add(texturePath, tobj);
                        }
                    }
                }
            }

            return Mobj;
        }
    }
}
