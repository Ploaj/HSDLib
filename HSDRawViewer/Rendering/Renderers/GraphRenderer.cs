using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace HSDRawViewer.Rendering.Renderers
{
    /// <summary>
    /// Helper class for rendering graph data for a list of keys
    /// </summary>
    public class GraphRenderer
    {
        private FOBJ_Player _player = new FOBJ_Player();

        public float MaxValue { get => _maxValue; }
        public float MinValue { get => _minValue; }

        private float _minValue;
        private float _maxValue;

        private List<float> _valueCache = new List<float>();

        private int _frameCount { get => _player.FrameCount; }
        
        private float _range { get => (_maxValue - _minValue); }

        private float _xScale;
        private float _yScale;

        private float _xOffset;
        private float _yOffset;

        private float _zoom = 10f;

        public bool RenderTangents { get; set; } = true;

        public Vector4 SelectedPointColor = new Vector4(1f, 1f, 0f, 1f);
        public Vector4 PointColor = new Vector4(1f, 1f, 1f, 1f);
        public Vector4 PointOutline = new Vector4(0f, 0f, 0f, 1f);

        public Vector4 LineColor = new Vector4(1f, 1f, 1f, 1f);

        public Vector4 SlopeColor = new Vector4(1f, 0f, 1f, 1f);
        public Vector4 SelectedSlopeColor = new Vector4(1f, 1f, 0f, 1f);

        public bool RenderPoints { get; set; } = true;

        /// <summary>
        /// Updates the render with changes to the keys
        /// </summary>
        public void SetKeys(FOBJ_Player player)
        {
            _player = player;
            UpdateKeys();
        }

        /// <summary>
        /// Updates the render with changes to the keys
        /// </summary>
        public void UpdateKeys()
        {
            _minValue = float.MaxValue;
            _maxValue = float.MinValue;
            _valueCache.Clear();
            for(int i = 0; i < (_player.FrameCount == 0 ? _player.Keys.Count : _player.FrameCount); i++)
            {
                var v = _player.GetValue(i);
                _minValue = Math.Min(_minValue, v);
                _maxValue = Math.Max(_maxValue, v);
                _valueCache.Add(v);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Draw(Rectangle bounds, int selectedFrameStart, int selectedFrameEnd)
        {
            if (_frameCount == 0)
                return;

            _xOffset = 0;
            _yOffset = _zoom;

            _xScale = bounds.Width / (float)_frameCount;
            _yScale = (bounds.Height - _zoom * 2) / _range;

            if (_yScale == 0)
                _yScale = 0.00001f;
            
            if (_range == 0)
                _yScale = bounds.Height / 1f;


            // draw line
            var prevX = 0f;
            var prevY = 0f;
            GL.Color4(LineColor);
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < _frameCount; i++)
            {
                var v = _valueCache[i];
                var x = i * _xScale;
                var y = (v - _minValue) * _yScale;

                x += _xOffset;
                y += _yOffset;

                if (i == 0)
                {
                    prevX = x;
                    prevY = y;
                }

                if (_player.GetState(i).op_intrp == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_CON)
                {
                    GL.Vertex2(x, prevY); GL.Vertex2(prevX, prevY);
                    //g.DrawLine(LineColor, new PointF(x, prevY), new PointF(prevX, prevY));

                    GL.Vertex2(x, y); GL.Vertex2(x, prevY);
                    //g.DrawLine(LineColor, new PointF(x, y), new PointF(x, prevY));
                }
                else
                {
                    GL.Vertex2(x, y); GL.Vertex2(prevX, prevY);
                    // g.DrawLine(LineColor, new PointF(x, y), new PointF(prevX, prevY));
                }

                prevX = x;
                prevY = y;
            }
            GL.End();

            // draw points
            if(RenderPoints && _player.Keys != null && _player.Keys.Count > 1)
            {
                for (int i = 0; i < _player.Keys.Count; i++)
                {
                    var key = _player.Keys[i];
                    var outTan = key.Tan;

                    if (key.InterpolationType == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SLP)
                        continue;

                    if (i + 1 < _player.Keys.Count && _player.Keys[i+1].InterpolationType == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SLP)
                        outTan = _player.Keys[i + 1].Tan;

                    var clr = PointColor;

                    var selected = key.Frame >= selectedFrameStart && key.Frame <= selectedFrameEnd;

                    if (selected)
                        clr = SelectedPointColor;

                    var x = key.Frame * _xScale;
                    var y = (key.Value - _minValue) * _yScale;

                    x += _xOffset;
                    y += _yOffset;

                    float prevFrame = 0 - 5;
                    float nextFrame = _player.FrameCount + 5;

                    if (i + 1 < _player.Keys.Count)
                        nextFrame = _player.Keys[i + 1].Frame;

                    if (i > 0)
                        prevFrame = _player.Keys[i - 1].Frame;

                    if (RenderTangents &&
                        (
                        key.InterpolationType == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL ||
                        key.InterpolationType == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL0
                        )
                       )
                        DrawTangent(key, prevFrame, nextFrame, outTan, selected);

                    var rect = new Rectangle((int)(x - _xScale / 2), (int)(y - _xScale / 2), (int)(_xScale), (int)(_xScale));

                    switch (key.InterpolationType)
                    {
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_NONE:
                            DrawCircle(PointOutline, clr, new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), rect.Width / 2);
                            //g.DrawEllipse(LineColor, rect);
                            break;
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_KEY:
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_CON:
                            //g.FillRectangle(clr, rect);
                            //g.DrawRectangle(PointOutline, rect);
                            DrawSquare(PointOutline, clr, rect);
                            break;
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN:
                            //g.TranslateTransform(x, y);
                            //g.RotateTransform(45);
                            //g.FillRectangle(clr, -_xScale / 2, -_xScale / 2, _xScale, _xScale);
                            //g.DrawRectangle(PointOutline, -_xScale / 2, -_xScale / 2, _xScale, _xScale);
                            //g.Transform = tra;
                            DrawDiamond(PointOutline, clr, rect);
                            break;
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL:
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL0:
                            //g.FillEllipse(clr, rect);
                            //g.DrawEllipse(PointOutline, rect);
                            DrawCircle(PointOutline, clr, new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), rect.Width / 2);
                            break;
                    }
                }
            }
        }

        private void DrawSquare(Vector4 outline, Vector4 fill, Rectangle r)
        {
            GL.Color4(fill);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(r.Left, r.Top);
            GL.Vertex2(r.Right, r.Top);
            GL.Vertex2(r.Right, r.Bottom);
            GL.Vertex2(r.Left, r.Bottom);

            GL.End();

            GL.Color4(outline);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex2(r.Left, r.Top);
            GL.Vertex2(r.Right, r.Top);
            GL.Vertex2(r.Right, r.Bottom);
            GL.Vertex2(r.Left, r.Bottom);

            GL.End();
        }

        private void DrawDiamond(Vector4 outline, Vector4 fill, Rectangle r)
        {
            GL.Color4(fill);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(r.X + r.Width / 2, r.Top);
            GL.Vertex2(r.Right, r.Y + r.Height / 2);
            GL.Vertex2(r.X + r.Width / 2, r.Bottom);
            GL.Vertex2(r.Left, r.Y + r.Height / 2);

            GL.End();

            GL.Color4(outline);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex2(r.X + r.Width / 2, r.Top);
            GL.Vertex2(r.Right, r.Y + r.Height / 2);
            GL.Vertex2(r.X + r.Width / 2, r.Bottom);
            GL.Vertex2(r.Left, r.Y + r.Height / 2);

            GL.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="fill"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        private void DrawCircle(Vector4 outline, Vector4 fill, Vector2 center, float radius)
        {
            int i;
            float x = center.X; 
            float y = center.Y;
            double twicePi = 2.0 * 3.142;

            GL.Color4(fill);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(x, y);
            for (i = 0; i <= 20; i++)
            {
                GL.Vertex2(
                    (x + (radius * Math.Cos(i * twicePi / 20))), (y + (radius * Math.Sin(i * twicePi / 20))));
            }
            GL.End();

            GL.Color4(outline);
            GL.Begin(PrimitiveType.LineLoop);
            for (i = 0; i < 20; i++)
            {
                GL.Vertex2(
                    (x + (radius * Math.Cos(i * twicePi / 20))), (y + (radius * Math.Sin(i * twicePi / 20))));
            }
            GL.End();
        }

        private float _tanLen = 5;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="key"></param>
        /// <param name="selected"></param>
        private void DrawTangent(FOBJKey key, float prevFrame, float nextFrame, float outTangent, bool selected)
        {
            var x = key.Frame;
            var y = key.Value;
            var tan = key.Tan;

            var i1 = -(_tanLen / 2);
            var i2 = (_tanLen / 2);

            var p = (float)Math.Sqrt((x - prevFrame) ) / 3;

            var one_x = (x + i1 * p) * _xScale + _xOffset;
            var one_y = (y - _minValue + tan * i1 * p) * _yScale + _yOffset;

            var two_x = (x) * _xScale + _xOffset;
            var two_y = (y - _minValue) * _yScale + _yOffset;

            p = (float)Math.Sqrt((nextFrame - x)) / 3;

            var one_x_o = (x) * _xScale + _xOffset;
            var one_y_o = (y - _minValue) * _yScale + _yOffset;

            var two_x_o = (x + i2 * p) * _xScale + _xOffset;
            var two_y_o = (y - _minValue + outTangent * i2 * p) * _yScale + _yOffset;


            float angle1 = (float)Math.Atan((tan * _yScale) / _xScale) * 180 / (float)Math.PI;
            float angle2 = (float)Math.Atan((outTangent * _yScale) / _xScale) * 180 / (float)Math.PI;

            var clr = selected ? SelectedSlopeColor : SlopeColor;

            GL.Color4(clr);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(one_x, one_y); GL.Vertex2(two_x, two_y);
            GL.Vertex2(one_x_o, one_y_o); GL.Vertex2(two_x_o, two_y_o);
            GL.End();
            //g.DrawLine(clr, one_x, one_y, two_x, two_y);
            //g.DrawLine(clr, one_x_o, one_y_o, two_x_o, two_y_o);

            //var tra = g.Transform;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(one_x, one_y, 0);
            GL.Rotate(angle1 - 180, 0, 0, 1);
            GL.Color4(clr);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(-7f, 3.5f); GL.Vertex2(0, 0);
            GL.Vertex2(0, 0); GL.Vertex2(-7f, -3.5f);
            GL.End();
            //g.TranslateTransform(one_x, one_y);
            //g.RotateTransform(angle1 - 180);
            //g.DrawLine(clr, -7f, 3.5f, 0, 0);
            //g.DrawLine(clr, 0, 0, -7f, -3.5f);
            GL.PopMatrix();

            //g.Transform = tra;

            GL.PushMatrix();
            GL.Translate(two_x_o, two_y_o, 0);
            GL.Rotate(angle2, 0, 0, 1);
            GL.Color4(clr);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(-7f, 3.5f); GL.Vertex2(0, 0);
            GL.Vertex2(0, 0); GL.Vertex2(-7f, -3.5f);
            GL.End();
            //g.TranslateTransform(two_x_o, two_y_o);
            //g.RotateTransform(angle2);
            //g.DrawLine(clr, -7f, 3.5f, 0, 0);
            //g.DrawLine(clr, 0, 0, -7f, -3.5f);
            GL.PopMatrix();

            //g.Transform = tra;
        }
        
    }
}
