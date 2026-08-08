using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrPartitionNode : GrDrawNode
    {
        public override bool HasTransform => false;

        private HashSet<int> zones { get; set; } = new HashSet<int>();



        [TypeConverter(typeof(ExpandableObjectConverter))]
        private KAR_grPartitionBucket _bucket { get; set; }

        public GrPartitionNode(int index, KAR_grPartitionBucket bucket, KAR_grPartitionBucket[] buckets, ushort[] zones) 
        {
            Text = $"{index:D3}_{bucket.CollTriangleCount}";
            Tag = this;
            _bucket = bucket;

            for (int i = bucket.ZoneIndexStart; i < bucket.ZoneIndexStart + bucket.ZoneIndexCount; i++)
            {
                this.zones.Add(zones[i]);
            }

            if (bucket.Child1 != -1)
            {
                Nodes.Add(new GrPartitionNode(bucket.Child1, buckets[bucket.Child1], buckets, zones));
            }

            if (bucket.Child2 != -1)
            {
                Nodes.Add(new GrPartitionNode(bucket.Child2, buckets[bucket.Child2], buckets, zones));
            }
        }

        public bool ContainsZone(int zone)
        {
            return zones.Contains(zone);
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.PositiveInfinity;
            return false;
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            return null;
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (selected_object == this)
            {
                DrawShape.DrawBox(Color.White, _bucket.MinX, _bucket.MinY, _bucket.MinZ, _bucket.MaxX, _bucket.MaxY, _bucket.MaxZ);
            }
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
        }
    }
}
