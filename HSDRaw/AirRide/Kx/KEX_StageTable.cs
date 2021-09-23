using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide.Kx
{
    public class KEX_StageTable : HSDAccessor
    {
        public override int TrimmedSize => 0x30;


        public KEX_StageMetaData MetaData { get => _s.GetReference<KEX_StageMetaData>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<KEX_StageStringTable> FileStrings { get => _s.GetReference<HSDArrayAccessor<KEX_StageStringTable>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDFixedLengthPointerArrayAccessor<KEX_StageFunctions> Functions { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KEX_StageFunctions>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDUIntArray RuntimePointers { get => _s.GetReference<HSDUIntArray>(0x0C); set => _s.SetReference(0x0C, value); }


        public HSDByteArray IconMatAnimIndex { get => _s.GetReference<HSDByteArray>(0x10); set => _s.SetReference(0x10, value); }

        public HSDByteArray IconIndexToStageIndex { get => _s.GetReference<HSDByteArray>(0x14); set => _s.SetReference(0x14, value); }

    }

    public class KEX_StageMetaData : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public int StageFileCount { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int StageKindCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

    }

    public class KEX_StageStringTable : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public string DataFilePath { get => _s.GetString(0x00); set => _s.SetString(0x00, value); }

        public string DataSymbol { get => _s.GetString(0x04); set => _s.SetString(0x04, value); }

        public string ModelFilePath { get => _s.GetString(0x08); set => _s.SetString(0x08, value); }

        public string ModelSymbol { get => _s.GetString(0x0C); set => _s.SetString(0x0C, value); }

        public string ModelMotionSymbol { get => _s.GetString(0x10); set => _s.SetString(0x10, value); }
    }

    public class KEX_StageFunctions : HSDAccessor
    {
        public override int TrimmedSize => 0xC;

        public int OnInit { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int SetupYakumono { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int Unknown { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }
}