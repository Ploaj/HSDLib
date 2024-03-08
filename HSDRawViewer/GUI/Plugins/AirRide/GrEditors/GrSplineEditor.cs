using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRawViewer.Rendering;
using System.Drawing;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    public class GrSplineEditor
    {
        public HSD_Spline[] _items { get; set; }

        private KAR_grSplineList _list;

        public GrSplineEditor(KAR_grSplineList splineList)
        {
            _list = splineList;
            _items = splineList.Splines;
        }

        public void Render(object selected)
        {
            foreach (var s in _items)
            {
                if (selected != null && s == (HSD_Spline)selected)
                    DrawShape.RenderSpline(s, Color.Yellow, Color.Red);
                else
                    DrawShape.RenderSpline(s, Color.Green, Color.Blue);
            }
        }

        public void Update()
        {
            if (_items != null && _items.Length > 0)
            {
                _list.Splines = _items;
            }
            else
            {
                _list.Splines = null;
            }
        }
    }
}
