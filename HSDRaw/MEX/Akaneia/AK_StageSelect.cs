using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX.Stages;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw.MEX.Akaneia
{
    public class AK_StagePages : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<AK_StagePage> Pages { get => _s.GetReference<HSDArrayAccessor<AK_StagePage>>(0x04); set => _s.SetReference(0x04, value); }

        //private static HSD_AnimJoint GenerateLeaveAnimation(HSD_AnimJoint original)
        //{
        //    var anim = HSDAccessor.DeepClone<HSD_AnimJoint>(original);

        //    foreach (var j in anim.TreeList)
        //    {
        //        if (j.AOBJ == null)
        //            continue;

        //        j.AOBJ.EndFrame = 20;

        //        foreach (var t in j.AOBJ.FObjDesc.List)
        //        {
        //            if (t.JointTrackType != JointTrackType.HSD_A_J_TRAX)
        //                continue;

        //            var keys = t.GetDecodedKeys();

        //            float start_value = keys[0].Value;
        //            float start_position = keys[keys.Count - 1].Value;
        //            float end_position = -36;

        //            foreach (var k in keys)
        //            {
        //                if (k.Value == start_value)
        //                {
        //                    k.Value = start_position;
        //                }
        //                else
        //                {
        //                    k.Value = end_position;
        //                }
        //            }

        //            t.SetKeys(keys, t.TrackType);
        //        }
        //    }

        //    return anim;
        //}


        private static HSD_FOBJ GenerateKeyAnim(JointTrackType type, float value)
        {
            var keys = new List<FOBJKey>();
            keys.Add(new FOBJKey()
            {
                Frame = 0,
                InterpolationType = GXInterpolationType.HSD_A_OP_KEY,
                Value = value
            });
            
            var fobj = new HSD_FOBJ();
            fobj.SetKeys(keys, type);

            return fobj;
        }

        private static HSD_AnimJoint GenerateScaleAnimJoint(float x, float y)
        {
            HSD_AnimJoint anim = new HSD_AnimJoint();
            anim.AOBJ = new HSD_AOBJ();

            var fobj1 = GenerateKeyAnim(JointTrackType.HSD_A_J_SCAX, x);
            var fobj2 = GenerateKeyAnim(JointTrackType.HSD_A_J_SCAY, y);

            anim.AOBJ.FObjDesc = HSD_FOBJDesc.FromFOBJs(new HSD_FOBJ[] { fobj1, fobj2 });

            return anim;
        }

        private static HSD_MatAnimJoint GenerateMatAnim(HSD_TOBJ tobj)
        {
            var matAnim = new HSD_MatAnimJoint();
            var child = new HSD_MatAnimJoint();
            matAnim.Child = child;

            child.MaterialAnimation = new HSD_MatAnim();

            child.MaterialAnimation.Next = new HSD_MatAnim()
            {
                TextureAnimation = new HSD_TexAnim()
            };

            child.MaterialAnimation.Next.TextureAnimation.FromTOBJs(new List<HSD_TOBJ>() { tobj } , true);

            return matAnim;
        }

        private static void TrimAnimation(HSD_AOBJ aobj, float start, float length)
        {
            aobj.EndFrame = length - 1;
            var fobjs = aobj.FObjDesc.List;
            HSD_FOBJDesc prev = null;

            foreach (var r in fobjs)
            {
                var keys = r.GetDecodedKeys();
                keys.RemoveAll(e => e.Frame < start || e.Frame >= start + length);
                foreach (var k in keys)
                    k.Frame -= start;

                if (keys.Count == 0)
                {
                    if (prev == null)
                        aobj.FObjDesc = r.Next;
                    else
                        prev.Next = r.Next;
                    continue;
                }

                FOBJ_Player player = new FOBJ_Player(r);

                if (keys[0].Frame != 0)
                {
                    keys.Insert(0, new FOBJKey()
                    {
                        Frame = 0,
                        Value = player.GetValue(start),
                        InterpolationType = GXInterpolationType.HSD_A_OP_CON
                    });
                }

                if (keys.Count > 0 &&
                    keys[keys.Count - 1].Frame != 49)
                {
                    keys.Add(new FOBJKey()
                    {
                        Frame = 49,
                        Value = player.GetValue(start + length - 1),
                        InterpolationType = GXInterpolationType.HSD_A_OP_CON
                    });
                }
                r.SetKeys(keys, r.TrackType);

                prev = r;
            }
        }

        public static HSD_AnimJoint Extract(HSD_AnimJoint anim, float start, float length)
        {
            var anim2 = DeepClone<HSD_AnimJoint>(anim);

            foreach (var a in anim2.TreeList)
            {
                if (a.AOBJ == null)
                    continue;

                TrimAnimation(a.AOBJ, start, length);
            }

            return anim2;
        }

        private static HSD_JOBJDesc GenerateModelPreviewDesc(HSD_JOBJ joint, HSD_AnimJoint animjoint, float start_frame)
        {
            // trim animation
            var fobjs = animjoint.AOBJ.FObjDesc.List;
            animjoint.AOBJ.EndFrame = 50;

            HSD_FOBJDesc prev = null;
            foreach (var f in fobjs)
            {
                var player = new FOBJ_Player(f);
                var keys = f.GetDecodedKeys();
                keys.RemoveAll(e => e.Frame <= start_frame || e.Frame > start_frame + 49);

                if (keys.Count == 0)
                {
                    continue;
                }
                else
                {
                    foreach (var k in keys)
                        k.Frame -= start_frame;
                    keys.Insert(0, new FOBJKey()
                    {
                        Frame = 0,
                        Value = player.GetValue(start_frame),
                        InterpolationType = GXInterpolationType.HSD_A_OP_CON
                    });
                    f.SetKeys(keys, f.TrackType);
                }

                if (prev == null)
                    animjoint.AOBJ.FObjDesc = f;
                else
                    prev.Next = f;
                prev = f;
            }
            prev.Next = null;

            var root_joint = new HSD_JOBJ()
            {
                Flags = JOBJ_FLAG.CLASSICAL_SCALING,
                SX = 1,
                SY = 1,
                SZ = 1,
            };
            root_joint.Child = joint;
            root_joint.UpdateFlags();

            var root_anim_joint = new HSD_AnimJoint();
            root_anim_joint.Child = animjoint;

            return new HSD_JOBJDesc()
            {
                RootJoint = root_joint,
                JointAnimations = new HSDNullPointerArrayAccessor<HSD_AnimJoint>() { Array = new HSD_AnimJoint[] { root_anim_joint } },
            };
        }

        public static HSDNullPointerArrayAccessor<HSD_JOBJDesc> ExtractPreviewCommonSceneModels(SBM_MnSelectStageDataTable stage_table)
        {
            var preview_model = stage_table.StagePreviewModel;
            var preview_animation = stage_table.StagePreviewAnimation;
            var preview_mat_animation = stage_table.StagePreviewMaterialAnimation;

            SortedList<int, HSD_JOBJDesc> indexToJOBJ = new SortedList<int, HSD_JOBJDesc>();

            // extract nodes
            var joints = preview_model.Child.Children;
            var anmjoints = preview_animation.Child.Children;
            var matanimjoints = preview_mat_animation.Child.Children;

            // clear model
            // this will be used as the base
            preview_model.Child.Child = null;
            preview_animation.Child.Child = null;
            preview_mat_animation.Child.Child = null;

            // loop through all joints
            for (int i = 0; i < joints.Length; i++)
            {
                var joint = joints[i];

                // clear hidden flag
                joint.Flags &= ~JOBJ_FLAG.HIDDEN;

                // remove from tree
                joint.Next = null;
                anmjoints[i].Next = null;
                if (i < matanimjoints.Length)
                    matanimjoints[i].Next = null;
                    
                // base and emblem
                if (i == 24 || i == 25)
                {
                    preview_model.Child.AddChild(joint);
                    preview_animation.Child.AddChild(anmjoints[i]);
                    if (i == 25)
                        TrimAnimation(matanimjoints[i].MaterialAnimation.TextureAnimation.AnimationObject, 0, 50);
                    preview_mat_animation.Child.AddChild(matanimjoints[i]);
                }
                else
                {
                    var branch = anmjoints[i].AOBJ.FObjDesc.List.Find(e => e.JointTrackType == JointTrackType.HSD_A_J_BRANCH);
                    var start_frame = branch.GetDecodedKeys().Find(e => e.Value > 0).Frame;
                    int index = (int)(start_frame / 50);
                    start_frame = index * 50;
                    
                    // venom hack
                    if (i == 21)
                    {
                        indexToJOBJ.Add(index + 1, GenerateModelPreviewDesc(joint, DeepClone<HSD_AnimJoint>(anmjoints[i]), 650));
                    }

                    indexToJOBJ.Add(index, GenerateModelPreviewDesc(joint, anmjoints[i], start_frame));

                }
            }

            List<HSD_AnimJoint> anims = new List<HSD_AnimJoint>();
            for (int i = 0; i < preview_animation.Child.AOBJ.EndFrame; i += 50)
            {
                anims.Add(Extract(preview_animation, i, 50));
            }

            HSD_JOBJDesc[] collections = new HSD_JOBJDesc[indexToJOBJ.Count + 1];

            collections[0] = new HSD_JOBJDesc()
            {
                RootJoint = preview_model,
                JointAnimations = new HSDNullPointerArrayAccessor<HSD_AnimJoint>() { Array = anims.ToArray() },
                MaterialAnimations = new HSDNullPointerArrayAccessor<HSD_MatAnimJoint>() { Array = new HSD_MatAnimJoint[] { preview_mat_animation } },
            };

            var sorted = indexToJOBJ.ToArray();
            for (int i = 0; i < indexToJOBJ.Count; i++)
            {
                collections[i + 1] = sorted[i].Value;
            }

            return new HSDNullPointerArrayAccessor<HSD_JOBJDesc>()
            {
                Array = collections
            }; ;
        }

        public static AK_StagePage GenerateFromMex(MEX_Data mex_data, MEX_mexMapData map_data, SBM_MnSelectStageDataTable stage_table)
        {
            var page = new AK_StagePage();
            page.Count = mex_data.MetaData.NumOfSSSIcons;
            page.Icons = new HSDArrayAccessor<AK_StageIcon>();
            page.PositionJoint = map_data.PositionModel;
            page.PositionAnimEnter = map_data.PositionAnimJoint;
            page.IconModel = HSDAccessor.DeepClone<HSD_JOBJ>(map_data.IconModel);

            var nameTextures = map_data.StageNameMaterialAnimation.Child.Child.MaterialAnimation.TextureAnimation.ToTOBJs();
            var iconTextures = map_data.IconMatAnimJoint.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs();

            int index = 0;
            foreach (var i in mex_data.MenuTable.SSSIconData.Array)
            {
                AK_StageIcon icon = new AK_StageIcon()
                {
                    ExternalID = i.ExternalID,
                    Width = i.CursorWidth,
                    Height = i.CursorWidth,
                    IconAnimJoint = GenerateScaleAnimJoint(i.OutlineWidth, i.OutlineHeight),
                    IconMatAnim = GenerateMatAnim(iconTextures[index + 2]),
                    NameTexture = nameTextures[index],
                    PreviewModelIndex = i.PreviewModelID,
                };

                {
                    icon.Flags = StageIconFlags.Locked;
                    icon.IconMatAnim = GenerateMatAnim(DeepClone<HSD_TOBJ>(iconTextures[0]));
                    icon.NameTexture = DeepClone<HSD_TOBJ>(nameTextures[29]);
                }

                page.Icons.Add(icon);
                index++;
            }

            return page;
        }
    }

    public class AK_StagePage : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public HSD_JOBJ PositionJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AnimJoint PositionAnimEnter { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_JOBJ IconModel { get => _s.GetReference<HSD_JOBJ>(0x08); set => _s.SetReference(0x08, value); }

        // 0x0C GOBJ

        public int Count { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public HSDArrayAccessor<AK_StageIcon> Icons { get => _s.GetReference<HSDArrayAccessor<AK_StageIcon>>(0x14); set => _s.SetReference(0x14, value); }

    }

    [Flags]
    public enum StageIconFlags
    {
        Locked = 1,
        RandomEnabled,
    }

    public enum AkStageType
    {
        Normal,
        TargetTest,
    }

    public class AK_StageIcon : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public int ExternalID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public StageIconFlags Flags { get => (StageIconFlags)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public float Width { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float Height { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public int PreviewModelIndex { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int EmblemIndex { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public AkStageType StageType { get => (AkStageType)_s.GetByte(0x18); set => _s.SetByte(0x18,(byte) value); }

        public HSD_JOBJ IconJoint { get => _s.GetReference<HSD_JOBJ>(0x20); set => _s.SetReference(0x20, value); }

        public HSD_AnimJoint IconAnimJoint { get => _s.GetReference<HSD_AnimJoint>(0x24); set => _s.SetReference(0x24, value); }

        public HSD_MatAnimJoint IconMatAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x28); set => _s.SetReference(0x28, value); }

        public HSD_TOBJ NameTexture { get => _s.GetReference<HSD_TOBJ>(0x2C); set => _s.SetReference(0x2C, value); }
    }
}
