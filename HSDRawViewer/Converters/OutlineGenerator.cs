using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HSDRawViewer.GUI.Dialog;

namespace HSDRawViewer.Converters
{
    public class OutlineSettings
    {
        [DisplayName("Outline Thickness"), Description("Thickness of the outline")]
        public float Size { get; set; } = 0.0375f;

        [DisplayName("Use Triangle Strips"), Description("Slower to generate, but better optimized for in-game")]
        public bool UseStrips { get; set; } = true;

        [DisplayName("Outline Color"), Description("Color of Outline")]
        public Color Color { get; set; } = Color.Black;
    }

    public class OutlineGenerator
    {

        public static HSD_DOBJ GenerateOutlineMesh(HSD_DOBJ DOBJ)
        {
            var settings = new OutlineSettings();

            using (PropertyDialog d = new PropertyDialog("Outline Settings", settings))
            {
                if (d.ShowDialog() != DialogResult.OK)
                    return null;
            }

            var pobjGen = new POBJ_Generator();
            pobjGen.UseTriangleStrips = settings.UseStrips;

            var newDOBJ = new HSD_DOBJ();
            newDOBJ.Mobj = new HSD_MOBJ()
            {
                Material = new HSD_Material()
                {
                    AmbientColor = Color.White,
                    SpecularColor = Color.Black,
                    DiffuseColor = settings.Color,
                    DIF_A = 255,
                    SPC_A = 255,
                    AMB_A = 255,
                    Shininess = 50,
                    Alpha = 1
                },
                RenderFlags = RENDER_MODE.CONSTANT
            };

            foreach (var pobj in DOBJ.Pobj.List)
            {
                var dl = pobj.ToDisplayList();

                var vertices = dl.Vertices;

                GXAttribName[] attrs = new GXAttribName[]
                {
                        GXAttribName.GX_VA_POS,
                        GXAttribName.GX_VA_NULL
                };

                if(pobj.HasAttribute(GXAttribName.GX_VA_PNMTXIDX))
                {
                    attrs = new GXAttribName[]
                    {
                        GXAttribName.GX_VA_PNMTXIDX,
                        GXAttribName.GX_VA_POS,
                        GXAttribName.GX_VA_NULL
                    };
                }

                List<GX_Vertex> newVerties = new List<GX_Vertex>();

                var offset = 0;
                foreach (var prim in dl.Primitives)
                {
                    var verts = vertices.GetRange(offset, prim.Count);
                    offset += prim.Count;

                    switch (prim.PrimitiveType)
                    {
                        case GXPrimitiveType.Quads:
                            verts = TriangleConverter.QuadToList(verts);
                            break;
                        case GXPrimitiveType.TriangleStrip:
                            verts = TriangleConverter.StripToList(verts);
                            break;
                        case GXPrimitiveType.Triangles:
                            break;
                        default:
                            Console.WriteLine(prim.PrimitiveType);
                            break;
                    }

                    newVerties.AddRange(verts);
                }

                // extrude
                for (int i = 0; i < newVerties.Count; i++)
                {
                    var v = newVerties[i];
                    v.POS.X += v.NRM.X * settings.Size;
                    v.POS.Y += v.NRM.Y * settings.Size;
                    v.POS.Z += v.NRM.Z * settings.Size;
                    //v.CLR0.R = settings.Color.R / 255f;
                    //v.CLR0.G = settings.Color.G / 255f;
                    //v.CLR0.B = settings.Color.B / 255f;
                    //v.CLR0.A = settings.Color.A / 255f;
                    newVerties[i] = v;
                }

                // invert faces
                for (int i = 0; i < newVerties.Count; i += 3)
                {
                    var temp = newVerties[i];
                    newVerties[i] = newVerties[i + 2];
                    newVerties[i + 2] = temp;
                }

                var newpobj = pobjGen.CreatePOBJsFromTriangleList(newVerties, attrs, dl.Envelopes);
                foreach (var p in newpobj.List)
                    p.Flags |= POBJ_FLAG.CULLBACK | POBJ_FLAG.SHAPESET_ADDITIVE;
                if (newDOBJ.Pobj == null)
                    newDOBJ.Pobj = newpobj;
                else
                    newDOBJ.Pobj.Add(newpobj);
            }

            pobjGen.SaveChanges();

            return newDOBJ;
        }

    }
}
