using HSDRaw.Common;
using HSDRawViewer.Rendering.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HSDRawViewer.Tools.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public class JointMap
    {
        public static string FileFilter = @"Joint Map (*.ini)|*.ini";

        private class JointInfo
        {
            public string Name;
            public float Error = 0.001f;
        }

        private readonly Dictionary<int, JointInfo> _indexToName = new();

        public string this[int i]
        {
            get { return _indexToName.ContainsKey(i) ? _indexToName[i].Name : null; }
            set
            {
                if (_indexToName.ContainsKey(i))
                    _indexToName[i].Name = value;
                else
                    _indexToName.Add(i, new JointInfo() { Name = value });
            }
        }

        public int Count { get => _indexToName.Count > 0 ? _indexToName.Keys.Max() + 1 : 0; }

        /// <summary>
        /// 
        /// </summary>
        public JointMap()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public JointMap(string filepath)
        {
            Load(filepath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOf(string name)
        {
            foreach (KeyValuePair<int, JointInfo> v in _indexToName)
                if (v.Value.Name == name)
                    return v.Key;

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetError(int index)
        {
            if (_indexToName.ContainsKey(index))
                return _indexToName[index].Error;

            return 0.01f;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _indexToName.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load(string filePath)
        {
            string text = File.ReadAllText(filePath);
            text = Regex.Replace(text, @"\#.*", "");

            string[] lines = text.Split('\n');

            _indexToName.Clear();
            foreach (string r in lines)
            {
                string[] args = r.Split('=');

                if (args.Length >= 2)
                {
                    string name = args[1].Trim();
                    if (int.TryParse(new string(args[0].Where(c => char.IsDigit(c)).ToArray()), out int i))
                    {
                        this[i] = name;
                        if (args.Length > 2 && float.TryParse(args[2], out float result))
                        {
                            _indexToName[i].Error = result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath, HSD_JOBJ root = null)
        {
            using FileStream stream = new(filePath, FileMode.Create);
            using StreamWriter w = new(stream);
            if (_indexToName.Count > 0)
            {
                foreach (KeyValuePair<int, JointInfo> b in _indexToName)
                    w.WriteLine($"JOBJ_{b.Key}={b.Value.Name}");
            }
            else
            {
                if (root != null)
                {
                    List<HSD_JOBJ> bones = root.TreeList;
                    int ji = 0;
                    foreach (HSD_JOBJ j in bones)
                    {
                        if (!string.IsNullOrEmpty(j.ClassName))
                            w.WriteLine($"JOBJ_{ji}={j.ClassName}");
                        ji++;
                    }
                }
            }
        }

        internal void InitFrom(HSD_JOBJ jobj)
        {
            Clear();
            int index = 0;
            foreach (var j in jobj.TreeList)
            {
                if (!string.IsNullOrEmpty(j.ClassName))
                {
                    _indexToName.Add(index, new JointInfo()
                    {
                        Name = j.ClassName,
                    });
                }
                index++;
            }
        }
    }
}
