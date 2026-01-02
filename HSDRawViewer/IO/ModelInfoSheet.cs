using HSDRaw.Common;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HSDRawViewer.Converters
{
    public class MI_Object
    {
        public string Name { get; set; }

        public string Ambient { get; set; }

        public string Diffuse { get; set; }

        public string Specular { get; set; }

        public float Shininess { get; set; }

        public float Alpha { get; set; }
    }

    public class ModelInfoSheet
    {
        private readonly HashSet<byte[]> textures = new();

        public List<MI_Object> Objects { get; set; } = new List<MI_Object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public ModelInfoSheet(HSD_JOBJ jobj)
        {
            ParseJoint(jobj);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ParseJoint(HSD_JOBJ jobj)
        {
            int ji = 0;
            foreach (HSD_JOBJ j in jobj.TreeList)
            {
                if (j.Dobj != null)
                {
                    int di = 0;
                    foreach (HSD_DOBJ dobj in j.Dobj.List)
                    {
                        HSD_MOBJ mobj = dobj.Mobj;
                        HSD_Material mat = mobj.Material;
                        Objects.Add(new MI_Object()
                        {
                            Name = $"Joint_{ji}_Object_{di}",
                            Ambient = mat.AmbientColor.ToArgb().ToString("X8"),
                            Diffuse = mat.DiffuseColor.ToArgb().ToString("X8"),
                            Specular = mat.SpecularColor.ToArgb().ToString("X8"),
                            Shininess = mat.Shininess,
                            Alpha = mat.Alpha,
                        });
                        di++;
                    }
                }
                ji++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        private void ProcessTexture(HSD_TOBJ texture)
        {
            foreach (HSD_TOBJ t in texture.List)
            {
                if (t.ImageData != null && t.ImageData.ImageData != null && !textures.Contains(t.ImageData.ImageData))
                {
                    //string name = $"Texture_{textures.Count}_{t.ImageData.Format}";
                    textures.Add(t.ImageData.ImageData);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Export(string filepath)
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(filepath, json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="root"></param>
        public static void Export(string filepath, HSD_JOBJ root)
        {
            new ModelInfoSheet(root).Export(filepath);
        }
    }
}
