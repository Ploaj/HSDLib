using System;
using System.Collections.Generic;

namespace HSDRaw.Common
{
    /// <summary>
    /// Envelopes contain weights to JOBJs references by displaylists PNDMTXID
    /// </summary>
    public class HSD_Envelope : HSDAccessor
    {
        public int EnvelopeCount { get { return (_s.Length / 8) - 1; } }

        public float[] Weights
        {
            get
            {
                float[] weights = new float[EnvelopeCount];
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = GetWeightAt(i);
                }
                return weights;
            }
        }

        public HSD_JOBJ[] JOBJs
        {
            get
            {
                HSD_JOBJ[] weights = new HSD_JOBJ[EnvelopeCount];
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = GetJOBJAt(i);
                }
                return weights;
            }
        }


        public Tuple<HSD_JOBJ, float> this[int i]
        {
            get
            {
                return new Tuple<HSD_JOBJ, float>(GetJOBJAt(i), GetWeightAt(i));
            }
        }

        public HSD_JOBJ GetJOBJAt(int index)
        {
            return _s.GetReference<HSD_JOBJ>(8 * index);
        }

        public float GetWeightAt(int index)
        {
            return _s.GetFloat(8 * index + 4);
        }

        public List<Tuple<HSD_JOBJ, float>> ToList()
        {
            List<Tuple<HSD_JOBJ, float>> tl = new List<Tuple<HSD_JOBJ, float>>();

            for (int i = 0; i < EnvelopeCount; i++)
                tl.Add(this[i]);

            return tl;
        }

        public void SetJOBJAt(int index, HSD_JOBJ jobj)
        {
            _s.SetReference(8 * index, jobj);
        }

        public void SetWeightAt(int index, float weight)
        {
            _s.SetFloat(8 * index + 4, weight);
        }

        public void Add(HSD_JOBJ jobj, float weight)
        {
            // create new if null
            if (_s == null)
                _s = new HSDStruct(8);

            // resize struct
            _s.Resize(_s.Length + 8);

            if (_s.Length == 8)
                _s.Resize(_s.Length + 8);

            // set at last position
            int position = ((_s.Length - 8) / 8) - 1;

            SetJOBJAt(position, jobj);
            SetWeightAt(position, weight);
        }

        public void Remove(int index)
        {
            if (_s.Length == 0 || index < 0 || index > EnvelopeCount)
                return;

            SetJOBJAt(index, null);
            SetWeightAt(index, 0);

            for (int i = index; i < EnvelopeCount; i++)
            {
                SetJOBJAt(i, GetJOBJAt(i + 1));
                SetWeightAt(i, GetWeightAt(i + 1));
            }

            SetJOBJAt(EnvelopeCount + 1, null);
            SetWeightAt(EnvelopeCount + 1, 0);

            _s.Resize(_s.Length - 8);
        }
    }

}
