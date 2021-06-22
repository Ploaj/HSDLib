namespace HSDRaw.Melee.Pl
{
    public class SBM_FighterActionTable : HSDAccessor
    {
        public int Count
        {
            get
            {
                return _s.Length / 0x18;
            }
        }

        public SBM_FighterAction[] Commands
        {
            get
            {
                SBM_FighterAction[] s = new SBM_FighterAction[Count];
                for(int i = 0; i < s.Length; i++)
                {
                    s[i] = new SBM_FighterAction();
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
