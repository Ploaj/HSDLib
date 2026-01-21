using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering;
using System;
using System.Drawing;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    public class GrRangeSplineEditor
    {
        public KAR_grRangeSpline[] _items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void Render(object selected)
        {
            foreach (KAR_grRangeSpline s in _items)
            {
                if (s == (KAR_grRangeSpline)selected)
                {
                    DrawShape.RenderSpline(s.LeftSpline, Color.Yellow, Color.Red);
                    DrawShape.RenderSpline(s.RightSpline, Color.Yellow, Color.Red);
                }
                else
                {
                    DrawShape.RenderSpline(s.LeftSpline, Color.Green, Color.Blue);
                    DrawShape.RenderSpline(s.RightSpline, Color.Green, Color.Blue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
