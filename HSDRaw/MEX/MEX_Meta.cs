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
        public override int TrimmedSize => 0x20;

        [DisplayName("Version Major")]
        public short VersionMajor { get => _s.GetInt16(0x00); internal set => _s.SetInt16(0x00, value); }

        [DisplayName("Mex Flags")]
        public MexFlags Flags { get => (MexFlags)_s.GetInt16(0x02); set => _s.SetInt16(0x02, (short)value); }

        [DisplayName("Internal ID Count")]
        public int NumOfInternalIDs { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [DisplayName("External ID Count")]
        public int NumOfExternalIDs { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [DisplayName("CSS Icon Count")]
        public int NumOfCSSIcons { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
        
        [DisplayName("SSS Icon Count")]
        public int NumOfSSSIcons { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        [DisplayName("SSM Count")]
        public int NumOfSSMs { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        [DisplayName("BGM Count")]
        public int NumOfMusic { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }
        
        [DisplayName("Effect Count")]
        public int NumOfEffects { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }
    }

}
