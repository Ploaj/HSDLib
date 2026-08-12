using HSDRawViewer.GUI.Plugins.GrTool;
using HSDRawViewer.GUI.Plugins.GrTool.Converters;
using HSDRawViewer.GUI.Plugins.GrTool.Render;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrSplineNode : GrDrawNode, IGrTranslate, IUndo
    {
        private bool CacheInvalid = true;

        private GrSplineCache GrSplineCache = new GrSplineCache();

        public Vector4 DisplayColor = Vector4.One;

        public Vector4 SelectedColor = new Vector4(1f, 1f, 0f, 1f);

        public Vector4 SelectedPointColor = new Vector4(1f, 1f, 0f, 1f);

        public GrSplineNode(KdSpline spline)
        {
            Text = "Spline";
            Tag = spline;
            Checked = true;
        }

        public virtual KdSpline GetSpline()
        {
            return Tag as KdSpline;
        }

        public void InvalidateCache()
        {
            CacheInvalid = true;
        }

        public override void OnTagPropertyUpdate(PropertyValueChangedEventArgs args)
        {
            if (GetSpline() is not KdSpline s) return;

            var prop = args.ChangedItem.PropertyDescriptor.Name;

            if (prop == nameof(KdSpline.Kind) ||
                prop == nameof(KdSpline.Tension) && s.Kind == KdSplineKind.Tension)
            {
                s.RebuildArcLengthData();
                InvalidateCache();
            }
        }

        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("Import Spline from OBJ...", null, (s, e) =>
            {
                if (GetSpline() == null) return;

                var splines = KdSplineIO.ImportSplines().ToArray();
                if (splines.Length == 0) return;

                var spline = GetSpline();
                spline.Points.Clear();
                spline.Points.AddRange(splines[0].Item2.Points);
                spline.RebuildArcLengthData();
                InvalidateCache();
            });

            menu.Items.Add("Export Spline to OBJ...", null, (s, e) =>
            {
                if (GetSpline() == null) return;
                KdSplineIO.ExportSplines(Text, null, new KdSpline[] { GetSpline() });
            });
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (GetSpline() is not KdSpline s) return;
            if (!Visible) return;

            if (CacheInvalid)
            {
                GrSplineCache.BuildCache(s);
                CacheInvalid = false;
            }

            bool isSelected = IsSelected || IsParentSelected();

            if (isSelected)
                GrSplineCache.DrawSpline(s, 0f, 1f, SelectedColor, 3.0f);

            GrSplineCache.DrawSpline(s, 0f, 1f, DisplayColor, isSelected ? 2.0f : 1.0f);

            if (IsSelected)
            {
                GrSplineCache.DrawSplinePoints(s, selected_object, DisplayColor, SelectedPointColor, 2.0f);
            }
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
        }

        private KdVector selected_point = null;

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            if (GetSpline() is not KdSpline s) return null;
            if (!Visible) return null;

            float distance = float.PositiveInfinity;
            selected_point = null;
            foreach (var p in s.Points)
            {
                if (pick.CheckScreenPoint(p.ToTkVector(), 4f, out float d))
                {
                    if (d < distance)
                    {
                        selected_point = p;
                        distance = d;
                    }
                }
            }

            return selected_point;
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.PositiveInfinity;

            if (GetSpline() is not KdSpline s) return false;
            if (!Visible) return false;

            if (GrSplineCache.TryPickLines(pick, out distance))
                return true;

            return false;
        }

        public bool CanTranslate(object selected_object)
        {
            if (GetSpline() is not KdSpline s) return false;
            if (!Visible) return false;

            if (selected_object == selected_point)
            {
                return true;
            }

            return false;
        }

        public Vector3 GetTranslate(object selected_object, LiveJObj joint)
        {
            if (GetSpline() is not KdSpline s) return Vector3.Zero;

            if (selected_object == selected_point)
            {
                return selected_point.ToTkVector();
            }

            return Vector3.Zero;
        }

        public void SetTranslate(object selected_object, LiveJObj joint, Vector3 value)
        {
            if (GetSpline() is not KdSpline s) return;

            if (selected_object == selected_point)
            {
                selected_point.X = value.X;
                selected_point.Y = value.Y;
                selected_point.Z = value.Z;
                InvalidateCache();
            }
        }

        private ObjectUndoManager _undo = new ObjectUndoManager();

        public void Undo(object selected_object)
        {
            _undo.Undo();

            if (GetSpline() is not KdSpline s) return;
            s.RebuildArcLengthData();
            InvalidateCache();
        }

        public void Redo(object selected_object)
        {
            _undo.Redo();

            if (GetSpline() is not KdSpline s) return;
            s.RebuildArcLengthData();
            InvalidateCache();
        }

        public void Commit(object selected_object)
        {
            if (selected_object is KdVector v)
            {
                _undo.Commit(v);
            }
        }

        public void ClearHistory()
        {
            _undo.ClearHistory();
        }
    }
}
