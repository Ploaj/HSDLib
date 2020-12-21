using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GCILib;
using HSDRawViewer.Tools;
using System.IO;
using System.Collections;
using HSDRawViewer.GUI.Extra;

namespace HSDRawViewer.GUI.Controls
{
    public partial class ISOEditor : UserControl
    {
        private GCISO _iso;
        private GCBanner _banner;
        
        // game data
        public string GameName { get => _iso.GameName; set => _iso.GameName = value; }
        public string GameCode { get => _iso.GameCodeString; set => _iso.GameCodeString = value; }
        public string MakerName { get => _iso.MakerCodeString; set => _iso.MakerCodeString = value; }

        public string OpenFilePath = null;
        public byte[] OpenFile = null;

        // banner data
        public string ShortName
        {
            get => _banner?.MetaData.ShortName;
            set
            {
                var md = _banner.MetaData;
                md.ShortName = value;
                _banner.MetaData = md;
                SaveBannerChanges();
            }
        }
        public string ShortMaker
        {
            get => _banner?.MetaData.ShortMaker;
            set
            {
                var md = _banner.MetaData;
                md.ShortMaker = value;
                _banner.MetaData = md;
                SaveBannerChanges();
            }
        }
        public string LongName
        {
            get => _banner?.MetaData.LongName;
            set
            {
                var md = _banner.MetaData;
                md.LongName = value;
                _banner.MetaData = md;
                SaveBannerChanges();
            }
        }
        public string LongMaker
        {
            get => _banner?.MetaData.LongMaker;
            set
            {
                var md = _banner.MetaData;
                md.LongMaker = value;
                _banner.MetaData = md;
                SaveBannerChanges();
            }
        }
        public string Description
        {
            get => _banner?.MetaData.Description;
            set
            {
                var md = _banner.MetaData;
                md.Description = value;
                _banner.MetaData = md;
                SaveBannerChanges();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="iso"></param>
        public ISOEditor(GCISO iso)
        {
            InitializeComponent();

            _iso = iso;

            var icons = new ImageList();
            icons.ImageSize = new System.Drawing.Size(24, 24);
            icons.Images.Add("file", Properties.Resources.ico_file);
            icons.Images.Add("disc", Properties.Resources.iso_disc);
            icons.Images.Add("folder", Properties.Resources.ico_folder);
            icons.Images.Add("hsd", Properties.Resources.ico_hatchling);
            fileTree.ImageList = icons;
            fileTree.Enabled = true;
            fileTree.NodeMouseClick += (sender, args) => fileTree.SelectedNode = args.Node;


            SetupFileTree();

            gameDataPanel.Enabled = true;
            tbGameName.DataBindings.Add("Text", this, "GameName");
            tbGameCode.DataBindings.Add("Text", this, "GameCode");
            tbMakerCode.DataBindings.Add("Text", this, "MakerName");

            _banner = _iso.FindBanner();
            if(_banner != null)
            {
                bannerDataPanel.Enabled = true;
                
                tbShortName.DataBindings.Add("Text", this, "ShortName");
                tbShortMaker.DataBindings.Add("Text", this, "ShortMaker");
                tbLongName.DataBindings.Add("Text", this, "LongName");
                tbLongMaker.DataBindings.Add("Text", this, "LongMaker");
                tbDescription.DataBindings.Add("Text", this, "Description");
                
                pictureIconBox.Image = BitmapTools.RGBAToBitmap(_banner.GetBannerImageRGBA8(), 96, 32);;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveBannerChanges()
        {
            if (_banner != null)
                _iso.SetBanner(_banner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            var file = FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

            if (file != null)
                pictureIconBox.Image.Save(file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            var file = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

            if (file != null)
            {
                var bmp = new Bitmap(file);

                // resize image if needed
                if(bmp.Width != 96 || bmp.Height != 32)
                {
                    var resize = BitmapTools.ResizeImage(bmp, 96, 32);
                    bmp.Dispose();
                    bmp = resize;
                }

                // set banner data
                _banner.SetBannerImageRGBA8(bmp.GetRGBAData());

                // update image display
                if (pictureIconBox.Image != null)
                    pictureIconBox.Image.Dispose();

                pictureIconBox.Image = bmp;

                // save banner changes
                SaveBannerChanges();
            }
        }

        #region File Tree

        private static string GetISOPath(TreeNode node)
        {
            var trimString = "root\\";

            string result = node.FullPath;

            if (result.StartsWith(trimString))
                result = result.Substring(trimString.Length);

            return result;
        }

        internal class RootNode : TreeNode
        {
            public RootNode()
            {
                Text = "root";
                ImageKey = "disc";
                SelectedImageKey = "disc";
            }
        }

        internal class FolderNode : TreeNode
        {
            public FolderNode()
            {
                Text = "root";
                ImageKey = "folder";
                SelectedImageKey = "folder";
            }
        }

        internal class FileNode : TreeNode
        {
            public FileNode()
            {
                Text = "root";
                ImageKey = "file";
                SelectedImageKey = "file";
            }

            public void UpdateImage(string text)
            {
                if(Text.EndsWith(".dat") || Text.EndsWith(".usd"))
                {
                    ImageKey = "hsd";
                    SelectedImageKey = "hsd";
                }
                else
                {
                    ImageKey = "file";
                    SelectedImageKey = "file";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetupFileTree()
        {
            var root = new RootNode();

            fileTree.Nodes.Clear();
            
            var files = _iso.GetAllFilePaths();

            var separators = new char[] {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
                };

            foreach (var f in files)
            {
                string[] directories = f.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                TreeNode node = root;

                for(int i = 0; i < directories.Length; i++)
                {
                    if(i == directories.Length - 1)
                    {
                        var file = new FileNode() { Text = directories[i] };
                        file.UpdateImage(directories[i]);
                        node.Nodes.Add(file);
                    }
                    else
                    {
                        FolderNode parent = null;
                        foreach (TreeNode n in node.Nodes)
                            if (n is FolderNode folder && folder.Text == directories[i])
                            {
                                parent = folder;
                                break;
                            }
                        if (parent == null)
                        {
                            parent = new FolderNode() { Text = directories[i] };
                            node.Nodes.Add(parent);
                        }
                        node = parent;
                    }
                }
            }
            
            SortNodes();

            fileTree.Nodes.Add(root);
            //fileTree.ExpandAll();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SortNodes()
        {
            fileTree.TreeViewNodeSorter = new NodeSorter();
            fileTree.Sort();
        }

        /// <summary>
        ///  Create a node sorter that implements the IComparer interface.
        /// </summary>
        public class NodeSorter : IComparer
        {
            // compare between two tree nodes
            public int Compare(object thisObj, object otherObj)
            {
                // Compare the types of the tags, returning the difference.
                if (otherObj is FolderNode && !(thisObj is FolderNode))
                    return 1;

                //alphabetically sorting
                return ((TreeNode)thisObj).Text.CompareTo(((TreeNode)otherObj).Text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node is FileNode file)
            {
                var path = GetISOPath(file);
                _iso.RenameFile(path, Path.GetDirectoryName(path) + "\\" + Path.GetFileName(e.Label));
                file.UpdateImage(e.Label);
                fileTree.BeginInvoke(new MethodInvoker(fileTree.Sort));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node is RootNode)
                e.CancelEdit = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            fileTree.SelectedNode = fileTree.GetNodeAt(e.Location);

            if (e.Button == MouseButtons.Right)
            {
                if (e.Node is FolderNode folder)
                    folderContextMenu.Show(fileTree, e.Location);
                if (e.Node is FileNode file)
                    fileContextMenu.Show(fileTree, e.Location);
                if (e.Node is RootNode root)
                    rootContextMenu.Show(fileTree, e.Location);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is FileNode file)
            {
                var f = FileIO.SaveFile("", file.Text);

                if (f != null)
                    File.WriteAllBytes(f, _iso.GetFileData(GetISOPath(file)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is FileNode file)
            {
                var f = FileIO.OpenFile("", file.Text);

                var path = GetISOPath(file);

                if (f != null)
                {
                    _iso.ReplaceFile(path, f);
                    MessageBox.Show("File replaced " + f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is FileNode file && MessageBox.Show("Delete " + file.Text + "?", "Delete File", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                var path = GetISOPath(file);

                Console.WriteLine(path);

                if (_iso.DeleteFileOrFolder(path))
                    file.Parent.Nodes.Remove(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is FolderNode folder && MessageBox.Show("Delete " + folder.Text + "?", "Delete File", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                var path = GetISOPath(folder);

                if (_iso.DeleteFileOrFolder(path))
                    folder.Parent.Nodes.Remove(folder);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is FolderNode folder)
            {
                var f = FileIO.OpenFolder();

                if (f != null)
                    ExtractFolder(folder, f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        private void ExtractFolder(TreeNode folder, string path = "")
        {
            path += folder.Text + "\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var n in folder.Nodes)
            {
                if (n is FileNode file)
                    File.WriteAllBytes(path + file.Text, _iso.GetFileData(GetISOPath(file)));

                if (n is FolderNode ff)
                    ExtractFolder(ff, path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is RootNode root)
            {
                var f = FileIO.OpenFolder();

                if (f != null)
                    ExtractFolder(root, f);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is FolderNode folder)
            {
                var f = FileIO.OpenFile("");

                var path = GetISOPath(folder);

                if (f != null)
                {
                    var fname = Path.GetFileName(f);
                    if (_iso.AddFile(path + "\\" + fname, f))
                    {
                        var file = new FileNode() { Text = fname };
                        file.UpdateImage(fname);
                        folder.Nodes.Add(file);
                        SortNodes();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is RootNode root)
            {
                var f = FileIO.OpenFile("");

                var path = GetISOPath(root);

                if (f != null)
                {
                    var fname = Path.GetFileName(f);

                    if (_iso.AddFile(fname, f))
                    {
                        var file = new FileNode() { Text = fname };
                        file.UpdateImage(fname);
                        root.Nodes.Add(file);
                        SortNodes();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left && fileTree.SelectedNode is FileNode file)
            {
                OpenFilePath = GetISOPath(file);
                OpenFile = _iso.GetFileData(OpenFilePath);
                
                if (Parent is ISOFileTool tool)
                    tool.Close();
            }
        }

        #endregion

    }
}
