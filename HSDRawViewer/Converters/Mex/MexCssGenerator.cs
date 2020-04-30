using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX.Menus;
using HSDRaw.Tools;
using HSDRawViewer.GUI.MEX;
using HSDRawViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Converters
{
    public class MexCssGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icons"></param>
        /// <returns></returns>
        public static Tuple<HSD_JOBJ, HSD_AnimJoint, HSD_MatAnimJoint> GenerateIconModel(MEX_CSSIconEntry[] icons)
        {
            HSD_JOBJ position_joint = new HSD_JOBJ();
            position_joint.Flags = JOBJ_FLAG.CLASSICAL_SCALING | JOBJ_FLAG.ROOT_XLU;
            position_joint.SX = 1; position_joint.SY = 1; position_joint.SZ = 1;

            HSD_AnimJoint anim_joint = new HSD_AnimJoint();

            HSD_MatAnimJoint matanim_joint = new HSD_MatAnimJoint();

            foreach (var ico in icons)
            {
                ico.Joint.Next = null;
                ico.AnimJoint.Next = null;
                ico.MatAnimJoint.Next = null;
                position_joint.AddChild(ico.Joint);
                anim_joint.AddChild(ico.AnimJoint);
                matanim_joint.AddChild(ico.MatAnimJoint);
            }

            return new Tuple<HSD_JOBJ, HSD_AnimJoint, HSD_MatAnimJoint>(position_joint, anim_joint, matanim_joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="mex"></param>
        public static void HookMexNode(SBM_SelectChrDataTable table, MEX_mexSelectChr mex)
        { 
            // hook csps
            table.MenuMaterialAnimation.Children[6].Children[0].MaterialAnimation = mex.CSPMatAnim;
            table.MenuMaterialAnimation.Children[6].Children[1].MaterialAnimation = mex.CSPMatAnim;
            table.MenuMaterialAnimation.Children[6].Children[2].MaterialAnimation = mex.CSPMatAnim;
            table.MenuMaterialAnimation.Children[6].Children[3].MaterialAnimation = mex.CSPMatAnim;
            table.SingleMenuMaterialAnimation.Children[6].Children[0].MaterialAnimation = mex.CSPMatAnim;
            table.PortraitMaterialAnimation.Children[2].Children[0].MaterialAnimation = mex.CSPMatAnim;

            // hook stock icons
            table.SingleMenuMaterialAnimation.Children[9].Children[0].MaterialAnimation = mex.StockMatAnim;
            table.SingleMenuMaterialAnimation.Children[9].Children[1].MaterialAnimation = mex.StockMatAnim;
            table.SingleMenuMaterialAnimation.Children[9].Children[2].MaterialAnimation = mex.StockMatAnim;
            table.SingleMenuMaterialAnimation.Children[9].Children[3].MaterialAnimation = mex.StockMatAnim;
            table.SingleMenuMaterialAnimation.Children[9].Children[4].MaterialAnimation = mex.StockMatAnim;

            // hook emblems
            table.MenuMaterialAnimation.Children[5].Children[0].MaterialAnimation.Next = mex.EmblemMatAnim;
            table.MenuMaterialAnimation.Children[5].Children[1].MaterialAnimation.Next = mex.EmblemMatAnim;
            table.MenuMaterialAnimation.Children[5].Children[2].MaterialAnimation.Next = mex.EmblemMatAnim;
            table.MenuMaterialAnimation.Children[5].Children[3].MaterialAnimation.Next = mex.EmblemMatAnim;
            table.SingleMenuMaterialAnimation.Children[5].Children[0].MaterialAnimation.Next = mex.EmblemMatAnim;
            table.PortraitMaterialAnimation.Children[1].Children[0].MaterialAnimation.Next = mex.EmblemMatAnim;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetMexNode(MEX_mexSelectChr mex, MEX_CSSIconEntry[] icons)
        {
            var posModel = GenerateIconModel(icons);
            mex.IconModel = posModel.Item1;
            mex.IconAnimJoint = posModel.Item2;
            mex.IconMatAnimJoint = posModel.Item3;

            // generate csp node
            var stride = MEXConverter.FighterIDCount - 3;
            List<FOBJKey> keys = new List<FOBJKey>();
            List<HSD_TOBJ> tobjs = new List<HSD_TOBJ>();
            foreach(var v in icons)
            {
                for(int i = 0; i < v.CSPs.Count; i++)
                {
                    var id = v.FighterExternalID;

                    if (id > 0x13)
                        id -= 1;

                    keys.Add(new FOBJKey() { Frame = stride * i + id, Value = tobjs.Count, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                    tobjs.Add(v.CSPs[i]);
                }
            }
            keys = keys.OrderBy(e => e.Frame).ToList();

            HSD_MatAnim anim = new HSD_MatAnim();
            anim.TextureAnimation = new HSD_TexAnim();
            anim.TextureAnimation.FromTOBJs(tobjs, false);
            anim.TextureAnimation.AnimationObject = new HSD_AOBJ();
            anim.TextureAnimation.AnimationObject.EndFrame = 1200;
            anim.TextureAnimation.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            anim.TextureAnimation.AnimationObject.FObjDesc.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TIMG);
            anim.TextureAnimation.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
            anim.TextureAnimation.AnimationObject.FObjDesc.Next.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TCLT);

            mex.CSPMatAnim = anim;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void LoadFromMEXNode(MEX_mexSelectChr mex, MEX_CSSIconEntry[] icons)
        {
            var csps = mex.CSPMatAnim.TextureAnimation.ToTOBJs();
            var cspKeys = mex.CSPMatAnim.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();

            var stride = MEXConverter.FighterIDCount - 3;

            var index = 0;
            foreach(var ico in icons)
            {
                // load joints
                if(index < mex.IconModel.Children.Length)
                {
                    ico.Joint = mex.IconModel.Children[index];
                    ico.Animation = MexMenuAnimationGenerator.FromAnimJoint(mex.IconAnimJoint.Children[index], ico.Joint);
                    ico.MatAnimJoint = mex.IconMatAnimJoint.Children[index];
                }

                // load csps
                ico.CSPs.Clear();
                int cspIndex = 0;
                while (true)
                {
                    var key = ico.FighterExternalID + (cspIndex * stride);
                    if (ico.FighterExternalID > 0x13)
                        key = ico.FighterExternalID + (cspIndex * stride) - 1;

                    var k = cspKeys.Find(e => e.Frame == key);

                    if (k == null)
                        break;

                    ico.CSPs.Add(csps[(int)k.Value]);

                    cspIndex++;
                }

                index++;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="icons"></param>
        /// <returns></returns>
        public static MEX_mexSelectChr GenerateMEXMapFromVanilla(SBM_SelectChrDataTable table, MEX_CSSIconEntry[] icons)
        {
            // generate icon model
            var icon_joint = HSDAccessor.DeepClone<HSD_JOBJ>(table.MenuModel.Children[2].Child);
            icon_joint.TX = 0;
            icon_joint.TY = 0;
            icon_joint.TZ = 0;
            icon_joint.Next = null;
            var center = RegenerateIcon(icon_joint);

            var icon_matanim_joint = HSDAccessor.DeepClone<HSD_MatAnimJoint>(table.MenuMaterialAnimation.Children[2].Child);
            icon_matanim_joint.Next = null;
            
            // generate mat anim node
            var joints = table.MenuModel.BreathFirstList;

            HSD_TOBJ[] tobjs = new HSD_TOBJ[icons.Max(e => e.FighterExternalID) + 1];

            JOBJManager m = new JOBJManager();
            m.SetJOBJ(table.MenuModel);
            m.SetAnimJoint(table.MenuAnimation);
            m.Frame = 600;
            m.UpdateNoRender();

            var csps = table.MenuMaterialAnimation.Children[6].Child.MaterialAnimation.TextureAnimation.ToTOBJs();
            var cspKeys = table.MenuMaterialAnimation.Children[6].Child.MaterialAnimation.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();

            var stride = 30;
            foreach (var ico in icons)
            {
                if (joints[ico.icon.JointID].Dobj == null)
                    continue;

                HSD_JOBJ pos = HSDAccessor.DeepClone<HSD_JOBJ>(icon_joint);
                pos.Dobj.Pobj.Attributes = icon_joint.Dobj.Pobj.Attributes;
                pos.Dobj.Next.Pobj.Attributes = icon_joint.Dobj.Pobj.Attributes;

                pos.Dobj.Next.Mobj.Textures = HSDAccessor.DeepClone<HSD_TOBJ>(joints[ico.icon.JointID].Dobj.Next.Mobj.Textures);

                var worldPosition = Vector3.TransformPosition(Vector3.Zero, m.GetWorldTransform(ico.icon.JointID));
                pos.TX = worldPosition.X + center.X;
                pos.TY = worldPosition.Y + center.Y;
                pos.TZ = worldPosition.Z + center.Z;
                
                ico.Joint = pos;
                ico.Animation = new MexMenuAnimation();
                ico.MatAnimJoint = HSDAccessor.DeepClone<HSD_MatAnimJoint>(icon_matanim_joint);

                // load csps
                // find key at stride
                ico.CSPs.Clear();
                int cspIndex = 0;
                while (true)
                {
                    var key = ico.FighterExternalID + (cspIndex * stride);
                    if (ico.FighterExternalID > 0x13)
                        key = ico.FighterExternalID + (cspIndex * stride) - 1;

                    var k = cspKeys.Find(e=>e.Frame == key);

                    if (k == null)
                        break;
                    
                    ico.CSPs.Add(csps[(int)k.Value]);

                    cspIndex++;
                }
            }

            m.RefreshRendering = true;
            
            MEX_mexSelectChr mex = new MEX_mexSelectChr();

            SetMexNode(mex, icons);
            
            mex.StockMatAnim = HSDAccessor.DeepClone<HSD_MatAnim>(table.SingleMenuMaterialAnimation.Children[9].Child.MaterialAnimation);
            mex.EmblemMatAnim = HSDAccessor.DeepClone<HSD_MatAnim>(table.MenuMaterialAnimation.Children[5].Child.MaterialAnimation.Next);
            
            return mex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootJOBJ"></param>
        private static Vector3 RegenerateIcon(HSD_JOBJ rootJOBJ)
        {
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var jobj in rootJOBJ.BreathFirstList)
                if (jobj.Dobj != null)
                    foreach (var dobj in jobj.Dobj.List)
                        if (dobj.Pobj != null)
                            foreach (var pobj in dobj.Pobj.List)
                                foreach (var v in pobj.ToDisplayList().Vertices)
                                {
                                    Min.X = Math.Min(Min.X, v.POS.X);
                                    Min.Y = Math.Min(Min.Y, v.POS.Y);
                                    Min.Z = Math.Min(Min.Z, v.POS.Z);
                                    Max.X = Math.Max(Max.X, v.POS.X);
                                    Max.Y = Math.Max(Max.Y, v.POS.Y);
                                    Max.Z = Math.Max(Max.Z, v.POS.Z);
                                }

            var center = (Min + Max) / 2;

            var compressor = new POBJ_Generator();
            foreach (var jobj in rootJOBJ.BreathFirstList)
            {
                if (jobj.Dobj != null)
                    foreach (var dobj in jobj.Dobj.List)
                    {
                        if (dobj.Pobj != null)
                        {
                            List<GX_Vertex> triList = new List<GX_Vertex>();

                            foreach (var pobj in dobj.Pobj.List)
                            {
                                var dl = pobj.ToDisplayList();
                                int off = 0;
                                foreach (var pri in dl.Primitives)
                                {
                                    var strip = dl.Vertices.GetRange(off, pri.Count);

                                    if (pri.PrimitiveType == GXPrimitiveType.TriangleStrip)
                                        TriangleConverter.StripToList(strip, out strip);

                                    if (pri.PrimitiveType == GXPrimitiveType.Quads)
                                        TriangleConverter.QuadToList(strip, out strip);

                                    off += pri.Count;

                                    for (int i = 0; i < strip.Count; i++)
                                    {
                                        var v = strip[i];

                                        v.POS.X -= center.X;
                                        v.POS.Y -= center.Y;
                                        v.POS.Z -= center.Z;

                                        strip[i] = v;
                                    }

                                    triList.AddRange(strip);
                                }
                            }

                            dobj.Pobj = compressor.CreatePOBJsFromTriangleList(triList, dobj.Pobj.Attributes.Select(e => e.AttributeName).ToArray(), null, null);
                        }
                    }
            }
            compressor.SaveChanges();

            center.X *= rootJOBJ.SX;
            center.Y *= rootJOBJ.SY;
            center.Z *= rootJOBJ.SZ;

            return center;
        }
    }
}
