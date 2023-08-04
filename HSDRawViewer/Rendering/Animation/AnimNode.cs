using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class AnimNode
    {
        public List<FOBJ_Player> Tracks = new List<FOBJ_Player>();

        public void AddLinearKey(JointTrackType track, float frame, float value)
        {
            var player = Tracks.FirstOrDefault(e => e.JointTrackType == track);
            if (player == null)
            {
                player = new FOBJ_Player();
                player.TrackType = (byte)track;
                Tracks.Add(player);
            }

            player.Keys.Add(new FOBJKey()
            { 
                Frame = frame,
                Value = value,
                InterpolationType = GXInterpolationType.HSD_A_OP_LIN
            });
        }
    }
}
