using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_ItemLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        public HSD_UShort[] Entries
        {
            get
            {
                var ar = _s.GetReference<HSDArrayAccessor<HSD_UShort>>(0x04);

                if (ar == null)
                    return new HSD_UShort[0];

                var arr = ar.Array;
                System.Array.Resize(ref arr, Count);
                return arr;
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    Count = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    Count = value.Length;
                    _s.GetCreateReference<HSDArrayAccessor<HSD_UShort>>(0x04).Array = value;
                }
            }
        }
    }
}
