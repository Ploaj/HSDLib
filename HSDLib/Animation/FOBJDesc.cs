namespace HSDLib.Animation
{
    public class FOBJDesc : IHSDList<FOBJDesc>
    {
        public override FOBJDesc Next { get; set; }
        
        public uint DataLength { get; set; }
        
        public uint StartFrame { get; set; }
        
        [HSDInLine()]
        public HSD_FOBJ Track { get; set; }
        
        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            Track.Data = Reader.ReadBuffer(Track.DataOffset, (int)DataLength);
        }

        public override void Save(HSDWriter Writer)
        {
            base.Save(Writer);

            Writer.WriteAt((int)(Writer.BaseStream.Position - 16), (uint)Track.Data.Length);
        }
    }
}
