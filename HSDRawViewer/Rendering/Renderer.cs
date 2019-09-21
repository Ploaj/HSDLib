using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Gr;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// A special renderer that attempts to translate the GX rendering from JOBJ into the OpenGL equivalent
    /// </summary>
    public class Renderer
    {
        private HSDAccessor currentAccessor;

        private RendererJOBJ JOBJRenderer = new RendererJOBJ();

        
        /// <summary>
        /// Clears all rendering cache
        /// </summary>
        public void ClearCache()
        {
            JOBJRenderer.ClearCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetAnimJoint(HSD_AnimJoint joint)
        {
            JOBJRenderer.SetAnimJoint(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            JOBJRenderer.SetFigaTree(tree);
        }

        /// <summary>
        /// Renders supported Accessors
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="accessor"></param>
        public void Render(Matrix4 Camera, HSDAccessor accessor)
        {
            if(currentAccessor != accessor)
            {
                ClearCache();
                currentAccessor = accessor;
            }

            if (accessor is HSD_JOBJ jobj)
                JOBJRenderer.Render(jobj);
            else
            if (accessor is SBM_Coll_Data coll)
                RenderColl_Data(coll);

            NextFrame();
        }

        public void NextFrame()
        {
            if (JOBJRenderer.Frame < JOBJRenderer.FrameCount)
                JOBJRenderer.Frame++;
            if (JOBJRenderer.Frame >= JOBJRenderer.FrameCount)
                JOBJRenderer.Frame = 0;
        }

        public void RenderColl_Data(SBM_Coll_Data collData)
        {
            var vertices = collData.Vertices;
            var links = collData.Links;
            foreach(var region in collData.AreaTables)
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

        private void RenderCollRegion(SBM_CollVertex[] vertices, SBM_CollLink[] links, int start, int count)
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
