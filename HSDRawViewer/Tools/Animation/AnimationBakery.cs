using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using System.ComponentModel;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw;

namespace HSDRawViewer.Tools.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public class AnimationBakery
    {
        public bool Optimize { get; set; } = true;

        public bool ApplyDiscontinutyFilter { get; set; } = true;

        public float OptimizeError { get; set; } = 0.01f;

        public float CompressionError { get; set; } = 0.01f;

        public uint TrimStartFrame { get; set; } = 0;

        public uint TrimEndFrame { get; set; } = 0;

        [Description("Desired length to stretch animation to. Use -1 if not desired. This is processed after trim frames.")]
        public int FitLength { get; set; } = -1;

        [DisplayName("Dummy Frame Gen")]
        [Description("Number of dummy frames to append to the start of the animation")]
        public int DummyFrameCount { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public void Bake(JointAnimManager anim, HSD_JOBJ model)
        {
            // bake animation
            anim.Bake();

            // trim frame range
            anim.Trim((int)TrimStartFrame, (int)TrimEndFrame);

            // append dummy frames
            for (int i = 0; i < DummyFrameCount; i++)
            {
                foreach (var n in anim.Nodes)
                {
                    foreach (var t in n.Tracks)
                    {
                        foreach (var k in t.Keys)
                        {
                            k.Frame++;
                        }
                        t.Keys.Insert(0, new FOBJKey()
                        {
                            Frame = 0,
                            Value = t.Keys[0].Value,
                            Tan = t.Keys[0].Tan,
                            InterpolationType = t.Keys[0].InterpolationType,
                        });
                    }
                }
            }

            // fit frame total
            if (FitLength > 0)
            {
                anim.ApplyFSMs(new FrameSpeedMultiplier[] {
                            new()
                                {
                                    Frame = 0,
                                    Rate = (TrimEndFrame - TrimStartFrame) / (float)FitLength
                                }}, false);
            }

            // optimize
            if (Optimize)
                anim.Optimize(model,
                    ApplyDiscontinutyFilter,
                    OptimizeError);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="model"></param>
        /// <param name="symbol"></param>
        public HSD_FigaTree BakeToFigatree(JointAnimManager anim, HSD_JOBJ model)
        {
            Bake(anim, model);
            return anim.ToFigaTree(CompressionError);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="model"></param>
        /// <param name="symbol"></param>
        public HSDRawFile BakeToFigatreeFile(JointAnimManager anim, HSD_JOBJ model, string symbol)
        {
            HSDRawFile file = new();
            file.Roots.Add(new HSDRootNode()
            {
                Name = symbol,
                Data = BakeToFigatree(anim, model),
            });
            return file;
        }
    }
}
