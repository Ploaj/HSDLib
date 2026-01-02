using HSDRaw.Common;
using HSDRaw.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer.IO.Model
{
    internal class HsdImportHelper
    {
        public string FolderPath { get; set; }

        public POBJ_Generator PObjGenerator { get; set; } = new POBJ_Generator();

        public HSD_JOBJ Root { get; internal set; }

        public Dictionary<string, HSD_TOBJ> TextureLookup = new Dictionary<string, HSD_TOBJ>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="skl"></param>
        private HsdImportHelper(string folderPath, HsdSkl skl)
        {
            foreach (var b in skl.Bones)
            {
                foreach (var m in b.Mesh)
                {
                    foreach (var t in m.Material.Textures)
                    {
                        if (!string.IsNullOrEmpty(t.File) && TextureLookup.ContainsKey(t.File))
                            continue;

                        var path = System.IO.Path.Combine(folderPath, t.File);

                        if (System.IO.File.Exists(path))
                        {
                            var tobj = new HSD_TOBJ();
                            tobj.ImportImage(path, t.ImageFormat, t.TlutFormat);
                            TextureLookup.Add(t.File, tobj);
                        }
                    }
                }
            }

            // generate bone tree
            HSD_JOBJ[] jobjs = skl.Bones.Select(b => b.ToJObj()).ToArray();
            for (int i = 0; i < skl.Bones.Count; i++)
            {
                if (skl.Bones[i].Parent == -1)
                {
                    if (Root == null)
                        Root = jobjs[i];
                    continue;
                }
                jobjs[skl.Bones[i].Parent].AddChild(jobjs[i]);
            }

            // import mesh data
            for (int i = 0; i < skl.Bones.Count; i++)
            {
                if (skl.Bones[i].Mesh == null || skl.Bones[i].Mesh.Count == 0)
                    continue;

                var dobjs = skl.Bones[i].Mesh.Select(e => e.ToDObj(this)).ToList();
                for (int j = 0; j < dobjs.Count - 1; j++)
                    dobjs[j].Next = dobjs[j + 1];
                jobjs[i].Dobj = dobjs[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            PObjGenerator.SaveChanges();
            Root.UpdateFlags();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static HSD_JOBJ ImportSklToJObj(string filePath)
        {
            var skl = HsdJsonHelper.Import<HsdSkl>(filePath);
            var dir = Path.GetDirectoryName(filePath);
            HsdImportHelper hlp = new HsdImportHelper(dir, skl);
            hlp.Save();
            return hlp.Root;
        }
    }
}
