using System.ComponentModel;

namespace HSDRaw.Melee.Pl
{
    public class SBM_PlayerSFXTable : HSDAccessor
    {
        public override int TrimmedSize => 0x38;

        [Browsable(false)]
        public Int32Table RandomSmashSFX { get => _s.GetReference<Int32Table>(0x00); set => _s.SetReference(0x00, value); }
        
        public int SFX_YouFreakinDied { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int SFX_MetalBox { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int SFX_StarKO { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int SFX_Jump { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int SFX_DoubleJump { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        [Description("Used for Air-Dodge, Ledge-Teeter, and Spot-Dodge")]
        public int SFX_Dodge { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        [Browsable(false)]
        public Int32Table LightHitSFX { get => _s.GetReference<Int32Table>(0x1C); set => _s.SetReference(0x1C, value); }

        [Browsable(false)]
        public Int32Table HeavyHitSFX { get => _s.GetReference<Int32Table>(0x20); set => _s.SetReference(0x20, value); }

        public int SFX_Tech { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        public int SFX_LedgeGrab { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        public int SFX_HeavyLift { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }

        public int SFX_GrabCatch { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        public int SFX_Cheer { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }
    }

}