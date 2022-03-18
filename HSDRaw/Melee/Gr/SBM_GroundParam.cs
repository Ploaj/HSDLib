using HSDRaw.MEX;
using System.ComponentModel;
using System.Drawing;

namespace HSDRaw.Melee.Gr
{
    public class SBM_GroundParam : HSDAccessor
    {
        public override int TrimmedSize => 0xDC;

        public float StageScale { get => _s.GetFloat(0x0); set => _s.SetFloat(0x0, value); }

        [TypeConverter(typeof(HexType))]
        public uint DeltaCam1 { get => (uint)_s.GetInt32(0x4); set => _s.SetInt32(0x4, (int)value); }

        public short FieldOfView { get => _s.GetInt16(0x8); set => _s.SetInt16(0x8, value); }

        public int CameraDistanceMin { get => _s.GetInt32(0xc); set => _s.SetInt32(0xc, value); }

        public int CameraDistanceMax { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int TiltScale { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public float VerticalRotation { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float HorizontalRotation { get => _s.GetFloat(0x1c); set => _s.SetFloat(0x1c, value); }

        public float Fixedness { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float BubbleMultiplier { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float CameraSpeedSmoothness { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public int Unknown1 { get => _s.GetInt32(0x2c); set => _s.SetInt32(0x2c, value); }

        public int PauseMinZ { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        public int PauseInitialZ { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        public int PauseMaxZ { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }

        public float PauseMaxAngleUp { get => _s.GetFloat(0x3c); set => _s.SetFloat(0x3c, value); }

        public float PauseMaxAngleDown { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float PauseMaxAngleLeft { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float PauseMaxAngleRight { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }

        public float Unknown2 { get => _s.GetFloat(0x4c); set => _s.SetFloat(0x4c, value); }

        public float FixedCamX { get => _s.GetFloat(0x50); set => _s.SetFloat(0x50, value); }

        public float FixedCamY { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }

        public float FixedCamZ { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }

        public float FixedFieldOfView { get => _s.GetFloat(0x5c); set => _s.SetFloat(0x5c, value); }

        public float FixedVerticalAngle { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }

        public float FixedHorizontalAngle { get => _s.GetFloat(0x64); set => _s.SetFloat(0x64, value); }

        public short UnknownShort0 { get => _s.GetInt16(0x68); set => _s.SetInt16(0x68, value); }

        public short ItSpawn_Capsule { get => _s.GetInt16(0x6A); set => _s.SetInt16(0x6A, value); }

        public short ItSpawn_Box { get => _s.GetInt16(0x6C); set => _s.SetInt16(0x6C, value); }

        public short ItSpawn_Barrel { get => _s.GetInt16(0x6E); set => _s.SetInt16(0x6E, value); }

        public short ItSpawn_Egg { get => _s.GetInt16(0x70); set => _s.SetInt16(0x70, value); }

        public short ItSpawn_PartyBall { get => _s.GetInt16(0x72); set => _s.SetInt16(0x72, value); }

        public short ItSpawn_BarrelCannon { get => _s.GetInt16(0x74); set => _s.SetInt16(0x74, value); }

        public short ItSpawn_Bobomb { get => _s.GetInt16(0x76); set => _s.SetInt16(0x76, value); }

        public short ItSpawn_MrSaturn { get => _s.GetInt16(0x78); set => _s.SetInt16(0x78, value); }

        public short ItSpawn_HeartContainer { get => _s.GetInt16(0x7A); set => _s.SetInt16(0x7A, value); }

        public short ItSpawn_MaxTomato { get => _s.GetInt16(0x7C); set => _s.SetInt16(0x7C, value); }

        public short ItSpawn_SuperStar { get => _s.GetInt16(0x7E); set => _s.SetInt16(0x7E, value); }

        public short ItSpawn_HomeRunBat { get => _s.GetInt16(0x80); set => _s.SetInt16(0x80, value); }

        public short ItSpawn_BeamSword { get => _s.GetInt16(0x82); set => _s.SetInt16(0x82, value); }

        public short ItSpawn_Parasol { get => _s.GetInt16(0x84); set => _s.SetInt16(0x84, value); }

        public short ItSpawn_GreenShell { get => _s.GetInt16(0x86); set => _s.SetInt16(0x86, value); }

        public short ItSpawn_RedShell { get => _s.GetInt16(0x88); set => _s.SetInt16(0x88, value); }

        public short ItSpawn_Raygun { get => _s.GetInt16(0x8A); set => _s.SetInt16(0x8A, value); }

        public short ItSpawn_Freezie { get => _s.GetInt16(0x8C); set => _s.SetInt16(0x8C, value); }

        public short ItSpawn_Food { get => _s.GetInt16(0x8E); set => _s.SetInt16(0x8E, value); }

        public short ItSpawn_ProximetyMine { get => _s.GetInt16(0x90); set => _s.SetInt16(0x90, value); }

        public short ItSpawn_Flipper { get => _s.GetInt16(0x92); set => _s.SetInt16(0x92, value); }

        public short ItSpawn_SuperScope { get => _s.GetInt16(0x94); set => _s.SetInt16(0x94, value); }

        public short ItSpawn_StarRod { get => _s.GetInt16(0x96); set => _s.SetInt16(0x96, value); }

        public short ItSpawn_LipStick { get => _s.GetInt16(0x98); set => _s.SetInt16(0x98, value); }

        public short ItSpawn_Fan { get => _s.GetInt16(0x9A); set => _s.SetInt16(0x9A, value); }

        public short ItSpawn_FireFlower { get => _s.GetInt16(0x9C); set => _s.SetInt16(0x9C, value); }

        public short ItSpawn_SuperMushroom { get => _s.GetInt16(0x9E); set => _s.SetInt16(0x9E, value); }

        public short ItSpawn_Unk1 { get => _s.GetInt16(0xA0); set => _s.SetInt16(0xA0, value); }

        public short ItSpawn_Unk2 { get => _s.GetInt16(0xA2); set => _s.SetInt16(0xA2, value); }

        public short ItSpawn_WarpStar { get => _s.GetInt16(0xA4); set => _s.SetInt16(0xA4, value); }

        public short ItSpawn_ScrewAttack { get => _s.GetInt16(0xA6); set => _s.SetInt16(0xA6, value); }

        public short ItSpawn_BunnyHood { get => _s.GetInt16(0xA8); set => _s.SetInt16(0xA8, value); }

        public short ItSpawn_MetalBox { get => _s.GetInt16(0xAA); set => _s.SetInt16(0xAA, value); }

        public short ItSpawn_CloakingDevice { get => _s.GetInt16(0xAC); set => _s.SetInt16(0xAC, value); }

        public short ItSpawn_Pokeball { get => _s.GetInt16(0xAE); set => _s.SetInt16(0xAE, value); }

        public SBM_GroundBGM[] BGMData
        {
            get
            {
                return _s.GetCreateReference<HSDArrayAccessor<SBM_GroundBGM>>(0xB0).Array;
            }
            set
            {
                if(value == null)
                {
                    _s.GetCreateReference<HSDArrayAccessor<SBM_GroundBGM>>(0xB0).Array = new SBM_GroundBGM[0];
                    BGMVariationCount = 0;
                    return;
                }
                _s.GetCreateReference<HSDArrayAccessor<SBM_GroundBGM>>(0xB0).Array = value;
                BGMVariationCount = value.Length;
            }
        }
        
        public int BGMVariationCount { get => _s.GetInt32(0xB4); internal set => _s.SetInt32(0xB4, value); }

        public Color BubbleColorTopLeft { get => _s.GetColorRGBA(0xB8); set => _s.SetColorRGBA(0xB8, value); }

        public Color BubbleColorTopMiddle { get => _s.GetColorRGBA(0xBC); set => _s.SetColorRGBA(0xBC, value); }

        public Color BubbleColorTopRight { get => _s.GetColorRGBA(0xC0); set => _s.SetColorRGBA(0xC0, value); }

        public Color BubbleColorSidesTop { get => _s.GetColorRGBA(0xC4); set => _s.SetColorRGBA(0xC4, value); }

        public Color BubbleColorSidesMiddle { get => _s.GetColorRGBA(0xC8); set => _s.SetColorRGBA(0xC8, value); }

        public Color BubbleColorSidesBottom { get => _s.GetColorRGBA(0xCC); set => _s.SetColorRGBA(0xCC, value); }

        public Color BubbleColorBottomLeft { get => _s.GetColorRGBA(0xD0); set => _s.SetColorRGBA(0xD0, value); }

        public Color BubbleColorBottomMiddle { get => _s.GetColorRGBA(0xD4); set => _s.SetColorRGBA(0xD4, value); }

        public Color BubbleColorBottomRight { get => _s.GetColorRGBA(0xD8); set => _s.SetColorRGBA(0xD8, value); }
    }

    public class SBM_GroundBGM : HSDAccessor
    {
        public override int TrimmedSize => 0x64;

        public int GrKind { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int MainMusic { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int AltMusic { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int SuddenDeathMainMusic { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int SuddenDeathAltMusic { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public short SongBehaviorFlag { get => _s.GetInt16(0x14); set => _s.SetInt16(0x14, value); }

        public short ChanceToPlayAltSong { get => _s.GetInt16(0x16); set => _s.SetInt16(0x16, value); }
    }
}
