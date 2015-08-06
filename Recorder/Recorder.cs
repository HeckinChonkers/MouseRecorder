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
        public static string CaptureSequence = "", LoadedSequence = "";
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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string fileContents = string.Empty;
            if (!string.IsNullOrEmpty(CaptureSequence))
            {
                if (MessageBox.Show("You have a previous recording stored. Would you like to save that recording before opening a new one?", "Previous Recording Stored", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Recording|*.rcd";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader reader = new StreamReader(ofd.FileName))
                   fileContents = reader.ReadToEnd();
            }

            LoadedSequence = fileContents.Replace("\r\n", "");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoadedSequence) && string.IsNullOrEmpty(CaptureSequence))
            {
                    MessageBox.Show("No recording loaded! Please load a recording or make a new one to play it back.");
                    return;
            }
            else if (string.IsNullOrEmpty(LoadedSequence))
            {
                LoadedSequence = CaptureSequence;
            }

            MessageBox.Show("This will take over your mouse and perform the recording. Press okay when ready.");
            this.WindowState = FormWindowState.Minimized;
            Playback();
        }

        private void Playback()
        {
            string[] instructions = LoadedSequence.Split(';');
            for (int i = 0; i < instructions.Count(); i++)
            {
                if (instructions[i].StartsWith("{") && instructions[i].EndsWith("}"))
                    VirtualMouse.MoveTo(instructions[i]);
                else if (instructions[i] == "LC")
                    VirtualMouse.LeftClick();
                else if (instructions[i] == "RC")
                    VirtualMouse.RightClick();

                Thread.Sleep(2);
            }
        }
    }
}
