using HSDRaw.GX;
using System;
using System.Collections.Generic;

namespace HSDRaw.Common
{
    [Flags]
    public enum POBJ_FLAG
    {
        UNKNOWN0 = (1 << 0),
        UNKNOWN1 = (1 << 1),
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
        
        /// <summary>
        /// List of GX Attributes
        /// Must end with NULL<see cref="GXAttribName"/> attribute name
        /// </summary>
        public GX_Attribute[] Attributes {
            get
            {
                var data = _s.GetReference<HSDAccessor>(0x08);
                var count = data._s.Length / 0x18;
                List<GX_Attribute> attributes = new List<GX_Attribute>();
                for(int i = 0; i < count; i++)
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
                if(re == null)
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
                if (!Flags.HasFlag(POBJ_FLAG.ENVELOPE) && !Flags.HasFlag(POBJ_FLAG.SHAPEANIM))
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
        
        /// <summary>
        /// Converts the pobj into an easier to read format <see cref="GX_DisplayList"/>
        /// Note: this operation is slow
        /// </summary>
        private GX_DisplayList DisplayList
        {
            get
            {
                return new GX_DisplayList(this);
            }
            set
            {
                Attributes = value.Attributes.ToArray();
                DisplayListBuffer = value.ToBuffer();
                if (value.Attributes.Find(e => e.AttributeName == GXAttribName.GX_VA_PNMTXIDX) != null)
                    EnvelopeWeights = value.Envelopes.ToArray();
                else
                    EnvelopeWeights = null;
            }
        }

        /// <summary>
        /// Wraps the data in the PBOJ into a <see cref="GX_DisplayList"/>
        /// </summary>
        /// <returns></returns>
        public GX_DisplayList ToDisplayList()
        {
            return DisplayList;
        }

        /// <summary>
        /// Transfers data from <see cref="GX_DisplayList"/> into POBJ structure
        /// </summary>
        /// <param name="dl"></param>
        public void FromDisplayList(GX_DisplayList dl)
        {
            DisplayList = dl;
        }

    }
}
