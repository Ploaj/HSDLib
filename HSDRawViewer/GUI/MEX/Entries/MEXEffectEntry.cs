namespace HSDRawViewer.GUI.MEX
{
    public class MEXEffectEntry
    {
        public string FileName { get; set; }

        public string Symbol { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(FileName) ? "-" : FileName;
        }
    }
}
