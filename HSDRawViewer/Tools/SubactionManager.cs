using HSDRaw.Tools.Melee;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace HSDRawViewer.Tools
{
    public class Subaction
    {
        public byte Code;

        public string Name;

        public string Description;

        public bool IsCustom = false;

        private int CodeSize
        {
            get
            {
                if (IsCustom)
                    return 8;
                else
                    return 6;
            }
        }

        public SubactionParameter[] Parameters = new SubactionParameter[0];

        public int BitSize
        {
            get
            {
                int size = CodeSize;

                if (Parameters == null || Parameters.Length == 0)
                    return 32;

                foreach (var param in Parameters)
                {
                    size += param.BitCount;
                }
                return size;
            }
        }

        public int ByteSize => BitSize / 8;

        public bool HasPointer
        {
            get
            {
                foreach (var v in Parameters)
                    if (v.IsPointer)
                        return true;
                return false;
            }
        }

        public int[] GetParameters(byte[] data)
        {
            Bitreader r = new Bitreader(data);

            r.Read(CodeSize);

            List<int> param = new List<int>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                var p = Parameters[i];

                if (p.Name.Contains("None"))
                    continue;

                var value = p.Signed ? r.ReadSigned(p.BitCount) : r.Read(p.BitCount);

                if (p.IsPointer)
                    continue;

                param.Add(value);
            }

            return param.ToArray();
        }

        public byte[] Compile(int[] parameters)
        {
            BitWriter w = new BitWriter();

            w.Write(Code >> (IsCustom ? 0 : 2), CodeSize);

            for(int i = 0; i < Parameters.Length; i++)
            {
                var bm = Parameters[i];

                if (bm.Name.Contains("None") || bm.IsPointer)
                {
                    w.Write(0, bm.BitCount);
                    continue;
                }

                var value = parameters[i];

                w.Write(value, bm.BitCount);
            }

            // they should all theoretically be aligned to 32 bits
            if (Parameters.Length == 0 && !IsCustom)
                w.Write(0, 26);

            return w.Bytes.ToArray();
        }

    }

    public class SubactionParameter
    {
        public string Name;

        public int BitCount;

        public string[] Enums;

        public string Description;

        public bool Signed = false;

        public bool Hex = false;

        public bool IsFloat = false;

        public bool IsPointer => Name.Equals("Pointer");

        public bool HasEnums => Enums != null && Enums.Length > 0;
    }

    public enum SubactionGroup
    {
        Fighter, 
        Item
    }

    public class SubactionManager
    {
        public static List<Subaction> FighterSubactions
        {
            get
            {
                if (_fighterSubactions == null)
                    LoadFromFile();
                return _fighterSubactions;
            }
        }
        private static List<Subaction> _fighterSubactions;

        public static List<Subaction> ItemSubactions
        {
            get
            {
                if (_itemSubactions == null)
                    LoadFromFile();
                return _itemSubactions;
            }
        }
        private static List<Subaction> _itemSubactions;

        public static List<Subaction> CustomSubactions
        {
            get
            {
                if (_customSubactions == null)
                    LoadFromFile();
                return _customSubactions;
            }
        }
        private static List<Subaction> _customSubactions;

        /// <summary>
        /// 
        /// </summary>
        private static void LoadFromFile()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string sa = "";
            string fsa = "";
            string isa = "";
            string csa = "";

            if (File.Exists(@"Melee\subactions_control.yaml"))
                sa = File.ReadAllText(@"Melee\subactions_control.yaml");

            if (File.Exists(@"Melee\subactions_fighter.yaml"))
                fsa = File.ReadAllText(@"Melee\subactions_fighter.yaml");

            if (File.Exists(@"Melee\subactions_item.yaml"))
                isa = File.ReadAllText(@"Melee\subactions_item.yaml");

            if (File.Exists(@"Melee\subactions_custom.yaml"))
                csa = File.ReadAllText(@"Melee\subactions_custom.yaml");

            var subs = deserializer.Deserialize<Subaction[]>(sa);
            var fsubs = deserializer.Deserialize<Subaction[]>(fsa);
            var isubs = deserializer.Deserialize<Subaction[]>(isa);
            var customsubs = deserializer.Deserialize<Subaction[]>(csa);

            if (subs != null && subs.Length != 0)
            {
                foreach (var s in subs)
                    s.Code <<= 2;

                _fighterSubactions = new List<Subaction>();
                if (fsubs != null && fsubs.Length != 0)
                {
                    foreach (var s in fsubs)
                        s.Code <<= 2;
                    _fighterSubactions.AddRange(subs);
                    _fighterSubactions.AddRange(fsubs);
                }

                _itemSubactions = new List<Subaction>();
                if (subs != null && subs.Length != 0)
                {
                    foreach (var s in isubs)
                        s.Code <<= 2;
                    _itemSubactions.AddRange(subs);
                    _itemSubactions.AddRange(isubs);
                }
            }

            _customSubactions = new List<Subaction>();
            if (customsubs != null && customsubs.Length != 0)
            {
                foreach (var s in customsubs)
                    s.IsCustom = true;
                _customSubactions.AddRange(customsubs);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Subaction GetSubaction(byte code, SubactionGroup group)
        {
            var sa = CustomSubactions.Find(e => e.Code == code && e.IsCustom);

            if (sa == null)
                sa = GetGroup(group).Find(e => e.Code == (code & 0xFC) && !e.IsCustom);

            return sa;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Subaction GetSubaction(string name, SubactionGroup group)
        {
            return GetGroup(group).Find(e => e.Name == name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static List<Subaction> GetGroup(SubactionGroup group)
        {
            switch (group)
            {
                case SubactionGroup.Item:
                    return ItemSubactions;
                default:
                    return FighterSubactions;
            }
        }
    }

    public sealed class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        private readonly ObjectSerializer nestedObjectSerializer;
        private readonly IEnumerable<IYamlTypeConverter> typeConverters;

        public YamlIEnumerableSkipEmptyObjectGraphVisitor
                          (IObjectGraphVisitor<IEmitter> nextVisitor,
                           IEnumerable<IYamlTypeConverter> typeConverters,
                           ObjectSerializer nestedObjectSerializer)
            : base(nextVisitor)
        {
            this.typeConverters = typeConverters != null
                ? typeConverters.ToList()
                : Enumerable.Empty<IYamlTypeConverter>();

            this.nestedObjectSerializer = nestedObjectSerializer;
        }

        public override bool Enter(IObjectDescriptor value, IEmitter context)
        {
            bool retVal;

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(value.Value.GetType()))
            {   // We have a collection
                var enumerableObject = (System.Collections.IEnumerable)value.Value;
                if (enumerableObject.GetEnumerator().MoveNext()) // Returns true if the collection is not empty.
                {   // Serialize it as normal.
                    retVal = base.Enter(value, context);
                }
                else
                {   // Skip this item.
                    retVal = false;
                }
            }
            else
            {   // Not a collection, normal serialization.
                retVal = base.Enter(value, context);
            }

            return retVal;
        }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            bool retVal = false;

            if (value.Value == null)
                return retVal;

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(value.Value.GetType()))
            {   // We have a collection
                var enumerableObject = (System.Collections.IEnumerable)value.Value;
                if (enumerableObject.GetEnumerator().MoveNext()) // Returns true if the collection is not empty.
                {   // Don't skip this item - serialize it as normal.
                    retVal = base.EnterMapping(key, value, context);
                }
                // Else we have an empty collection and the initialized return value of false is correct.
            }
            else
            {   // Not a collection, normal serialization.
                retVal = base.EnterMapping(key, value, context);
            }

            return retVal;
        }
    }
}
