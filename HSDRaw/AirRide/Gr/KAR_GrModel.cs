namespace HSDRaw.AirRide.Gr
{
    public class KAR_grModel : HSDAccessor
    {
        public override int TrimmedSize => 0x10;
        
        public KAR_grMainModel MainModel { get => _s.GetReference<KAR_grMainModel>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grSkyBoxModel SkyboxModel { get => _s.GetReference<KAR_grSkyBoxModel>(0x04); set => _s.SetReference(0x04, value); }

        //public KAR_GrUnk2Model Unk2Model { get; set; }

        //public KAR_GrUnk3Model Unk3Model { get; set; }
    }
}
