using System;
using System.ComponentModel;

namespace HSDRaw.Melee
{
    public class SBM_BoneLookupTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        private HSDAccessor CommonAttribute { get => _s.GetCreateReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        [Category("0 - General")]
        public int BoneCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [Category("1 - Bone Indices")]
        public byte Top { get => CommonAttribute._s.GetByte(0x00); set { CommonAttribute._s.SetByte(0x00, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte Trans { get => CommonAttribute._s.GetByte(0x01); set { CommonAttribute._s.SetByte(0x01, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte XRot { get => CommonAttribute._s.GetByte(0x02); set { CommonAttribute._s.SetByte(0x02, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte YRot { get => CommonAttribute._s.GetByte(0x03); set { CommonAttribute._s.SetByte(0x03, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte Hip { get => CommonAttribute._s.GetByte(0x04); set { CommonAttribute._s.SetByte(0x04, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte Waist { get => CommonAttribute._s.GetByte(0x05); set { CommonAttribute._s.SetByte(0x05, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LLegA { get => CommonAttribute._s.GetByte(0x06); set { CommonAttribute._s.SetByte(0x06, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LLeg { get => CommonAttribute._s.GetByte(0x07); set { CommonAttribute._s.SetByte(0x07, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LKnee { get => CommonAttribute._s.GetByte(0x08); set { CommonAttribute._s.SetByte(0x08, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LFootA { get => CommonAttribute._s.GetByte(0x09); set { CommonAttribute._s.SetByte(0x09, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LFoot { get => CommonAttribute._s.GetByte(0x0A); set { CommonAttribute._s.SetByte(0x0A, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RLegA { get => CommonAttribute._s.GetByte(0x0B); set { CommonAttribute._s.SetByte(0x0B, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RLeg { get => CommonAttribute._s.GetByte(0x0C); set { CommonAttribute._s.SetByte(0x0C, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RKnee { get => CommonAttribute._s.GetByte(0x0D); set { CommonAttribute._s.SetByte(0x0D, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RFootA { get => CommonAttribute._s.GetByte(0x0E); set { CommonAttribute._s.SetByte(0x0E, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RFoot { get => CommonAttribute._s.GetByte(0x0F); set { CommonAttribute._s.SetByte(0x0F, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte WaistB { get => CommonAttribute._s.GetByte(0x10); set { CommonAttribute._s.SetByte(0x10, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte Bust { get => CommonAttribute._s.GetByte(0x11); set { CommonAttribute._s.SetByte(0x11, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LClavicle { get => CommonAttribute._s.GetByte(0x12); set { CommonAttribute._s.SetByte(0x12, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LShoulderA { get => CommonAttribute._s.GetByte(0x13); set { CommonAttribute._s.SetByte(0x13, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LShoulder { get => CommonAttribute._s.GetByte(0x14); set { CommonAttribute._s.SetByte(0x14, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LArm { get => CommonAttribute._s.GetByte(0x15); set { CommonAttribute._s.SetByte(0x15, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LHand { get => CommonAttribute._s.GetByte(0x16); set { CommonAttribute._s.SetByte(0x16, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LIndex1 { get => CommonAttribute._s.GetByte(0x17); set { CommonAttribute._s.SetByte(0x17, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LIndex2 { get => CommonAttribute._s.GetByte(0x18); set { CommonAttribute._s.SetByte(0x18, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LMiddle1 { get => CommonAttribute._s.GetByte(0x19); set { CommonAttribute._s.SetByte(0x19, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LMiddle2 { get => CommonAttribute._s.GetByte(0x1A); set { CommonAttribute._s.SetByte(0x1A, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LRing1 { get => CommonAttribute._s.GetByte(0x1B); set { CommonAttribute._s.SetByte(0x1B, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LRing2 { get => CommonAttribute._s.GetByte(0x1C); set { CommonAttribute._s.SetByte(0x1C, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LPinky1 { get => CommonAttribute._s.GetByte(0x1D); set { CommonAttribute._s.SetByte(0x1D, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LPinky2 { get => CommonAttribute._s.GetByte(0x1E); set { CommonAttribute._s.SetByte(0x1E, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LHave { get => CommonAttribute._s.GetByte(0x1F); set { CommonAttribute._s.SetByte(0x1F, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LThumb1 { get => CommonAttribute._s.GetByte(0x020); set { CommonAttribute._s.SetByte(0x20, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte LThumb2 { get => CommonAttribute._s.GetByte(0x21); set { CommonAttribute._s.SetByte(0x21, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte NeckN { get => CommonAttribute._s.GetByte(0x22); set { CommonAttribute._s.SetByte(0x22, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte HeadN { get => CommonAttribute._s.GetByte(0x23); set { CommonAttribute._s.SetByte(0x23, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RClavicle { get => CommonAttribute._s.GetByte(0x24); set { CommonAttribute._s.SetByte(0x24, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RShoulderA { get => CommonAttribute._s.GetByte(0x25); set { CommonAttribute._s.SetByte(0x25, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RShoulder { get => CommonAttribute._s.GetByte(0x26); set { CommonAttribute._s.SetByte(0x26, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RArm { get => CommonAttribute._s.GetByte(0x27); set { CommonAttribute._s.SetByte(0x27, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RHand { get => CommonAttribute._s.GetByte(0x28); set { CommonAttribute._s.SetByte(0x28, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RIndex1 { get => CommonAttribute._s.GetByte(0x29); set { CommonAttribute._s.SetByte(0x29, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RIndex2 { get => CommonAttribute._s.GetByte(0x2A); set { CommonAttribute._s.SetByte(0x2A, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RMiddle1 { get => CommonAttribute._s.GetByte(0x2B); set { CommonAttribute._s.SetByte(0x2B, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RMiddle2 { get => CommonAttribute._s.GetByte(0x2C); set { CommonAttribute._s.SetByte(0x2C, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RRing1 { get => CommonAttribute._s.GetByte(0x2D); set { CommonAttribute._s.SetByte(0x2D, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RRing2 { get => CommonAttribute._s.GetByte(0x2E); set { CommonAttribute._s.SetByte(0x2E, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RPinky1 { get => CommonAttribute._s.GetByte(0x2F); set { CommonAttribute._s.SetByte(0x2F, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RPinky2 { get => CommonAttribute._s.GetByte(0x30); set { CommonAttribute._s.SetByte(0x030, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RHave { get => CommonAttribute._s.GetByte(0x31); set { CommonAttribute._s.SetByte(0x31, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RThumb1 { get => CommonAttribute._s.GetByte(0x32); set { CommonAttribute._s.SetByte(0x32, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte RThumb2 { get => CommonAttribute._s.GetByte(0x33); set { CommonAttribute._s.SetByte(0x33, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
        public byte Throw { get => CommonAttribute._s.GetByte(0x34); set { CommonAttribute._s.SetByte(0x34, value); CreateReverseLookup(); } }

        [Category("1 - Bone Indices")]
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

    public class SBM_PlCoFighterBoneExt : HSDAccessor
    {
        public override int TrimmedSize => 0x08;
        
        public SBM_PlCoFighterBoneExtEntry[] Entries
        {
            get
            {
                if (_s.GetReference<HSDArrayAccessor<SBM_PlCoFighterBoneExtEntry>>(0) == null)
                    return null;

                return _s.GetReference<HSDArrayAccessor<SBM_PlCoFighterBoneExtEntry>>(0).Array;
            }
            set
            {
                if(value != null && value.Length > 0)
                {
                    _s.GetCreateReference<HSDArrayAccessor<SBM_PlCoFighterBoneExtEntry>>(0).Array = value;
                    _s.SetInt32(0x04, value.Length);
                }
                else
                {
                    _s.SetReference(0x00, null);
                    _s.SetInt32(0x04, 0);
                }
            }
        }
    }

    public class SBM_PlCoFighterBoneExtEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public byte Value1 { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte Value2 { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte Value3 { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte Value4 { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }
    }
}
