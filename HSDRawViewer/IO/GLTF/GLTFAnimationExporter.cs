using HSDRaw.Common;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools.Animation;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HSDRawViewer.IO.GLTF
{
    public static class GLTFAnimationExporter
    {
        public static void Export(
            string path,
            string animationName,
            HSD_JOBJ skeleton,
            JointMap map,
            JointAnimManager anim)
        {
            var model = ModelRoot.CreateModel();

            var scene = model.UseScene("Scene");

            int jointIndex = 0;
            List<Node> nodes = new();
            Node CreateSkeleton(HSD_JOBJ jobj, Node parent)
            {
                Node node;

                var name = map[jointIndex];
                if (string.IsNullOrEmpty(name))
                    name = $"JOBJ_{jointIndex}";
                jointIndex++;

                if (parent == null)
                    node = scene.CreateNode(name);
                else
                    node = parent.CreateNode(name);

                nodes.Add(node);

                var r = Math3D.EulerToQuat(jobj.RX, jobj.RY, jobj.RZ);
                node.LocalTransform = new AffineTransform(
                    new Vector3(jobj.SX, jobj.SY, jobj.SZ),
                    new Quaternion(r.X, r.Y, r.Z, r.W),
                    new Vector3(jobj.TX, jobj.TY, jobj.TZ));

                if (jobj.Child != null)
                    CreateSkeleton(jobj.Child, node);

                if (jobj.Next != null)
                    CreateSkeleton(jobj.Next, parent);

                return node;
            }

            // Create the skeleton hierarchy.
            var root = CreateSkeleton(skeleton, null);

            var skin = model.CreateSkin();
            skin.Name = $"Model_Skin";
            skin.BindJoints(nodes.ToArray());

            // Create the animation.
            var animation = model.CreateAnimation(animationName);

            LiveJObj live = new LiveJObj(skeleton);
            int i = 0;
            foreach (var boneAnim in anim.Nodes)
            {
                var node = nodes[i];
                var jobj = live.GetJObjAtIndex(i);

                AddBoneAnimation(
                    animation,
                    node,
                    jobj,
                    boneAnim);
                i++;
            }

            model.SaveGLB(path);
        }

        private static void AddBoneAnimation(
            Animation animation,
            Node node,
            LiveJObj jobj,
            AnimNode data)
        {
            if (data.Tracks.Count <= 0) return;

            int maxFrames = (int)Math.Ceiling(data.Tracks.Max(e => e.Keys.Max(k => k.Frame)));

            if (maxFrames <= 0) return;

            var has_translation = data.Tracks.Any(e =>
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAX ||
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAY ||
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAZ);

            var has_rotation = data.Tracks.Any(e=>
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTX ||
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTY ||
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTZ);

            var has_scale = data.Tracks.Any(e =>
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_SCAX ||
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_SCAY ||
                e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_SCAZ);

            var translation = new Dictionary<float, Vector3>();
            var rotate = new Dictionary<float, Quaternion>();
            var scale = new Dictionary<float, Vector3>();

            for (float i = 0; i <= maxFrames; i++)
            {
                var frame = i / 60.0f;
                jobj.ApplyAnimation(data.Tracks, i);

                if (has_translation)
                {
                    translation.Add(frame, new Vector3(jobj.Translation.X, jobj.Translation.Y, jobj.Translation.Z));
                }

                if (has_rotation)
                {
                    var r = Math3D.EulerToQuat(jobj.Rotation.Xyz);
                    rotate.Add(frame, new Quaternion(r.X, r.Y, r.Z, r.W));
                }

                if (has_scale)
                {
                    scale.Add(frame, new Vector3(jobj.Scale.X, jobj.Scale.Y, jobj.Scale.Z));
                }
            }

            if (translation.Count > 0)
            {
                animation.CreateTranslationChannel(
                    node,
                    translation);
            }

            if (rotate.Count > 0)
            {
                animation.CreateRotationChannel(
                    node,
                    rotate);
            }

            if (scale.Count > 0)
            {
                animation.CreateScaleChannel(
                    node,
                    scale);
            }
        }
    }
}
