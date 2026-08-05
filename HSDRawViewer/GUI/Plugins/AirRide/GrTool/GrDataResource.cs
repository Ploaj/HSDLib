using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public class GrDataResource
    {
        private KdFile _data;

        public ObservableList<KdMesh> Meshes = new ObservableList<KdMesh>();

        public ObservableList<KdZone> Zones = new ObservableList<KdZone>();

        public void Load(KAR_grData d)
        {
            var kd = new KdFile(d);
            _data = kd;

            foreach (var m in kd.Collisions)
                Meshes.Add(m);

            foreach (var m in kd.Zones)
                Zones.Add(m);
        }

        internal void Save(LiveJObj jobj, KAR_grData d)
        {
            _data.Collisions = Meshes.ToList();
            _data.Zones = Zones.ToList();

            _data.ImportIntoNode(jobj, d);
        }
    }
}
