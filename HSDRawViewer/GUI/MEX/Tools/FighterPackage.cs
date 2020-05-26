using HSDRaw;
using HSDRaw.MEX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.TypeInspectors;

namespace HSDRawViewer.GUI.MEX.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class MEXTypeInspector : TypeInspectorSkeleton
    {
        private readonly ITypeInspector _innerTypeDescriptor;

        public MEXTypeInspector(ITypeInspector innerTypeDescriptor)
        {
            _innerTypeDescriptor = innerTypeDescriptor;
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            var props = _innerTypeDescriptor.GetProperties(type, container);
            props = props.Where(p => p.Type != typeof(HSDStruct) && p.Name != "trimmedSize" && p.Name != "costumeCount");
            return props;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FighterPackage
    {
        public MEXFighterEntry Fighter;
        public List<MEX_Item> Items = new List<MEX_Item>();
        public MEXEffectEntry Effect;
        public MEXEffectEntry KirbyEffect;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FighterPackage DeserializeFile(string filePath)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .Build();

            return deserializer.Deserialize<FighterPackage>(File.ReadAllText(filePath));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FighterPackage Deserialize(string data)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .Build();

            return deserializer.Deserialize<FighterPackage>(data);
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
