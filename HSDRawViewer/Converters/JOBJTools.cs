using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Rendering;
using OpenTK;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
                        j.Flags |= JOBJ_FLAG.XLU | JOBJ_FLAG.TEXEDGE;
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootjobj"></param>
        private static void ApplyNarutoMaterials(HSD_JOBJ rootjobj)
        {
            foreach (var j in rootjobj.BreathFirstList)
            {
                if (j.Dobj != null)
                    foreach (var d in j.Dobj.List)
                    {
                        d.Mobj.Material.SPC_A = 255;
                        d.Mobj.Material.SPC_B = 0;
                        d.Mobj.Material.SPC_G = 0;
                        d.Mobj.Material.SPC_R = 0;
                        d.Mobj.Material.Shininess = 50;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void MergeIntoOneObject(HSD_JOBJ jobj)
        {
            // gather all pobjs

            HSD_DOBJ result = null;

            List<HSD_POBJ> pobj = new List<HSD_POBJ>();
            foreach(var j in jobj.BreathFirstList)
            {
                if (j.Dobj != null)
                {
                    foreach (var d in j.Dobj.List)
                        if (d.Pobj != null)
                            foreach (var p in d.Pobj.List)
                                pobj.Add(p);

                    if (result == null)
                    {
                        result = j.Dobj;
                        result.Next = null;
                    }
                }
                j.Dobj = null;
            }
            
            // link pobjs
            for(int i = 0; i < pobj.Count; i++)
            {
                if (i == pobj.Count - 1)
                    pobj[i].Next = null;
                else
                    pobj[i].Next = pobj[i + 1];
            }

            // put them all in the first dobj
            result.Pobj = pobj[0];
            jobj.Dobj = result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ExportTextures(HSD_JOBJ jobj)
        {
            var folder = Tools.FileIO.OpenFolder();

            if(!string.IsNullOrEmpty(folder))
            {
                HashSet<int> textures = new HashSet<int>();

                // get all tobjs
                foreach(var j in jobj.BreathFirstList)
                {
                    if (j.Dobj != null)
                        foreach (var dobj in j.Dobj.List)
                        {
                            if(dobj.Mobj != null && dobj.Mobj.Textures != null)
                            {
                                foreach(var tobj in dobj.Mobj.Textures.List)
                                {
                                    // generate hashes and export textures and formatting

                                    var hash = ComputeHash(tobj.GetDecodedImageData());

                                    if(!textures.Contains(hash))
                                    {
                                        using (var bmp = TOBJConverter.ToBitmap(tobj))
                                            bmp.Save(System.IO.Path.Combine(folder, TOBJConverter.FormatName(hash.ToString("X8"), tobj) + ".png"));

                                        textures.Add(hash);
                                    }
                                }
                            }
                        }
                }
            }
        }

        //https://stackoverflow.com/questions/16340/how-do-i-generate-a-hashcode-from-a-byte-array-in-c/16381
        private static int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ImportTextures(HSD_JOBJ jobj)
        {
            var folder = Tools.FileIO.OpenFolder();

            if (!string.IsNullOrEmpty(folder))
            {
                // get all tobjs
                Dictionary<int, HSD_TOBJ> hashToImage = new Dictionary<int, HSD_TOBJ>();

                // load all textures from file
                foreach(var tf in System.IO.Directory.GetFiles(folder))
                {
                    if (tf.ToLower().EndsWith(".png"))
                    {
                        var fn = System.IO.Path.GetFileNameWithoutExtension(tf);
                        if(fn.Length >= 8 && 
                            int.TryParse(fn.Substring(0, 8), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int hash) && 
                            TOBJConverter.FormatFromString(fn, out HSDRaw.GX.GXTexFmt texFmt, out HSDRaw.GX.GXTlutFmt tlutFmt))
                        {
                            hashToImage.Add(hash, TOBJConverter.ImportTOBJFromFile(tf, texFmt, tlutFmt));
                        }
                    }
                }

                // get all tobjs
                foreach (var j in jobj.BreathFirstList)
                {
                    if (j.Dobj != null)
                        foreach (var dobj in j.Dobj.List)
                        {
                            if (dobj.Mobj != null && dobj.Mobj.Textures != null)
                            {
                                foreach (var tobj in dobj.Mobj.Textures.List)
                                {
                                    // generate hashes and export textures and formatting

                                    var hash = ComputeHash(tobj.GetDecodedImageData());

                                    if (hashToImage.ContainsKey(hash))
                                    {
                                        var imgClone = HSDAccessor.DeepClone<HSD_TOBJ>(hashToImage[hash]);
                                        tobj.ImageData = imgClone.ImageData;
                                        tobj.TlutData = imgClone.TlutData;
                                    }
                                }
                            }
                        }
                }
            }
        }
    }
}
