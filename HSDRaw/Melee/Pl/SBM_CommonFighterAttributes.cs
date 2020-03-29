using System;
using System.ComponentModel;

namespace HSDRaw.Melee.Pl
{
    public class SBM_CommonFighterAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0x184;
        
        public float InitialWalkSpeed { get => _s.GetFloat(0x000); set => _s.SetFloat(0x000, value); }

        public float WalkAcceleration { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float MaxWalkSpeed { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float WalkAnimationSpeed { get => _s.GetFloat(0x00C); set => _s.SetFloat(0x0C, value); }

        public float MidWalkPoint { get => _s.GetFloat(0x010); set => _s.SetFloat(0x10, value); }

        public float FastWalkSpeed { get => _s.GetFloat(0x014); set => _s.SetFloat(0x14, value); }

        public float Friction { get => _s.GetFloat(0x018); set => _s.SetFloat(0x18, value); }

        public float InitialDashSpeed { get => _s.GetFloat(0x01C); set => _s.SetFloat(0x1C, value); }

        public float StopTurnInitialSpeedA { get => _s.GetFloat(0x020); set => _s.SetFloat(0x20, value); }

        public float StopTurnInitialSpeedB { get => _s.GetFloat(0x0024); set => _s.SetFloat(0x24, value); }

        public float InitialRunSpeed { get => _s.GetFloat(0x028); set => _s.SetFloat(0x28, value); }

        public float RunAnimationScale { get => _s.GetFloat(0x02C); set => _s.SetFloat(0x2C, value); }

        public float DashLockoutDirection { get => _s.GetFloat(0x030); set => _s.SetFloat(0x30, value); }

        public float DashDurationBeforeRunning { get => _s.GetFloat(0x034); set => _s.SetFloat(0x34, value); }

        public float JumpStartupLag { get => _s.GetFloat(0x038); set => _s.SetFloat(0x38, value); }

        public float InitialHorizontalJumpVelocity { get => _s.GetFloat(0x03C); set => _s.SetFloat(0x3C, value); }

        public float InitialVerticalJumpVelocity { get => _s.GetFloat(0x040); set => _s.SetFloat(0x40, value); }

        public float GroundToAirJumpMomentumMultiplier { get => _s.GetFloat(0x044); set => _s.SetFloat(0x44, value); }

        public float MaximumShorthopHorizontalVelocity { get => _s.GetFloat(0x048); set => _s.SetFloat(0x48, value); }

        public float MaximumShorthopVerticalVelocity { get => _s.GetFloat(0x04C); set => _s.SetFloat(0x4C, value); } // ?

        public float HorizontalAirJumpMultiplier { get => _s.GetFloat(0x050); set => _s.SetFloat(0x50, value); }

        public float VerticalAirJumpMultiplier { get => _s.GetFloat(0x054); set => _s.SetFloat(0x54, value); }

        public int NumberOfJumps { get => _s.GetInt32(0x058); set => _s.SetInt32(0x58, value); }

        public float Gravity { get => _s.GetFloat(0x05C); set => _s.SetFloat(0x5C, value); }

        public float TerminalVelocity { get => _s.GetFloat(0x060); set => _s.SetFloat(0x60, value); }

        public float AerialSpeed { get => _s.GetFloat(0x064); set => _s.SetFloat(0x64, value); }

        public float AerialFriction { get => _s.GetFloat(0x068); set => _s.SetFloat(0x68, value); }

        public float MaxAerialHorizontalSpeed { get => _s.GetFloat(0x06C); set => _s.SetFloat(0x6C, value); }

        public float AirFriction { get => _s.GetFloat(0x070); set => _s.SetFloat(0x70, value); }

        public float FastFallTerminalVelocity { get => _s.GetFloat(0x074); set => _s.SetFloat(0x74, value); }

        public float TiltTurnForcedVelocity { get => _s.GetFloat(0x078); set => _s.SetFloat(0x78, value); } // TODO: what does this mean?

        public float Jab2Window { get => _s.GetFloat(0x07C); set => _s.SetFloat(0x7C, value); }

        public float Jab3Window { get => _s.GetFloat(0x080); set => _s.SetFloat(0x80, value); }

        public float FramesToChangeDirectionOnStandingTurn { get => _s.GetFloat(0x084); set => _s.SetFloat(0x84, value); }

        public float Weight { get => _s.GetFloat(0x088); set => _s.SetFloat(0x88, value); }

        public float ModelScale { get => _s.GetFloat(0x08C); set => _s.SetFloat(0x8C, value); }

        public float ShieldSize { get => _s.GetFloat(0x090); set => _s.SetFloat(0x90, value); }

        public float ShieldBreakInitialVelocity { get => _s.GetFloat(0x094); set => _s.SetFloat(0x94, value); }

        public int RapidJapWindow { get => _s.GetInt32(0x098); set => _s.SetInt32(0x98, value); }

        public float ClankSpeedMultiplier { get => _s.GetFloat(0x09C); set => _s.SetFloat(0x9C, value); }

        public float HitByItemFlag { get => _s.GetFloat(0x0A0); set => _s.SetFloat(0xA0, value); }

        public int Unknown0xA4 { get => _s.GetInt32(0x0A4); set => _s.SetInt32(0xA4, value); }

        public float LedgeJumpHorizontalVelocity { get => _s.GetFloat(0x0A8); set => _s.SetFloat(0xA8, value); }

        public float LedgeJumpVerticalVelocity { get => _s.GetFloat(0x0AC); set => _s.SetFloat(0xAC, value); }

        public float ItemThrowVelocity { get => _s.GetFloat(0x0B0); set => _s.SetFloat(0xB0, value); }

        public float ItemThrowDamageScale { get => _s.GetFloat(0x0B4); set => _s.SetFloat(0xB4, value); }

        public float RunSideSpecialMomentum { get => _s.GetFloat(0x0B8); set => _s.SetFloat(0xB8, value); }

        public float EggSize { get => _s.GetFloat(0x0BC); set => _s.SetFloat(0xBC, value); }

        public float EggHurtbox { get => _s.GetFloat(0x0C0); set => _s.SetFloat(0xC0, value); }

        public float EggHurtboxX { get => _s.GetFloat(0x0C4); set => _s.SetFloat(0xC4, value); }

        public float EggHurtboxY { get => _s.GetFloat(0x0C8); set => _s.SetFloat(0xC8, value); }

        public float EggHurtboxZ { get => _s.GetFloat(0x0CC); set => _s.SetFloat(0xCC, value); }

        public float Unknown0xD0 { get => _s.GetFloat(0x0D0); set => _s.SetFloat(0xD0, value); }

        public float Unknown0xD4 { get => _s.GetFloat(0x0d4); set => _s.SetFloat(0xD4, value); }

        public float EggHurtboxRadius { get => _s.GetFloat(0x0d8); set => _s.SetFloat(0xD8, value); }

        public float KirbyNeutralSpecialStarSwallowDamage { get => _s.GetFloat(0xdc); set => _s.SetFloat(0xDC, value); }

        public float KirbyNeutralSpecialStarDamage { get => _s.GetFloat(0x0e0); set => _s.SetFloat(0xE0, value); }

        public float NormalLandingLag { get => _s.GetFloat(0x0e4); set => _s.SetFloat(0xE4, value); }

        public float NairLandingLag { get => _s.GetFloat(0x0e8); set => _s.SetFloat(0xE8, value); }

        public float FairLandingLag { get => _s.GetFloat(0x0ec); set => _s.SetFloat(0xEC, value); }

        public float BairLandingLag { get => _s.GetFloat(0x0f0); set => _s.SetFloat(0xF0, value); }

        public float UairLandingLag { get => _s.GetFloat(0x0f4); set => _s.SetFloat(0xF4, value); }

        public float DairLandingLag { get => _s.GetFloat(0x0f8); set => _s.SetFloat(0xF8, value); }

        public float VictoryScreenModelScale { get => _s.GetFloat(0x0fc); set => _s.SetFloat(0xFC, value); }

        public float WallTechX { get => _s.GetFloat(0x100); set => _s.SetFloat(0x100, value); }

        public float WallJumpHorizontalVelocity { get => _s.GetFloat(0x104); set => _s.SetFloat(0x104, value); }

        public float WallJumpVerticalVelocity { get => _s.GetFloat(0x108); set => _s.SetFloat(0x108, value); }

        public float CeilingTechXDirection { get => _s.GetFloat(0x10C); set => _s.SetFloat(0x10C, value); }

        public float Unknown0x110 { get => _s.GetFloat(0x110); set => _s.SetFloat(0x110, value); }

        public float LeftBunnyHoodX { get => _s.GetFloat(0x114); set => _s.SetFloat(0x114, value); }

        public float LeftBunnyHoodY { get => _s.GetFloat(0x118); set => _s.SetFloat(0x118, value); }

        public float LeftBunnyHoodZ { get => _s.GetFloat(0x11c); set => _s.SetFloat(0x11C, value); }

        public float RightBunnyHoodX { get => _s.GetFloat(0x120); set => _s.SetFloat(0x120, value); }

        public float RightBunnyHoodY { get => _s.GetFloat(0x124); set => _s.SetFloat(0x124, value); }

        public float RightBunnyHoodZ { get => _s.GetFloat(0x128); set => _s.SetFloat(0x128, value); }

        public float BunnyHoodSize { get => _s.GetFloat(0x12C); set => _s.SetFloat(0x12C, value); }

        public float FlowerX { get => _s.GetFloat(0x130); set => _s.SetFloat(0x130, value); }

        public float FlowerY { get => _s.GetFloat(0x134); set => _s.SetFloat(0x134, value); }

        public float FlowerZ { get => _s.GetFloat(0x138); set => _s.SetFloat(0x138, value); }

        public float FlowerSize { get => _s.GetFloat(0x13C); set => _s.SetFloat(0x13C, value); }

        public float ScrewAttackUpwardKnockback { get => _s.GetFloat(0x140); set => _s.SetFloat(0x140, value); }

        public float ScrewAttackEffectSize { get => _s.GetFloat(0x144); set => _s.SetFloat(0x144, value); }

        public float Unknown0x148 { get => _s.GetFloat(0x148); set => _s.SetFloat(0x148, value); }

        public float BubbleRatio { get => _s.GetFloat(0x14C); set => _s.SetFloat(0x14C, value); }

        public float FreezeOffset1 { get => _s.GetFloat(0x150); set => _s.SetFloat(0x150, value); }

        public float FreezeOffset2 { get => _s.GetFloat(0x154); set => _s.SetFloat(0x154, value); }

        public float FreezeEscapeHeight { get => _s.GetFloat(0x158); set => _s.SetFloat(0x158, value); }

        public float FreezeEscapeXMomentum { get => _s.GetFloat(0x15C); set => _s.SetFloat(0x15c, value); }

        public float FrozenSize { get => _s.GetFloat(0x160); set => _s.SetFloat(0x160, value); }

        public float WarpStarHitboxScaling { get => _s.GetFloat(0x164); set => _s.SetFloat(0x164, value); }

        public float Unknown0x168 { get => _s.GetFloat(0x168); set => _s.SetFloat(0x168, value); }

        public int CameraZoomTargetBone { get => _s.GetInt32(0x16C); set => _s.SetInt32(0x16C, value); }

        public float MagnifiedXSway { get => _s.GetFloat(0x170); set => _s.SetFloat(0x170, value); }

        public float MagnifiedYSway { get => _s.GetFloat(0x174); set => _s.SetFloat(0x174, value); }

        public float MagnifiedZSway { get => _s.GetFloat(0x178); set => _s.SetFloat(0x178, value); }

        public float FootstoolYOffset { get => _s.GetFloat(0x17C); set => _s.SetFloat(0x17C, value); }

        [DisplayName("Weight Independent Throw Flags"), Description("Throws that are weight independent. Weight independent throw's speed are not affected by character weight")]
        public WeightDependentFlag WeightIndependentThrows { get => (WeightDependentFlag)(_s.GetInt32(0x180)>>24); set => _s.SetInt32(0x180, (int)value << 24); }
    }

    [Flags]
    public enum WeightDependentFlag
    {
        None = 0,
        FThrow = 1,
        BThrow = 2,
        UThrow = 4,
        DThrow = 8,
        All = 0xF
    }
}
