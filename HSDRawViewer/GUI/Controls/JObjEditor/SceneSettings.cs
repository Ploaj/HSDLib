using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.GX;
using HSDRawViewer.Rendering.Models;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    /// <summary>
    /// 
    /// </summary>
    public class SceneSettings
    {
        public float Frame { get; set; } = 0;

        public bool CSPMode { get; set; } = false;

        public bool ShowGrid { get; set; } = true;

        public bool ShowBackdrop { get; set; } = true;

        public Camera Camera { get; set; }

        public JobjDisplaySettings Settings { get; set; }

        public GXLightParam Lighting { get; set; }

        public JointAnimManager Animation { get; set; }

        public int[] HiddenNodes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SceneSettings Deserialize(string filePath)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

            return deserializer.Deserialize<SceneSettings>(File.ReadAllText(filePath));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Serialize(string filepath)
        {
            var builder = new SerializerBuilder();
            builder.WithNamingConvention(CamelCaseNamingConvention.Instance);

            using (StreamWriter writer = File.CreateText(filepath))
            {
                builder.Build().Serialize(writer, this);
            }
        }
    }
}
