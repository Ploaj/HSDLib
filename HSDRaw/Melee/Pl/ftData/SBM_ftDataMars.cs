using System.ComponentModel;

namespace HSDRaw.Melee.Pl.ftData
{
    public class SBM_AttrMars : HSDAccessor
    {
        public override int TrimmedSize => 0x98; // total size of structure

        public int SpecialN_MaxLoopsUntilCharged { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int SpecialN_BaseDamage { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int SpecialN_DamageAddedPerLoop { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        //public float SpecialSGravity { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SBM_AfterImageDesc AfterImage
        {
            get => new SBM_AfterImageDesc() { _s = _s.GetEmbeddedStruct(0x78, 0x20) };
            set => _s.SetEmbededStruct(0x78, value._s);
        }
    }

    public class SBM_ftDataMars : SBM_FighterData
    {
        public SBM_AttrMars UniqueAttributes { get => _s.GetReference<SBM_AttrMars>(0x04); set => _s.SetReference(0x04, value); }
    }
}
