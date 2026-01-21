using HSDRaw.Common.Animation;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Linq;

namespace HSDRawViewer.IO.Model
{
    public class HsdAnim
    {
        public List<HsdAnimJoint> Bones { get; set; } = new List<HsdAnimJoint>();

        [JsonIgnore]
        public float FrameMax => Bones.Select(b => b.FrameMax).DefaultIfEmpty(0).Max();

        public HsdAnim() { }

        public static HsdAnim FromFigatree(HSD_FigaTree tree)
        {
            HsdAnim a = new HsdAnim();

            foreach (var n in tree.Nodes)
            {
                var j = new HsdAnimJoint();
                // no parent information from tree, but that's fine

                foreach (var t in n.Tracks)
                {
                    j.Tracks.Add(new HsdAnimTrack(t.ToFOBJ()));
                }

                a.Bones.Add(j);
            }

            return a;
        }

        public static HSD_FigaTree ToFigaTree(HsdAnim anim)
        {
            HSD_FigaTree ft = new HSD_FigaTree()
            {
                Type = 1,
                FrameCount = anim.FrameMax,
            };

            List<FigaTreeNode> nodes = new List<FigaTreeNode>();
            foreach (var b in anim.Bones)
            {
                var n = new FigaTreeNode();
                n.Tracks = b.Tracks.Select(e =>
                {
                    var t = new HSD_Track();
                    t.FromFOBJ(e.ToFObj());
                    return t;
                }).ToList();
                nodes.Add(n);
            }

            ft.Nodes = nodes;
            return ft;
        }
    }
}
