using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRaw.Tools.TriangleConverter
{
    public class PrimitiveGroup
    {
        public List<ushort> _nodes = new List<ushort>();

        public List<PointTriangle> _triangles = new List<PointTriangle>();
        public List<PointTriangleStrip> _tristrips = new List<PointTriangleStrip>();

        private void AddTriangle(PointTriangle t)
        {
            _triangles.Add(t);
            if (!_nodes.Contains(t._x.PNMTXIDX)) _nodes.Add(t._x.PNMTXIDX);
            if (!_nodes.Contains(t._y.PNMTXIDX)) _nodes.Add(t._y.PNMTXIDX);
            if (!_nodes.Contains(t._z.PNMTXIDX)) _nodes.Add(t._z.PNMTXIDX);
        }
        private void AddTristrip(PointTriangleStrip t)
        {
            _tristrips.Add(t);
            foreach (GX_Vertex p in t._points)
                if (!_nodes.Contains(p.PNMTXIDX))
                    _nodes.Add(p.PNMTXIDX);
        }

        public bool TryAdd(PrimitiveClass p)
        {
            if (p is PointTriangleStrip)
                return TryAdd(p as PointTriangleStrip);
            else if (p is PointTriangle)
                return TryAdd(p as PointTriangle);
            return false;
        }

        private bool TryAdd(PointTriangleStrip t)
        {
            List<ushort> newIds = new List<ushort>();
            foreach (GX_Vertex p in t._points)
            {
                ushort id = p.PNMTXIDX;
                if (!_nodes.Contains(id) && !newIds.Contains(id))
                    newIds.Add(id);
            }

            if (newIds.Count + _nodes.Count <= 10)
            {
                AddTristrip(t);
                return true;
            }
            return false;
        }

        private bool TryAdd(PointTriangle t)
        {
            List<ushort> newIds = new List<ushort>();

            ushort x = t._x.PNMTXIDX;
            ushort y = t._y.PNMTXIDX;
            ushort z = t._z.PNMTXIDX;

            if (!_nodes.Contains(x) && !newIds.Contains(x)) newIds.Add(x);
            if (!_nodes.Contains(y) && !newIds.Contains(y)) newIds.Add(y);
            if (!_nodes.Contains(z) && !newIds.Contains(z)) newIds.Add(z);

            //There's a limit of 10 matrices per group...
            if (newIds.Count + _nodes.Count <= 10)
            {
                AddTriangle(t);
                return true;
            }
            return false;
        }
    }

    public class PrimitiveClass
    {
        public virtual List<GX_Vertex> Points { get; set; }
        public static int Compare(PrimitiveClass p1, PrimitiveClass p2)
        {
            return p1.GetType() == p2.GetType() ? 0 : p1 is PointTriangleStrip ? -1 : 1;
        }
    }

    public class PointTriangleStrip : PrimitiveClass
    {
        public List<GX_Vertex> _points = new List<GX_Vertex>();

        public override List<GX_Vertex> Points
        {
            get { return _points; }
            set { _points = value; }
        }
    }

    public class PointTriangle : PrimitiveClass
    {
        public GX_Vertex _x;
        public GX_Vertex _y;
        public GX_Vertex _z;

        public GX_Vertex this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return _x;
                    case 1: return _y;
                    case 2: return _z;
                }
                return new GX_Vertex();
            }
            set
            {
                switch (i)
                {
                    case 0: _x = value; break;
                    case 1: _y = value; break;
                    case 2: _z = value; break;
                }
            }
        }

        public override List<GX_Vertex> Points
        {
            get
            {
                return new List<GX_Vertex>() { _x, _y, _z };
            }
            set
            {
                _x = value[0];
                _y = value[1];
                _z = value[2];
            }
        }

        public PointTriangle() { }
        public PointTriangle(GX_Vertex x, GX_Vertex y, GX_Vertex z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public bool Contains(GX_Vertex f)
        {
            if (_x == f) return true;
            if (_y == f) return true;
            if (_z == f) return true;
            return false;
        }
    }
}
