using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_FtDemoSymbolNames : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public string Result
        {
            get => ResultS?.Value;
            set
            {
                if (ResultS == null)
                    ResultS = new HSD_String();
                ResultS.Value = value;
            }
        }

        public string Intro
        {
            get => IntroS?.Value;
            set
            {
                if (IntroS == null)
                    IntroS = new HSD_String();
                IntroS.Value = value;
            }
        }

        public string Ending
        {
            get => EndingS?.Value;
            set
            {
                if (EndingS == null)
                    EndingS = new HSD_String();
                EndingS.Value = value;
            }
        }

        public string ViWait
        {
            get => ViWaitS?.Value;
            set
            {
                if (ViWaitS == null)
                    ViWaitS = new HSD_String();
                ViWaitS.Value = value;
            }
        }

        private HSD_String ResultS { get => _s.GetReference<HSD_String>(0x00); set => _s.SetReference(0x00, value); }

        private HSD_String IntroS { get => _s.GetReference<HSD_String>(0x04); set => _s.SetReference(0x04, value); }

        private HSD_String EndingS { get => _s.GetReference<HSD_String>(0x08); set => _s.SetReference(0x08, value); }

        private HSD_String ViWaitS { get => _s.GetReference<HSD_String>(0x0C); set => _s.SetReference(0x0C, value); }
    }
}
