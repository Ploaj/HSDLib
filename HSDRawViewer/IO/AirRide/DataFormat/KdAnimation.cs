using HSDRaw.Common.Animation;
using HSDRawViewer.GUI.PropertyGrid;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdAnimation
    {
        [Browsable(false)]
        public HSD_AnimJoint Animation { get; set; }

        public float FrameCount { get => Animation.AOBJ.EndFrame; }

        [TypeConverter(typeof(ListConverter<string>))]
        public List<string> TrackList { get => Animation.AOBJ.FObjDesc.List.Select(e => e.JointTrackType.ToString()).ToList(); }

        public KdAnimation(HSD_AnimJoint animation)
        {
            Animation = animation;
        }
    }
}
