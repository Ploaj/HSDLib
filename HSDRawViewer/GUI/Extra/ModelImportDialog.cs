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

            public MeshItem( IOMesh mesh)
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
            foreach (var m in _model.Meshes)
            {
                meshList.Items.Add(new MeshItem(m));
                
            }
            // selectAllMesh_Click(null, null);

            // fill material list
            foreach (var m in _scene.Materials)
            {
                materialList.Items.Add(new MaterialItem(m));
            }
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

                var meshes = export.meshes.ToList();
                var materials = export.materials.ToList();

                foreach (MeshItem m in meshList.Items)
                {
                    var mesh = meshes.Find(e => e.Name.Equals(m.settings.Name));

                    if (mesh != null)
                    {
                        mesh._poly = m.settings._poly;
                        m.settings = mesh;
                    }
                }
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
