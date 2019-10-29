using System.Windows.Forms;

namespace HSDRawViewer.Tools
{
    public class FileIO
    {
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
        public static string OpenFile(string filter)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = filter;

                if(d.ShowDialog() == DialogResult.OK)
                {
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
        public static string SaveFile(string filter, string defaultName)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = filter;

                d.FileName = defaultName;

                if (d.ShowDialog() == DialogResult.OK)
                {
                    return d.FileName;
                }
            }
            return null;
        }
    }
}
