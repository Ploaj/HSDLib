using System.Text;

namespace HSDRaw.Melee.Pl
{
    public class SBM_FighterCommand : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public string Name {
            get
            {
                var re = _s.GetBuffer(0x0);
                if (re == null)
                    return null;
                else
                {
                    StringBuilder b = new StringBuilder();
                    for(int i = 0; i < re.Length; i++)
                    {
                        if (re[i] != 0)
                        {
                            b.Append((char)re[i]);
                        }
                        else
                            break;
                    }
                    return b.ToString();
                }
            }
            set
            {
                if(value == null)
                {
                    _s.SetReference(0x00, null);
                }
                var re = _s.GetCreateReference<HSDAccessor>(0x00);
                byte[] data = new byte[value.Length+1];
                var bytes = UTF8Encoding.UTF8.GetBytes(value);
                for (int i = 0; i < value.Length; i++)
                    data[i] = bytes[i];
                re._s.SetData(data);
            }
        }

        public int AnimationOffset { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int AnimationSize { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public SBM_FighterSubactionData SubAction { get => _s.GetReference<SBM_FighterSubactionData>(0x0C); set => _s.SetReference(0x0C, value); }

        public uint Flags { get => (uint)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (int)value); }
        
    }

    public class SBM_FighterSubactionData : HSDAccessor
    {
        public override int Trim()
        {
            _s.CanBeBuffer = false;
            _s.Align = false;
            return base.Trim();
        }
    }

    public class SBM_ItemSubactionData : SBM_FighterSubactionData
    {

    }
}
