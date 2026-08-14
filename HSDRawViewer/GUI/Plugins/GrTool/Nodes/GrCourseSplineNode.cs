using HSDRawViewer.GUI.Plugins.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryCourseSpline : GrCategoryNode<KdCourseSpline, GrCourseSplineNode>
    {
        public GrCategoryCourseSpline(string name, ObservableList<KdCourseSpline> list, KdCourseSplineSetup setup) : base(name, list)
        {
            Tag = setup;
        }

        protected override GrCourseSplineNode CreateChild(KdCourseSpline m)
        {
            return new GrCourseSplineNode(m);
        }

        private void ImportSplines(IEnumerable<(string, KdSpline)> splines)
        {
            TreeView.BeginUpdate();

            foreach (var s in splines)
            {
                var cs = new KdCourseSpline();
                cs.Spline = s.Item2;
                cs.FromString(s.Item1);

                list.Add(cs);
            }
            TreeView.EndUpdate();
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
        {
            if (selected_node != this) return;

            menu.Items.Add("Import and Add All...", null, (s, e) =>
            {
                var splines = KdSplineIO.ImportSplines().ToArray();

                if (splines.Length == 0) return;

                ImportSplines(splines);
            });

            menu.Items.Add("Import and Replace All...", null, (send, e) =>
            {
                var splines = KdSplineIO.ImportSplines().ToArray();

                if (splines.Length == 0) return;

                list.Clear();

                ImportSplines(splines);
            });

            menu.Items.Add("Export All...", null, (s, e) =>
            {
                List<KdSpline> splines = new List<KdSpline>();
                List<string> names = new List<string>();
                foreach (var l in list)
                {
                    splines.Add(l.Spline);
                    names.Add(l.ToString());
                }
                KdSplineIO.ExportSplines(Text, names.ToArray(), splines);
            });
        }

    }

    public class GrCourseSplineNode : GrSplineNode
    {
        public override KdSpline GetSpline()
        {
            if (Tag is not KdCourseSpline spline) return null;
            return spline.Spline;
        }

        public GrCourseSplineNode(KdCourseSpline course_spline) : base(course_spline.Spline)
        {
            DisplayColor = new OpenTK.Mathematics.Vector4(0f, 0f, 1f, 1f);
            Tag = course_spline;
        }
    }
}
