using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class Form1 : Form
    {
        public static string CaptureSequence = "";
        public static bool bCapRun = false, bThreadRunning = false;
        BackgroundWorker CaptureWatcher = new BackgroundWorker();
        public static AutoResetEvent endKeyCaught = new AutoResetEvent(false);

        public Form1()
        {
            CaptureWatcher.WorkerSupportsCancellation = true;
            CaptureWatcher.DoWork += CaptureWatcher_DoWork;
            InitializeComponent();
            bThreadRunning = true;
            CaptureWatcher.RunWorkerAsync();
        }

        void CaptureWatcher_DoWork(object sender, DoWorkEventArgs e)
        {
            while (bThreadRunning)
            {
                endKeyCaught.WaitOne();

                toolStripButton1_Click(null, new EventArgs());
            }
        }

        public void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (bCapRun)
            {
                bCapRun = false;
                InterceptMouse.StopCapture();
                InterceptKeyboard.StopCapture();
                Console.WriteLine(CaptureSequence);
                toolStripButton1.Image = global::Recorder.Properties.Resources.offIcon;
            }
            else
            {
                bCapRun = true;
                InterceptKeyboard.StartCapture();
                InterceptMouse.StartCapture();
                toolStripButton1.Image = global::Recorder.Properties.Resources.recordIcon;
            }
        }
    }
}
