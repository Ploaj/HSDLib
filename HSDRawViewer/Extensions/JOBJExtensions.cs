using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Rendering;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace HSDRawViewer
{
    public static class JOBJExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<HSD_JOBJ, Matrix4> ApplyMeleeFighterTransforms(HSD_JOBJ root)
        {
            Dictionary<HSD_JOBJ, Matrix4> newTransforms = new();

            ZeroOutRotations(newTransforms, root, Matrix4.Identity, Matrix4.Identity);

            return newTransforms;
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, Vector3> FighterDefaults = new() {

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
            Matrix4 oldTransform =
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Math3D.CreateMatrix4FromEuler(root.RX, root.RY, root.RZ) *
                Matrix4.CreateTranslation(root.TX, root.TY, root.TZ) * oldParent;

            Vector3 targetPoint = Vector3.TransformPosition(Vector3.Zero, oldTransform);

            string trimName = root.ClassName;

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

            Vector3 relPoint = Vector3.TransformPosition(targetPoint, parentTransform.Inverted());

            root.TX = relPoint.X;
            root.TY = relPoint.Y;
            root.TZ = relPoint.Z;

            if (trimName != null && trimName.Equals("TransN")) // special case
            {
                root.TX = 0;
                root.TY = 0;
                root.TZ = 0;
            }

            Matrix4 newTransform =
                Matrix4.CreateScale(root.SX, root.SY, root.SZ) *
                Math3D.CreateMatrix4FromEuler(root.RX, root.RY, root.RZ) *
                Matrix4.CreateTranslation(root.TX, root.TY, root.TZ) * parentTransform;

            newTransforms.Add(root, newTransform);

            foreach (HSD_JOBJ c in root.Children)
                ZeroOutRotations(newTransforms, c, oldTransform, newTransform);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootjobj"></param>
        private static void ApplyNarutoMaterials(HSD_JOBJ rootjobj)
        {
            foreach (HSD_JOBJ j in rootjobj.TreeList)
            {
                if (j.Dobj != null)
                    foreach (HSD_DOBJ d in j.Dobj.List)
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

            List<HSD_POBJ> pobj = new();
            foreach (HSD_JOBJ j in jobj.TreeList)
            {
                if (j.Dobj != null)
                {
                    foreach (HSD_DOBJ d in j.Dobj.List)
                        if (d.Pobj != null)
                            foreach (HSD_POBJ p in d.Pobj.List)
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
            for (int i = 0; i < pobj.Count; i++)
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
            string folder = Tools.FileIO.OpenFolder();

            if (!string.IsNullOrEmpty(folder))
            {
                HashSet<int> textures = new();

                // get all tobjs
                foreach (HSD_JOBJ j in jobj.TreeList)
                {
                    if (j.Dobj != null)
                        foreach (HSD_DOBJ dobj in j.Dobj.List)
                        {
                            if (dobj.Mobj != null && dobj.Mobj.Textures != null)
                            {
                                foreach (HSD_TOBJ tobj in dobj.Mobj.Textures.List)
                                {
                                    // generate hashes and export textures and formatting

                                    int hash = ComputeHash(tobj.GetDecodedImageData());

                                    if (!textures.Contains(hash))
                                    {
                                        using (Image<SixLabors.ImageSharp.PixelFormats.Bgra32> img = tobj.ToImage())
                                            img.SaveAsPng(Path.Combine(folder, tobj.FormatName(hash.ToString("X8")) + ".png"));

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
            string folder = Tools.FileIO.OpenFolder();

            if (!string.IsNullOrEmpty(folder))
            {
                // get all tobjs
                Dictionary<int, HSD_TOBJ> hashToImage = new();

                // load all textures from file
                foreach (string tf in Directory.GetFiles(folder))
                {
                    if (tf.ToLower().EndsWith(".png"))
                    {
                        string fn = Path.GetFileNameWithoutExtension(tf);
                        if (fn.Length >= 8 &&
                            int.TryParse(fn.Substring(0, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int hash) &&
                            TOBJExtentions.FormatFromString(fn, out GXTexFmt texFmt, out GXTlutFmt tlutFmt))
                        {
                            hashToImage.Add(hash, TOBJExtentions.ImportTObjFromFile(tf, texFmt, tlutFmt));
                        }
                    }
                }

                // get all tobjs
                foreach (HSD_JOBJ j in jobj.TreeList)
                {
                    if (j.Dobj != null)
                        foreach (HSD_DOBJ dobj in j.Dobj.List)
                        {
                            if (dobj.Mobj != null && dobj.Mobj.Textures != null)
                            {
                                foreach (HSD_TOBJ tobj in dobj.Mobj.Textures.List)
                                {
                                    // generate hashes and export textures and formatting

                                    int hash = ComputeHash(tobj.GetDecodedImageData());

                                    if (hashToImage.ContainsKey(hash))
                                    {
                                        HSD_TOBJ imgClone = HSDAccessor.DeepClone<HSD_TOBJ>(hashToImage[hash]);
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
