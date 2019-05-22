using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSDLib;
using HSDLib.Common;
using HSDLib.Animation;
using HALSysDATViewer.Nodes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HALSysDATViewer.Rendering;

namespace HALSysDATViewer
{
    public partial class Form1 : Form
    {
        public HSDFile HSD;
        public JOBJRenderer Renderer;

        public Camera Camera = new Camera();
        bool ReadyToRender = false;

        private string FileName;

        public Form1()
        {
            InitializeComponent();
            ReadyToRender = true;
        }

        public void OpenDAT(string FNAME)
        {
            HSD = new HSDFile(FNAME);
            FileName = FNAME;
            RefreshNodes();
        }

        public void SaveDAT(string FNAME)
        {
            if(HSD != null)
                HSD.Save(FNAME);
        }

        private void RefreshNodes()
        {
            nodeTree.Nodes.Clear();
            nodeTree.BeginUpdate();
            foreach(HSDRoot root in HSD.Roots)
            {
                //FolderNode n = new FolderNode() { Text = root.Name };

                //if (root.Node is HSD_JOBJ)
                //    new Node_JOBJ((HSD_JOBJ)root.Node, n);

                Node_Generic generic = new Node_Generic(root.Node);
                nodeTree.Nodes.Add(generic);
                generic.Open();
            }
            nodeTree.EndUpdate();
            //nodeTree.ExpandAll();
        }

        private void nodeTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(nodeTree.SelectedNode is IBaseNode)
            {
                ((IBaseNode)nodeTree.SelectedNode).ParseData(dataGridView1);
            }
            if (nodeTree.SelectedNode is Node_JOBJ)
            {
                HSD_JOBJ j = ((Node_JOBJ)nodeTree.SelectedNode).JOBJ;
                if (j.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT))
                    Renderer.RootNode = j;
            }
            if (nodeTree.SelectedNode is Node_Generic)
            {
                IHSDNode Node = ((Node_Generic)nodeTree.SelectedNode).Node;
                if(Node is HSD_JOBJ)
                {
                    HSD_JOBJ j = (HSD_JOBJ)Node;
                    if (j.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) || j.Flags.HasFlag(JOBJ_FLAG.ROOT_OPA))
                        Renderer.RootNode = j;
                }
                if (Node is HSD_FigaTree)
                {
                    Renderer.FigaTree = (HSD_FigaTree)Node;
                    trackBar1.Maximum = (int)((HSD_FigaTree)Node).FrameCount;
                }

            }
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            Camera.renderWidth = glControl.Width;
            Camera.renderHeight = glControl.Height;
            Camera.Update();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var timer = new Timer();
            timer.Interval = 1000 / 120;
            timer.Tick += new EventHandler(Application_Idle);
            timer.Start();

            Renderer = new JOBJRenderer();
            //Renderer.SetHSD(smd.RootJOBJ);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (this.IsDisposed)
                return;

            if (ReadyToRender)
            {
                //glViewport.Invalidate();
                Render();
            }
        }

        private void Render()
        {
            glControl.MakeCurrent();
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // Bind the default framebuffer in case it was set elsewhere.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Push all attributes so we don't have to clean up later
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.ClearColor(Color.DarkSlateGray);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);


            if (glControl.ClientRectangle.Contains(glControl.PointToClient(Cursor.Position))
             && glControl.Focused)
            {
                Camera.Update();
            }
            try
            {
                if (OpenTK.Input.Mouse.GetState() != null)
                    Camera.mouseSLast = OpenTK.Input.Mouse.GetState().WheelPrecise;
            }
            catch
            {
            }
            
            GL.UseProgram(0);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 mat = Camera.mvpMatrix;
            GL.LoadMatrix(ref mat);

            // objects shouldn't show through opaque parts of floor
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);

            DrawTools.DrawFloor(Camera.mvpMatrix);

            //RenderHere
            Renderer.Render(ref Camera);

            GL.PopAttrib();
            glControl.SwapBuffers();
        }

        private void openDATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "HSD |*.dat";

                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenDAT(ofd.FileName);
                }
            }
        }

        private void exportDATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "HSD |*.dat";

                sfd.FileName = System.IO.Path.GetFileName(FileName);

                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveDAT(sfd.FileName);
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Renderer.SetFrame(trackBar1.Value);
        }
    }
}
