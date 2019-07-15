using HSDRaw.GX;

namespace HSDRaw.Common
{
    /// <summary>
    /// Image Object
    /// </summary>
    public class HSD_IOBJ : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public short Width { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Height { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public GXTexFmt Format { get => (GXTexFmt)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public byte[] Data { get => _s.GetBuffer(0x08); set => _s.SetBuffer(0x08, value); }
    }
}
