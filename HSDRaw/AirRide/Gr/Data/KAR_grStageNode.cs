namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grStageNode : HSDAccessor
    {
        public override int TrimmedSize => 0xE8;

        public int Unk1 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }
        public float MachineAccel { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }
        public float StageScale { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }
        public float UnkGravity { get => _s.GetFloat(0xc); set => _s.SetFloat(0xc, value); }
        public float GravityX { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float GravityY { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float GravityZ { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public int FogFlags { get => _s.GetInt32(0x1c); set => _s.SetInt32(0x1c, value); }
        public float UnkItemRestituion { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public float UnkF1 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }
        public float UnkF2 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float UnkF3 { get => _s.GetFloat(0x2c); set => _s.SetFloat(0x2c, value); }
        public float UnkF4 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }
        public float UnkF5 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }
        public float UnkF6 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }
        public float UnkF7 { get => _s.GetFloat(0x3c); set => _s.SetFloat(0x3c, value); }
        public float CoRWall { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }
        public float CoRBreakableObjects { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }
        public float CoRMovingDisks { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }
        public float CoRUnk1 { get => _s.GetFloat(0x4c); set => _s.SetFloat(0x4c, value); }
        public float CoRUnk2 { get => _s.GetFloat(0x50); set => _s.SetFloat(0x50, value); }
        public float CoRUnk3 { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }
        public float CoRUnk4 { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }
        public float CoRUnk5 { get => _s.GetFloat(0x5c); set => _s.SetFloat(0x5c, value); }
        public float MinimapScale { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }
        public float MinimapPlayerX { get => _s.GetFloat(0x64); set => _s.SetFloat(0x64, value); }
        public float MinimapPlayerY { get => _s.GetFloat(0x68); set => _s.SetFloat(0x68, value); }
        public float MinimapPlayerZ { get => _s.GetFloat(0x6c); set => _s.SetFloat(0x6c, value); }
        public int Unused1 { get => _s.GetInt32(0x70); set => _s.SetInt32(0x70, value); }
        public int Unused2 { get => _s.GetInt32(0x74); set => _s.SetInt32(0x74, value); }
        public KAR_StageNodeFloats UnkFloats1 { get => _s.GetReference<KAR_StageNodeFloats>(0x78); set => _s.SetReference(0x78, value); }
        public KAR_StageNodeFloats UnkFloats2 { get => _s.GetReference<KAR_StageNodeFloats>(0x7c); set => _s.SetReference(0x7c, value); }
        public int Flags { get => _s.GetInt32(0x80); set => _s.SetInt32(0x80, value); }
        public float UnusedAccelerationBoostPadH { get => _s.GetFloat(0x84); set => _s.SetFloat(0x84, value); }
        public float UnusedAccelerationBoostPadH2 { get => _s.GetFloat(0x88); set => _s.SetFloat(0x88, value); }
        public float UnusedAccelerationTimeL { get => _s.GetFloat(0x8c); set => _s.SetFloat(0x8c, value); }
        public float AccelerationBoostPadL { get => _s.GetFloat(0x90); set => _s.SetFloat(0x90, value); }
        public float AccelerationBoostPadL2 { get => _s.GetFloat(0x94); set => _s.SetFloat(0x94, value); }
        public float AccelerationTimeL { get => _s.GetFloat(0x98); set => _s.SetFloat(0x98, value); }
        public float AccelerationBoostGateH { get => _s.GetFloat(0x9c); set => _s.SetFloat(0x9c, value); }
        public float AccelerationBoostGateH2 { get => _s.GetFloat(0xa0); set => _s.SetFloat(0xa0, value); }
        public float AccelerationTimeBoostGateH { get => _s.GetFloat(0xa4); set => _s.SetFloat(0xa4, value); }
        public float AccelerationBoostGateL { get => _s.GetFloat(0xa8); set => _s.SetFloat(0xa8, value); }
        public float AccelerationBoostGateL2 { get => _s.GetFloat(0xac); set => _s.SetFloat(0xac, value); }
        public float AccelerationTimeBoostGateL { get => _s.GetFloat(0xb0); set => _s.SetFloat(0xb0, value); }
        public float AccelerationBoostRing { get => _s.GetFloat(0xb4); set => _s.SetFloat(0xb4, value); }
        public float AccelerationBoostRing2 { get => _s.GetFloat(0xb8); set => _s.SetFloat(0xb8, value); }
        public float AccelerationTimeBoostRing { get => _s.GetFloat(0xbc); set => _s.SetFloat(0xbc, value); }
        public float UnkUnused1 { get => _s.GetFloat(0xc0); set => _s.SetFloat(0xc0, value); }
        public float UnkUnused2 { get => _s.GetFloat(0xc4); set => _s.SetFloat(0xc4, value); }
        public float UnkUnused3 { get => _s.GetFloat(0xc8); set => _s.SetFloat(0xc8, value); }
        public float OoBMinXArea { get => _s.GetFloat(0xcc); set => _s.SetFloat(0xcc, value); }
        public float OoBMinYArea { get => _s.GetFloat(0xd0); set => _s.SetFloat(0xd0, value); }
        public float OoBMinZArea { get => _s.GetFloat(0xd4); set => _s.SetFloat(0xd4, value); }
        public float OoBMaxXArea { get => _s.GetFloat(0xd8); set => _s.SetFloat(0xd8, value); }
        public float OoBMaxYArea { get => _s.GetFloat(0xdc); set => _s.SetFloat(0xdc, value); }
        public float OoBMaxZArea { get => _s.GetFloat(0xe0); set => _s.SetFloat(0xe0, value); }
        public KAR_StagePadCountPointer PointerToBoostPad { get => _s.GetReference<KAR_StagePadCountPointer>(0xe4); set => _s.SetReference(0xe4, value); }
    }

    public class KAR_StagePadCountPointer : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public KAR_StagePadCount PadCount { get => _s.GetReference<KAR_StagePadCount>(0x0); set => _s.SetReference(0x0, value); }
    }

    public class KAR_StagePadCount : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Index0 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public int Index1 { get => _s.GetInt32(0x4); set => _s.SetInt32(0x4, value); }
    }

    public class KAR_StageNodeFloats : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public float Float1 { get => _s.GetFloat(0x0); set => _s.SetFloat(0x0, value); }

        public float Float2 { get => _s.GetFloat(0x4); set => _s.SetFloat(0x4, value); }

        public float Float3 { get => _s.GetFloat(0x8); set => _s.SetFloat(0x8, value); }

        public float Float4 { get => _s.GetFloat(0xc); set => _s.SetFloat(0xc, value); }

        public float Float5 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float Float6 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
    }
}
