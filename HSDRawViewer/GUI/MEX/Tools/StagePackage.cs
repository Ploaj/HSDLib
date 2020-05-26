using HSDRaw.MEX;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.GUI.MEX.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class StagePackage
    {
        public MEXStageEntry Stage;
        public List<MEX_Item> Items = new List<MEX_Item>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static StagePackage DeserializeFile(string filePath)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .Build();

            return deserializer.Deserialize<StagePackage>(File.ReadAllText(filePath));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static StagePackage Deserialize(string data)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .Build();

            return deserializer.Deserialize<StagePackage>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Serialize(string filepath)
        {
            var builder = new SerializerBuilder();
            builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
            builder.WithTypeInspector(inspector => new MEXTypeInspector(inspector));

            using (StreamWriter writer = File.CreateText(filepath))
            {
                builder.Build().Serialize(writer, this);
            }
        }

    }
}
