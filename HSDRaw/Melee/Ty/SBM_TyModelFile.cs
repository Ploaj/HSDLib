using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.Melee.Ty
{

    public class SBM_TyModelFileEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x54;

        public int TrophyIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public string FileName
        {
            get
            {
                return Encoding.ASCII.GetString(_s.GetSubData(0x04, 0x20));
            }
            set
            {
                _s.SetSubData(Encoding.ASCII.GetBytes(value), 0x04, 0x20, 0);
            }
        }
        public string SymbolName
        {
            get
            {
                return Encoding.ASCII.GetString(_s.GetSubData(0x24, 0x30));
            }
            set
            {
                _s.SetSubData(Encoding.ASCII.GetBytes(value), 0x24, 0x30, 0);
            }
        }
    }
}
