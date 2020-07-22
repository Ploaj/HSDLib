using HSDRaw.AirRide.Gr;
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

        public DrawOrder DrawOrder => throw new NotImplementedException();

        private DataNode _node;

        /// <summary>
        /// 
        /// </summary>
        private KAR_grMainModel mainModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void LoadMainModel(KAR_grMainModel model)
        {
            mainModel = model;
            _jointEditor.SetJOBJ(model.RootNode);
            arrayMemberEditor1.SetArrayFromProperty(model.ModelBounding, "ViewRegions");
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
                foreach (var v in mainModel.ModelBounding.ViewRegions)
                {
                    DrawShape.DrawBox(Color.Red, v.MinX, v.MinY, v.MinZ, v.MaxX, v.MaxY, v.MaxZ);
                }
        }
    }
}
