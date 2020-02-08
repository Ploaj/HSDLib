using HSDRaw.GX;
using System.Collections.Generic;

namespace HSDRaw.Common
{
    public class HSD_ShapeSet : HSDAccessor
    {
        public ushort Flags { get => _s.GetUInt16(0x00); set => _s.SetUInt16(0x00, value); }

        public ushort ShapeCount { get => _s.GetUInt16(0x02); set => _s.SetUInt16(0x02, value); }
        
        public int VertexIndexCount { get => _s.GetInt32(0x04); internal set => _s.SetInt32(0x04, value); }

        public GX_Attribute[] VertexAttributes
        {
            get
            {
                var data = _s.GetReference<HSDAccessor>(0x08);
                if (data == null)
                    return null;
                var count = data._s.Length / 0x18;
                List<GX_Attribute> attributes = new List<GX_Attribute>();
                for (int i = 0; i < count; i++)
                {
                    attributes.Add(new GX_Attribute());
                    attributes[i]._s = data._s.GetEmbeddedStruct(i * 0x18, 0x18);
                    if (attributes[i].AttributeName == GXAttribName.GX_VA_NULL)
                        break;
                }
                return attributes.ToArray();
            }
            set
            {
                if (value.Length == 0)
                {
                    _s.SetReference(0x08, null);
                    return;
                }

                var re = _s.GetReference<HSDAccessor>(0x08);
                if (re == null)
                {
                    _s.SetReferenceStruct(0x08, new HSDStruct());
                    re = _s.GetReference<HSDAccessor>(0x08);
                }

                re._s.Resize(0x18 * value.Length);
                re._s.References.Clear();

                int off = 0;
                foreach (var v in value)
                {
                    re._s.SetEmbededStruct(off, v._s);
                    off += v.TrimmedSize;
                }
            }
        }

        public List<short[]> VertexIndices
        {
            get
            {
                var t = _s.GetReference<HSDAccessor>(0x0C);
                if (t == null)
                    return null;
                List<short[]> tables = new List<short[]>();

                foreach(var r in t._s.References)
                {
                    var a = r.Value;
                    if (a == null)
                        return null;
                    short[] data = new short[VertexIndexCount];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = a.GetInt16(i * 2);
                    tables.Add(data);
                }
                return tables;
            }
            set
            {
                if (value == null)
                {
                    _s.SetReference(0x0C, null);
                    VertexIndexCount = 0;
                }
                else
                {
                    var t = _s.GetCreateReference<HSDAccessor>(0x0C);
                    t._s.Resize(4 * value.Count);

                    for(int j = 0; j < value.Count; j++)
                    {
                        var a = new HSDAccessor();
                        var v = value[j];

                        a._s.Resize(2 * v.Length);
                        VertexIndexCount = v.Length;

                        for (int i = 0; i < v.Length; i++)
                            a._s.SetInt16(i * 2, v[i]);

                        t._s.SetReference(j * 4, a);
                    }

                }
            }
        }

        public int NormalIndexCount { get => _s.GetInt32(0x10); internal set => _s.SetInt32(0x10, value); }
        
        public GX_Attribute[] NormalAttributes
        {
            get
            {
                var data = _s.GetReference<HSDAccessor>(0x14);
                if (data == null)
                    return null;
                var count = data._s.Length / 0x18;
                List<GX_Attribute> attributes = new List<GX_Attribute>();
                for (int i = 0; i < count; i++)
                {
                    attributes.Add(new GX_Attribute());
                    attributes[i]._s = data._s.GetEmbeddedStruct(i * 0x18, 0x18);
                    if (attributes[i].AttributeName == GXAttribName.GX_VA_NULL)
                        break;
                }
                return attributes.ToArray();
            }
            set
            {
                if (value.Length == 0)
                {
                    _s.SetReference(0x08, null);
                    return;
                }

                var re = _s.GetReference<HSDAccessor>(0x14);
                if (re == null)
                {
                    _s.SetReferenceStruct(0x14, new HSDStruct());
                    re = _s.GetReference<HSDAccessor>(0x14);
                }

                re._s.Resize(0x18 * value.Length);
                re._s.References.Clear();

                int off = 0;
                foreach (var v in value)
                {
                    re._s.SetEmbededStruct(off, v._s);
                    off += v.TrimmedSize;
                }
            }
        }

        public List<short[]> NormalIndicies
        {
            get
            {
                var t = _s.GetReference<HSDAccessor>(0x18);
                if (t == null)
                    return null;
                List<short[]> tables = new List<short[]>();

                foreach (var r in t._s.References)
                {
                    var a = r.Value;
                    if (a == null)
                        return null;
                    short[] data = new short[NormalIndexCount];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = a.GetInt16(i * 2);
                    tables.Add(data);
                }
                return tables;
            }
            set
            {
                if (value == null)
                {
                    _s.SetReference(0x18, null);
                    VertexIndexCount = 0;
                }
                else
                {
                    var t = _s.GetCreateReference<HSDAccessor>(0x18);
                    t._s.Resize(4 * value.Count);

                    for (int j = 0; j < value.Count; j++)
                    {
                        var a = new HSDAccessor();
                        var v = value[j];

                        a._s.Resize(2 * v.Length);
                        NormalIndexCount = v.Length;

                        for (int i = 0; i < v.Length; i++)
                            a._s.SetInt16(i * 2, v[i]);

                        t._s.SetReference(j * 4, a);
                    }

                }
            }
        }
    }
}
