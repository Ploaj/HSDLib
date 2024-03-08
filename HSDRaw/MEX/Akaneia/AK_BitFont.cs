using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.MEX.Akaneia
{
    public class AK_BitFont : HSDNullPointerArrayAccessor<AK_BitFontChar>
    {

    }

    public class AK_BitFontChar : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public char Character { get => (char)_s.GetByte(0x00); set => _s.SetByte(0x00, (byte)value); }

        public byte Kerning { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte Spacing { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte[] Data { get => _s.GetBuffer(0x04); set => _s.SetBuffer(0x04, value); }
    }
}
