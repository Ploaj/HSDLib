using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace HSDRawViewer.Converters
{
    public class EmblemModel
    {
        public List<float[]> v = new List<float[]>();
        public List<int[]> f = new List<int[]>();

        public Vector2 xRange;
        public Vector2 yRange;

        public float aspectX;
        public float aspectY;
    }
    class EmblemConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objFileStream"></param>
        /// <param name="XRange"></param>
        /// <param name="YRange"></param>
        /// <param name="aspectX"></param>
        /// <param name="aspectY"></param>
        public static EmblemModel GenerateEmblemModelFromOBJ(Stream objFileStream)
        {
             var v = new List<float[]>();
             var f = new List<int[]>();

            float MAXX = float.MinValue, MINX = float.MaxValue;
            float MAXY = float.MinValue, MINY = float.MaxValue;

            using (StreamReader r = new StreamReader(objFileStream))
            {
                objFileStream.Position = 0;
                while (!r.EndOfStream)
                {
                    var a = System.Text.RegularExpressions.Regex.Replace(r.ReadLine(), @"\s+", " ").Trim().Split(' ');

                    if (a[0] == "v" && a.Length >= 4)
                    {
                        float f1, f2, f3;
                        if (float.TryParse(a[1], out f1) && float.TryParse(a[2], out f2) && float.TryParse(a[3], out f3))
                        {
                            MAXX = Math.Max(MAXX, f1);
                            MAXY = Math.Max(MAXY, f3);
                            MINX = Math.Min(MINX, f1);
                            MINY = Math.Min(MINY, f3);
                            v.Add(new float[] { f1, f2, f3 });
                        }
                    }
                    if (a[0] == "f" && a.Length >= 4)
                    {
                        int f1, f2, f3;
                        if (int.TryParse(a[1].Split('/')[0], out f1) && int.TryParse(a[2].Split('/')[0], out f2) && int.TryParse(a[3].Split('/')[0], out f3))
                            f.Add(new int[] { f3, f2, f1 });
                    }
                }

                float aspectX = Math.Abs(MAXX - MINX) / Math.Abs(MAXY - MINY);
                float aspectY = Math.Abs(MAXY - MINY) / Math.Abs(MAXX - MINX);

                if (aspectX < 1)
                    aspectY = 1;

                if (aspectY < 1)
                    aspectX = 1;
                
                return new EmblemModel()
                {
                    v = v,
                    f = f,
                    aspectX = aspectX,
                    aspectY = aspectY,
                    xRange = new Vector2(MINX, MAXX),
                    yRange = new Vector2(MINY, MAXY)
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objFileStream"></param>
        /// <returns></returns>
        public static HSD_JOBJ GenerateEmblemModel(EmblemModel model)
        {
            List<GX_Vertex> vertexList = new List<GX_Vertex>();
            foreach (var ve in model.f)
            {
                foreach(var i in ve)
                {
                    var x = ((model.v[i - 1][0] - model.xRange.X) / Math.Abs(model.xRange.Y - model.xRange.X)) * model.aspectX;
                    var y = (1 - (model.v[i - 1][2] - model.yRange.X) / Math.Abs(model.xRange.Y - model.yRange.X)) * model.aspectY;

                    vertexList.Add(new GX_Vertex()
                    {
                        POS = new GXVector3(x * 6 - 3, y * 6 - 3, 0)
                    });
                }
            }

            HSD_JOBJ jobj = new HSD_JOBJ();
            jobj.Flags = JOBJ_FLAG.CLASSICAL_SCALING | JOBJ_FLAG.XLU;
            jobj.SX = 3;
            jobj.SY = 3;
            jobj.SZ = 3;
            jobj.TZ = 67.4f;

            HSD_DOBJ dobj = new HSD_DOBJ();
            dobj.Mobj = new HSD_MOBJ()
            {
                RenderFlags = RENDER_MODE.ALPHA_COMPAT | RENDER_MODE.DIFFUSE_MAT | RENDER_MODE.DF_NONE | RENDER_MODE.XLU,
                Material = new HSD_Material()
                {
                    Alpha = 0.6f,
                    Shininess = 50,
                    DiffuseColor = Color.FromArgb(255, 128, 128, 230),
                    SpecularColor = Color.FromArgb(255, 255, 255, 255),
                    AmbientColor = Color.FromArgb(255, 128, 128, 128),
                }
            };
            jobj.Dobj = dobj;

            POBJ_Generator pobjGen = new POBJ_Generator();
            GXAttribName[] attrs = new GXAttribName[] { GXAttribName.GX_VA_POS, GXAttribName.GX_VA_NULL };
            dobj.Pobj = pobjGen.CreatePOBJsFromTriangleList(vertexList, attrs, null);
            dobj.Pobj.Flags = 0;
            pobjGen.SaveChanges();
            
            return jobj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objFileStream"></param>
        /// <returns></returns>
        public static HSD_TOBJ GenerateEmblemIconImage(EmblemModel model)
        {
            // normalize range
            foreach (var ve in model.v)
            {
                ve[0] = ((ve[0] - model.xRange.X) / Math.Abs(model.xRange.Y - model.xRange.X)) * model.aspectX;
                ve[2] = (1 - (ve[2] - model.yRange.X) / Math.Abs(model.xRange.Y - model.yRange.X)) * model.aspectY;
            }

            using (Bitmap b = new Bitmap(80, 64))
            {
                var brushback = new SolidBrush(Color.Black);
                var brush = new SolidBrush(Color.White);

                int icoWidth = 64, icoHeight = 64;

                if (model.aspectX < 1)
                {
                    icoWidth = b.Height;
                    icoHeight = b.Height;
                }
                if (model.aspectY < 1)
                {
                    icoWidth = b.Width;
                    icoHeight = b.Width;
                }

                icoWidth -= icoWidth / 9;
                icoHeight -= icoHeight / 9;

                int xoff = b.Width / 2 - icoWidth / 2;
                int yoff = b.Height / 2 + icoHeight / 2;

                using (Graphics g = Graphics.FromImage(b))
                {
                    g.FillRectangle(brushback, 0, 0, b.Width, b.Height);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    GraphicsPath path = new GraphicsPath();
                    foreach (var ve in model.f)
                    {
                        path.AddPolygon(new PointF[] {
                            new PointF(xoff + model.v[ve[0] - 1][0] * icoWidth , yoff - model.v[ve[0] - 1][2] * icoHeight ),
                            new PointF(xoff + model.v[ve[1] - 1][0] * icoWidth , yoff - model.v[ve[1] - 1][2] * icoHeight ),
                            new PointF(xoff + model.v[ve[2] - 1][0] * icoWidth , yoff - model.v[ve[2] - 1][2] * icoHeight ) });
                    }
                    g.FillPath(brush, path);
                }

                return TOBJConverter.BitmapToTOBJ(b, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.IA8);
            }
        }
    }
}
