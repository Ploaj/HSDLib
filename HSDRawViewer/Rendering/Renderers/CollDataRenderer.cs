using HSDRaw.Melee.Gr;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering.Renderers
{
    public class CollDataRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collData"></param>
        public static void RenderColl_Data(SBM_Coll_Data collData)
        {
            var vertices = collData.Vertices;
            var links = collData.Links;
            foreach (var region in collData.AreaTables)
            {
                RenderCollRegion(vertices, links, region.BottomLinkIndex, region.BottomLinkCount);
                RenderCollRegion(vertices, links, region.LeftLinkIndex, region.LeftLinkCount);
                RenderCollRegion(vertices, links, region.RightLinkIndex, region.RightLinkCount);
                RenderCollRegion(vertices, links, region.TopLinkIndex, region.TopLinkCount);
            }
        }

        private static Dictionary<CollMaterial, Vector3> materialToColor = new Dictionary<CollMaterial, Vector3>()
        {
            { CollMaterial.Basic, new Vector3(0x80 / 255f, 0x80 / 255f, 0x80 / 255f) },
            { CollMaterial.Rock, new Vector3(0x80 / 255f, 0x60 / 255f, 0x60 / 255f) },
            { CollMaterial.Grass, new Vector3(0x40 / 255f, 0xff / 255f, 0x40 / 255f) },
            { CollMaterial.Dirt, new Vector3(0xc0 / 255f, 0x60 / 255f, 0x60 / 255f) },
            { CollMaterial.Wood, new Vector3(0xC0 / 255f, 0x80 / 255f, 0x40 / 255f) },
            { CollMaterial.HeavyMetal, new Vector3(0x60 / 255f, 0x40 / 255f, 0x40 / 255f) },
            { CollMaterial.LightMetal, new Vector3(0x40 / 255f, 0x40 / 255f, 0x40 / 255f) },
            { CollMaterial.UnkFlatZone, new Vector3(0xC0 / 255f, 0xC0 / 255f, 0xC0 / 255f) },
            { CollMaterial.AlienGoop, new Vector3(0xDF / 255f, 0x8F / 255f, 0x7F / 255f) },
            { CollMaterial.Water, new Vector3(0x30 / 255f, 0x30 / 255f, 0xFF / 255f) },
            { CollMaterial.Glass, new Vector3(0xC0 / 255f, 0xC0 / 255f, 0xFF / 255f) },
            { CollMaterial.Checkered, new Vector3(0xFF / 255f, 0xFF / 255f, 0xC0 / 255f) },
            { CollMaterial.FlatZone, new Vector3(0xC0 / 255f, 0xC0 / 255f, 0xC0 / 255f) },
        };

        private static void RenderCollRegion(SBM_CollVertex[] vertices, SBM_CollLink[] links, int start, int count)
        {
            GL.Begin(PrimitiveType.Quads);

            for (int i = start; i < start + count; i++)
            {
                var link = links[i];
                var vert1 = vertices[link.VertexIndex1];
                var vert2 = vertices[link.VertexIndex2];

                GL.Color3(materialToColor[(CollMaterial)link.Material]);
                GL.Vertex3(vert1.X, vert1.Y, 10);
                GL.Vertex3(vert2.X, vert2.Y, 10);
                GL.Vertex3(vert2.X, vert2.Y, -10);
                GL.Vertex3(vert1.X, vert1.Y, -10);
            }

            GL.End();
        }

    }
}
