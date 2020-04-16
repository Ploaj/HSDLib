namespace HSDRaw.MEX.Menus
{
    public class MEX_IconData : HSDAccessor
    {
        public MEX_CSSIcon[] Icons
        {
            get
            {
                return _s.GetEmbeddedAccessorArray<MEX_CSSIcon>(0xDC, (_s.Length - 0xDC) / 0x1C);
            }
            set
            {
                _s.SetEmbeddedAccessorArray(0xDC, value);
            }
        }

    }
}
