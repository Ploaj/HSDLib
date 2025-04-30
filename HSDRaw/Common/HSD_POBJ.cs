using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw.Common
{
    [Flags]
    public enum POBJ_FLAG
    {
        SHAPESET_AVERAGE = (1 << 0),
        SHAPESET_ADDITIVE = (1 << 1),
        UNKNOWN2 = (1 << 2),
        ANIM = (1 << 3),
        SHAPEANIM = (1 << 12),
        ENVELOPE = (1 << 13),
        CULLBACK = (1 << 14),
        CULLFRONT = (1 << 15)
    }
    
    /// <summary>
    /// Polygon Objects contain display list rendering information
    /// Using ToDisplayList<see cref="ToDisplayList"/> for easier processing
    /// </summary>
    public class HSD_POBJ : HSDListAccessor<HSD_POBJ>
    {
        public override int TrimmedSize { get; } = 0x18;

        public override HSD_POBJ Next { get => _s.GetReference<HSD_POBJ>(0x04); set => _s.SetReference(0x04, value); }

        public HSDAccessor Attributes { get => _s.GetReference<HSDAccessor>(0x08); set => _s.SetReference(0x08, value); }

        public POBJ_FLAG Flags { get => (POBJ_FLAG)_s.GetUInt16(0x0C); set => _s.SetUInt16(0x0C, (ushort)value); }

        public int DisplayListSize { get => _s.GetInt16(0x0E) * 32; internal set => _s.SetInt16(0x0E, (short)(value / 32)); }
        
        public byte[] DisplayListBuffer
        {
            get => _s.GetBuffer(0x10);
            internal set
            {
                _s.SetBuffer(0x10, value);
                DisplayListSize = value.Length;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public HSD_ShapeSet ShapeSet
        {
            get
            {
                if (Flags.HasFlag(POBJ_FLAG.SHAPEANIM))
                {
                    return _s.GetReference<HSD_ShapeSet>(0x14);
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    Flags = Flags & ~POBJ_FLAG.ENVELOPE;
                    Flags |= POBJ_FLAG.SHAPEANIM;
                }
                else
                {
                    Flags &= ~POBJ_FLAG.SHAPEANIM;
                }
                _s.SetReference(0x14, value);
            }
        }

        /// <summary>
        /// Transforms model by single bound jobj
        /// </summary>
        public HSD_JOBJ SingleBoundJOBJ
        {
            get
            {
                if (!Flags.HasFlag(POBJ_FLAG.ENVELOPE) && !Flags.HasFlag(POBJ_FLAG.SHAPEANIM) &&  !Flags.HasFlag(POBJ_FLAG.UNKNOWN2))
                {
                    return _s.GetReference<HSD_JOBJ>(0x14);
                }
                return null;
            }
            set
            {
                if(value != null)
                {
                    Flags = Flags & ~POBJ_FLAG.ENVELOPE;
                    Flags = Flags & ~POBJ_FLAG.SHAPEANIM;
                }

                _s.SetReference(0x14, value);
            }
        }

        /// <summary>
        /// Contains the Envelope Weights for this polygon
        /// </summary>
        public HSD_Envelope[] EnvelopeWeights
        {
            get
            {
                if (Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                {
                    return _s.GetReference<HSDNullPointerArrayAccessor<HSD_Envelope>>(0x14).Array;
                }
                return null;
            }
            set
            {
                Flags = Flags & ~POBJ_FLAG.ENVELOPE;
                if (value != null && value.Length > 0)
                {
                    Flags = Flags | POBJ_FLAG.ENVELOPE;
                    _s.GetCreateReference<HSDNullPointerArrayAccessor<HSD_Envelope>>(0x14).Array = value;
                }
                else
                    _s.SetReference(0x14, null);
            }
        }

        public string AttributesTypes
        {
            get
            {
                return string.Join(", ", ToGXAttributes().Select(e=>e.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasAttribute(GXAttribName name)
        {
            var attr = ToGXAttributes();
            foreach (var v in attr)
                if (v.AttributeName == name)
                    return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GX_Attribute[] ToGXAttributes()
        {
            var frankenStruct = new HSDStruct();

            frankenStruct.AppendStruct(_s.GetReference<HSDAccessor>(0x08)._s);

            if (ShapeSet != null)
            {
                if (ShapeSet._s.GetReference<HSDAccessor>(0x08)._s != _s.GetReference<HSDAccessor>(0x08)._s)
                    frankenStruct.AppendStruct(ShapeSet._s.GetReference<HSDAccessor>(0x08)._s);

                if (ShapeSet._s.GetReference<HSDAccessor>(0x14) != null)
                    frankenStruct.AppendStruct(ShapeSet._s.GetReference<HSDAccessor>(0x14)._s);
            }

            var count = frankenStruct.Length / 0x18;
            List<GX_Attribute> attributes = new List<GX_Attribute>();
            for (int i = 0; i < count; i++)
            {
                attributes.Add(new GX_Attribute());
                attributes[i]._s = frankenStruct.GetEmbeddedStruct(i * 0x18, 0x18);
                if (attributes[i].AttributeName == GXAttribName.GX_VA_NULL)
                    break;
            }
            return attributes.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void FromAttributes(GX_Attribute[] value)
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

        /// <summary>
        /// Wraps the data in the POBJ into a <see cref="GX_DisplayList"/>
        /// </summary>
        /// <returns></returns>
        public GX_DisplayList ToDisplayList(GX_Attribute[] attrs = null)
        {
            return new GX_DisplayList(this, attrs);
        }

        /// <summary>
        /// Transfers data from <see cref="GX_DisplayList"/> into POBJ structure
        /// </summary>
        /// <param name="dl"></param>
        public void FromDisplayList(GX_DisplayList dl)
        {
            FromAttributes(dl.Attributes.ToArray());

            DisplayListBuffer = dl.ToBuffer();

            if (dl.Attributes.Find(e => e.AttributeName == GXAttribName.GX_VA_PNMTXIDX) != null)
                EnvelopeWeights = dl.Envelopes.ToArray();
            else
                EnvelopeWeights = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int Trim()
        {
            var trimmed = base.Trim();

            var attr = _s.GetReference<HSDAccessor>(0x08);
            // align attribute buffers
            if (attr != null)
                foreach (var att in attr._s.References)
                    att.Value.IsBufferAligned = true;

            // align display buffer
            if (_s.GetReference<HSDAccessor>(0x10) != null)
                _s.GetReference<HSDAccessor>(0x10)._s.IsBufferAligned = true;

            // align envelope buffer
            if (Flags.HasFlag(POBJ_FLAG.ENVELOPE))
            {
                var evn = _s.GetReference<HSDAccessor>(0x14);
                if(evn != null)
                {
                    evn._s.IsBufferAligned = true;
                    evn._s.CanBeDuplicate = false;
                }

            }

            // guarentee attribute sequence
            // yes this is the messiest hack
            if (ShapeSet != null)
            {
                HSDStruct attrS = null;
                HSDStruct attrPos = null;
                HSDStruct attrNrm = null; 

                if (_s.References.ContainsKey(0x08))
                    attrS = _s.References[0x08];

                if (ShapeSet._s.References.ContainsKey(0x08))
                    attrPos = ShapeSet._s.References[0x08];

                if (ShapeSet._s.References.ContainsKey(0x14))
                    attrNrm = ShapeSet._s.References[0x14];

                if (attrS != attrPos)
                {
                    if (attrS != null)
                        attrS._nextStruct = attrPos;

                    if (attrPos != null)
                        attrPos._nextStruct = attrNrm;

                    if(attrNrm != null)
                        attrNrm._nextStruct = null;
                }
                else
                {
                    if (attrS != null)
                        attrS._nextStruct = attrNrm;

                    if (attrNrm != null)
                        attrNrm._nextStruct = null;
                }
            }

            return trimmed;
        }

    }
}
