namespace HSDRaw.Melee.Pl
{
    public class SBM_CommonFighterAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0x184;

        public float InitialWalkSpeed { get => _s.GetFloat(0x000); set => _s.SetFloat(0x000, value); }

        public float WalkAcceleration { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float MaxWalkSpeed { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float WalkAnimationSpeed { get => _s.GetFloat(0x00C); set => _s.SetFloat(0x0C, value); }

        public float Unknown0x10 { get => _s.GetFloat(0x010); set => _s.SetFloat(0x10, value); }

        public float WalkAnimationSpeed2 { get => _s.GetFloat(0x014); set => _s.SetFloat(0x14, value); }

        public float Friction { get => _s.GetFloat(0x018); set => _s.SetFloat(0x18, value); }

        public float InitialDashSpeed { get => _s.GetFloat(0x01C); set => _s.SetFloat(0x1C, value); }

        public float StopTurnInitialSpeed { get => _s.GetFloat(0x020); set => _s.SetFloat(0x20, value); }

        public float Unknown0x24 { get => _s.GetFloat(0x0024); set => _s.SetFloat(0x24, value); }

        public float InitialRunSpeed { get => _s.GetFloat(0x028); set => _s.SetFloat(0x28, value); }

        public float RunSpeed { get => _s.GetFloat(0x02C); set => _s.SetFloat(0x2C, value); }

        public float RunAcceleration { get => _s.GetFloat(0x030); set => _s.SetFloat(0x30, value); }

        public float Unknown0x34 { get => _s.GetFloat(0x034); set => _s.SetFloat(0x34, value); }

        public float JumpStartUpLag { get => _s.GetFloat(0x038); set => _s.SetFloat(0x38, value); }

        public float Unknown0x3C { get => _s.GetFloat(0x03C); set => _s.SetFloat(0x3C, value); }

        public float InitialJumpVerticalSpeed { get => _s.GetFloat(0x040); set => _s.SetFloat(0x40, value); }

        public float Unknown0x44 { get => _s.GetFloat(0x044); set => _s.SetFloat(0x44, value); }

        public float InitialJumpHorizontalSpeed { get => _s.GetFloat(0x048); set => _s.SetFloat(0x48, value); }

        public float InitialShortHopVerticalSpeed { get => _s.GetFloat(0x04C); set => _s.SetFloat(0x4C, value); } // ?

        public float AirJumpMultiplier { get => _s.GetFloat(0x050); set => _s.SetFloat(0x50, value); }

        public float Unkown0x54 { get => _s.GetFloat(0x054); set => _s.SetFloat(0x54, value); }

        public int NumberOfJumps { get => _s.GetInt32(0x058); set => _s.SetInt32(0x58, value); }

        public float Gravity { get => _s.GetFloat(0x05C); set => _s.SetFloat(0x5C, value); }

        public float TerminalVelocity { get => _s.GetFloat(0x060); set => _s.SetFloat(0x60, value); }

        public float AerialSpeed { get => _s.GetFloat(0x064); set => _s.SetFloat(0x64, value); }

        public float AerialFriction { get => _s.GetFloat(0x068); set => _s.SetFloat(0x68, value); }

        public float MaxAerialHorizontalSpeed { get => _s.GetFloat(0x06C); set => _s.SetFloat(0x6C, value); }

        public float Unknown0x70 { get => _s.GetFloat(0x070); set => _s.SetFloat(0x70, value); }

        public float FastFallTerminalVelocity { get => _s.GetFloat(0x074); set => _s.SetFloat(0x74, value); }

        public float TiltTurnForcedVelocity { get => _s.GetFloat(0x078); set => _s.SetFloat(0x78, value); } // TODO: what does this mean?

        public float Unknown0x7C { get => _s.GetFloat(0x07C); set => _s.SetFloat(0x7C, value); }

        public float Unknown0x80 { get => _s.GetFloat(0x080); set => _s.SetFloat(0x80, value); }

        public float Unknown0x84 { get => _s.GetFloat(0x084); set => _s.SetFloat(0x84, value); }

        public float Weight { get => _s.GetFloat(0x088); set => _s.SetFloat(0x88, value); }

        public float ModelScale { get => _s.GetFloat(0x08C); set => _s.SetFloat(0x8C, value); }

        public float ShieldSize { get => _s.GetFloat(0x090); set => _s.SetFloat(0x90, value); }

        public float ShieldBreakInitialVelocity { get => _s.GetFloat(0x094); set => _s.SetFloat(0x94, value); }

        public float Unknown0x98 { get => _s.GetFloat(0x098); set => _s.SetFloat(0x98, value); }

        public float Unknown0x9C { get => _s.GetFloat(0x09C); set => _s.SetFloat(0x9C, value); }

        public float Unknown0xA0 { get => _s.GetFloat(0x0A0); set => _s.SetFloat(0xA0, value); }

        public int Unknown0xA4 { get => _s.GetInt32(0x0A4); set => _s.SetInt32(0xA4, value); }

        public float LedgeJumpHorizontalVelocity { get => _s.GetFloat(0x0A8); set => _s.SetFloat(0xA8, value); }

        public float LedgeJumpVerticalVelocity { get => _s.GetFloat(0x0AC); set => _s.SetFloat(0xAC, value); }

        public float Unknown0xB0 { get => _s.GetFloat(0x0B0); set => _s.SetFloat(0xB0, value); }

        public float Unknown0xB4 { get => _s.GetFloat(0x0B4); set => _s.SetFloat(0xB4, value); }

        public float Unknown0xB8 { get => _s.GetFloat(0x0B8); set => _s.SetFloat(0xB8, value); }

        public float Unknown0xBC { get => _s.GetFloat(0x0BC); set => _s.SetFloat(0xBC, value); }

        public float Unknown0xC0 { get => _s.GetFloat(0x0C0); set => _s.SetFloat(0xC0, value); }

        public float Unknown0xC4 { get => _s.GetFloat(0x0C4); set => _s.SetFloat(0xC4, value); }

        public float Unknown0xC8 { get => _s.GetFloat(0x0C8); set => _s.SetFloat(0xC8, value); }

        public float Unknown0xCC { get => _s.GetFloat(0x0CC); set => _s.SetFloat(0xCC, value); }

        public float Unknown0xD0 { get => _s.GetFloat(0x0D0); set => _s.SetFloat(0xD0, value); }

        public float Unknown0xD4 { get => _s.GetFloat(0x0d4); set => _s.SetFloat(0xD4, value); }

        public float Unknown0xD8 { get => _s.GetFloat(0x0d8); set => _s.SetFloat(0xD8, value); }

        public float NormalLandingLag { get => _s.GetFloat(0xdc); set => _s.SetFloat(0xDC, value); }

        public float NairLandingLag { get => _s.GetFloat(0x0e0); set => _s.SetFloat(0xE0, value); }

        public float Unknown0xE4 { get => _s.GetFloat(0x0e4); set => _s.SetFloat(0xE4, value); }

        public float Unknown0xE8 { get => _s.GetFloat(0x0e8); set => _s.SetFloat(0xE8, value); }

        public float FairLandingLag { get => _s.GetFloat(0x0ec); set => _s.SetFloat(0xEC, value); }

        public float BairLandingLag { get => _s.GetFloat(0x0f0); set => _s.SetFloat(0xF0, value); }

        public float DairLandingLag { get => _s.GetFloat(0x0f4); set => _s.SetFloat(0xF4, value); }

        public float UairLandingLag { get => _s.GetFloat(0x0f8); set => _s.SetFloat(0xF8, value); }

        public float Unknown0xFC { get => _s.GetFloat(0x0fc); set => _s.SetFloat(0xFC, value); }

        public float Unknown0x100 { get => _s.GetFloat(0x100); set => _s.SetFloat(0x100, value); }

        public float Unknown0x104 { get => _s.GetFloat(0x104); set => _s.SetFloat(0x104, value); }

        public float Unknown0x108 { get => _s.GetFloat(0x108); set => _s.SetFloat(0x108, value); }

        public float Unknown0x10C { get => _s.GetFloat(0x10C); set => _s.SetFloat(0x10C, value); }

        public float Unknown0x110 { get => _s.GetFloat(0x110); set => _s.SetFloat(0x110, value); }

        public float Unknown0x114 { get => _s.GetFloat(0x114); set => _s.SetFloat(0x114, value); }

        public float Unknown0x118 { get => _s.GetFloat(0x118); set => _s.SetFloat(0x118, value); }

        public float Unknown0x11C { get => _s.GetFloat(0x11c); set => _s.SetFloat(0x11C, value); }

        public float Unknown0x120 { get => _s.GetFloat(0x120); set => _s.SetFloat(0x120, value); }

        public float Unknown0x124 { get => _s.GetFloat(0x124); set => _s.SetFloat(0x124, value); }

        public float Unknown0x128 { get => _s.GetFloat(0x128); set => _s.SetFloat(0x128, value); }

        public float Unknown0x12C { get => _s.GetFloat(0x12C); set => _s.SetFloat(0x12C, value); }

        public float Unknown0x130 { get => _s.GetFloat(0x130); set => _s.SetFloat(0x130, value); }

        public float Unknown0x134 { get => _s.GetFloat(0x134); set => _s.SetFloat(0x134, value); }

        public float Unknown0x138 { get => _s.GetFloat(0x138); set => _s.SetFloat(0x138, value); }

        public float Unknown0x13C { get => _s.GetFloat(0x13C); set => _s.SetFloat(0x13C, value); }

        public float Unknown0x140 { get => _s.GetFloat(0x140); set => _s.SetFloat(0x140, value); }

        public float Unknown0x144 { get => _s.GetFloat(0x144); set => _s.SetFloat(0x144, value); }

        public float Unknown0x148 { get => _s.GetFloat(0x148); set => _s.SetFloat(0x148, value); }

        public float Unknown0x14C { get => _s.GetFloat(0x14C); set => _s.SetFloat(0x14C, value); }

        public float Unknown0x150 { get => _s.GetFloat(0x150); set => _s.SetFloat(0x150, value); }

        public float Unknown0x154 { get => _s.GetFloat(0x154); set => _s.SetFloat(0x154, value); }

        public float Unknown0x158 { get => _s.GetFloat(0x158); set => _s.SetFloat(0x158, value); }

        public float Unknown0x15C { get => _s.GetFloat(0x15C); set => _s.SetFloat(0x15c, value); }

        public float Unknown0x160 { get => _s.GetFloat(0x160); set => _s.SetFloat(0x160, value); }

        public float Unknown0x164 { get => _s.GetFloat(0x164); set => _s.SetFloat(0x164, value); }

        public float Unknown0x168 { get => _s.GetFloat(0x168); set => _s.SetFloat(0x168, value); }

        public float Unknown0x16C { get => _s.GetFloat(0x16C); set => _s.SetFloat(0x16c, value); }

        public float Unknown0x170 { get => _s.GetFloat(0x170); set => _s.SetFloat(0x170, value); }

        public float Unknown0x174 { get => _s.GetFloat(0x174); set => _s.SetFloat(0x174, value); }

        public float Unknown0x178 { get => _s.GetFloat(0x178); set => _s.SetFloat(0x178, value); }

        public float Unknown0x17C { get => _s.GetFloat(0x17C); set => _s.SetFloat(0x17C, value); }

        public float Unknown0x180 { get => _s.GetFloat(0x180); set => _s.SetFloat(0x180, value); }
    }
}
