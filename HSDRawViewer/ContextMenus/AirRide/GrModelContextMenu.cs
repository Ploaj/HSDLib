using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRawViewer.Converters;
using HSDRawViewer.Tools.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.AirRide
{
    public class GrModelContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(KAR_grMainModel) };

        /// <summary>
        /// 
        /// </summary>
        public GrModelContextMenu()
        {
            ToolStripMenuItem genPages = new ToolStripMenuItem("Recalculate Model Bounding");
            genPages.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grMainModel data)
                {
                    data.ModelBounding = CalculateModelBounding(data.RootNode);
                }
            };
            Items.Add(genPages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private static KAR_grModelBounding CalculateModelBounding(HSD_JOBJ jobj)
        {
            ModelExporter exp = new ModelExporter(
                jobj, 
                new ModelExportSettings()
                { 
                    ExportTextures = false,
                }, 
                new JointMap());

            List<KAR_grStaticBoundingBox> staticBoxes = new List<KAR_grStaticBoundingBox>();
            List<KAR_grDynamicBoundingBoxes> dynamicBoxes = new List<KAR_grDynamicBoundingBoxes>();
            List<ushort> staticIndices = new List<ushort>();

            // calculate mesh bounding
            int meshIndex = 0;
            foreach (var m in exp.Scene.Models[0].Meshes)
            {
                float minx = float.MaxValue, miny = float.MaxValue, minz = float.MaxValue;
                float maxx = float.MinValue, maxy = float.MinValue, maxz = float.MinValue;
                foreach (var v in m.Vertices)
                {
                    minx = Math.Min(minx, v.Position.X);
                    miny = Math.Min(miny, v.Position.Y);
                    minz = Math.Min(minz, v.Position.Z);
                    maxx = Math.Max(maxx, v.Position.X);
                    maxy = Math.Max(maxy, v.Position.Y);
                    maxz = Math.Max(maxz, v.Position.Z);
                }

                if (m.Vertices.Count == 0)
                    minx = maxx = miny = maxy = minz = maxz = 0;

                staticBoxes.Add(new KAR_grStaticBoundingBox()
                {
                    MinX = minx,
                    MinY = miny,
                    MinZ = minz,
                    MaxX = maxx,
                    MaxY = maxy,
                    MaxZ = maxz
                });

                // TODO: dynamic (mesh with rigging)
                var boneIndex = -1;

                if (boneIndex == -1)
                {
                    staticIndices.Add((ushort)meshIndex);
                }
                else
                {
                    dynamicBoxes.Add(new KAR_grDynamicBoundingBoxes()
                    {
                        BoneIndex = boneIndex,
                        Indices = new ushort[] { (ushort)meshIndex },
                        MinX = minx,
                        MinY = miny,
                        MinZ = minz,
                        MaxX = maxx,
                        MaxY = maxy,
                        MaxZ = maxz
                    });
                }

                meshIndex++;
            }

            // calculate view regions
            // TODO: generate view regions
            KAR_grViewRegion region = new KAR_grViewRegion();
            region.MinX = -10000;
            region.MinY = -10000;
            region.MinZ = -10000;
            region.MaxX = 10000;
            region.MaxY = 10000;
            region.MaxZ = 10000;
            region.Indices = staticIndices.ToArray();

            // update bounding struct
            var bound = new KAR_grModelBounding();
            bound.ViewRegions = new KAR_grViewRegion[]
            {
                region
            };
            bound.StaticBoundingBoxes = staticBoxes.ToArray();
            bound.DynamicBoundingBoxes = dynamicBoxes.ToArray();

            return bound;
        }
    }
}
