namespace HSDRaw.Melee.Pl.ftData
{
    public class SBM_AttrMario : HSDAccessor
    {
        public override int TrimmedSize => 0x84; // total size of structure

        public float SpecialSHorizontalMomentum { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float SpecialSHorizontalVelocity { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float SpecialSVerticalMomentum { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float SpecialSGravity { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float SpecialSGravityLimit { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public int CapeItemKind { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public float SpecialHiFallAirMobility { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float SpecialHiLandingLag { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public float SpecialHiStickReverseThreshold { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float SpecialHiStickThreshold { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float SpecialHiStickControl { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float SpecialHiInitialXMomemtum { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }

        public float SpecialHiInitialGravity { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        public float SpecialHiInitialYMomemtum { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        public float SpecialLwGroundedRiseResistance { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }

        public float SpecialLwBaseAirSpeed { get => _s.GetFloat(0x3C); set => _s.SetFloat(0x3C, value); }

        public float SpecialLwXVelocityClamp { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float SpecialLwXAccel { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float SpecialLwXDrift { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }

        public float SpeicalLwAirSpeedDecel { get => _s.GetFloat(0x4C); set => _s.SetFloat(0x4C, value); }

        public int SpecialLwStateVar2 { get => _s.GetInt32(0x50); set => _s.SetInt32(0x50, value); }

        public float SpecialLwRisingTapPower { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }

        public float SpecialLwTerminalVelocity { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }

        public int SpecialLwFreefallToggle { get => _s.GetInt32(0x5C); set => _s.SetInt32(0x5C, value); }

        //ReflectDesc reflect_data;                 //0x60
    }

    public class SBM_ftDataMario : SBM_FighterData
    {
        public SBM_AttrMario UniqueAttributes { get => _s.GetReference<SBM_AttrMario>(0x04); set => _s.SetReference(0x04, value); }
    }
}
