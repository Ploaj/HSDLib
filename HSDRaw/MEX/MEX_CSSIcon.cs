using System.ComponentModel;

namespace HSDRaw.MEX
{
    public enum Status
    {
        NotUnlocked,
        Unlocked,
        UnlockedAndVisible
    }

    public class MEX_CSSIcon : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        [DisplayName("Unknown Char ID"), Description("")]
        public byte CharUNKID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        [DisplayName("External Char ID"), Description("")]
        public byte ExternalCharID { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        [DisplayName("Status"), Description("")]
        public Status StatusID { get => (Status)_s.GetByte(0x02); set => _s.SetByte(0x02, (byte)value); }

        [Browsable(false)]
        public byte IsAnimated { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }

        [DisplayName("Unknown ID"), Description("")]
        public byte UnkID { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }

        [DisplayName("Joint ID"), Description("")]
        public byte JointID { get => _s.GetByte(0x05); set => _s.SetByte(0x05, value); }

        [DisplayName("Sound Effect ID"), Description("")]
        public int SFXID { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [DisplayName("X"), Description("")]
        public float X1 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        [DisplayName("Y"), Description("")]
        public float Y1 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        [DisplayName("Width"), Description("")]
        public float X2 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        [DisplayName("Height"), Description("")]
        public float Y2 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public override string ToString()
        {
            return $"{ExternalCharID} - ({X1}, {X2}, {Y1}, {Y2})";
        }
    }
}
