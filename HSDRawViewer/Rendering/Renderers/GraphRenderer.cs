using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HSDRawViewer.Rendering.Renderers
{
    /// <summary>
    /// Helper class for rendering graph data for a list of keys
    /// </summary>
    public class GraphRenderer
    {
        private FOBJ_Player _player = new FOBJ_Player();

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
        
        public Brush SelectedPointColor = new SolidBrush(Color.Yellow);
        public Brush PointColor = new SolidBrush(Color.White);
        public Pen PointOutline = new Pen(Color.Black);

        public Pen LineColor = new Pen(Color.White);

        public Pen SlopeColor = new Pen(Color.FromArgb(255, 255, 64, 255));
        public Pen SelectedSlopeColor = new Pen(Color.Yellow);

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
            for(int i = 0; i < _player.FrameCount; i++)
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
        public void Draw(Graphics g, Rectangle bounds, int selectedFrameStart, int selectedFrameEnd)
        {
            _xOffset = 0;
            _yOffset = _zoom;

            _xScale = bounds.Width / (float)(_frameCount);
            _yScale = (bounds.Height - _zoom * 2) / _range;

            if (_yScale < 1)
                _yScale = 1;
            
            if (_range == 0)
                _yScale = bounds.Height / 1f;


            // draw line
            var prevX = 0f;
            var prevY = 0f;
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

                g.DrawLine(LineColor, new PointF(x, y), new PointF(prevX, prevY));

                prevX = x;
                prevY = y;
            }

            // draw points
            if(RenderPoints)
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

                    if (RenderTangents &&
                        (
                        key.InterpolationType == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL ||
                        key.InterpolationType == HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL0
                        )
                       )
                        DrawTangent(g, key, outTan, selected);

                    var rect = new Rectangle((int)(x - _xScale / 2), (int)(y - _xScale / 2), (int)(_xScale), (int)(_xScale));

                    switch (key.InterpolationType)
                    {
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_NONE:
                            g.DrawEllipse(LineColor, rect);
                            break;
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_KEY:
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_CON:
                            g.FillRectangle(clr, rect);
                            g.DrawRectangle(PointOutline, rect);
                            break;
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN:
                            g.TranslateTransform(x, y);
                            g.RotateTransform(45);
                            g.FillRectangle(clr, -_xScale / 2, -_xScale / 2, _xScale, _xScale);
                            g.DrawRectangle(PointOutline, -_xScale / 2, -_xScale / 2, _xScale, _xScale);
                            g.ResetTransform();
                            break;
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL:
                        case HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_SPL0:
                            g.FillEllipse(clr, rect);
                            g.DrawEllipse(PointOutline, rect);
                            break;
                    }
                }
            }
        }

        private float _tanLen = 5;
        private float _precision = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="key"></param>
        /// <param name="selected"></param>
        private void DrawTangent(Graphics g, FOBJKey key, float outTangent, bool selected)
        {
            var x = key.Frame;
            var y = key.Value;
            var tan = key.Tan;

            var i1 = -(_tanLen / 2);
            var i2 = (_tanLen / 2);

            var p = (float)Math.Sqrt(_precision / 4.0f);

            var one_x = (x + i1 * p) * _xScale + _xOffset;
            var one_y = (y - _minValue + tan * i1 * p) * _yScale + _yOffset;

            var two_x = (x) * _xScale + _xOffset;
            var two_y = (y - _minValue) * _yScale + _yOffset;


            var one_x_o = (x) * _xScale + _xOffset;
            var one_y_o = (y - _minValue) * _yScale + _yOffset;

            var two_x_o = (x + i2 * p) * _xScale + _xOffset;
            var two_y_o = (y - _minValue + outTangent * i2 * p) * _yScale + _yOffset;


            float angle1 = (float)Math.Atan((tan * _yScale) / _xScale) * 180 / (float)Math.PI;
            float angle2 = (float)Math.Atan((outTangent * _yScale) / _xScale) * 180 / (float)Math.PI;

            var clr = selected ? SelectedSlopeColor : SlopeColor;

            g.DrawLine(clr, one_x, one_y, two_x, two_y);
            g.DrawLine(clr, one_x_o, one_y_o, two_x_o, two_y_o);

            g.TranslateTransform(one_x, one_y);
            g.RotateTransform(angle1 - 180);
            g.DrawLine(clr, -7f, 3.5f, 0, 0);
            g.DrawLine(clr, 0, 0, -7f, -3.5f);
            g.ResetTransform();

            g.TranslateTransform(two_x_o, two_y_o);
            g.RotateTransform(angle2);
            g.DrawLine(clr, -7f, 3.5f, 0, 0);
            g.DrawLine(clr, 0, 0, -7f, -3.5f);
            g.ResetTransform();
        }
        
    }
}
