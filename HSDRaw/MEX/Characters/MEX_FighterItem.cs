using HSDRaw.Common;

namespace HSDRaw.MEX.Characters
{
    public class MEX_FighterItem : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public HSD_UShort[] MEXItems
        {
            get
            {
                var arr = _s.GetCreateReference<HSDArrayAccessor<HSD_UShort>>(0x04).Array;
                System.Array.Resize(ref arr, Count);
                return arr;
            }
            set
            {
                Count = value.Length;
                _s.GetCreateReference<HSDArrayAccessor<HSD_UShort>>(0x04).Array = value;
            }
        }
    }
}
