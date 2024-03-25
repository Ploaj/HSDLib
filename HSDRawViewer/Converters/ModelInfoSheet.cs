using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        private HashSet<byte[]> textures = new HashSet<byte[]>();

        public List<MI_Object> Objects { get; set; } = new List<MI_Object>();

        public ModelInfoSheet()
        {
            // Necessary in order to deserialize
        }

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
            foreach (var j in jobj.TreeList)
            {
                if (j.Dobj != null)
                {
                    var di = 0;
                    foreach (var dobj in j.Dobj.List)
                    {
                        var mobj = dobj.Mobj;
                        var mat = mobj.Material;
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
            foreach (var t in texture.List)
            {
                if (t.ImageData != null && t.ImageData.ImageData != null && !textures.Contains(t.ImageData.ImageData))
                {
                    var name = $"Texture_{textures.Count}_{t.ImageData.Format}";
                    textures.Add(t.ImageData.ImageData);
                }
            }
        }

        public void updateJobj(HSD_JOBJ jobj)
        {
            int objectIndex = 0;
            foreach (var j in jobj.TreeList)
            {
                if (j.Dobj != null)
                {
                    foreach (var dobj in j.Dobj.List)
                    {
                        MI_Object current = this.Objects[objectIndex];

                        var mobj = dobj.Mobj;
                        var mat = mobj.Material;

                        mat.AmbientColor = System.Drawing.Color.FromArgb(Convert.ToInt32(current.Ambient, 16));
                        mat.DiffuseColor = System.Drawing.Color.FromArgb(Convert.ToInt32(current.Diffuse, 16));
                        mat.SpecularColor = System.Drawing.Color.FromArgb(Convert.ToInt32(current.Specular, 16));
                        mat.Shininess = current.Shininess;
                        mat.Alpha = current.Alpha;
                        objectIndex++;
                    }
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
        public static ModelInfoSheet Import(string filepath)
        {
            string json = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<ModelInfoSheet>(json);
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
