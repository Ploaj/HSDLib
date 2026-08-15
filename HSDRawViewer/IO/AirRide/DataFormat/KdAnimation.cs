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

        public float FrameCount { get => Animation.AOBJ != null ? Animation.AOBJ.EndFrame : 0; }

        [TypeConverter(typeof(ListConverter<string>))]
        public List<string> TrackList 
        { 
            get
            {
                if (Animation == null) return null;
                if (Animation.AOBJ == null) return null;
                if (Animation.AOBJ.FObjDesc == null) return null;

                return Animation.AOBJ.FObjDesc.List.Select(e => e.JointTrackType.ToString()).ToList();
            }
        }

        public KdAnimation()
        {
            Animation = new HSD_AnimJoint();
        }
        
        public KdAnimation(HSD_AnimJoint animation)
        {
            Animation = animation;
        }
    }
}
