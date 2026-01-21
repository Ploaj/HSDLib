using HSDRawViewer.Rendering.Models;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace HSDRawViewer.Tools.Animation
{
    public class AnimationRemapper
    {
        public LiveJObj SourceJoint { get; set; }

        public LiveJObj TargetJoint { get; set; }

        public JointMap SourceMap { get; set; }

        public JointMap TargetMap { get; set; }

        private struct BindData
        {
            public Quaternion rotDelta;          // source → target rotation delta

            public Vector3 targetLocalPosBind;   // target bind local position
            public Vector3 sourceBindPos;        // source bind local/world position

            public Quaternion sourceBindRot;     // source bind world rotation
            public Quaternion targetBindRot;     // target bind world rotation
            public float scale;                  // optional length scale for translation
            public bool translate;               // true if this joint should follow translation (e.g., root/hip)
        }

        private BindData[] BindOffsets;

        public class IKGroup
        {
            public string Name { get; set; }

            public LiveJObj b0 { get; set; }

            public LiveJObj b1 { get; set; }

            public LiveJObj b2 { get; set; }

            public Vector3 Effector;

            public Vector3 BindNormal { get; set; }

            public bool PreserveEndRotation { get; set; } = true;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="b0"></param>
            /// <param name="b1"></param>
            /// <param name="b2"></param>
            public IKGroup(LiveJObj b0, LiveJObj b1, LiveJObj b2)
            {
                this.b0 = b0;
                this.b1 = b1;
                this.b2 = b2;

                // reset model to bind pose
                b0.Root.ResetTransforms();
                b0.Root.RecalculateTransforms(null, true);

                // extract default effector
                Effector = b2.WorldTransform.ExtractTranslation();
                Effector = new Vector3(Effector.X, Effector.Y + 1, Effector.Z);

                // calculate bind normal
                BindNormal = IKSolver.CalculateBindNormal(
                    b0.WorldTransform,
                    b1.WorldTransform,
                    b2.WorldTransform);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="j"></param>
            /// <returns></returns>
            private static Quaternion GetParentBindMatrix(LiveJObj j)
            {
                if (j.Parent == null)
                    return Quaternion.Identity;

                return j.Parent.WorldTransform.ExtractRotation().Normalized().Inverted();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="j"></param>
            /// <param name="q"></param>
            private static void SetWorldRotation(LiveJObj j, Quaternion q)
            {
                j.Rotation = new Vector4((GetParentBindMatrix(j) * q).ExtractRotationEuler());
                j.RecalculateTransforms(null, true);
            }

            /// <summary>
            /// 
            /// </summary>
            public void Solve()
            {
                if (b0 == null || b1 == null || b2 == null)
                    return;

                var res = IKSolver.Solve3BoneWorld(
                    b0.WorldTransform,
                    b1.WorldTransform,
                    b2.WorldTransform,
                    Effector,
                    BindNormal
                    );

                var footBefore = b2.WorldTransform.ExtractRotation().Normalized();

                SetWorldRotation(b0, res.LegWorldRotation);
                SetWorldRotation(b1, res.KneeWorldRotation);

                if (PreserveEndRotation)
                    SetWorldRotation(b2, footBefore);
            }
        }

        public List<IKGroup> IKGroups { get; } = new List<IKGroup>();

        public AnimationRemapper(
            LiveJObj sourceJoint,
            LiveJObj targetJoint,
            JointMap sourceMap,
            JointMap targetMap)
        {
            SourceJoint = sourceJoint;
            TargetJoint = targetJoint;
            SourceMap = sourceMap;
            TargetMap = targetMap;

            // put both in bind pose
            SourceJoint.ResetTransforms();
            SourceJoint.RecalculateTransforms(null, true);

            TargetJoint.ResetTransforms();
            TargetJoint.RecalculateTransforms(null, true);

            // calculate bind offset
            BindOffsets = new BindData[TargetMap.Count];
            for (int i = 0; i < TargetMap.Count; i++)
            {
                var from = SourceMap.IndexOf(TargetMap[i]);

                if (from == -1)
                    continue;

                var source = SourceJoint.GetJObjAtIndex(from);
                var target = TargetJoint.GetJObjAtIndex(i);

                bool translate = true;

                if (TargetMap[i].Equals("XRotN"))
                    translate = true;

                float sourceBoneLength = (source.Parent != null)
                    ? (source.WorldTransform.ExtractTranslation() - source.Parent.WorldTransform.ExtractTranslation()).Length
                    : 1f;

                float targetBoneLength = (target.Parent != null)
                    ? (target.WorldTransform.ExtractTranslation() - target.Parent.WorldTransform.ExtractTranslation()).Length
                    : 1f;

                BindOffsets[i] = new BindData()
                {
                    rotDelta = source.WorldTransform.ExtractRotation().Inverted() * target.WorldTransform.ExtractRotation(),

                    targetLocalPosBind = target.Translation,
                    targetBindRot = target.WorldTransform.ExtractRotation(),

                    sourceBindPos = source.Translation,
                    sourceBindRot = source.WorldTransform.ExtractRotation(),

                    scale = (sourceBoneLength > 0f) ? targetBoneLength / sourceBoneLength * 1 : 1,
                    translate = translate,
                };
            }
        }

        public IKGroup Add2BoneIKGroup(string name, string b0, string b1, string b2)
        {
            if (TargetMap == null ||
                TargetJoint == null)
                return null;

            var i0 = TargetMap.IndexOf(b0);
            var i1 = TargetMap.IndexOf(b1);
            var i2 = TargetMap.IndexOf(b2);

            if (i0 == -1 ||
                i1 == -1 ||
                i2 == -1)
                return null;

            var ik = new IKGroup(
                TargetJoint.GetJObjAtIndex(i0),
                TargetJoint.GetJObjAtIndex(i1),
                TargetJoint.GetJObjAtIndex(i2));

            IKGroups.Add(ik);

            return ik;
        }

        private static void ConstrainWorldTransform(
                LiveJObj source,
                LiveJObj target,
                BindData bind
            )
        {
            // --- 1. Extract source world rotation ---
            Quaternion sourceWorldRot = source.WorldTransform.ExtractRotation();

            // --- 2. Compute target world rotation using bind delta ---
            Quaternion targetWorldRot = sourceWorldRot * bind.rotDelta;

            // --- 3. Compute translation ---
            Vector3 targetTranslation;

            if (bind.translate)
            {
                // Step A: delta from source bind
                Vector3 deltaSource = source.Translation - bind.sourceBindPos;

                // Step B: rotate delta into target space
                Quaternion sourceToTarget = bind.targetBindRot * Quaternion.Invert(bind.sourceBindRot);
                Vector3 deltaTarget = Vector3.Transform(deltaSource, sourceToTarget);

                // Step C: scale for proportion differences
                deltaTarget *= bind.scale;

                // Step D: add to target local bind position
                targetTranslation = bind.targetLocalPosBind + deltaTarget;
            }
            else
            {
                // For limbs: keep target local bind translation
                targetTranslation = bind.targetLocalPosBind;
            }

            // --- 4. Convert rotation into target local space ---
            Quaternion targetLocalRot =
                target.Parent != null
                    ? Quaternion.Invert(target.Parent.WorldTransform.ExtractRotation()) * targetWorldRot
                    : targetWorldRot;

            // --- 5. Apply translation ---
            target.Translation = targetTranslation;

            // --- 6. Apply rotation ---
            target.Rotation = new Vector4(targetLocalRot.ExtractRotationEuler());

            // --- 7. Keep scale or recompute if needed ---
            target.Scale = source.Scale; // TODO: reorient scale?

            // --- 8. Update transforms ---
            target.RecalculateTransforms(null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Solve(bool process_ik)
        {
            TargetJoint.ResetTransforms();
            TargetJoint.RecalculateTransforms(null, true);

            for (int i = 0; i < TargetMap.Count; i++)
            {
                var from = SourceMap.IndexOf(TargetMap[i]);

                if (from == -1)
                    continue;

                var source = SourceJoint.GetJObjAtIndex(from);
                var target = TargetJoint.GetJObjAtIndex(i);

                ConstrainWorldTransform(source, target, BindOffsets[i]);
            }

            if (process_ik)
            {
                foreach (var ik in IKGroups)
                {
                    ik.Solve();
                }
                TargetJoint.RecalculateTransforms(null, true);
            }
        }

    }
}
