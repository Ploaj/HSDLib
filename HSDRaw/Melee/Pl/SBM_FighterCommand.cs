using System.Text;
using HSDRaw.Melee.Cmd;
using HSDRaw.Common.Animation;
using HSDRaw.Common;

namespace HSDRaw.Melee.Pl
{
    public class SBM_FighterCommand : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public HSD_String SymbolName { get => _s.GetReference<HSD_String>(0x00); set => _s.SetReference(0x00, value); }

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
                else
                {
                    var re = _s.GetCreateReference<HSDAccessor>(0x00);
                    byte[] data = new byte[value.Length + 1];
                    var bytes = UTF8Encoding.UTF8.GetBytes(value);
                    for (int i = 0; i < value.Length; i++)
                        data[i] = bytes[i];
                    re._s.SetData(data);
                }
            }
        }

        public int AnimationOffset { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public HSD_FigaTree Animation { get => _s.GetReference<HSD_FigaTree>(0x04); set { _s.SetReference(0x04, value);  AnimationSize = value._s.Length; } }

        public int AnimationSize { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public SBM_FighterSubactionData SubAction { get => _s.GetReference<SBM_FighterSubactionData>(0x0C); set => _s.SetReference(0x0C, value); }

        public uint Flags { get => (uint)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (int)value); }
        
    }
}
