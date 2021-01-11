using System.Drawing;

namespace HSDRaw.MEX.Misc
{
    public class MEX_Misc : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDArrayAccessor<MEX_GawColor> GawColors { get => _s.GetReference<HSDArrayAccessor<MEX_GawColor>>(0x00); set => _s.SetReference(0x00, value); }
    }

    public class MEX_GawColor : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public Color FillColor { get => _s.GetColorRGBA(0x00); set => _s.SetColorRGBA(0x00, value); }

        public Color OutlineColor { get => _s.GetColorRGBA(0x04); set => _s.SetColorRGBA(0x04, value); }
    }
}
