using HSDRaw.Tools.Melee;

namespace HSDRaw.Melee
{
    /// <summary>
    /// 
    /// </summary>
    public class SBM_SISData : HSDAccessor
    {
        public HSDAccessor ImageData { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor CharacterSpacingParams { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        // everything else is op codes for drawing

        public SIS_Data[] SISData
        {
            get
            {
                SIS_Data[] data = new SIS_Data[Length - 2];
                for(int i = 2; i < Length; i++)
                    data[i - 2] = GetTextData(i);
                return data;
            }
            set
            {
                _s.Resize(8);
                for(int i = 0; i < value.Length; i++)
                    SetTextData(i + 2, value[i]);
            }
        }

        public int Length { get => _s.Length / 4; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SIS_Data GetTextData(int index)
        {
            if (index == 0 || index == 1)
                return null;

            return _s.GetReference<SIS_Data>(index * 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        public void SetTextData(int index, SIS_Data data)
        {
            if (index == 0 || index == 1)
                return;

            if (index >= Length)
                _s.Resize((index + 1) * 4);

            _s.SetReference(index * 4, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public int AddTextData(SIS_Data data)
        {
            if (Length < 2)
                _s.Resize(8);

            _s.Resize((Length + 1) * 4);

            _s.SetReference((Length - 1) * 4, data);

            return Length - 1;
        }

        /// <summary>
        /// No need to align the text data
        /// </summary>
        public override void SetStructFlags()
        {
            for (int i = 8; i < _s.Length; i += 4)
                if (_s.GetReference<HSDAccessor>(i) != null)
                    _s.GetReference<HSDAccessor>(i)._s.Align = false;

            base.SetStructFlags();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SIS_Data : HSDAccessor
    {
        public string TextCode
        {
            get
            {
                return MeleeMenuText.DeserializeString(_s.GetData());
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _s.SetData(new byte[4]);
                else
                {
                    MeleeMenuText txt = new MeleeMenuText();

                    if (txt.FromString(value))
                        _s.SetData(txt.Data);
                }
            }
        }
    }
}
