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
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = filter;

                if (d.ShowDialog() == DialogResult.OK)
                {
                    return d.FileName;
                }
            }
            return null;
        }
    }
}
