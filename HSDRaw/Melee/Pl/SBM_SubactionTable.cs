namespace HSDRaw.Melee.Pl
{
    public class SBM_SubActionTable : HSDAccessor
    {
        public int Count
        {
            get
            {
                return _s.Length / 0x18;
            }
        }

        public SBM_FighterSubAction[] Subactions
        {
            get
            {
                SBM_FighterSubAction[] s = new SBM_FighterSubAction[Count];
                for(int i = 0; i < s.Length; i++)
                {
                    s[i] = new SBM_FighterSubAction();
                    s[i]._s = _s.GetEmbeddedStruct(i * 0x18, 0x18);
                }
                return s;
            }
            set
            {
                _s.References.Clear();
                _s.Resize(value.Length * 0x18);
                for(int i = 0; i < value.Length; i++)
                {
                    _s.SetEmbededStruct(i * 0x18, value[i]._s);
                }
            }
        }
    }
}
