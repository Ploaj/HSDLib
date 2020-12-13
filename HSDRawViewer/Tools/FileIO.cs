using System;
using System.IO;
using System.Windows.Forms;
using VCDiff.Decoders;
using VCDiff.Encoders;
using VCDiff.Includes;

namespace HSDRawViewer.Tools
{
    public class FileIO
    {
        private static string PrevSaveLocation = null;

        private static string PrevOpenLocation = null;

        public static string NORMAL_EXTENSIONS = "HSD (*.dat,*.usd,*.ssm,*.sem)|*.dat;*.usd;*.ssm;*.sem";

        public static string DIFF_EXTENSIONS = "HSD (*.dat.diff,*.usd.diff,*.ssm.diff,*.sem.diff, *.diff)|*.diff";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string OpenFolder()
        {
            using (OpenFolderDialog d = new OpenFolderDialog())
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    return d.SelectedPath;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string OpenFile(string filter, string filename = "")
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = filter;
                d.FileName = filename;

                if (PrevOpenLocation != null)
                {
                    d.InitialDirectory = PrevOpenLocation;
                }

                if(d.ShowDialog() == DialogResult.OK)
                {
                    PrevOpenLocation = Path.GetDirectoryName(d.FileName);
                    return d.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string[] OpenFiles(string filter)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = filter;
                d.Multiselect = true;

                if (d.ShowDialog() == DialogResult.OK)
                {
                    return d.FileNames;
                }
            }
            return null;
        }

        internal static void SaveDiffToFile(Stream origStream, Stream modifiedStream, Stream diffStream)
        {
            VCCoder coder = new VCCoder(origStream, modifiedStream, diffStream);
            VCDiffResult result = coder.Encode(); //encodes with no checksum and not interleaved
            if (result != VCDiffResult.SUCCESS)
            {
                //error was not able to encode properly
            }
        }

        internal static void MergeDiffToDat(Stream origStream, Stream modifiedStream, Stream mergedStream)
        {
            VCDecoder decoder = new VCDecoder(origStream, modifiedStream, mergedStream);
            VCDiffResult result = decoder.Start(); //encodes with no checksum and not interleaved
            if (result != VCDiffResult.SUCCESS)
            {
                //error was not able to encode properly
            }
            else
            {
                long bytesWritten = 0;
                result = decoder.Decode(out bytesWritten);

                if (result != VCDiffResult.SUCCESS)
                {

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string SaveFile(string filter)
        {
            return SaveFile(filter, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string SaveFile(string filter, string defaultName, string caption = "Save File")
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Title = caption;
                d.Filter = filter;

                d.FileName = defaultName;

                if (PrevSaveLocation != null)
                {
                    d.InitialDirectory = PrevSaveLocation;
                }

                if (d.ShowDialog() == DialogResult.OK)
                {
                    PrevSaveLocation = Path.GetDirectoryName(d.FileName);
                    return d.FileName;
                }
            }
            return null;
        }
    }
}
