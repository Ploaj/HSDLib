using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.Tools
{
    public class ParticleManager
    {
        [Serializable]
        public class ParticleDescriptor
        {
            public byte Code;

            public byte Range;

            public string Name;

            public string Description;

            public string ParamDesc;

            public string[] Params;

            public override string ToString()
            {
                return $"{Name}";
            }
        }

        private static bool Initialized = false;

        private static ParticleDescriptor[] Descriptors;

        private static Dictionary<byte, ParticleDescriptor> ParticleDict = new Dictionary<byte, ParticleDescriptor>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ParticleDescriptor[] GetDescriptors()
        {
            Init();

            return Descriptors;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void Init()
        {
            if (Initialized)
                return;

            Console.WriteLine("Initialized");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string sa = "";

            if (File.Exists(@"Melee\ptcl.yml"))
                sa = File.ReadAllText(@"Melee\ptcl.yml");

            Descriptors = deserializer.Deserialize<ParticleDescriptor[]>(sa);

            foreach (var v in Descriptors)
            {
                for (byte i = v.Code; i < v.Code + v.Range && i > 0; i++)
                    if(!ParticleDict.ContainsKey(i))
                        ParticleDict.Add(i, v);
            }

                Initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ParticleDescriptor GetParticleDescriptor(byte code)
        {
            Init();

            if (ParticleDict.ContainsKey(code))
                return ParticleDict[code];

            foreach (var d in Descriptors)
                if (code >= d.Code && code < d.Code + d.Range)
                    return d;

            return null;
        }
    }
}
