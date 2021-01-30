using HSDRaw.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HSDRawViewer.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class JointMap
    {

        private Dictionary<int, string> _indexToName = new Dictionary<int, string>();

        public string this[int i]
        {
            get { return _indexToName.ContainsKey(i) ? _indexToName[i] : null; }
            set 
            {
                if (_indexToName.ContainsKey(i))
                    _indexToName[i] = value;
                else
                    _indexToName.Add(i, value);
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
            foreach (var v in _indexToName)
                if (v.Value == name)
                    return v.Key;

            return -1;
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
            var text = File.ReadAllText(filePath);
            text = Regex.Replace(text, @"\#.*", "");

            var lines = text.Split('\n');

            _indexToName.Clear();
            foreach (var r in lines)
            {
                var args = r.Split('=');

                if (args.Length == 2)
                {
                    var name = args[1].Trim();
                    var i = 0;
                    if (int.TryParse(new string(args[0].Where(c => char.IsDigit(c)).ToArray()), out i))
                    {
                        this[i] = name;
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
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (StreamWriter w = new StreamWriter(stream))
                if (_indexToName.Count > 0)
                {
                    foreach (var b in _indexToName)
                        w.WriteLine($"JOBJ_{b.Key}={b.Value}");
                }
                else
                {
                    if(root != null)
                    {
                        var bones = root.BreathFirstList;
                        var ji = 0;
                        foreach (var j in bones)
                        {
                            if (!string.IsNullOrEmpty(j.ClassName))
                                w.WriteLine($"JOBJ_{ji}={j.ClassName}");
                            ji++;
                        }
                    }
                }
        }
    }
}
