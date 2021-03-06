﻿using HSDRaw.Common;
using HSDRaw.MEX.Sounds;

namespace HSDRaw.MEX.Stages
{
    public class MEX_StageData : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public HSDArrayAccessor<MEX_StageIDTable> StageIDTable { get => _s.GetReference<HSDArrayAccessor<MEX_StageIDTable>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_StageReverb> ReverbTable { get => _s.GetReference<HSDArrayAccessor<MEX_StageReverb>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<MEX_StageCollision> CollisionTable { get => _s.GetReference<HSDArrayAccessor<MEX_StageCollision>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<MEX_ItemLookup> StageItemLookup { get => _s.GetReference<HSDArrayAccessor<MEX_ItemLookup>>(0x0C); set => _s.SetReference(0x0C, value); }
        
        public HSDFixedLengthPointerArrayAccessor<HSD_String> StageNames { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<MEX_Playlist> StagePlaylists { get => _s.GetReference<HSDArrayAccessor<MEX_Playlist>>(0x14); set => _s.SetReference(0x14, value); }

        public override void New()
        {
            base.New();
            StageIDTable = new HSDArrayAccessor<MEX_StageIDTable>();
            ReverbTable = new HSDArrayAccessor<MEX_StageReverb>();
            CollisionTable = new HSDArrayAccessor<MEX_StageCollision>();
            StageItemLookup = new HSDArrayAccessor<MEX_ItemLookup>();
            StageNames = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            StagePlaylists = new HSDArrayAccessor<MEX_Playlist>();
        }

    }

    public class MEX_StageIDTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public int StageID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Unknown1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int Unknown2 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public override string ToString()
        {
            return "StageID: " + StageID;
        }
    }

    public class MEX_StageReverb : HSDAccessor
    {
        public override int TrimmedSize => 0x03;

        public byte SSMID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte Reverb { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte Unknown { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }
    }
}
