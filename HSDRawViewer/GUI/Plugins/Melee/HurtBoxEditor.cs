using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Renderers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    // This plugin is so extra lol
    /// <summary>
    /// Special Plugin for Rendering Hurtboxes for fighters
    /// </summary>
    [SupportedTypes(new Type[] { typeof(SBM_HurtboxBank<SBM_Hurtbox>) })]
    public partial class HurtBoxEditor : EditorBase, IDrawable
    {
        public override DataNode Node
        {
            get => _node; set { _node = value;  HBBank = value.Accessor as SBM_HurtboxBank<SBM_Hurtbox>; }
        }
        private DataNode _node;

        private SBM_HurtboxBank<SBM_Hurtbox> HBBank
        {
            get => _bank;
            set
            {
                _bank = value;
                if(_bank != null)
                {
                    editor.SetArrayFromProperty(value, "Hurtboxes");
                }
            }
        }
        private SBM_HurtboxBank<SBM_Hurtbox> _bank;

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
            viewport.DisplayGrid = true;
            previewPanel.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();
            viewport.Visible = false;

            FormClosing += (sender, args) =>
            {
                JOBJManager.CleanupRendering();
                viewport.Dispose();
                editor.Dispose();
            };
        }


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
            var list = new List<SBM_Hurtbox>();
            foreach (SBM_Hurtbox v in editor.GetItems())
                list.Add(v);
            hurtboxRenderer.Render(JOBJManager, list, (HSDAccessor)selected);
        }

        private HurtboxRenderer hurtboxRenderer = new HurtboxRenderer();

        private JOBJManager JOBJManager = new JOBJManager();

        /// <summary>
        /// 
        /// </summary>
        private void LoadModel(HSD_JOBJ jobj)
        {
            JOBJManager.RefreshRendering = true;
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
