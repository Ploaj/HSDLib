using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HSDRawViewer.GUI
{
    public partial class ProgressBarDisplay : Form
    {
        public delegate double WorkDelegate(object sender, DoWorkEventArgs e);
        private ProgressClass ProgressClass;

        public ProgressBarDisplay(ProgressClass p)
        {
            InitializeComponent();

            ProgressClass = p;

            backgroundWorker1.WorkerSupportsCancellation = false;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += p.DoWork;

            CenterToScreen();
        }

        public void DoWork()
        {
            if(!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
            progressBar1.Value = Math.Min(100, e.ProgressPercentage);
            Text = ProgressClass.ProgressStatus;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }

    public class ProgressClass
    {
        public string ProgressStatus { get; internal set; }

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Work(worker);
        }

        public virtual void Work(BackgroundWorker w)
        {

        }
    }

}
