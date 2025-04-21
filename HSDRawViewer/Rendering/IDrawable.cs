﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Drawing;
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
        void ViewportKeyPress(KeyEventArgs kbState);

        void ScreenClick(MouseButtons button, PickInformation pick);

        void ScreenDoubleClick(PickInformation pick);

        void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY);

        void ScreenSelectArea(PickInformation start, PickInformation end);

        bool FreezeCamera();
    }

    /// <summary>
    /// Defines a drawable interface for use with the Viewport Control
    /// </summary>
    public interface IDrawable
    {
        DrawOrder DrawOrder { get; }

        /// <summary>
        /// draw
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        void Draw(Camera cam, int windowWidth, int windowHeight);

        /// <summary>
        /// Init any gl resources you wish to use
        /// </summary>
        void GLInit();

        /// <summary>
        /// Free any gl resources
        /// </summary>
        void GLFree();
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
            Draw(null, windowWidth, windowHeight);
        }

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            GL.Begin(PrimitiveType.TriangleFan);

            GL.Color3(Color);

            GL.Vertex3(Position); // center of circle
            for (int i = 0; i <= 20; i++)
            {
                GL.Vertex3(
                    (Position.X + (Radius * Math.Cos(i * Math3D.TwoPI / 20))),
                    (Position.Y + (Radius * Math.Sin(i * Math3D.TwoPI / 20))),
                    Position.Z);
            }

            GL.End();
        }

        public void InitializeDrawing()
        {
        }

        public void GLInit()
        {
        }

        public void GLFree()
        {
        }
    }
}
