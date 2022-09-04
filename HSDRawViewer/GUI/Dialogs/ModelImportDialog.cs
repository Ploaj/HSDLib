using HSDRawViewer.Converters;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.GUI.Extra
{
    public partial class ModelImportDialog : Form
    {
        private class JointNode : TreeNode
        {
            public IOBone _bone { get; internal set; }

            public JointNode(IOBone bone)
            {
                this._bone = bone;

                Text = bone.Name;

                foreach (var c in bone.Children)
                {
                    Nodes.Add(new JointNode(c));
                }
            }
        }

        private class MeshItem : ListViewItem
        {
            public MeshImportSettings settings { get; internal set; }

            public MeshItem(IOMesh mesh)
            {
                Text = mesh.Name;
                settings = new MeshImportSettings(mesh);
            }
        }

        private class MaterialItem : ListViewItem
        {
            public MaterialImportSettings settings { get; internal set; }

            public MaterialItem(IOMaterial mesh)
            {
                Text = mesh.Name;
                settings = new MaterialImportSettings(mesh);
            }
        }

        private IOScene _scene;

        private IOModel _model;
        public ModelImportSettings settings { get; internal set; } = new ModelImportSettings();

        public Dictionary<IOMesh, MeshImportSettings> meshSettings { get; internal set; } = new Dictionary<IOMesh, MeshImportSettings>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        public ModelImportDialog(IOScene scene, IOModel model)
        {
            InitializeComponent();

            //
            _scene = scene;
            _model = model;

            //
            Init();
                
            // set settings
            mainProperty.SelectedObject = settings;

            // bone tree select
            boneTree.AfterSelect += (obj, args) =>
            {
                if (boneTree.SelectedNode is JointNode node)
                    boneProperty.SelectedObject = node._bone;
                else
                    boneProperty.SelectedObject = null;
            };

            // mesh list select
            meshList.SelectedIndexChanged += (obj, args) =>
            {
                object[] obs = new object[meshList.SelectedItems.Count];
                for (int i = 0; i < obs.Length; i++)
                    obs[i] = ((MeshItem)meshList.SelectedItems[i]).settings;
                meshProperty.SelectedObjects = obs;
            };

            // material list select
            materialList.SelectedIndexChanged += (obj, args) =>
            {
                object[] obs = new object[materialList.SelectedItems.Count];
                for (int i = 0; i < obs.Length; i++)
                    obs[i] = ((MaterialItem)materialList.SelectedItems[i]).settings;
                materialProperty.SelectedObjects = obs;
            };

            DialogResult = DialogResult.None;

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MeshImportSettings> GetMeshSettings()
        {
            foreach (MeshItem i in meshList.Items)
                yield return i.settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MaterialImportSettings> GetMaterialSettings()
        {
            foreach (MaterialItem i in materialList.Items)
                yield return i.settings;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            // fill bone tree
            if (_model.Skeleton != null)
            {
                foreach (var b in _model.Skeleton.RootBones)
                {
                    boneTree.Nodes.Add(new JointNode(b));
                }
            }
            boneTree.ExpandAll();

            // fill mesh list
            bool hasVertexAlpha = false;
            foreach (var m in _model.Meshes)
            {
                meshList.Items.Add(new MeshItem(m));

                if (!hasVertexAlpha && m.Vertices.Any(e => (m.HasColorSet(0) && e.Colors[0].W != 1) || (m.HasColorSet(1) && e.Colors[1].W != 1)))
                    hasVertexAlpha = true;
                
            }
            if (meshList.Items.Count > 0)
                meshList.Items[0].Selected = true;
            // selectAllMesh_Click(null, null);

            // fill material list
            foreach (var m in _scene.Materials)
            {
                materialList.Items.Add(new MaterialItem(m));
            }

            // auto detect vertex alpha
            if (hasVertexAlpha)
                settings.VertexColorFormat = HSDRaw.GX.GXCompTypeClr.RGBA8;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllMesh_Click(object sender, EventArgs e)
        {
            meshList.BeginUpdate(); 
            foreach (ListViewItem item in meshList.Items)
                item.Selected = true;
            meshList.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllMaterials_Click(object sender, EventArgs e)
        {
            materialList.BeginUpdate();
            foreach (ListViewItem item in materialList.Items)
                item.Selected = true;
            materialList.EndUpdate();
        }

        private static string YamlFilter = @"Yaml (*.yml)|*.yml";

        private class SettingsExport
        {
            public MeshImportSettings[] meshes;
            public MaterialImportSettings[] materials;

            public int[] highpoly;
            public int[] lowpoly;
            public int[] metalpoly;

            public MeshPosition[] positions;
            public DobjInfo[] objects;
        }

        private class MeshPosition
        {
            public string Name;
            public int Position;
        }

        private class DobjInfo
        {
            public int Position;
            public int Count;
            public int Joint;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExportSettings()
        {
            var f = Tools.FileIO.SaveFile(YamlFilter);

            if (f != null)
            {
                var settings = new SettingsExport()
                {
                    meshes = GetMeshSettings().ToArray(),
                    materials = GetMaterialSettings().ToArray(),
                };

                var builder = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance);

                using (StreamWriter writer = File.CreateText(f))
                    builder.Build().Serialize(writer, settings);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ImportSettings()
        {
            var f = Tools.FileIO.OpenFile(YamlFilter);

            if (f != null)
            {
                var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

                SettingsExport export =  (SettingsExport)deserializer.Deserialize<SettingsExport>(File.ReadAllText(f));

                // apply mesh settings
                if (export.meshes != null)
                {
                    var meshes = export.meshes.ToList();
                    foreach (MeshItem m in meshList.Items)
                    {
                        var mesh = meshes.Find(e => e.Name.Equals(m.settings.Name));

                        if (mesh != null)
                        {
                            mesh._poly = m.settings._poly;
                            m.settings = mesh;
                        }
                    }
                }

                // apply material settings
                if (export.materials != null)
                {
                    var materials = export.materials.ToList();
                    foreach (MaterialItem m in materialList.Items)
                    {
                        var mesh = materials.Find(e => e.Name.Equals(m.settings.Name));

                        if (mesh != null)
                        {
                            mesh._material = m.settings._material;
                            m.settings = mesh;
                        }
                    }
                }

                // apply order settings
                int total = 0;
                if (export.highpoly != null)
                    total = Math.Max(total, export.highpoly.Max());
                if (export.lowpoly != null)
                    total = Math.Max(total, export.lowpoly.Max());
                if (export.metalpoly != null)
                    total = Math.Max(total, export.metalpoly.Max());
                if (export.positions != null)
                    total = Math.Max(total, export.positions.Max(e=>e.Position));
                if (export.objects != null)
                    total = Math.Max(total, export.objects.Max(e => e.Position));

                // 
                if (total != 0)
                {
                    // index starts at 0 so add 1
                    total += 1;

                    // gather set positions
                    Dictionary<int, IOMesh> SetPositions = new Dictionary<int, IOMesh>();
                    if (export.positions != null)
                    {
                        foreach (var i in export.positions)
                        {
                            var mesh = _model.Meshes.Find(e => e.Name.Equals(i.Name));

                            if (mesh != null && !SetPositions.ContainsKey(i.Position))
                            {
                                _model.Meshes.Remove(mesh);
                                SetPositions.Add(i.Position, mesh);
                            }
                        }
                    }

                    // gather low and high polys
                    var high = _model.Meshes.Where(e => !e.Name.Contains("LOW")).ToList();
                    var low = _model.Meshes.Where(e => e.Name.Contains("LOW")).ToList();

                    // reorder mesh list to account for poly indices
                    List<IOMesh> newList = new List<IOMesh>();
                    List<IOMesh> dummies = new List<IOMesh>();
                    int highIndex = 0;
                    int lowIndex = 0;
                    for (int i = 0; i < total; i++)
                    {
                        IOMesh current = null;

                        // check for set position
                        if (SetPositions.ContainsKey(i))
                        {
                            current = SetPositions[i];
                        }
                        // skip positions that have texture count 0
                        else if (export.objects != null && export.objects.Any(e => e.Position == i && e.Count == 0))
                        {
                            current = new IOMesh() { Name = "Dummy" };
                            dummies.Add(current);
                        }
                        else
                        // if this is high poly spot add actual mesh
                        if (export.highpoly != null && export.highpoly.Any(e => e == i) && highIndex < high.Count)
                        {
                            current = high[highIndex];
                            highIndex++;
                        }
                        else
                        // if this is low poly spot add actual mesh
                        if (export.lowpoly != null && export.lowpoly.Any(e => e == i) && lowIndex < low.Count)
                        {
                            current = low[lowIndex];
                            lowIndex++;
                        }
                        else
                        {
                            // otherwise add dummy mesh
                            current = new IOMesh() { Name = "Dummy" };
                            dummies.Add(current);
                        }

                        // apply reflective flag if in reflective slot
                        if (current != null)
                        {
                            newList.Add(current);

                            if (export.metalpoly != null && export.metalpoly.Any(e => e == i) && !current.Name.Contains("REFLECTIVE2"))
                                current.Name += "_REFLECTIVE2";
                        }
                    }

                    // account for missing slots by just appending them
                    for (int i = highIndex; i < high.Count; i++)
                        newList.Add(high[i]);
                    for (int i = lowIndex; i < low.Count; i++)
                        newList.Add(low[i]);

                    if (highIndex < high.Count || lowIndex < low.Count)
                    {
                        MessageBox.Show("The imported model has more high or low polygons than the settings allow.\nThis may cause issues with some models.", "Too Many Polygons", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    // gether mesh items as a list
                    List<MeshItem> meshItems = new List<MeshItem>();
                    foreach (MeshItem m in meshList.Items)
                        meshItems.Add(m);

                    // add settings for dummy
                    foreach (var d in dummies)
                        meshItems.Add(new MeshItem(d)
                        {
                            settings = new MeshImportSettings()
                            {
                                IsDummy = true,
                                ForceTextureCount = 1,
                                _poly = d
                            }
                        });

                    // remove single binds as they will alter the order of mesh
                    foreach (var m in meshItems)
                        m.settings.SingleBind = false;

                    // sort to match new list order
                    meshItems = meshItems.OrderBy(e=>newList.IndexOf(e.settings._poly)).ToList();

                    // force texture counts
                    if (export.objects != null)
                    {
                        foreach (var i in export.objects)
                        {
                            if (i.Position < meshItems.Count)
                            {
                                meshItems[i.Position].settings.ForceTextureCount = i.Count;
                                meshItems[i.Position].settings.SingleBindJoint = _model.Skeleton.GetBoneByIndex(i.Joint).Name;
                            }
                        }
                    }

                    // add mesh items
                    meshList.Items.Clear();
                    foreach (var a in meshItems)
                        meshList.Items.Add(a);

                    // set new mesh list in model
                    _model.Meshes.Clear();
                    _model.Meshes.AddRange(newList);
                }

            }
        }

        private void exportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportSettings();
        }

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportSettings();
        }
    }
}
