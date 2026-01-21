using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace HSDRawViewer.IO.Model
{
    public enum HsdInterpolation
    {
        CON,
        LIN,
        SPL,
    }

    public class HsdAnimKey
    {
        [JsonPropertyName("i")]
        public HsdInterpolation I { get; set; }

        [JsonPropertyName("f")]
        public float F { get; set; }

        [JsonPropertyName("v")]
        public float V { get; set; }

        [JsonPropertyName("in")]
        public float In { get; set; }

        [JsonPropertyName("out")]
        public float Out { get; set; }
    }

    public class HsdAnimTrack
    {
        public int Type { get; set; }

        [JsonConverter(typeof(JsonInlineListConverter<HsdAnimKey>))]
        public List<HsdAnimKey> Keys { get; set; } = new List<HsdAnimKey>();

        [JsonIgnore]
        public float FrameMax => Keys.Select(b => b.F).DefaultIfEmpty(0).Max();

        public HsdAnimTrack() { }

        public HsdAnimTrack(HSD_FOBJ fobj)
        {
            Type = fobj.TrackType;
            var keys = fobj.GetDecodedKeys();

            float current_slope = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                switch (k.InterpolationType)
                {
                    case GXInterpolationType.HSD_A_OP_CON:
                    case GXInterpolationType.HSD_A_OP_KEY: // key is when there is only a single keyframe
                        Keys.Add(new HsdAnimKey()
                        {
                            F = k.Frame,
                            V = k.Value,
                            I = HsdInterpolation.CON,
                        });
                        break;
                    case GXInterpolationType.HSD_A_OP_LIN:
                        Keys.Add(new HsdAnimKey()
                        {
                            F = k.Frame,
                            V = k.Value,
                            I = HsdInterpolation.LIN,
                        });
                        break;
                    case GXInterpolationType.HSD_A_OP_SPL:
                        Keys.Add(new HsdAnimKey()
                        {
                            F = k.Frame,
                            V = k.Value,
                            I = HsdInterpolation.SPL,
                            In = k.Tan,
                            Out = k.Tan,
                        });
                        current_slope = k.Tan;
                        break;
                    case GXInterpolationType.HSD_A_OP_SPL0:
                        Keys.Add(new HsdAnimKey()
                        {
                            F = k.Frame,
                            V = k.Value,
                            I = HsdInterpolation.SPL,
                            In = 0,
                            Out = 0,
                        });
                        current_slope = 0;
                        break;
                    case GXInterpolationType.HSD_A_OP_SLP: // modifies the previous key's out and the next frames in
                        //Keys[Keys.Count - 1].Out = k.Tan;
                        //current_slope = k.Tan;
                        break;
                }
            }

            // pre/post infinity
            if (Keys.Count > 0)
                Keys[0].In = Keys[0].Out;
            if (Keys.Count > 1)
                Keys[Keys.Count - 1].Out = Keys[Keys.Count - 1].In;
        }

        public HSD_FOBJ ToFObj()
        {
            List<FOBJKey> keys = new List<FOBJKey>();

            for (int i = 0; i < Keys.Count; i++)
            {
                var k = Keys[i];
                var key = new FOBJKey()
                {
                    Frame = k.F,
                    Value = k.V,
                };

                switch (k.I)
                {
                    case HsdInterpolation.CON:
                        key.InterpolationType = GXInterpolationType.HSD_A_OP_CON;
                        break;
                    case HsdInterpolation.LIN:
                        key.InterpolationType = GXInterpolationType.HSD_A_OP_LIN;
                        break;
                    case HsdInterpolation.SPL:

                        if (k.In == 0 && k.Out == 0)
                        {
                            key.InterpolationType = GXInterpolationType.HSD_A_OP_SPL0;
                        }
                        else if (k.In == k.Out || i == Keys.Count - 1 || i == 0)
                        {
                            key.Tan = k.In;
                            key.InterpolationType = GXInterpolationType.HSD_A_OP_SPL;
                        }
                        else
                        {
                            // go ahead and store this key with the in slope
                            key.InterpolationType = GXInterpolationType.HSD_A_OP_SPL;
                            key.Tan = k.In;
                            keys.Add(key);

                            // change key to store out slope
                            key = new FOBJKey()
                            {
                                Tan = k.Out,
                                InterpolationType = GXInterpolationType.HSD_A_OP_SLP,
                            };
                        }

                        break;
                }

                keys.Add(key);
            }

            if (keys.Count == 1)
                keys[0].InterpolationType = GXInterpolationType.HSD_A_OP_KEY;

            var fobj = new HSD_FOBJ();
            fobj.SetKeys(keys, (JointTrackType)Type);
            return fobj;
        }
    }
}
