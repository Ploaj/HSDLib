using HSDRaw.Common;
using HSDRawViewer.Rendering;
using OpenTK;
using System.Collections.Generic;

namespace HSDRawViewer.Converters
{
    public class JOBJTools
    {
        /// <summary>
        /// Automatically updates jobj flags
        /// </summary>
        public static void UpdateJOBJFlags(HSD_JOBJ jobj)
        {
            var list = jobj.BreathFirstList;
            list.Reverse();

            foreach (var j in list)
            {
                if(j.Dobj != null)
                {
                    bool xlu = false;
                    bool opa = false;

                    foreach (var dobj in j.Dobj.List)
                    {
                        if (dobj.Mobj != null && dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU))
                        {
                            j.Flags |= JOBJ_FLAG.XLU;
                            j.Flags |= JOBJ_FLAG.TEXEDGE;
                            xlu = true;
                        }
                        else
                        {
                            j.Flags &= ~JOBJ_FLAG.XLU;
                            j.Flags &= ~JOBJ_FLAG.TEXEDGE;
                            opa = true;
                        }

                        if (dobj.Mobj != null && dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.DIFFUSE))
                            j.Flags |= JOBJ_FLAG.LIGHTING;
                        else
                            j.Flags &= ~JOBJ_FLAG.LIGHTING;

                        if (dobj.Mobj != null && dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.SPECULAR))
                            j.Flags |= JOBJ_FLAG.SPECULAR;
                        else
                            j.Flags &= ~JOBJ_FLAG.SPECULAR;

                        if (dobj.Pobj != null)
                        {
                            j.Flags &= ~JOBJ_FLAG.ENVELOPE_MODEL;
                            foreach (var pobj in dobj.Pobj.List)
                            {
                                if (pobj.Flags.HasFlag(POBJ_FLAG.ENVELOPE))
                                    j.Flags |= JOBJ_FLAG.ENVELOPE_MODEL;
                            }
                        }
                    }

                    if(opa)
                        j.Flags |= JOBJ_FLAG.OPA;
                    else
                        j.Flags &= ~JOBJ_FLAG.OPA;

                    if (xlu)
                        j.Flags |= JOBJ_FLAG.XLU;
                    else
                        j.Flags &= ~JOBJ_FLAG.XLU;
                }

                if (j.InverseWorldTransform != null)
                    j.Flags |= JOBJ_FLAG.SKELETON;
                else
                    j.Flags &= ~JOBJ_FLAG.SKELETON;

                if (ChildHasFlag(j.Child, JOBJ_FLAG.XLU))
                    j.Flags |= JOBJ_FLAG.ROOT_XLU;
                else
                    j.Flags &= ~JOBJ_FLAG.ROOT_XLU;

                if (ChildHasFlag(j.Child, JOBJ_FLAG.OPA))
                    j.Flags |= JOBJ_FLAG.ROOT_OPA;
                else
                    j.Flags &= ~JOBJ_FLAG.ROOT_OPA;

                if (ChildHasFlag(j.Child, JOBJ_FLAG.TEXEDGE))
                    j.Flags |= JOBJ_FLAG.ROOT_TEXEDGE;
                else
                    j.Flags &= ~JOBJ_FLAG.ROOT_TEXEDGE;
            }

            if (ChildHasFlag(jobj.Child, JOBJ_FLAG.SKELETON))
                jobj.Flags |= JOBJ_FLAG.SKELETON_ROOT;
            else
                jobj.Flags &= ~JOBJ_FLAG.SKELETON_ROOT;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private static bool ChildHasFlag(HSD_JOBJ jobj, JOBJ_FLAG flag)
        {
            if (jobj == null)
                return false;

            bool hasFlag = jobj.Flags.HasFlag(flag);

            foreach (var c in jobj.Children)
            {
                if (ChildHasFlag(c, flag))
                    hasFlag = true;
            }

            if (jobj.Next != null)
            {
                if (ChildHasFlag(jobj.Next, flag))
                    hasFlag = true;
            }

            return hasFlag;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<HSD_JOBJ, Matrix4> ApplyMeleeFighterTransforms(HSD_JOBJ root)
        {
            Dictionary<HSD_JOBJ, Matrix4> newTransforms = new Dictionary<HSD_JOBJ, Matrix4>();

            ZeroOutRotations(newTransforms, root, Matrix4.Identity, Matrix4.Identity);

            return newTransforms;
        }

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, Vector3> FighterDefaults = new Dictionary<string, Vector3>() {

            // Every other bone has 0 rotation

{ "LLegJA", new Vector3(-1.570796f, 0, -1.570796f) },
{ "LFootJA", new Vector3(0, 0, -1.570796f) },
{ "RLegJA", new Vector3(-1.570796f, 0, -1.570796f) },
{ "RFootJA", new Vector3(0, 0, -1.570796f) },
{ "LShoulderJA", new Vector3(-1.570796f, 0, 0) },
{ "RShoulderJA", new Vector3(-1.570796f, 0, 3.141592f) },

{ "LLegC", new Vector3(-1.570796f, 0, -1.570796f) },
{ "LFootC", new Vector3(0, 0, -1.570796f) },
{ "RLegC", new Vector3(-1.570796f, 0, -1.570796f) },
{ "RFootC", new Vector3(0, 0, -1.570796f) },
{ "LShoulderC", new Vector3(-1.570796f, 0, 0) },
{ "RShoulderC", new Vector3(-1.570796f, 0, 3.141592f) }
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="newWorldMatrices"></param>
        /// <param name="root"></param>
        /// <param name="parentTransform"></param>
        private static void ZeroOutRotations(Dictionary<HSD_JOBJ, Matrix4> newTransforms, HSD_JOBJ root, Matrix4 oldParent, Matrix4 parentTransform)
        {
            var oldTransform =
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(root.RZ, root.RY, root.RX)) *
                Matrix4.CreateTranslation(root.TX, root.TY, root.TZ) * oldParent;

            var targetPoint = Vector3.TransformPosition(Vector3.Zero, oldTransform);
            
            var trimName = root.ClassName;

            if (trimName != null && FighterDefaults.ContainsKey(trimName))
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
                root.RX = FighterDefaults[trimName].X;
                root.RY = FighterDefaults[trimName].Y;
                root.RZ = FighterDefaults[trimName].Z;
            }
            else
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
                root.RX = 0;
                root.RY = 0;
                root.RZ = 0;
            }

            Matrix4 currentTransform =
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(root.RZ, root.RY, root.RX)) *
                parentTransform;

            var relPoint = Vector3.TransformPosition(targetPoint, parentTransform.Inverted());

            root.TX = relPoint.X;
            root.TY = relPoint.Y;
            root.TZ = relPoint.Z;

            if (trimName != null && trimName.Equals("TransN")) // special case
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
            }

            var newTransform =
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(root.RZ, root.RY, root.RX)) *
                Matrix4.CreateTranslation(root.TX, root.TY, root.TZ) * parentTransform;

            newTransforms.Add(root, newTransform);

            foreach (var c in root.Children)
                ZeroOutRotations(newTransforms, c, oldTransform, newTransform);
        }
    }
}
