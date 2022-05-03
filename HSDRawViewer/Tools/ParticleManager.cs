using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;

namespace HSDRawViewer.Tools
{
    [Serializable]
    public class ParticleDescriptor
    {
        public byte Code;

        public string Name;

        public string Description;

        /// <summary>
        /// ParamDesc
        /// w - wait
        /// b - byte
        /// s - short
        /// f - float
        /// e - extended byte
        /// v - flag vector3
        /// c - flag color
        /// l - flag step color
        /// m - material flag vector2
        /// </summary>

        public string ParamDesc;

        public string[] Params;

        public override string ToString()
        {
            return Name;
        }
    }

    public class ParticleManager
    {

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

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string sa = "";

            if (File.Exists(@"Melee\ptcl.yml"))
                sa = File.ReadAllText(@"Melee\ptcl.yml");

            Descriptors = deserializer.Deserialize<ParticleDescriptor[]>(sa);

            foreach (var v in Descriptors)
            {
                if (!ParticleDict.ContainsKey(v.Code))
                    ParticleDict.Add(v.Code, v);
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
                if (code == d.Code)
                    return d;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<ParticleEvent> DecompileCode(byte[] cmdList, int cmdPtr = 0)
        {
            List<ParticleEvent> events = new List<ParticleEvent>();

            while (true)
            {
                if (ParticleEvent.ReadEvent(cmdList, ref cmdPtr, out ParticleEvent evn))
                {
                    events.Add(evn);
                }
                else
                {
                    if (evn != null)
                        events.Add(evn);

                    break;
                }
            }

            return events;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static byte[] CompileCode(IEnumerable<ParticleEvent> events)
        {
            List<byte> o = new List<byte>();

            byte lastCode = 0;

            foreach (var v in events)
            {
                o.AddRange(v.CompileCode());
                lastCode = v.Code;
            }

            if (lastCode != 0xFD)
                o.Add(0xFF);

            return o.ToArray();
        }
    }
}
