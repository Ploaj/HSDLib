using System;
using System.ComponentModel;

namespace HSDRaw.MEX
{
    [Flags]
    public enum MexFlags
    {
        ContainMoveLogic = 1,
        ContainItemState = 2,
        ContainMapGOBJs = 4,
    }

    public class MEX_Meta : HSDAccessor
    {
        public override int TrimmedSize => 0x40;

        [DisplayName("Version Major")]
        public byte VersionMajor { get => _s.GetByte(0x00); internal set => _s.SetByte(0x00, value); }

        [DisplayName("Version Minor")]
        public byte VersionMinor { get => _s.GetByte(0x01); internal set => _s.SetByte(0x01, value); }

        [DisplayName("m-ex Flags")]
        public MexFlags Flags { get => (MexFlags)_s.GetInt16(0x02); set => _s.SetInt16(0x02, (short)value); }

        [DisplayName("Internal ID Count")]
        public int NumOfInternalIDs { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [DisplayName("External ID Count")]
        public int NumOfExternalIDs { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [DisplayName("CSS Icon Count")]
        public int NumOfCSSIcons { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        [DisplayName("Internal Stage Count")]
        public int NumOfInternalStage { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        [DisplayName("External Stage Count")]
        public int NumOfExternalStage { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
        
        [DisplayName("SSS Icon Count")]
        public int NumOfSSSIcons { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        [DisplayName("SSM Count")]
        public int NumOfSSMs { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        [DisplayName("BGM Count")]
        public int NumOfMusic { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }
        
        [DisplayName("Effect Count")]
        public int NumOfEffects { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        [DisplayName("Boot Up Scene")]
        public int EnterScene { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        [DisplayName("Last Major Scene ID")]
        public int LastMajor { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }

        [DisplayName("Last Minor Scene ID")]
        public int LastMinor { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        [DisplayName("Trophy Count")]
        public int TrophyCount { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        [DisplayName("Trophy SD Offset")]
        public int TrophySDOffset { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }
    }

}
