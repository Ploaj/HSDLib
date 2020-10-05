using System.Text;

namespace HSDRaw.MEX.Scenes
{
    public class MEX_MajorScene : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public bool Preload { get => _s.GetByte(0x00) == 1; set => _s.SetByte(0x00, value ? (byte)1 : (byte)0); }

        public byte MajorSceneID { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public int LoadFunction { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int UnloadFunction { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int OnBootFunction { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public HSDArrayAccessor<MEX_MinorScene> MinorScene { get => _s.GetReference<HSDArrayAccessor<MEX_MinorScene>>(0x10); set => _s.SetReference(0x10, value); }

        public string FileName
        {
            get
            {
                var re = _s.GetBuffer(0x14);
                if (re == null)
                    return null;
                else
                {
                    StringBuilder b = new StringBuilder();
                    for (int i = 0; i < re.Length; i++)
                    {
                        if (re[i] != 0)
                            b.Append((char)re[i]);
                        else
                            break;
                    }
                    return b.ToString();
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _s.SetReference(0x14, null);
                }
                else
                {

                    var re = _s.GetCreateReference<HSDAccessor>(0x14);
                    byte[] data = new byte[value.Length + 1];
                    var bytes = UTF8Encoding.UTF8.GetBytes(value);
                    for (int i = 0; i < value.Length; i++)
                        data[i] = bytes[i];
                    re._s.SetData(data);
                }
            }
        }
    }
}
