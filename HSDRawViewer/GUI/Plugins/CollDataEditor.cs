using System;
using WeifenLuo.WinFormsUI.Docking;
using HSDRaw;
using HSDRaw.Melee.Gr;
using System.Windows.Forms;
using HSDRawViewer.Rendering.Renderers;
using OpenTK;
using System.Collections.Generic;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class CollDataEditor : DockContent, EditorBase
    {
        private ViewportControl vp;

        public CollDataEditor()
        {
            InitializeComponent();

            vp = new ViewportControl();
            vp.Dock = DockStyle.Fill;
            viewBox.Controls.Add(vp);
        }

        public Type[] SupportedTypes => new Type[] { typeof(SBM_Coll_Data) };

        private SBM_Coll_Data CollData;

        ~CollDataEditor()
        {
            vp.Dispose();
        }

        public HSDAccessor GetAccessor()
        {
            return CollData;
        }

        public void SetAccessor(HSDAccessor a)
        {
            CollData = a as SBM_Coll_Data;
            vp.AddRenderer(CollData, new CollDataRenderer());

            List<Vector2> Vectors = new List<Vector2>();
            foreach(var v in CollData.Vertices)
            {
                Vectors.Add(new Vector2(v.X, v.Y));
            }

            vp.FrameView(Vectors);
        }
    }
}
