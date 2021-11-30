using System.Windows.Forms;
using HSDRaw;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;
using HSDRaw.Common;
using HSDRawViewer.Rendering.Models;

namespace HSDRawViewer.Rendering
{
    public class CommonViewport : DockContent, IDrawable
    {
        public ViewportControl glViewport;

        public int ViewportWidth => glViewport.Width;
        public int ViewportHeight => glViewport.Height;

        public Camera Camera;

        public bool ReadyToRender { get; internal set; } = false;

        private HSDAccessor _selectedAccessor { get; set; }

        public DrawOrder DrawOrder => DrawOrder.First;

        public CommonViewport()
        {
            Text = "Viewport";

            //_glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            glViewport = new ViewportControl();
            glViewport.Dock = DockStyle.Fill;
            glViewport.EnableFloor = true;

            glViewport.AddRenderer(this);

            Controls.Add(glViewport);

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    //MainForm.Instance.TryClose(this);
                }
            };

            Sim.iterations = 3;
            Point o = null;
            for (int i = 0; i < 10; i++)
            {
                var p = Sim.CreatePoint(new Vector3(i * 5, 0, 0), 0.005f);

                if (o != null)
                    Sim.CreateSpring(p, o).CalculateLength();

                o = p;
            }
        }

        private class Point
        {
            public Vector3 pos;
            public Vector3 oldpos;
            public float gravity;
            public float friction;

            public int JOBJ_Index = -1;

            public void Update()
            {
                // process velocity
                Vector3 vel = pos - oldpos;
                vel *= friction;

                // update mass location
                oldpos = pos;
                pos = pos + vel;

                // process gravity
                pos.Y -= gravity;
            }
        }

        private class Spring
        {
            public Point p0;
            public Point p1;
            float length;

            public void CalculateLength()
            {
                length = Vector3.Distance(p0.pos, p1.pos);
            }

            public void Update()
            {
                Vector3 dx = p1.pos - p0.pos;
                float distance = Vector3.Distance(p1.pos, p0.pos);

                //if (min_length == 0 || distance < min_length)
                {
                    float diff = length - distance;
                    float percent = diff / distance / 2;
                    Vector3 off = dx * percent;

                    //if (!spring->p0->pinned)
                    p0.pos -= off;

                    //if (!spring->p1->pinned)
                    p1.pos += off;
                }
            }
        }

        private class Simulation
        {
            public int mass_num = 0;
            public int spring_num = 0;
            public int iterations = 0;
            public Point[] masses = new Point[20 + 1];
            public Spring[] springs = new Spring[20];

            public Point CreatePoint(Vector3 pos, float gravity)
            {
                Point mass = new Point()
                {
                    gravity = gravity,
                    friction = 1, // 0.99f,
                    pos = pos,
                    oldpos = pos,
                };
                masses[mass_num] = mass;
                mass_num++;
                return mass;
            }

            public Spring CreateSpring(Point p0, Point p1)
            {
                Spring spring = new Spring();
                spring.p0 = p0;
                spring.p1 = p1;

                springs[spring_num] = spring;
                spring_num++;
                return spring;
            }

            /// 
            /// Updates the manager for a new frame
            /// 
            public void UpdateConstrains(Vector3 pos)
            {
                // Update Springs
                for (int i = 0; i < spring_num; i++)
                {
                    Spring spring = springs[i];
                    spring.Update();
                }

                // Update Constrains
                for (int i = 0; i < mass_num; i++)
                {
                    Point mass = masses[i];

                    // pin spring 0
                    if (i == 5)
                        mass.pos = pos;
                }
            }

            /// 
            /// Updates the manager for a new frame
            /// 
            public void Update(Vector3 pos)
            {
                // Update Masses
                for (int i = 0; i < mass_num; i++)
                {
                    masses[i].Update();
                }

                for (int i = 0; i < iterations; i++)
                    UpdateConstrains(pos);
            }

        }

        private Simulation Sim = new Simulation();

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            GL.PushMatrix();
            var scale = Matrix4.CreateScale(10, 10, 10);
            //GL.MultMatrix(ref scale);

            Sim.Update(Vector3.Zero);

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.DepthTest);
            GL.LineWidth(2f);
            GL.Begin(PrimitiveType.Lines);

            for (int i = 0; i < Sim.spring_num; i++)
            {
                var spr = Sim.springs[i];

                GL.Color3(1f, 1f, 1f);
                GL.Vertex3(spr.p0.pos);

                GL.Color3(1f, 1f, 1f);
                GL.Vertex3(spr.p1.pos);

                var cross = spr.p0.pos + Vector3.Cross(Vector3.UnitX, (spr.p0.pos - spr.p1.pos).Normalized()).Normalized();

                GL.Color3(1f, 0, 0);
                GL.Vertex3(spr.p0.pos);

                GL.Color3(1f, 0, 0);
                GL.Vertex3(cross);

                var cross2 = spr.p0.pos + Vector3.Cross(Vector3.UnitY, (spr.p0.pos - spr.p1.pos).Normalized()).Normalized();

                GL.Color3(0, 1f, 0);
                GL.Vertex3(spr.p0.pos);

                GL.Color3(0, 1f, 0);
                GL.Vertex3(cross2);
            }

            GL.End();
            GL.PopAttrib();
            GL.PopMatrix();
        }
    }
}
