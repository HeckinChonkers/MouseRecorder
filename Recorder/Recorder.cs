using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class Recorder : Form
    {
        public static string CaptureSequence = "";
        public static bool bCapRun = false, bThreadRunning = false;
        BackgroundWorker CaptureWatcher = new BackgroundWorker();
        public static AutoResetEvent endKeyCaught = new AutoResetEvent(false);
        SaveFileDialog sfd;

        public Recorder()
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
                if (!string.IsNullOrEmpty(CaptureSequence))
                {
                    if (MessageBox.Show("You have a previous recording stored. Would you like to save that recording before starting a new one?", "Previous Recording Stored", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        sfd = new SaveFileDialog();
                        sfd.Filter = "Recording | *.rcd";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            using (StreamWriter file = new StreamWriter(sfd.FileName))
                            {
                                file.WriteLine(CaptureSequence);
                            }
                        }
                    }

                    CaptureSequence = string.Empty;
                }
                bCapRun = true;
                InterceptKeyboard.StartCapture();
                InterceptMouse.StartCapture();
                toolStripButton1.Image = global::Recorder.Properties.Resources.recordIcon;
            }
        }
    }
}
