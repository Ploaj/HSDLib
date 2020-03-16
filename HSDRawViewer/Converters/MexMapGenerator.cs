using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX.Stages;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Converters
{
    public enum MexMapAnimType
    {
        None,
        SlideInFromLeft,
        SlideInFromRight,
        SlideInFromTop,
        SlideInFromBottom,
        GrowFromNothing
    }

    public class MexMapSpace
    {
        public float X;
        public float Y;

        public HSD_TOBJ IconTexture;

        public MexMapAnimType AnimType = MexMapAnimType.None;
        public int SlideInFrame = 0;
    }

    public class MexMapGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="MapSpaces"></param>
        /// <returns></returns>
        public static MEX_mexMapData GenerateMexMap(SBM_MnSelectStageDataTable stage)
        {
            return GenerateMexMap(stage, GenerateSpacesFromDefault(stage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="MapSpaces"></param>
        public static MEX_mexMapData GenerateMexMap(SBM_MnSelectStageDataTable stage, IEnumerable<MexMapSpace> MapSpaces)
        {
            MEX_mexMapData mapData = new MEX_mexMapData();

            var texanim = new HSD_TexAnim();
            HSD_JOBJ root = new HSD_JOBJ()
            {
                SX = 1, SY = 1, SZ = 1,
                Flags = JOBJ_FLAG.CLASSICAL_SCALING
            };
            HSD_AnimJoint animRoot = new HSD_AnimJoint();
            List<FOBJKey> Keys = new List<FOBJKey>();

            {
                var tobjs = stage.IconMaterialAnimation.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
                var index = texanim.AddImage(tobjs[0]);
                Keys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                index = texanim.AddImage(tobjs[1]);
                Keys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            }

            foreach (var v in MapSpaces)
            {
                var index = texanim.AddImage(v.IconTexture);
                Keys.Add(new FOBJKey() { Frame = index, Value = index, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                root.AddChild(new HSD_JOBJ()
                {
                    TX = v.X,
                    TY = v.Y,
                    SX = 1,
                    SY = 1,
                    SZ = 1,
                    Flags = JOBJ_FLAG.CLASSICAL_SCALING
                });
                animRoot.AddChild(new HSD_AnimJoint());
            }
            Keys.Add(new FOBJKey() { Frame = 1600, Value = 0, InterpolationType = GXInterpolationType.HSD_A_OP_CON });

            texanim.GXTexMapID = HSDRaw.GX.GXTexMapID.GX_TEXMAP0;
            texanim.AnimationObject = new HSD_AOBJ();
            texanim.AnimationObject.EndFrame = 1600;
            texanim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            texanim.AnimationObject.FObjDesc.SetKeys(Keys, (byte)TexTrackType.HSD_A_T_TIMG);
            texanim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
            texanim.AnimationObject.FObjDesc.Next.SetKeys(Keys, (byte)TexTrackType.HSD_A_T_TCLT);

            var iconJOBJ = HSDAccessor.DeepClone<HSD_JOBJ>(stage.Icon3Model);
            iconJOBJ.Child = iconJOBJ.Child.Next;
            var iconAnimJoint = HSDAccessor.DeepClone<HSD_AnimJoint>(stage.Icon3Animation);
            iconAnimJoint.Child = iconAnimJoint.Child.Next;
            var iconMatAnimJoint = HSDAccessor.DeepClone<HSD_MatAnimJoint>(stage.Icon3MaterialAnimation);
            iconMatAnimJoint.Child = iconMatAnimJoint.Child.Next;
            iconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation = texanim;

            mapData.IconModel = iconJOBJ;
            mapData.IconAnimJoint = iconAnimJoint;
            mapData.IconMatAnimJoint = iconMatAnimJoint;
            mapData.PositionModel = root;
            mapData.PositionAnimJoint = animRoot;

            return mapData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        private static List<MexMapSpace> GenerateSpacesFromDefault(SBM_MnSelectStageDataTable stage)
        {
            List<MexMapSpace> spaces = new List<MexMapSpace>();

            var tex0 = stage.Icon3MaterialAnimation.Child.Next.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tex0_extra = stage.Icon3MaterialAnimation.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tex1 = stage.Icon2MaterialAnimation.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();
            var tex2 = stage.IconMaterialAnimation.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();

            var g1 = tex0.Length - 2;
            var g2 = tex0.Length - 2 + tex1.Length - 2;
            var g3 = tex0.Length - 2 + tex1.Length - 2 + 1;

            for (int i = 0; i < stage.PositionModel.Children.Length; i++)
            {
                HSD_TOBJ tobj = null;
                var anim = stage.PositionAnimation.Children[i].AOBJ.FObjDesc.GetDecodedKeys();
                var Y = stage.PositionModel.Children[i].TY;

                if (i >= g3)
                {
                    tobj = tex2[i - g3 + 2];
                }
                else
                if (i >= g2)
                    tobj = null; // RandomIcon
                else
                if (i >= g1)
                    tobj = tex1[i - g1 + 2];
                else
                {
                    tobj = tex0[i + 2];
                    spaces.Add(new MexMapSpace()
                    {
                        X = anim[anim.Count - 1].Value,
                        Y = Y,
                        IconTexture = tobj
                    });
                    Y -= 5.6f;
                    tobj = tex0_extra[i + 2];
                }

                spaces.Add(new MexMapSpace()
                {
                    X = anim[anim.Count - 1].Value,
                    Y = Y,
                    IconTexture = tobj
                });
            }
            return spaces;
        }
        
    }
}
