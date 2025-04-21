using HSDRaw.Common;
using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public class DummyThrowModel
    {

        private static readonly Tuple<int, int, string, Vector3, Vector3>[] dummy = new Tuple<int, int, string, Vector3, Vector3>[]
        {
            new(0, -1, "TopN", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
new(1, 0, "TransN", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
new(2, 1, "XRotN", new Vector3(0f, 5.667229f, 0f), new Vector3(0f, 0f, 0f)),
new(3, 2, "YRotN", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
new(4, 3, "HipN", new Vector3(0f, -0.674781f, 0f), new Vector3(0f, 0f, 0f)),
new(5, 4, "WaistN", new Vector3(0f, 0.980071f, 0.026823f), new Vector3(0f, 0f, 0f)),
new(6, 4, "LLegJA", new Vector3(1.118786f, 0.004514f, 0.028483f), new Vector3(-1.570796f, 0f, -1.570796f)),
new(7, 6, "LLegJ", new Vector3(0f, 0f, 0f), new Vector3(-0.000185f, -0.007398f, 0.01191f)),
new(8, 7, "LKneeJ", new Vector3(1.908784f, 0f, 0f), new Vector3(0f, 0f, 0.017453f)),
new(9, 8, "LFootJA", new Vector3(2.840618f, -0.010152f, -0.008727f), new Vector3(0f, 0f, -1.570796f)),
new(10, 9, "LFootJ", new Vector3(0f, 0f, -1E-06f), new Vector3(-0.016257f, -0.003122f, 0.0415f)),
new(11, 4, "RLegJA", new Vector3(-1.116982f, 0.004514f, 0.028483f), new Vector3(-1.570796f, 0f, -1.570796f)),
new(12, 11, "RLegJ", new Vector3(1E-06f, 0f, 0f), new Vector3(-0.000375f, 0.012381f, 0.019316f)),
new(13, 12, "RKneeJ", new Vector3(1.908801f, 0f, 0f), new Vector3(0f, 0f, 0.017453f)),
new(14, 13, "RFootJA", new Vector3(2.840476f, 0.009582f, 0.015539f), new Vector3(0f, 0f, -1.570796f)),
new(15, 14, "RFootJ", new Vector3(0f, 0f, 0f), new Vector3(0.043408f, 0.006766f, 0.033519f)),
new(16, 4, "WaistNb", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
new(17, 4, "Bust", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
new(18, 4, "LShoulderN", new Vector3(0.857711f, 1.959548f, -0.210775f), new Vector3(0f, 0f, 0f)),
new(19, 18, "LShoulderJA", new Vector3(1.071524f, -0.008565f, -0.010922f), new Vector3(-1.570796f, 0f, 0f)),
new(20, 19, "LShoulderJ", new Vector3(1E-06f, 0f, 0f), new Vector3(-0.000916f, -0.000212f, 0.000463f)),
new(21, 20, "LArmJ", new Vector3(1.371953f, 0f, 0f), new Vector3(0f, 0f, -0.017453f)),
new(22, 21, "LHandN", new Vector3(1.567077f, -0.002016f, 0.008884f), new Vector3(0f, 0f, -0.00183f)),
new(23, 22, "L1stNa", new Vector3(1.353498f, -0.49066f, 0.310858f), new Vector3(0f, -0.03f, 0f)),
new(24, 23, "L1stNb", new Vector3(0.529526f, 0.003669f, -0.023101f), new Vector3(0f, 0f, 0f)),
new(25, 22, "L2ndNa", new Vector3(1.401493f, -0.109522f, 0.406319f), new Vector3(0f, -0.03f, 0f)),
new(26, 25, "L2ndNb", new Vector3(0.502049f, 0.008902f, -0.057054f), new Vector3(0f, 0f, 0f)),
new(27, 22, "L3rdNa", new Vector3(1.334389f, 0.283752f, 0.38992f), new Vector3(0f, -0.03f, 0f)),
new(28, 27, "L3rdNb", new Vector3(0.51451f, 0.007826f, -0.035095f), new Vector3(0f, 0f, 0f)),
new(29, 22, "L4thNa", new Vector3(1.273919f, 0.640955f, 0.306142f), new Vector3(0f, -0.03f, 0f)),
new(30, 29, "L4thNb", new Vector3(0.469108f, 0.00308f, -0.036106f), new Vector3(0f, 0f, 0f)),
new(31, 22, "LHaveN", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
new(32, 22, "LThumbNa", new Vector3(0.868668f, -0.877267f, -0.249331f), new Vector3(0.027693f, 0.031139f, -0.301742f)),
new(33, 32, "LThumbNb", new Vector3(0.511563f, 0.001014f, -0.002829f), new Vector3(0f, 0f, 0.159777f)),
new(34, 4, "NeckN", new Vector3(0f, 2.318129f, -0.247906f), new Vector3(0f, 0f, 0f)),
new(35, 34, "HeadN", new Vector3(0f, 1.021454f, 0.177739f), new Vector3(0f, 0f, 0f)),
new(36, 4, "RShoulderN", new Vector3(-0.857711f, 1.959548f, -0.210775f), new Vector3(0f, 0f, 0f)),
new(37, 36, "RShoulderJA", new Vector3(-1.071524f, -0.008561f, -0.010922f), new Vector3(-1.570796f, 0f, -3.141593f)),
new(38, 37, "RShoulderJ", new Vector3(0f, 0f, 0f), new Vector3(-0.000157f, -0.00835f, 0.012295f)),
new(39, 38, "RArmJ", new Vector3(1.348806f, 0f, -1E-06f), new Vector3(0f, 0f, -0.021377f)),
new(40, 39, "RHandN", new Vector3(1.58657f, -0.00032f, -0.051569f), new Vector3(0f, 0f, 0f)),
new(41, 40, "R1stNa", new Vector3(1.334072f, -0.535143f, -0.340872f), new Vector3(0f, 0f, 0f)),
new(42, 41, "R1stNb", new Vector3(0.526347f, -0.007068f, 0.041505f), new Vector3(0f, 0f, 0f)),
new(43, 40, "R2ndNa", new Vector3(1.363156f, -0.1685f, -0.424815f), new Vector3(0f, 0f, 0f)),
new(44, 43, "R2ndNb", new Vector3(0.543656f, 0.016739f, 0.046163f), new Vector3(0f, 0f, 0f)),
new(45, 40, "R3rdNa", new Vector3(1.354416f, 0.243202f, -0.353267f), new Vector3(0f, 0f, 0f)),
new(46, 45, "R3rdNb", new Vector3(0.508615f, -0.002402f, 0.00349f), new Vector3(0f, 0f, 0f)),
new(47, 40, "R4thNa", new Vector3(1.308195f, 0.599803f, -0.268514f), new Vector3(0f, 0f, 0f)),
new(48, 47, "R4thNb", new Vector3(0.455786f, 0.012776f, 0.011842f), new Vector3(0f, 0f, 0f)),
new(49, 40, "RHaveN", new Vector3(1.349564f, 0.000697f, 0.135002f), new Vector3(-0.022782f, -1.570796f, 3.1503f)),
new(50, 40, "RThumbNa", new Vector3(0.907776f, -0.903599f, 0.271255f), new Vector3(-0.006012f, -0.006951f, -0.368603f)),
new(51, 50, "RThumbNb", new Vector3(0.479429f, -0.003342f, -0.005467f), new Vector3(0f, 0f, 0.128251f)),
new(52, 3, "ThrowN", new Vector3(0f, -5.667229f, 0f), new Vector3(0f, 0f, 0f)),
new(53, 0, "Extra", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)),
        };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static HSD_JOBJ GenerateThrowDummy()
        {
            HSD_JOBJ root = new();
            root.ClassName = dummy[0].Item3;
            root.Flags = JOBJ_FLAG.CLASSICAL_SCALING;
            root.TX = dummy[0].Item4.X;
            root.TY = dummy[0].Item4.Y;
            root.TZ = dummy[0].Item4.Z;
            root.RX = dummy[0].Item5.X;
            root.RY = dummy[0].Item5.Y;
            root.RZ = dummy[0].Item5.Z;
            root.SX = 1f;
            root.SY = 1f;
            root.SZ = 1f;

            HSD_JOBJ[] joints = new HSD_JOBJ[dummy.Length];
            joints[0] = root;

            for (int i = 1; i < dummy.Length; i++)
            {
                HSD_JOBJ jobj = new();
                jobj.ClassName = dummy[i].Item3;
                jobj.Flags = JOBJ_FLAG.CLASSICAL_SCALING;
                jobj.TX = dummy[i].Item4.X;
                jobj.TY = dummy[i].Item4.Y;
                jobj.TZ = dummy[i].Item4.Z;
                jobj.RX = dummy[i].Item5.X;
                jobj.RY = dummy[i].Item5.Y;
                jobj.RZ = dummy[i].Item5.Z;
                jobj.SX = 1f;
                jobj.SY = 1f;
                jobj.SZ = 1f;
                joints[i] = jobj;
                HSD_JOBJ parent = joints[dummy[i].Item2];
                if (parent.Child == null)
                    parent.Child = jobj;
                else
                {
                    HSD_JOBJ c = parent.Child;
                    while (c.Next != null)
                        c = c.Next;
                    c.Next = jobj;
                }
            }

            return root;
        }

    }
}
