using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Converters.Melee;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Converters.SBM
{
    public class SSFModelGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static HSD_JOBJ GenerateModel(SSFGroup group)
        {
            var jobj = new HSD_JOBJ()
            {
                Flags = JOBJ_FLAG.CLASSICAL_SCALING,
                SX = 1,
                SY = 1,
                SZ = 1
            };

            if (group.Lines.Count == 0)
                return jobj;

            var attributes = new GXAttribName[]
            {
                GXAttribName.GX_VA_POS,
                GXAttribName.GX_VA_NRM,
                GXAttribName.GX_VA_NULL
            };

            var triangleList = new List<GX_Vertex>();

            foreach (var l in group.Lines)
            {
                var v1 = group.Vertices[l.Vertex1];
                var v2 = group.Vertices[l.Vertex2];
                var nrm = new OpenTK.Mathematics.Vector3(v1.X - v2.X, v1.Y - v2.Y, 0).Normalized();
                var normal = new GXVector3(nrm.Y, -nrm.X, nrm.Z);

                triangleList.Add(new GX_Vertex() { POS = new GXVector3(v1.X, v1.Y, 5), NRM = normal });
                triangleList.Add(new GX_Vertex() { POS = new GXVector3(v1.X, v1.Y, -5), NRM = normal });
                triangleList.Add(new GX_Vertex() { POS = new GXVector3(v2.X, v2.Y, 5), NRM = normal });

                triangleList.Add(new GX_Vertex() { POS = new GXVector3(v2.X, v2.Y, -5), NRM = normal });
                triangleList.Add(new GX_Vertex() { POS = new GXVector3(v2.X, v2.Y, 5), NRM = normal });
                triangleList.Add(new GX_Vertex() { POS = new GXVector3(v1.X, v1.Y, -5), NRM = normal });
            }

            var gen = new HSDRaw.Tools.POBJ_Generator();
            jobj.Dobj = new HSD_DOBJ()
            {
                Mobj = new HSD_MOBJ()
                {
                    RenderFlags = RENDER_MODE.DIFFUSE,
                    Material = new HSD_Material()
                    {
                        Alpha = 1,
                        AmbientColor = System.Drawing.Color.Gray,
                        DiffuseColor = System.Drawing.Color.Gray,
                        SpecularColor = System.Drawing.Color.White,
                    }
                },
                Pobj = gen.CreatePOBJsFromTriangleList(triangleList, attributes, null)
            };
            gen.SaveChanges();

            Console.WriteLine(gen.CreatePOBJsFromTriangleList(triangleList, attributes, null).Attributes);


            return jobj;
        }
    }
}
