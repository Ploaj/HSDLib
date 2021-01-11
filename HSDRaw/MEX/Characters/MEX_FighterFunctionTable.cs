namespace HSDRaw.MEX
{
    public class MEX_FighterFunctionTable : HSDAccessor
    {
        public override int TrimmedSize => 0x100;

        public HSDUIntArray OnLoad { get => _s.GetReference<HSDUIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public HSDUIntArray OnDeath { get => _s.GetReference<HSDUIntArray>(0x04); set => _s.SetReference(0x04, value); }

        public HSDUIntArray OnUnknown { get => _s.GetReference<HSDUIntArray>(0x08); set => _s.SetReference(0x08, value); }


        // can have references or pointers for move logic
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>> MoveLogic
        {
            get
            {
                return _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>>>(0x0C);
            }
            set => _s.SetReference(0x0C, value);
        }

        public HSDUIntArray MoveLogicPointers
        {
            get
            {
                return _s.GetReference<HSDUIntArray>(0x0C);
            }
            set => _s.SetReference(0x0C, value);
        }


        public HSDUIntArray SpecialN { get => _s.GetReference<HSDUIntArray>(0x10); set => _s.SetReference(0x10, value); }

        public HSDUIntArray SpecialNAir { get => _s.GetReference<HSDUIntArray>(0x14); set => _s.SetReference(0x14, value); }

        public HSDUIntArray SpecialS { get => _s.GetReference<HSDUIntArray>(0x18); set => _s.SetReference(0x18, value); }

        public HSDUIntArray SpecialSAir { get => _s.GetReference<HSDUIntArray>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDUIntArray SpecialHi { get => _s.GetReference<HSDUIntArray>(0x20); set => _s.SetReference(0x20, value); }

        public HSDUIntArray SpecialHiAir { get => _s.GetReference<HSDUIntArray>(0x24); set => _s.SetReference(0x24, value); }

        public HSDUIntArray SpecialLw { get => _s.GetReference<HSDUIntArray>(0x28); set => _s.SetReference(0x28, value); }

        public HSDUIntArray SpecialLwAir { get => _s.GetReference<HSDUIntArray>(0x2C); set => _s.SetReference(0x2C, value); }

        public HSDUIntArray OnAbsorb { get => _s.GetReference<HSDUIntArray>(0x30); set => _s.SetReference(0x30, value); }

        public HSDUIntArray onItemPickup { get => _s.GetReference<HSDUIntArray>(0x34); set => _s.SetReference(0x34, value); }

        public HSDUIntArray onMakeItemInvisible { get => _s.GetReference<HSDUIntArray>(0x38); set => _s.SetReference(0x38, value); }

        public HSDUIntArray onMakeItemVisible { get => _s.GetReference<HSDUIntArray>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDUIntArray onItemDrop { get => _s.GetReference<HSDUIntArray>(0x40); set => _s.SetReference(0x40, value); }

        public HSDUIntArray onItemCatch { get => _s.GetReference<HSDUIntArray>(0x44); set => _s.SetReference(0x44, value); }

        public HSDUIntArray onUnknownItemRelated { get => _s.GetReference<HSDUIntArray>(0x48); set => _s.SetReference(0x48, value); }

        public HSDUIntArray onUnknownCharacterModelFlags1 { get => _s.GetReference<HSDUIntArray>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDUIntArray onUnknownCharacterModelFlags2 { get => _s.GetReference<HSDUIntArray>(0x50); set => _s.SetReference(0x50, value); }

        public HSDUIntArray onHit { get => _s.GetReference<HSDUIntArray>(0x54); set => _s.SetReference(0x54, value); }

        public HSDUIntArray onUnknownEyeTextureRelated { get => _s.GetReference<HSDUIntArray>(0x58); set => _s.SetReference(0x58, value); }

        public HSDUIntArray onFrame { get => _s.GetReference<HSDUIntArray>(0x5C); set => _s.SetReference(0x5C, value); }

        public HSDUIntArray onActionStateChange { get => _s.GetReference<HSDUIntArray>(0x60); set => _s.SetReference(0x60, value); }

        public HSDUIntArray onRespawn { get => _s.GetReference<HSDUIntArray>(0x64); set => _s.SetReference(0x64, value); }

        public HSDUIntArray onModelRender { get => _s.GetReference<HSDUIntArray>(0x68); set => _s.SetReference(0x68, value); }

        public HSDUIntArray onShadowRender { get => _s.GetReference<HSDUIntArray>(0x6C); set => _s.SetReference(0x6C, value); }

        public HSDUIntArray onUnknownMultijump { get => _s.GetReference<HSDUIntArray>(0x70); set => _s.SetReference(0x70, value); }

        public HSDUIntArray onActionStateChangeWhileEyeTextureIsChanged { get => _s.GetReference<HSDUIntArray>(0x74); set => _s.SetReference(0x74, value); }

        public HSDUIntArray onTwoEntryTable { get => _s.GetReference<HSDUIntArray>(0x78); set => _s.SetReference(0x78, value); }

        // special function tables
        public HSDUIntArray enterFloat { get => _s.GetReference<HSDUIntArray>(0x7C); set => _s.SetReference(0x7C, value); }

        public HSDUIntArray enterSpecialDoubleJump { get => _s.GetReference<HSDUIntArray>(0x80); set => _s.SetReference(0x80, value); }

        public HSDUIntArray enterTether { get => _s.GetReference<HSDUIntArray>(0x84); set => _s.SetReference(0x84, value); }

        public HSDUIntArray onLand { get => _s.GetReference<HSDUIntArray>(0x88); set => _s.SetReference(0x88, value); }

        public HSDUIntArray onSmashForward { get => _s.GetReference<HSDUIntArray>(0x8C); set => _s.SetReference(0x8C, value); }

        public HSDUIntArray onSmashUp { get => _s.GetReference<HSDUIntArray>(0x90); set => _s.SetReference(0x90, value); }

        public HSDUIntArray onSmashDown { get => _s.GetReference<HSDUIntArray>(0x94); set => _s.SetReference(0x94, value); }

        public HSDUIntArray onExtRstAnim { get => _s.GetReference<HSDUIntArray>(0x98); set => _s.SetReference(0x98, value); }

        public HSDUIntArray onIndexExtResultAnim { get => _s.GetReference<HSDUIntArray>(0x9C); set => _s.SetReference(0x9C, value); }

        public HSDUIntArray DemoMoveLogic { get => _s.GetReference<HSDUIntArray>(0xA0); set => _s.SetReference(0xA0, value); }

        public HSDUIntArray onThrowFw { get => _s.GetReference<HSDUIntArray>(0xA4); set => _s.SetReference(0xA4, value); }

        public HSDUIntArray onThrowBk { get => _s.GetReference<HSDUIntArray>(0xA8); set => _s.SetReference(0xA8, value); }

        public HSDUIntArray onThrowHi { get => _s.GetReference<HSDUIntArray>(0xAC); set => _s.SetReference(0xAC, value); }

        public HSDUIntArray onThrowLw { get => _s.GetReference<HSDUIntArray>(0xB0); set => _s.SetReference(0xB0, value); }
        
        public HSDUIntArray getTrailData { get => _s.GetReference<HSDUIntArray>(0xB4); set => _s.SetReference(0xB4, value); }

        public override void New()
        {
            base.New();

            OnLoad = new HSDUIntArray();
            OnDeath = new HSDUIntArray();
            OnUnknown = new HSDUIntArray();
            MoveLogic = new HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<MEX_MoveLogic>>();
            SpecialN = new HSDUIntArray();
            SpecialNAir = new HSDUIntArray();
            SpecialHi = new HSDUIntArray();
            SpecialHiAir = new HSDUIntArray();
            SpecialLw = new HSDUIntArray();
            SpecialLwAir = new HSDUIntArray();
            SpecialS = new HSDUIntArray();
            SpecialSAir = new HSDUIntArray();
            OnAbsorb = new HSDUIntArray();
            onItemPickup = new HSDUIntArray();
            onMakeItemInvisible = new HSDUIntArray();
            onMakeItemVisible = new HSDUIntArray();
            onItemDrop = new HSDUIntArray();
            onItemCatch = new HSDUIntArray();
            onUnknownItemRelated = new HSDUIntArray();
            onUnknownCharacterModelFlags1 = new HSDUIntArray();
            onUnknownCharacterModelFlags2 = new HSDUIntArray();
            onHit = new HSDUIntArray();
            onUnknownEyeTextureRelated = new HSDUIntArray();
            onFrame = new HSDUIntArray();
            onActionStateChange = new HSDUIntArray();
            onRespawn = new HSDUIntArray();
            onModelRender = new HSDUIntArray();
            onShadowRender = new HSDUIntArray();
            onUnknownMultijump = new HSDUIntArray();
            onActionStateChangeWhileEyeTextureIsChanged = new HSDUIntArray();
            onTwoEntryTable = new HSDUIntArray();
            enterFloat = new HSDUIntArray();
            enterSpecialDoubleJump = new HSDUIntArray();
            enterTether = new HSDUIntArray();
            onLand = new HSDUIntArray();
            onSmashForward = new HSDUIntArray();
            onSmashUp = new HSDUIntArray();
            onSmashDown = new HSDUIntArray();
            onExtRstAnim = new HSDUIntArray();
            onIndexExtResultAnim = new HSDUIntArray();
            DemoMoveLogic = new HSDUIntArray();
            onThrowFw = new HSDUIntArray();
            onThrowBk = new HSDUIntArray();
            onThrowHi = new HSDUIntArray();
            onThrowLw = new HSDUIntArray();
            getTrailData = new HSDUIntArray();
        }
    }
}
