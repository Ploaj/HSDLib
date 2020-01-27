using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Shapes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    // This plugin is so extra lol
    /// <summary>
    /// Special Plugin for Rendering Hurtboxes for fighters
    /// </summary>
    public partial class HurtBoxEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(SBM_HurtboxBank) };

        public DataNode Node
        {
            get => _node; set { _node = value;  HBBank = value.Accessor as SBM_HurtboxBank; }
        }
        private DataNode _node;

        private SBM_HurtboxBank HBBank
        {
            get => _bank;
            set
            {
                _bank = value;
                if(_bank != null)
                {
                    editor.SetProperty(value, "Hurtboxes");
                }
            }
        }
        private SBM_HurtboxBank _bank;

        public DrawOrder DrawOrder => DrawOrder.Last;
        
        private ViewportControl viewport;
        private ArrayMemberEditor editor;

        /// <summary>
        /// 
        /// </summary>
        public HurtBoxEditor()
        {
            InitializeComponent();

            editor = new ArrayMemberEditor();
            editor.Dock = DockStyle.Fill;
            hurtboxPanel.Controls.Add(editor);
            
            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            previewPanel.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();
            viewport.Visible = false;

            FormClosing += (sender, args) =>
            {
                JOBJManager.ClearRenderingCache();
                viewport.Dispose();
                editor.Dispose();
            };
        }

        private Vector3 SelectedHurtboxColor = new Vector3(1, 1, 1);
        private Vector3 HurtboxColor = new Vector3(1, 1, 0);

        private Dictionary<SBM_Hurtbox, Capsule> HurtboxToCapsule = new Dictionary<SBM_Hurtbox, Capsule>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            JOBJManager.Render(cam);

            var selected = editor.SelectedObject;
            foreach (SBM_Hurtbox v in editor.GetItems())
            {
                var clr = HurtboxColor;
                var a = 0.25f;
                if(selected == v)
                {
                    clr = SelectedHurtboxColor;
                    a = 0.6f;
                }
                var transform = JOBJManager.GetWorldTransform(v.BoneIndex);

                if (!HurtboxToCapsule.ContainsKey(v))
                    HurtboxToCapsule.Add(v, new Capsule(new Vector3(v.X1, v.Y1, v.Z1), new Vector3(v.X2, v.Y2, v.Z2), v.Size));
                
                var cap = HurtboxToCapsule[v];
                cap.SetParameters(new Vector3(v.X1, v.Y1, v.Z1), new Vector3(v.X2, v.Y2, v.Z2), v.Size);
                cap.Draw(transform, new Vector4(clr, a));
            }
        }

        private JOBJManager JOBJManager = new JOBJManager();

        /// <summary>
        /// 
        /// </summary>
        private void LoadModel(HSD_JOBJ jobj)
        {
            JOBJManager.ClearRenderingCache();
            JOBJManager.SetJOBJ(jobj);
            if (!viewport.Visible)
                viewport.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadModel_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if(f != null)
            {
                var hsd = new HSDRawFile(f);
                if (hsd.Roots[0].Data is HSD_JOBJ jobj)
                    LoadModel(jobj);
            }
        }
    }
}
