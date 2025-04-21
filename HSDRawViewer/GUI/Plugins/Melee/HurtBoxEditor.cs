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
    /// <summary>
    /// Special Plugin for Rendering Hurtboxes for fighters
    /// </summary>
    [SupportedTypes(new Type[] { typeof(SBM_HurtboxBank<SBM_Hurtbox>) })]
    public partial class HurtBoxEditor : PluginBase, IDrawable
    {
        public override DataNode Node
        {
            get => _node; set { _node = value; HBBank = value.Accessor as SBM_HurtboxBank<SBM_Hurtbox>; }
        }
        private DataNode _node;

        private SBM_HurtboxBank<SBM_Hurtbox> HBBank
        {
            get => _bank;
            set
            {
                _bank = value;
                if (_bank != null)
                {
                    editor.SetArrayFromProperty(value, "Hurtboxes");
                }
            }
        }
        private SBM_HurtboxBank<SBM_Hurtbox> _bank;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private readonly ViewportControl viewport;
        private readonly ArrayMemberEditor editor;

        private readonly HurtboxRenderer hurtboxRenderer = new();

        private readonly RenderJObj RenderJObj = new();

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
                viewport.Dispose();
                editor.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
            RenderJObj.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            RenderJObj.FreeResources();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            RenderJObj.Render(cam);

            object selected = editor.SelectedObject;
            List<SBM_Hurtbox> list = new();
            foreach (SBM_Hurtbox v in editor.GetItems())
                list.Add(v);

            hurtboxRenderer.Render(RenderJObj.RootJObj, list, (HSDAccessor)selected);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModel(HSD_JOBJ jobj)
        {
            RenderJObj.LoadJObj(jobj);

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
            string path = System.IO.Path.GetDirectoryName(MainForm.Instance.FilePath);
            string filename = System.IO.Path.GetFileName(MainForm.Instance.FilePath).Replace(".dat", "Nr.dat");

            path = System.IO.Path.Combine(path, filename);

            if (System.IO.File.Exists(path))
            {
                HSDRawFile hsd = new(path);
                if (hsd.Roots[0].Data is HSD_JOBJ jobj)
                    LoadModel(jobj);
            }
            else
            {
                string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                if (f != null)
                {
                    HSDRawFile hsd = new(f);
                    if (hsd.Roots[0].Data is HSD_JOBJ jobj)
                        LoadModel(jobj);
                }
            }

        }
    }
}
