using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HSDRaw;
using System.IO;
using HSDRawViewer.Tools;
using System.ComponentModel;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXExternalFileControl : GroupBox
    {
        /// <summary>
        /// 
        /// </summary>
        private class FileSaver : ProgressClass
        {
            private List<FileTracker> trackers;

            public FileSaver(List<FileTracker> trackers)
            {
                this.trackers = trackers;
            }

            public override void Work(BackgroundWorker w)
            {
                int index = 0;
                foreach (var t in trackers)
                {
                    ProgressStatus = "Saving " + t.FileName;
                    t.Save();
                    w.ReportProgress((int)(((float)index / trackers.Count) * 100f));
                    index++;
                }

                w.ReportProgress(100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class FileTracker
        {
            public string FileName;

            public string[] Symbols;

            public bool Loaded { get => File != null; }

            private string FilePath;

            public HSDRawFile File;

            public Button LoadedLabel;

            public Button ImportButton;

            /// <summary>
            /// 
            /// </summary>
            public FileTracker()
            {
                ImportButton = (new Button() { Image = Properties.Resources.ts_importfile, Dock = DockStyle.Left, Width = 100, Height = 50 });
                LoadedLabel = (new Button() { Image = Properties.Resources.ts_x, Dock = DockStyle.Left, Width = 50, Height = 50, Enabled = false });

                ImportButton.Click += (sender, args) =>
                {
                    LoadFile();
                };
            }

            /// <summary>
            /// 
            /// </summary>
            private void LoadFile()
            {
                var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), FileName);

                if (!System.IO.File.Exists(path))
                    path = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, FileName);
                
                if (path != null)
                {
                    var file = new HSDRawFile(path);

                    if (file.Roots.Find(s => Symbols.Contains(s.Name)) != null)
                    {
                        File = file;
                        FilePath = path;
                        LoadedLabel.Image = Properties.Resources.ts_check;
                        ImportButton.Visible = false;

                        OnFileLoaded.Invoke(this, null);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Save()
            {
                if (Loaded)
                {
                    File.TrimData();
                    File.Save(FilePath);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                ImportButton.Dispose();
                LoadedLabel.Dispose();
            }
        }

        private List<FileTracker> Trackers = new List<FileTracker>();

        public static EventHandler OnFileLoaded;

        /// <summary>
        /// 
        /// </summary>
        public MEXExternalFileControl()
        {
            InitializeComponent();

            Text = "External Files";

            Disposed += (sender, args) =>
            {
                foreach (var t in Trackers)
                    t.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveFiles()
        {
            if (!Trackers.Exists(e => e.Loaded))
                return;

            using (ProgressBarDisplay pb = new ProgressBarDisplay(new FileSaver(Trackers)))
            {
                pb.DoWork();
                pb.ShowDialog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddExternal(string fileName, string[] symbols, string description = "")
        {
            FileTracker t = new FileTracker()
            {
                FileName = fileName,
                Symbols = symbols
            };
            Trackers.Add(t);

            Panel p = new Panel();
            p.Controls.Add(new Label() { Text = description, Dock = DockStyle.Left, Width = 800 });
            p.Controls.Add(t.ImportButton);
            p.Controls.Add(t.LoadedLabel);
            p.Controls.Add(new Label() { Text = $"\"{fileName}\"", Dock = DockStyle.Left, Width = 200 });
            p.Height = 24;

            p.Dock = DockStyle.Top;

            Controls.Add(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HSDAccessor GetSymbol(string symbol)
        {
            foreach (var tracker in Trackers)
            {
                if (tracker != null && tracker.Loaded && tracker.File.Roots.Find(e => e.Name == symbol) != null)
                    return tracker.File.Roots.Find(e => e.Name == symbol).Data;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HSDRawFile GetFile(string fileName)
        {
            var tracker = Trackers.Find(e => e.FileName == fileName);

            if (tracker != null && tracker.Loaded)
                return tracker.File;

            return null;
        }
    }
}
