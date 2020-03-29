namespace HSDRaw.Melee.Pl
{
    public class SBM_FighterBoneIDs : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int HeadBone { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int RightArm { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int LeftLeg { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int RightLeg { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int LeftArm { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
    }
}
