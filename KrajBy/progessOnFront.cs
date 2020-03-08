using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;

using Microsoft.Phone.Controls;

using Phone.Controls;

namespace KrajBy
{
    class progessOnFront
    {
        //Для диспатчера
        PhoneApplicationPage p;
        // Остановка
        bool stoped = false;

        private BackgroundWorker backgroundWorker;
        private Phone.Controls.ProgressIndicator progress;


        public void Show(PhoneApplicationPage sender)
        {
            stoped = false;
            p = sender;

            if (this.progress == null)
            {
                this.progress = new Phone.Controls.ProgressIndicator();
            }

            // Initiaze background worker
            if (backgroundWorker == null)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            }
            progress.ProgressType = ProgressTypes.WaitCursor;
            backgroundWorker.WorkerReportsProgress = false;

            progress.Show();
            backgroundWorker.RunWorkerAsync();
        }

        public void Hide()
        {
            stoped = true;
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            p.Dispatcher.BeginInvoke(() =>
            {
                progress.ProgressBar.Value = e.ProgressPercentage;
            }
         );
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            p.Dispatcher.BeginInvoke(() =>
            {
                progress.Hide();
            }
          );
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!stoped)
                Thread.Sleep(50);
        }
    
    }
}
