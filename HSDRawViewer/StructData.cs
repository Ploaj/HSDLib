using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRawViewer
{
    public class StructData
    {
        public string Name { get; set; }

        public Dictionary<int, string> Map { get; set; } = new Dictionary<int, string>();

        public void Read(StreamReader r)
        {
            Name = r.ReadLine();
            var line = r.ReadLine();
            while(line != "end")
            {
                var args = line.Split(',');
                Map.Add(int.Parse(args[0], System.Globalization.NumberStyles.HexNumber), args[1]);
                line = r.ReadLine();
            }
        }
    }
}
