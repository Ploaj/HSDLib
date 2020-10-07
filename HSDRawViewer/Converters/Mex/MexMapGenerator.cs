using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX.Stages;
using HSDRaw.Tools;
using HSDRawViewer.GUI.MEX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.Converters
{
    public class MexMapGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="icons"></param>
        public static MEX_mexMapData GenerateMexMap(SBM_MnSelectStageDataTable stage, IEnumerable<MEXStageIconEntry> icons)
        {
            MEX_mexMapData mapData = new MEX_mexMapData();
            
            var nameTexAnim = new HSD_TexAnim();
            var nameKeys = new List<FOBJKey>();

            var iconTexAnim = new HSD_TexAnim();
            var iconKeys = new List<FOBJKey>();

            HSD_JOBJ root = new HSD_JOBJ()
            {
                SX = 1, SY = 1, SZ = 1,
                Flags = JOBJ_FLAG.CLASSICAL_SCALING
            };

            HSD_AnimJoint animRoot = new HSD_AnimJoint();

            // extra
            {
                var tobjs = stage.IconLargeMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();

                var index = iconTexAnim.AddImage(tobjs[0]);
                iconKeys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });

                index = iconTexAnim.AddImage(tobjs[1]);
                iconKeys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            }
            
            foreach (var v in icons)
            {
                var index = iconTexAnim.AddImage(v.IconTOBJ);
                if (index == -1)
                    index = 0;
                iconKeys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });

                index = nameTexAnim.AddImage(v.NameTOBJ);
                if (index == -1)
                    index = 0;
                nameKeys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });

                v.Joint.Next = null;
                v.Joint.Child = null;
                v.AnimJoint.Next = null;
                v.AnimJoint.Child = null;

                root.AddChild(v.Joint);
                animRoot.AddChild(v.AnimJoint);
            }

            iconKeys.Add(new FOBJKey() { Frame = 1600, Value = 0, InterpolationType = GXInterpolationType.HSD_A_OP_CON });

            iconTexAnim.GXTexMapID = HSDRaw.GX.GXTexMapID.GX_TEXMAP0;
            iconTexAnim.AnimationObject = new HSD_AOBJ();
            iconTexAnim.AnimationObject.EndFrame = 1600;
            iconTexAnim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            iconTexAnim.AnimationObject.FObjDesc.SetKeys(iconKeys, (byte)TexTrackType.HSD_A_T_TIMG);
            iconTexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
            iconTexAnim.AnimationObject.FObjDesc.Next.SetKeys(iconKeys, (byte)TexTrackType.HSD_A_T_TCLT);

            var iconJOBJ = HSDAccessor.DeepClone<HSD_JOBJ>(stage.IconDoubleModel);
            iconJOBJ.Child = iconJOBJ.Child.Next;

            var iconAnimJoint = HSDAccessor.DeepClone<HSD_AnimJoint>(stage.IconDoubleAnimJoint);
            iconAnimJoint.Child = iconAnimJoint.Child.Next;

            var iconMatAnimJoint = HSDAccessor.DeepClone<HSD_MatAnimJoint>(stage.IconDoubleMatAnimJoint);
            iconMatAnimJoint.Child = iconMatAnimJoint.Child.Next;
            iconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation = iconTexAnim;

            var iconNameAnim = HSDAccessor.DeepClone<HSD_MatAnimJoint>(stage.StageNameMatAnimJoint);
            nameTexAnim.AnimationObject = new HSD_AOBJ();
            nameTexAnim.AnimationObject.EndFrame = 1600;
            nameTexAnim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            nameTexAnim.AnimationObject.FObjDesc.SetKeys(nameKeys, (byte)TexTrackType.HSD_A_T_TIMG);
            iconNameAnim.Child.Child.MaterialAnimation.TextureAnimation = nameTexAnim;

            mapData.IconModel = iconJOBJ;
            mapData.IconAnimJoint = iconAnimJoint;
            mapData.IconMatAnimJoint = iconMatAnimJoint;
            mapData.PositionModel = root;
            mapData.PositionAnimJoint = animRoot;
            mapData.StageNameMaterialAnimation = iconNameAnim;

            return mapData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void LoadIconDataFromSymbol(MEX_mexMapData data, MEXStageIconEntry[] icons)
        {
            var tobjs = data.IconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tobjanim = data.IconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();

            var nametobjs = data.StageNameMaterialAnimation.Child.Child.MaterialAnimation.TextureAnimation.ToTOBJs();
            var nametobjanim = data.StageNameMaterialAnimation.Child.Child.MaterialAnimation.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();

            for (int i = 0; i < data.PositionModel.Children.Length; i++)
            {
                icons[i].Joint = data.PositionModel.Children[i];
                icons[i].Animation = MexMenuAnimationGenerator.FromAnimJoint(data.PositionAnimJoint.Children[i], icons[i].Joint);
                icons[i].IconTOBJ = tobjs[(int)tobjanim[i + 2].Value];
                icons[i].NameTOBJ = nametobjs[(int)nametobjanim[i].Value];
            }
        }

        private static Dictionary<int, int> unswizzle = new Dictionary<int, int>()
        {
            { 13, 11},
            { 14, 12},
            { 15, 13},
            { 16, 14},
            { 17, 15},

            { 18, 16},

            { 11, 17},
            { 12, 18},
        };

        private static Dictionary<int, int> texunswizzle = new Dictionary<int, int>()
        {
            { 0, 0},
            { 1, 1},
            { 2, 2},
            { 3, 4},
            { 4, 5},
            { 5, 6},
            { 6, 3},
            { 7, 9},
            { 8, 8},
            { 9, 7},
            { 10, 10},
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public static void LoadIconDataFromVanilla(SBM_MnSelectStageDataTable stage, MEXStageIconEntry[] icons)
        {
            var tex0 = stage.IconDoubleMatAnimJoint.Child.Next.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tex0_extra = stage.IconDoubleMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tex1 = stage.IconLargeMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tex2 = stage.IconSpecialMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();

            var nameTOBJs = stage.StageNameMatAnimJoint.Child.Child.MaterialAnimation.TextureAnimation.ToTOBJs();
            var nameTOBJsAnim = stage.StageNameMatAnimJoint.Child.Child.MaterialAnimation.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();

            var g1 = tex0.Length - 2;
            var g2 = tex0.Length - 2 + tex1.Length - 2;
            var g3 = tex0.Length - 2 + tex1.Length - 2 + tex2.Length - 2;

            int icoIndex = 0;
            for (int i = 0; i < stage.PositionModel.Children.Length; i++)
            {
                var childIndex = i;
                if (unswizzle.ContainsKey(i))
                    childIndex = unswizzle[i];

                HSD_TOBJ icon = null;
                HSD_TOBJ name = null;
                var anim = stage.PositionAnimation.Children[childIndex].AOBJ.FObjDesc;
                var keys = anim.GetDecodedKeys();
                var Y = stage.PositionModel.Children[childIndex].TY;
                var Z = stage.PositionModel.Children[childIndex].TZ;
                var SX = 1f;
                var SY = 1f;
                int nameTex = i;

                if (i >= g3)
                {
                    //RandomIcon
                    name = nameTOBJs[(int)nameTOBJsAnim[nameTOBJsAnim.Count - 1].Value];
                }
                else
                if (i >= g2)
                {
                    name = nameTOBJs[(int)nameTOBJsAnim[24 + (i - g2)].Value];
                    icon = tex2[i - g2 + 2];
                    SX = 0.8f;
                    SY = 0.8f;
                }
                else
                if (i >= g1)
                {
                    name = nameTOBJs[(int)nameTOBJsAnim[22 + texunswizzle[i - g1]].Value];
                    icon = tex1[i - g1 + 2];
                    SY = 1.1f;
                }
                else
                {
                    icon = tex0[texunswizzle[i] + 2];
                    name = nameTOBJs[(int)nameTOBJsAnim[texunswizzle[i]].Value * 2];

                    icons[icoIndex].X = keys[keys.Count - 1].Value;
                    icons[icoIndex].Y = Y;
                    icons[icoIndex].Z = Z;
                    icons[icoIndex].Joint.SX = SX;
                    icons[icoIndex].Joint.SY = SY;
                    icons[icoIndex].IconTOBJ = icon;
                    icons[icoIndex].NameTOBJ = name;
                    icons[icoIndex].Animation = MexMenuAnimationGenerator.FromAnimJoint(stage.PositionAnimation.Children[childIndex], icons[icoIndex].Joint);
                    icoIndex++;

                    Y -= 5.6f;
                    Z = 0;
                    icon = tex0_extra[texunswizzle[i] + 2];
                    name = nameTOBJs[(int)nameTOBJsAnim[texunswizzle[i]].Value * 2 + 1];
                }
                
                icons[icoIndex].X = keys[keys.Count - 1].Value;
                icons[icoIndex].Y = Y;
                icons[icoIndex].Z = Z;
                icons[icoIndex].Joint.SX = SX;
                icons[icoIndex].Joint.SY = SY;
                icons[icoIndex].IconTOBJ = icon;
                icons[icoIndex].NameTOBJ = name;
                icons[icoIndex].Animation = MexMenuAnimationGenerator.FromAnimJoint(stage.PositionAnimation.Children[childIndex], icons[icoIndex].Joint);
                icoIndex++;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Bitmap GenerateStageName(string name, string location)
        {
            /*using (PrivateFontCollection coll = new PrivateFontCollection())
            {
                // new Font(ApplicationSettings.FolkProFont, 30, FontStyle.Italic);
                //new Font(ApplicationSettings.Palatino, 15);
            }*/
            Font font = null;
            Font font2 = null;
            using (InstalledFontCollection installedFontCollection = new InstalledFontCollection())
            {
                font = new Font(installedFontCollection.Families.FirstOrDefault(e => e.Name.Equals("A-OTF Folk Pro H")), 30, FontStyle.Italic);
                font2 = new Font(installedFontCollection.Families.FirstOrDefault(e=>e.Name.Equals("Palatino Linotype")), 13, FontStyle.Bold);
            }

            if(font == null || font2 == null)
                return null;

            Bitmap bmp = new Bitmap(224, 56);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                {
                    g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, bmp.Width, bmp.Height));
                }
                {
                    SizeF stringSize = TextRenderer.MeasureText(location, font2);

                    var stringWidth = stringSize.Width;
                    var scale = 0.95f;
                    var newwidth = stringWidth * scale;

                    g.ResetTransform();
                    g.ScaleTransform(scale, 1);
                    g.DrawString(location, font2, new SolidBrush(Color.White), (224 / 2) / scale, -4, stringFormat);
                }
                {
                    SizeF stringSize = TextRenderer.MeasureText(name, font);

                    var stringWidth = stringSize.Width;
                    var scale = 0.75f;
                    var newwidth = stringWidth * scale;
                    if (newwidth > bmp.Width)
                    {
                        scale = (bmp.Width * 0.95f) / stringWidth;
                        newwidth = stringWidth * scale;
                    }

                    //System.Console.WriteLine(name + " " + stringWidth + " " + scale + " " + stringSize.ToString());

                    g.ResetTransform();
                    g.ScaleTransform(scale, 1);
                    g.DrawString(name, font, new SolidBrush(Color.White), (224 / 2) / scale, 3, stringFormat);
                }
            }

            font.Dispose();
            font2.Dispose();
            stringFormat.Dispose();

            return bmp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Bitmap GenerateMenuOptionImage(string name)
        {
            float fontSize = 19f;

            Font font = null;
            using (InstalledFontCollection installedFontCollection = new InstalledFontCollection())
                font = new Font(installedFontCollection.Families.FirstOrDefault(e => e.Name.Equals("A-OTF Folk Pro H")), fontSize, FontStyle.Regular);

            if (font == null)
                return null;

            Bitmap bmp = new Bitmap(176, 30);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.Bilinear;

                {
                    g.FillRectangle(new SolidBrush(Color.Transparent), new RectangleF(0, 0, bmp.Width, bmp.Height));
                }
                {
                    var scale = 1.025f;
                    SizeF stringSize = TextRenderer.MeasureText(name, font);

                    var stringWidth = stringSize.Width;
                    var newwidth = stringWidth * scale;
                    if (newwidth > bmp.Width )
                    {
                        scale = bmp.Width  / stringWidth;
                    }

                    //System.Console.WriteLine(name + " " + stringWidth + " " + scale + " " + stringSize.ToString() + " " + font.FontFamily.GetEmHeight(font.Style));


                    g.ResetTransform();
                    g.ScaleTransform(scale, 1);

                    GraphicsPath p = new GraphicsPath();
                    var em = g.DpiY * font.SizeInPoints / 72f;
                    p.AddString(name, font.FontFamily, (int)font.Style, em, new PointF((176 / 2) / scale, -8), stringFormat);
                    g.DrawPath(new Pen(Color.Black, 4), p);
                    g.FillPath(new SolidBrush(Color.White), p);
                }
            }

            font.Dispose();
            stringFormat.Dispose();

            return bmp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Bitmap GenerateMenuHeaderImage(string name)
        {
            float fontSize = 19f;

            Font font = null;
            using (InstalledFontCollection installedFontCollection = new InstalledFontCollection())
                font = new Font(installedFontCollection.Families.FirstOrDefault(e => e.Name.Equals("A-OTF Folk Pro H")), fontSize, FontStyle.Italic);

            if (font == null)
                return null;

            Bitmap bmp = new Bitmap(168, 28);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.Bilinear;

                {
                    g.FillRectangle(new SolidBrush(Color.Transparent), new RectangleF(0, 0, bmp.Width, bmp.Height));
                }
                {
                    var scale = 1f;
                    SizeF stringSize = TextRenderer.MeasureText(name, font);

                    var stringWidth = stringSize.Width;
                    var newwidth = stringWidth * scale;
                    if (newwidth > bmp.Width)
                    {
                        scale = bmp.Width / stringWidth;
                    }

                    //System.Console.WriteLine(name + " " + stringWidth + " " + scale + " " + stringSize.ToString() + " " + font.FontFamily.GetEmHeight(font.Style));


                    g.ResetTransform();
                    g.ScaleTransform(scale, 1);

                    GraphicsPath p = new GraphicsPath();
                    var em = g.DpiY * font.SizeInPoints / 72f;
                    p.AddString(name, font.FontFamily, (int)font.Style, em, new PointF((168 / 2) / scale, -8), stringFormat);
                    g.FillPath(new SolidBrush(Color.White), p);
                }
            }

            font.Dispose();
            stringFormat.Dispose();

            return bmp;
        }
    }
}
