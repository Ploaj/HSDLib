using HSDRaw.Common;
using HSDRawViewer.Rendering.Models;
using System.Collections.Generic;

namespace HSDRawViewer.IO.Model
{
    public class HsdSkl
    {
        public class Bone
        {
            public string Name { get; set; }

            public int Parent { get; set; }

            public float TX { get; set; }
            public float TY { get; set; }
            public float TZ { get; set; }

            public float RX { get; set; }
            public float RY { get; set; }
            public float RZ { get; set; }

            public float SX { get; set; }
            public float SY { get; set; }
            public float SZ { get; set; }

            public List<HsdMsh> Mesh { get; set; } = new List<HsdMsh>();

            public Bone()
            {
                Name = "Bone";
                Parent = -1;
                SX = 1;
                SY = 1;
                SZ = 1;
            }

            public Bone(string name, int parent, HSD_JOBJ jobj, LiveJObj root)
            {
                Name = name;
                Parent = parent;

                TX = jobj.TX;
                TY = jobj.TY;
                TZ = jobj.TZ;

                RX = jobj.RX;
                RY = jobj.RY;
                RZ = jobj.RZ;

                SX = jobj.SX;
                SY = jobj.SY;
                SZ = jobj.SZ;

                // proces mesh data
                if (jobj.Dobj != null)
                {
                    foreach (var d in jobj.Dobj.List)
                    {
                        Mesh.Add(new HsdMsh(root, jobj, d));
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public HSD_JOBJ ToJObj()
            {
                return new HSD_JOBJ()
                {
                    ClassName = Name,
                    Flags = JOBJ_FLAG.CLASSICAL_SCALING,
                    TX = TX,
                    TY = TY,
                    TZ = TZ,
                    RX = RX,
                    RY = RY,
                    RZ = RZ,
                    SX = SX,
                    SY = SY,
                    SZ = SZ,
                };
            }
        }

        public List<Bone> Bones { get; set; } = new List<Bone>();

        /// <summary>
        /// 
        /// </summary>
        public HsdSkl()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public HsdSkl(HSD_JOBJ jobj)
        {
            var root = new LiveJObj(jobj);
            ProcessBone(jobj, -1, root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="parent"></param>
        private void ProcessBone(HSD_JOBJ jobj, int parent, LiveJObj root)
        {
            // add bone
            var index = Bones.Count;
            Bones.Add(new Bone($"JOBJ_{index}", parent, jobj, root));

            // process child
            if (jobj.Child != null)
                ProcessBone(jobj.Child, index, root);

            // process sibling
            if (jobj.Next != null)
                ProcessBone(jobj.Next, parent, root);
        }
    }
}
