using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private float _zoom = 0.8f;
        
        public Brush SelectedPointColor = new SolidBrush(Color.Yellow);
        public Brush PointColor = new SolidBrush(Color.White);
        public Pen PointOutline = new Pen(Color.Black);
        public Pen LineColor = new Pen(Color.White);
        public Pen SlopeColor = new Pen(Color.Purple);

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
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            _xScale = bounds.Width / (float)_frameCount;
            _yScale = bounds.Height / _range;

            if (_yScale < 1)
                _yScale = 1;

            //_xScale *= _zoom;
            //_yScale *= _zoom;

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
                    var x = _player.Keys[i].Frame * _xScale;
                    var y = (_player.Keys[i].Value - _minValue) * _yScale;

                    g.FillEllipse(_player.Keys[i].Frame >= selectedFrameStart && _player.Keys[i].Frame <= selectedFrameEnd ? SelectedPointColor : PointColor, new RectangleF(x - _xScale / 2, y - _xScale / 2, _xScale, _xScale));
                    g.DrawEllipse(PointOutline, new RectangleF(x - _xScale / 2, y - _xScale / 2, _xScale, _xScale));
                }
            }
        }
        
    }
}
