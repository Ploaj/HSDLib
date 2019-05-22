namespace HSDLib.Animation
{
    public class HSD_Track : IHSDNode
    {
        public ushort DataSize { get; set; }
        
        public ushort Padding1 { get; set; }

        [HSDInLine()]
        public HSD_FOBJ Track { get; set; }
        
        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            
            Track.Data = Reader.ReadBuffer(Track.DataOffset, DataSize);
        }

        public override void Save(HSDWriter Writer)
        {
            base.Save(Writer);

            int start = (int)Writer.BaseStream.Position;
            Writer.WriteAt(start, (uint)Track.Data.Length << 16);
        }
    }
}
