using HSDRaw.Common;
using HSDRawViewer.Rendering;
using OpenTK;
using System.Collections.Generic;

namespace HSDRawViewer.Converters
{
    public class JOBJTools
    {
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
        private static Dictionary<string, Vector3> FighterDefaults = new Dictionary<string, Vector3>() { { "TopN", new Vector3(0, 0, 0) },
{ "TransN", new Vector3(0, 0, 0) },
{ "XRotN", new Vector3(0, 0, 0) },
{ "YRotN", new Vector3(0, 0, 0) },
{ "HipN", new Vector3(0, 0, 0) },
{ "WaistN", new Vector3(0, 0, 0) },
{ "LLegJA", new Vector3(-1.570796f, 0, -1.570796f) },
{ "LLegJ", new Vector3(-0.0001849213f, -0.007395464f, 0.01190592f) },
{ "LKneeJ", new Vector3(0, 0, 0.01745588f) },
{ "LFootJA", new Vector3(0, 0, -1.570796f) },
{ "LFootJ", new Vector3(-0.01625728f, -0.003122046f, 0.04150072f) },
{ "RLegJA", new Vector3(-1.570796f, 0, -1.570796f) },
{ "RLegJ", new Vector3(-0.0003749531f, 0.01237885f, 0.01931265f) },
{ "RKneeJ", new Vector3(0, 0, 0.01745588f) },
{ "RFootJA", new Vector3(0, 0, -1.570796f) },
{ "RFootJ", new Vector3(0.04340675f, 0.006765825f, 0.03351802f) },
{ "BustN", new Vector3(0, 0, 0) },
{ "LShoulderN", new Vector3(0, 0, 0) },
{ "LShoulderJA", new Vector3(-1.570796f, 0, 0) },
{ "LShoulderJ", new Vector3(-0.0008534141f, -0.0001975292f, 0.0004313609f) },
{ "LArmJ", new Vector3(0, 0, -0.01745588f) },
{ "LHandN", new Vector3(0, 0, -0.001827065f) },
{ "L1stNa", new Vector3(0, -0.0299967f, 0) },
{ "L1stNb", new Vector3(0, 0, 0) },
{ "L2ndNa", new Vector3(0, -0.0299967f, 0) },
{ "L2ndNb", new Vector3(0, 0, 0) },
{ "L3rdNa", new Vector3(0, -0.0299967f, 0) },
{ "L3rdNb", new Vector3(0, 0, 0) },
{ "L4thNa", new Vector3(0, -0.0299967f, 0) },
{ "L4thNb", new Vector3(0, 0, 0) },
{ "LHaveN", new Vector3(0, 0, -0.001827065f) },
{ "LThumbNa", new Vector3(0.02769301f, 0.03113901f, -0.3017421f) },
{ "LThumbNb", new Vector3(0, 0, 0.159777f) },
{ "NeckN", new Vector3(0, 0, 0) },
{ "HeadN", new Vector3(0, 0, 0) },
{ "RShoulderN", new Vector3(0, 0, 0) },
{ "RShoulderJA", new Vector3(-1.570796f, 0, 3.141592f) },
{ "RShoulderJ", new Vector3(-0.0001569438f, -0.008347717f, 0.01229164f) },
{ "RArmJ", new Vector3(0, 0, -0.0213731f) },
{ "RHandN", new Vector3(0, 0, 0) },
{ "R1stNa", new Vector3(0, 0, 0) },
{ "R1stNb", new Vector3(0, 0, 0) },
{ "R2ndNa", new Vector3(0, 0, 0) },
{ "R2ndNb", new Vector3(0, 0, 0) },
{ "R3rdNa", new Vector3(0, 0, 0) },
{ "R3rdNb", new Vector3(0, 0, 0) },
{ "R4thNa", new Vector3(0, 0, 0) },
{ "R4thNb", new Vector3(0, 0, 0) },
{ "RHaveN", new Vector3(0, -1.570796f, 3.127518f) },
{ "RThumbNa", new Vector3(-0.006011998f, -0.006951f, -0.3686029f) },
{ "RThumbNb", new Vector3(0, 0, 0.1282514f) },
{ "ThrowN", new Vector3(0, 0, 0) },
{ "Extra", new Vector3(0, 0, 0) }};

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
