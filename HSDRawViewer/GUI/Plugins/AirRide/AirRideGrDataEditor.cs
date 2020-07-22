using HSDRaw.AirRide.Gr.Data;
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
    public partial class AirRideGrDataEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(KAR_grData) };

        public DataNode Node { get => _node;
            set
            {
                _node = value;
                if (_node.Accessor is KAR_grData data)
                    LoadData(data);
            }
        }
        private DataNode _node;
        private KAR_grData _data;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private ViewportControl _viewport = new ViewportControl();

        private AirRideGrCollisionManager collisionManager = new AirRideGrCollisionManager();

        /// <summary>
        /// 
        /// </summary>
        public AirRideGrDataEditor()
        {
            InitializeComponent();
            _viewport = new ViewportControl();
            _viewport.Dock = DockStyle.Fill;
            _viewport.AddRenderer(this);
            tabControl1.TabPages[0].Controls.Add(_viewport);

            FormClosing += (sender, args) =>
            {
                _viewport.RemoveRenderer(this);
                _viewport.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void LoadData(KAR_grData data)
        {
            _data = data;
            collisionManager.LoadCollision(data.CollisionNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (_data == null)
                return;

            collisionManager.RenderPrimitives(renderCollisionsToolStripMenuItem.Checked, zonesToolStripMenuItem.Checked);
        }
    }
}
