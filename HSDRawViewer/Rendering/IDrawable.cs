using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering
{
    public enum DrawOrder
    {
        First,
        Middle,
        Last
    }

    /// <summary>
    /// Extension of <see cref="IDrawable"/> that also implements interactive elements
    /// </summary>
    public interface IDrawableInterface : IDrawable
    {
        void ScreenClick(MouseButtons button, PickInformation pick);

        void ScreenDoubleClick(PickInformation pick);

        void ScreenDrag(PickInformation pick, float deltaX, float deltaY);

        void ScreenSelectArea(PickInformation start, PickInformation end);
    }

    /// <summary>
    /// Defines a drawable interface for use with the Viewport Control
    /// </summary>
    public interface IDrawable
    {
        DrawOrder DrawOrder { get; }

        void Draw(int windowWidth, int windowHeight);
    }

    /// <summary>
    /// 
    /// </summary>
    public class DrawableCircle : IDrawable
    {
        public Vector3 Position { get; set; } = Vector3.Zero;

        public float Radius { get; set; } = 1;

        public Color Color { get; set; } = Color.White;

        public DrawOrder DrawOrder => DrawOrder.Middle;

        public DrawableCircle(float X, float Y, float Radius)
        {
            Position = new Vector3(X, Y, 0);
            this.Radius = Radius;
        }

        public void Draw(int windowWidth, int windowHeight)
        {
            GL.Begin(PrimitiveType.TriangleFan);

            GL.Color3(Color);
            
            GL.Vertex3(Position); // center of circle
            for (var i = 0; i <= 20; i++)
            {
                GL.Vertex3(
                    (Position.X + (Radius * Math.Cos(i * Math3D.TwoPI / 20))),
                    (Position.Y + (Radius * Math.Sin(i * Math3D.TwoPI / 20))),
                    Position.Z);
            }

            GL.End();
        }
    }
}
