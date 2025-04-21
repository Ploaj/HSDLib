using System.ComponentModel;

namespace HSDRaw.MEX.Characters
{
    public class MEX_ResultCameraParam : HSDAccessor
    {
        public override int TrimmedSize => 0x60;

        [DisplayName("X Offset (Win 1)")]
        public float Win1XOffset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        [DisplayName("Y Offset (Win 1)")]
        public float Win1YOffset { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        [DisplayName("Zoom (Win 1)")]
        public float Win1Scale { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        [DisplayName("X Offset (Win 2)")]
        public float Win2XOffset { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        [DisplayName("Y Offset (Win 2)")]
        public float Win2YOffset { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        [DisplayName("Zoom (Win 2)")]
        public float Win2Scale { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        [DisplayName("X Offset (Win 3)")]
        public float Win3XOffset { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        [DisplayName("Y Offset (Win 3)")]
        public float Win3YOffset { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        [DisplayName("Zoom (Win 3)")]
        public float Win3Scale { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        [DisplayName("X Offset (Lose)")]
        public float LoseXOffset { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        [DisplayName("Y Offset (Lose)")]
        public float LoseYOffset { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        [DisplayName("Zoom (Lose)")]
        public float LoseScale { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }


        [DisplayName("X Offset (1st Place)")]
        public float Rank1XOffset { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        [DisplayName("Y Offset (1st Place)")]
        public float Rank1YOffset { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }


        [DisplayName("X Offset (2nd Place)")]
        public float Rank2XOffset { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        [DisplayName("Y Offset (2nd Place)")]
        public float Rank2YOffset { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }


        [DisplayName("X Offset (3rd Place)")]
        public float Rank3XOffset { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }

        [DisplayName("Y Offset (3rd Place)")]
        public float Rank3YOffset { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }


        [DisplayName("X Offset (4th Place)")]
        public float Rank4XOffset { get => _s.GetFloat(0x3C); set => _s.SetFloat(0x3C, value); }

        [DisplayName("Y Offset (4th Place)")]
        public float Rank4YOffset { get => _s.GetFloat(0x4C); set => _s.SetFloat(0x4C, value); }


    }
}
