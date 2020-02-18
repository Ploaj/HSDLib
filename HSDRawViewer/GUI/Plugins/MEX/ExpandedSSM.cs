namespace HSDRawViewer.GUI.Plugins.MEX
{
    public class MEX_EffectEntry
    {
        public string FileName { get; set; }

        public string Symbol { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(FileName) ? "-" : FileName;
        }
    }

    public class ExpandedSSM
    {
        public string Name { get; set; } = "file.ssm";

        public int Flag { get; set; } = 0;

        public override string ToString()
        {
            return Name;
        }
    }
}
