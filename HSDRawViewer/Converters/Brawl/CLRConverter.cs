using HSDRaw;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters.Brawl
{
    public class CLRConverter
    {
        public static readonly string ImportFilter = "CLR0 (*.clr0)|*.clr0";

        public static void Import(MatAnimManager manager, JointMap map)
        {
            var f = FileIO.OpenFile(ImportFilter, "");

            if (string.IsNullOrEmpty(f))
                return;

            using (var s = new FileStream(f, FileMode.Open))
            using (var e = new BinaryReaderExt(s))
                ImportFromCLR(e, map);
        }

        public static void ImportFromCLR(BinaryReaderExt e, JointMap map)
        {
            e.Seek(0);
            e.BigEndian = true;

            // check header
            if (!e.ReadString(4).Equals("CLR0"))
                throw new Exception("Invalid CLR0 header");

            e.Skip(4); // subfile length
            int file_version = e.ReadInt32();
            e.Skip(4); // brres offset

            // check file version
            if (file_version != 3)
                throw new Exception($"Unsupported CLR0 version {file_version}");

            uint section_offset = e.ReadUInt32();
            string anim_name = e.ReadString(e.ReadInt32(), -1);

            // clr header
            e.Skip(4); // unused
            int frame_count = e.ReadUInt16();
            int mat_count = e.ReadUInt16();
            int loop_flag = e.ReadInt32();

            // section 0
            e.Seek(section_offset);

            //brres index group
            BRRESGroup[] groups = BRRESGroup.ReadGroups(e);
            foreach (var g in groups)
            {
                System.Diagnostics.Debug.WriteLine($"{g.Name} {g.DataP.ToString("X8")}");
            }

        }
    }
}
