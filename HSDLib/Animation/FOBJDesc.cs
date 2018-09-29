namespace HSDLib.Animation
{
    public class FOBJDesc : IHSDList<FOBJDesc>
    {
        [FieldData(typeof(FOBJDesc))]
        public override FOBJDesc Next { get; set; }
        
        [FieldData(typeof(uint))]
        public uint DataLength { get; set; }

        [FieldData(typeof(uint))]
        public uint StartFrame { get; set; }
        
        [FieldData(typeof(HSD_FOBJ), true, 0x8)]
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
