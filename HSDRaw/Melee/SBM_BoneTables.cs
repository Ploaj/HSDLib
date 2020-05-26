using System;

namespace HSDRaw.Melee
{
    public class SBM_BoneTableBank : HSDFixedLengthPointerArrayAccessor<SBM_BoneLookupTable>
    {

    }

    public class SBM_BoneLookupTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        private HSDAccessor CommonAttribute { get => _s.GetCreateReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public int BoneCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public byte Top { get => CommonAttribute._s.GetByte(0x00); set { CommonAttribute._s.SetByte(0x00, value); CreateReverseLookup(); } }

        public byte Trans { get => CommonAttribute._s.GetByte(0x01); set { CommonAttribute._s.SetByte(0x01, value); CreateReverseLookup(); } }

        public byte XRot { get => CommonAttribute._s.GetByte(0x02); set { CommonAttribute._s.SetByte(0x02, value); CreateReverseLookup(); } }

        public byte YRot { get => CommonAttribute._s.GetByte(0x03); set { CommonAttribute._s.SetByte(0x03, value); CreateReverseLookup(); } }

        public byte Hip { get => CommonAttribute._s.GetByte(0x04); set { CommonAttribute._s.SetByte(0x04, value); CreateReverseLookup(); } }

        public byte Waist { get => CommonAttribute._s.GetByte(0x05); set { CommonAttribute._s.SetByte(0x05, value); CreateReverseLookup(); } }

        public byte LLegA { get => CommonAttribute._s.GetByte(0x06); set { CommonAttribute._s.SetByte(0x06, value); CreateReverseLookup(); } }

        public byte LLeg { get => CommonAttribute._s.GetByte(0x07); set { CommonAttribute._s.SetByte(0x07, value); CreateReverseLookup(); } }

        public byte LKnee { get => CommonAttribute._s.GetByte(0x08); set { CommonAttribute._s.SetByte(0x08, value); CreateReverseLookup(); } }

        public byte LFootA { get => CommonAttribute._s.GetByte(0x09); set { CommonAttribute._s.SetByte(0x09, value); CreateReverseLookup(); } }

        public byte LFoot { get => CommonAttribute._s.GetByte(0x0A); set { CommonAttribute._s.SetByte(0x0A, value); CreateReverseLookup(); } }

        public byte RLegA { get => CommonAttribute._s.GetByte(0x0B); set { CommonAttribute._s.SetByte(0x0B, value); CreateReverseLookup(); } }

        public byte RLeg { get => CommonAttribute._s.GetByte(0x0C); set { CommonAttribute._s.SetByte(0x0C, value); CreateReverseLookup(); } }

        public byte RKnee { get => CommonAttribute._s.GetByte(0x0D); set { CommonAttribute._s.SetByte(0x0D, value); CreateReverseLookup(); } }

        public byte RFootA { get => CommonAttribute._s.GetByte(0x0E); set { CommonAttribute._s.SetByte(0x0E, value); CreateReverseLookup(); } }

        public byte RFoot { get => CommonAttribute._s.GetByte(0x0F); set { CommonAttribute._s.SetByte(0x0F, value); CreateReverseLookup(); } }

        public byte WaistB { get => CommonAttribute._s.GetByte(0x10); set { CommonAttribute._s.SetByte(0x10, value); CreateReverseLookup(); } }

        public byte Bust { get => CommonAttribute._s.GetByte(0x11); set { CommonAttribute._s.SetByte(0x11, value); CreateReverseLookup(); } }

        public byte LClavicle { get => CommonAttribute._s.GetByte(0x12); set { CommonAttribute._s.SetByte(0x12, value); CreateReverseLookup(); } }

        public byte LShoulderA { get => CommonAttribute._s.GetByte(0x13); set { CommonAttribute._s.SetByte(0x13, value); CreateReverseLookup(); } }

        public byte LShoulder { get => CommonAttribute._s.GetByte(0x14); set { CommonAttribute._s.SetByte(0x14, value); CreateReverseLookup(); } }

        public byte LArm { get => CommonAttribute._s.GetByte(0x15); set { CommonAttribute._s.SetByte(0x15, value); CreateReverseLookup(); } }

        public byte LHand { get => CommonAttribute._s.GetByte(0x16); set { CommonAttribute._s.SetByte(0x16, value); CreateReverseLookup(); } }

        public byte LIndex1 { get => CommonAttribute._s.GetByte(0x17); set { CommonAttribute._s.SetByte(0x17, value); CreateReverseLookup(); } }

        public byte LIndex2 { get => CommonAttribute._s.GetByte(0x18); set { CommonAttribute._s.SetByte(0x18, value); CreateReverseLookup(); } }

        public byte LMiddle1 { get => CommonAttribute._s.GetByte(0x19); set { CommonAttribute._s.SetByte(0x19, value); CreateReverseLookup(); } }

        public byte LMiddle2 { get => CommonAttribute._s.GetByte(0x1A); set { CommonAttribute._s.SetByte(0x1A, value); CreateReverseLookup(); } }

        public byte LRing1 { get => CommonAttribute._s.GetByte(0x1B); set { CommonAttribute._s.SetByte(0x1B, value); CreateReverseLookup(); } }

        public byte LRing2 { get => CommonAttribute._s.GetByte(0x1C); set { CommonAttribute._s.SetByte(0x1C, value); CreateReverseLookup(); } }

        public byte LPinky1 { get => CommonAttribute._s.GetByte(0x1D); set { CommonAttribute._s.SetByte(0x1D, value); CreateReverseLookup(); } }

        public byte LPinky2 { get => CommonAttribute._s.GetByte(0x1E); set { CommonAttribute._s.SetByte(0x1E, value); CreateReverseLookup(); } }

        public byte LHave { get => CommonAttribute._s.GetByte(0x1F); set { CommonAttribute._s.SetByte(0x1F, value); CreateReverseLookup(); } }

        public byte LThumb1 { get => CommonAttribute._s.GetByte(0x020); set { CommonAttribute._s.SetByte(0x20, value); CreateReverseLookup(); } }

        public byte LThumb2 { get => CommonAttribute._s.GetByte(0x21); set { CommonAttribute._s.SetByte(0x21, value); CreateReverseLookup(); } }

        public byte NeckN { get => CommonAttribute._s.GetByte(0x22); set { CommonAttribute._s.SetByte(0x22, value); CreateReverseLookup(); } }

        public byte HeadN { get => CommonAttribute._s.GetByte(0x23); set { CommonAttribute._s.SetByte(0x23, value); CreateReverseLookup(); } }

        public byte RClavicle { get => CommonAttribute._s.GetByte(0x24); set { CommonAttribute._s.SetByte(0x24, value); CreateReverseLookup(); } }

        public byte RShoulderA { get => CommonAttribute._s.GetByte(0x25); set { CommonAttribute._s.SetByte(0x25, value); CreateReverseLookup(); } }

        public byte RShoulder { get => CommonAttribute._s.GetByte(0x26); set { CommonAttribute._s.SetByte(0x26, value); CreateReverseLookup(); } }

        public byte RArm { get => CommonAttribute._s.GetByte(0x27); set { CommonAttribute._s.SetByte(0x27, value); CreateReverseLookup(); } }

        public byte RHand { get => CommonAttribute._s.GetByte(0x28); set { CommonAttribute._s.SetByte(0x28, value); CreateReverseLookup(); } }

        public byte RIndex1 { get => CommonAttribute._s.GetByte(0x29); set { CommonAttribute._s.SetByte(0x29, value); CreateReverseLookup(); } }

        public byte RIndex2 { get => CommonAttribute._s.GetByte(0x2A); set { CommonAttribute._s.SetByte(0x2A, value); CreateReverseLookup(); } }

        public byte RMiddle1 { get => CommonAttribute._s.GetByte(0x2B); set { CommonAttribute._s.SetByte(0x2B, value); CreateReverseLookup(); } }

        public byte RMiddle2 { get => CommonAttribute._s.GetByte(0x2C); set { CommonAttribute._s.SetByte(0x2C, value); CreateReverseLookup(); } }

        public byte RRing1 { get => CommonAttribute._s.GetByte(0x2D); set { CommonAttribute._s.SetByte(0x2D, value); CreateReverseLookup(); } }

        public byte RRing2 { get => CommonAttribute._s.GetByte(0x2E); set { CommonAttribute._s.SetByte(0x2E, value); CreateReverseLookup(); } }

        public byte RPinky1 { get => CommonAttribute._s.GetByte(0x2F); set { CommonAttribute._s.SetByte(0x2F, value); CreateReverseLookup(); } }

        public byte RPinky2 { get => CommonAttribute._s.GetByte(0x30); set { CommonAttribute._s.SetByte(0x030, value); CreateReverseLookup(); } }

        public byte RHave { get => CommonAttribute._s.GetByte(0x31); set { CommonAttribute._s.SetByte(0x31, value); CreateReverseLookup(); } }

        public byte RThumb1 { get => CommonAttribute._s.GetByte(0x32); set { CommonAttribute._s.SetByte(0x32, value); CreateReverseLookup(); } }

        public byte RThumb2 { get => CommonAttribute._s.GetByte(0x33); set { CommonAttribute._s.SetByte(0x33, value); CreateReverseLookup(); } }

        public byte Throw { get => CommonAttribute._s.GetByte(0x34); set { CommonAttribute._s.SetByte(0x34, value); CreateReverseLookup(); } }

        public byte Extra { get => CommonAttribute._s.GetByte(0x35); set { CommonAttribute._s.SetByte(0x35, value); CreateReverseLookup(); } }

        private void CreateReverseLookup()
        {
            var d = CommonAttribute._s.GetData();
            var boneCount = 0;
            for (int i = 0; i < 0x36; i++)
            {
                var boneIndex = d[i];
                if (boneIndex != 255)
                {
                    boneCount = Math.Max(boneCount, boneIndex + 1);
                }
            }

            BoneCount = boneCount;

            var reverseTable = _s.GetCreateReference<HSDAccessor>(0x00);
            reverseTable._s.Resize(boneCount);

            for (int i = 0; i < reverseTable._s.Length; i++)
                reverseTable._s.SetByte(i, 0xFF);

            if (reverseTable._s.Length % 4 != 0)
                reverseTable._s.Resize(reverseTable._s.Length + (4 - (reverseTable._s.Length % 4)));

            for (int i = 0; i < 0x36; i++)
                if (d[i] != 255)
                    reverseTable._s.SetByte(d[i], (byte)i);
        }

        public override void New()
        {
            base.New();

            CommonAttribute = new HSDAccessor();
            CommonAttribute._s.Resize(0x38);
            for (int i = 0; i <= 53; i++)
                CommonAttribute._s.SetByte(i, 0xFF);
        }
    }
}
