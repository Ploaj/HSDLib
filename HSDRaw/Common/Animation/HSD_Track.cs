using HSDRaw.Tools;
using System.Collections.Generic;

namespace HSDRaw.Common.Animation
{
    /// <summary>
    /// Frame object with additional data length field
    /// used in <see cref="HSD_FigaTree"/>
    /// </summary>
    public class HSD_Track : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;
        
        public short DataLength { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public HSD_FOBJ FOBJ
        {
            get
            {
                HSD_FOBJ fobj = new HSD_FOBJ();
                fobj._s = _s.GetEmbeddedStruct(0x04, 0x8);
                return fobj;
            }
            set
            {
                _s.SetEmbededStruct(0x04, value._s);
                if (value == null)
                    DataLength = (short)value._s.Length;
                else
                    DataLength = 0;
            }
        }

        public List<FOBJKey> GetKeys()
        {
            return FOBJ.GetDecodedKeys();
        }
    }
}
