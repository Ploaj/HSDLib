using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Globalization;

namespace HSDRawViewer
{
    public static class JOBJExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public static void CleanRootNode(this HSD_JOBJ jobj)
        {
            var joints = jobj.BreathFirstList;

            Matrix4 rot = Matrix4.Identity; ;
            for (int i = 0; i < joints.Count; i++)
            {
                var j = joints[i];

                if(i == 0)
                {
                    rot = Math3D.CreateMatrix4FromEuler(j.RX, j.RY, j.RZ);
                    j.RX = 0;
                    j.RY = 0;
                    j.RZ = 0;
                }
                else
                {
                    // fix position
                    var pos = Vector3.TransformNormal(new Vector3(j.TX, j.TY, j.TZ), rot);
                    j.TX = pos.X;
                    j.TY = pos.Y;
                    j.TZ = pos.Z;
                }

                // clean scale
                j.SX = 1;
                j.SY = 1;
                j.SZ = 1;
            }
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
                Math3D.CreateMatrix4FromEuler(root.RX, root.RY, root.RZ) *
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
                Math3D.CreateMatrix4FromEuler(root.RX, root.RY, root.RZ) *
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
                Math3D.CreateMatrix4FromEuler(root.RX, root.RY, root.RZ) *
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
