using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_FighterFunctionTable : HSDAccessor
    {
        public override int TrimmedSize => 0x100;

        public HSDArrayAccessor<HSD_UInt> OnLoad { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<HSD_UInt> OnDeath { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<HSD_UInt> OnUnknown { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x08); set => _s.SetReference(0x08, value); }


        // can have references or pointers for move logic
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>> MoveLogic
        {
            get
            {
                return _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>>>(0x0C);
            }
            set => _s.SetReference(0x0C, value);
        }

        public HSDArrayAccessor<HSD_UInt> MoveLogicPointers {
            get
            {
                return _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x0C);
            } set => _s.SetReference(0x0C, value); }


        public HSDArrayAccessor<HSD_UInt> SpecialN { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialNAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialS { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialSAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialHi { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x20); set => _s.SetReference(0x20, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialHiAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x24); set => _s.SetReference(0x24, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialLw { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x28); set => _s.SetReference(0x28, value); }

        public HSDArrayAccessor<HSD_UInt> SpecialLwAir { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x2C); set => _s.SetReference(0x2C, value); }

        public HSDArrayAccessor<HSD_UInt> OnAbsorb { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x30); set => _s.SetReference(0x30, value); }

        public HSDArrayAccessor<HSD_UInt> onItemPickup { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x34); set => _s.SetReference(0x34, value); }

        public HSDArrayAccessor<HSD_UInt> onMakeItemInvisible { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x38); set => _s.SetReference(0x38, value); }
        
        public HSDArrayAccessor<HSD_UInt> onMakeItemVisible { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDArrayAccessor<HSD_UInt> onItemDrop { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x40); set => _s.SetReference(0x40, value); }

        public HSDArrayAccessor<HSD_UInt> onItemCatch { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x44); set => _s.SetReference(0x44, value); }

        public HSDArrayAccessor<HSD_UInt> onUnknownItemRelated { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x48); set => _s.SetReference(0x48, value); }

        public HSDArrayAccessor<HSD_UInt> onUnknownCharacterModelFlags1 { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDArrayAccessor<HSD_UInt> onUnknownCharacterModelFlags2 { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x50); set => _s.SetReference(0x50, value); }

        public HSDArrayAccessor<HSD_UInt> onHit { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x54); set => _s.SetReference(0x54, value); }

        public HSDArrayAccessor<HSD_UInt> onUnknownEyeTextureRelated { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x58); set => _s.SetReference(0x58, value); }

        public HSDArrayAccessor<HSD_UInt> onFrame { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x5C); set => _s.SetReference(0x5C, value); }

        public HSDArrayAccessor<HSD_UInt> onActionStateChange { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x60); set => _s.SetReference(0x60, value); }

        public HSDArrayAccessor<HSD_UInt> onRespawn { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x64); set => _s.SetReference(0x64, value); }

        public HSDArrayAccessor<HSD_UInt> onModelRender { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x68); set => _s.SetReference(0x68, value); }

        public HSDArrayAccessor<HSD_UInt> onShadowRender { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x6C); set => _s.SetReference(0x6C, value); }

        public HSDArrayAccessor<HSD_UInt> onUnknownMultijump { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x70); set => _s.SetReference(0x70, value); }

        public HSDArrayAccessor<HSD_UInt> onActionStateChangeWhileEyeTextureIsChanged { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x74); set => _s.SetReference(0x74, value); }

        public HSDArrayAccessor<HSD_UInt> onTwoEntryTable { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x78); set => _s.SetReference(0x78, value); }

        // special function tables
        public HSDArrayAccessor<HSD_UInt> enterFloat { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x7C); set => _s.SetReference(0x7C, value); }

        public HSDArrayAccessor<HSD_UInt> enterSpecialDoubleJump { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x80); set => _s.SetReference(0x80, value); }

        public HSDArrayAccessor<HSD_UInt> enterTether { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x84); set => _s.SetReference(0x84, value); }

        public HSDArrayAccessor<HSD_UInt> onLand { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x88); set => _s.SetReference(0x88, value); }

        public HSDArrayAccessor<HSD_UInt> onSmashForward { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x8C); set => _s.SetReference(0x8C, value); }

        public HSDArrayAccessor<HSD_UInt> onSmashUp { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x90); set => _s.SetReference(0x90, value); }

        public HSDArrayAccessor<HSD_UInt> onSmashDown { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x94); set => _s.SetReference(0x94, value); }

        public HSDArrayAccessor<HSD_UInt> onExtRstAnim { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x98); set => _s.SetReference(0x98, value); }

        public HSDArrayAccessor<HSD_UInt> onIndexExtResultAnim { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0x9C); set => _s.SetReference(0x9C, value); }

        public HSDArrayAccessor<HSD_UInt> DemoMoveLogic { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0xA0); set => _s.SetReference(0xA0, value); }

        public HSDArrayAccessor<HSD_UInt> getTrailData { get => _s.GetReference<HSDArrayAccessor<HSD_UInt>>(0xA4); set => _s.SetReference(0xA4, value); }

    }
}
