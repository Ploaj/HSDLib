using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Tools.Optimizers
{
    public class FileSizeChecker
    {
        public static int AnimationHeap { get; } = 0x96c800;

        public static int FighterHeap { get; } = 0;//0x64B400 - 500 - 0x226ACE - 0x20000;

        private class Info
        {
            public string Id;
            public int DataFile;
            public int LargestCostumeFile;
            public int EffectFile;
            public int AnimationFile;

            public int FighterTotalSize { get => DataFile + LargestCostumeFile + EffectFile; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesystem"></param>
        /// <returns></returns>
        public static IEnumerable<string> RunReport(string filesystem)
        {
            //gather all player ids
            var plist = new List<Info>();

            // gather info
            foreach (var f in System.IO.Directory.GetFiles(filesystem))
            {
                var fname = System.IO.Path.GetFileNameWithoutExtension(f);
                if (fname.Length >= 4)
                {
                    var id = fname.Substring(2, 2);
                    if (id == "Co")
                        continue;

                    var p = plist.Find(e => e.Id == id);

                    if (p == null)
                    {
                        p = new Info() { Id = id };
                        plist.Add(p);
                    }

                    // data
                    if (fname.StartsWith("Pl") && fname.Length == 4)
                    {
                        p.DataFile = (int)new System.IO.FileInfo(f).Length;
                    }
                    // costume
                    if (fname.StartsWith("Pl") && fname.Length == 6)
                    {
                        if (fname.EndsWith("AJ"))
                        {
                            p.AnimationFile = (int)new System.IO.FileInfo(f).Length;
                        }
                        else
                        {
                            p.LargestCostumeFile = Math.Max(p.LargestCostumeFile, (int)new System.IO.FileInfo(f).Length);
                        }
                    }
                    // effect
                    if (fname.StartsWith("Ef") && fname.Length == 8)
                    {
                        p.EffectFile = (int)new System.IO.FileInfo(f).Length;
                    }
                }
            }

            // return size report
            foreach (var o in plist.OrderByDescending(e => e.FighterTotalSize))
            {
                if (o.AnimationFile != 0)
                    yield return $"{o.Id} DataSize: 0x{o.FighterTotalSize.ToString("X8")} " +
                        $"AnimationSize: 0x{o.AnimationFile.ToString("X8")} " +
                        $"TotalSize: 0x{(o.FighterTotalSize + o.AnimationFile).ToString("X8")} " +
                        $"{(o.FighterTotalSize > 0x000E9350 ? $"FighterData over limit! Reduce by: {(o.FighterTotalSize - 0x000E9350) / 1000}KB" : "")}";
            }

            // pick 3 largest animation files
            // start with zelda + sheik
            int animSize = plist.Find(e => e.Id == "Zd").AnimationFile + plist.Find(e => e.Id == "Sk").AnimationFile;
            var animOrder = plist.OrderByDescending(e => e.Id == "Sk" || e.Id == "Zd" || e.Id == "Gl" || e.Id == "Gk" || e.Id == "Bo" ? 0 : e.AnimationFile).ToArray();
            animSize += animOrder[0].AnimationFile + animOrder[1].AnimationFile + animOrder[2].AnimationFile;

            if (animSize > AnimationHeap)
                yield return $"Animation Heap Overflow: Zd, Sk, {animOrder[0].Id}, {animOrder[1].Id}, {animOrder[2].Id} reduce by {(animSize - AnimationHeap) / 1000}KB";

            // now do the same for data
            int dataSize = plist.Find(e => e.Id == "Zd").FighterTotalSize + plist.Find(e => e.Id == "Sk").FighterTotalSize;
            var dataOrder = plist.OrderByDescending(e => e.Id == "Sk" || e.Id == "Zd" ? 0 : e.FighterTotalSize).ToArray();

            if (dataSize > FighterHeap)
                yield return $"Fighter Heap Overflow: Zd, Sk, {dataOrder[0].Id}, {dataOrder[1].Id}, {dataOrder[2].Id}";

            //var s = new string[] { "Zd", "Sk", "Pe", "Fe", "Lc" };
            //System.Diagnostics.Debug.WriteLine("Animation: " + plist.Where(e => s.Contains(e.Id)).Sum(e => e.AnimationFile).ToString("X8"));

            //System.Diagnostics.Debug.WriteLine("Fighter: " + plist.Where(e => s.Contains(e.Id)).Sum(e => e.LargestCostumeFile + e.DataFile + e.EffectFile).ToString("X8"));
        }
    }
}
