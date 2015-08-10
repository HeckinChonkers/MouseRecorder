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
        public static StringBuilder LoadedSequence;
        public static bool bCapRun, bThreadRunning, isDirty;
        public static Queue<string> CaptureQueue;
        BackgroundWorker CaptureWatcher = new BackgroundWorker();
        public static AutoResetEvent endKeyCaught = new AutoResetEvent(false);
        SaveFileDialog sfd;

        public Recorder()
        {
            CaptureQueue = new Queue<string>();
            LoadedSequence = new StringBuilder();
            CaptureWatcher.WorkerSupportsCancellation = true;
            CaptureWatcher.DoWork += CaptureWatcher_DoWork;
            InitializeComponent();
            bThreadRunning = true;
            isDirty = false;
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
                toolStripButton1.Image = Properties.Resources.offIcon;
            }
            else
            {
                if (CaptureQueue.Count > 0 && isDirty)
                {
                    if (MessageBox.Show("You have a previous recording stored. Would you like to save that recording before starting a new one?", "Previous Recording Stored", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        sfd = new SaveFileDialog();
                        sfd.Filter = "Recording | *.rcd";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            using (StreamWriter file = new StreamWriter(sfd.FileName))
                            {
                                foreach (string instr in CaptureQueue)
                                {
                                    file.Write(instr);
                                }
                            }
                        }
                    }
                    isDirty = false;
                }

                bCapRun = true;
                CaptureQueue.Clear();
                InterceptKeyboard.StartCapture();
                InterceptMouse.StartCapture();
                isDirty = true;
                toolStripButton1.Image = Properties.Resources.recordIcon;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string fileContents = string.Empty;
            if (CaptureQueue.Count > 0 && isDirty)
            {
                if (MessageBox.Show("You have a previous recording stored. Would you like to save that recording before opening a new one?", "Previous Recording Stored", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    sfd = new SaveFileDialog();
                    sfd.Filter = "Recording | *.rcd";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter file = new StreamWriter(sfd.FileName))
                        {
                            foreach (string instr in CaptureQueue)
                            {
                                file.Write(instr);
                            }
                        }
                    }
                }
                isDirty = false;
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

            CaptureQueue.Clear();
            LoadIntoCaptureQueue(fileContents);
        }

        private void LoadIntoCaptureQueue(string fileContents)
        {
            string[] instructions = fileContents.Split(';');
            foreach (string instr in instructions)
            {
                CaptureQueue.Enqueue(instr + ';');
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if ((LoadedSequence == null || LoadedSequence.ToString() == "") && CaptureQueue.Count == 0)
            {
                    MessageBox.Show("No recording loaded! Please load a recording or make a new one to play it back.");
                    return;
            }

            LoadedSequence.Clear();

            foreach (string instr in CaptureQueue)
            {
                LoadedSequence.Append(instr);
            }

            MessageBox.Show("This will take over your mouse and perform the recording. Press okay when ready.");
            this.WindowState = FormWindowState.Minimized;
            Playback();
        }

        private void Playback()
        {
            string[] instructions = LoadedSequence.ToString().Split(';');
            for (int i = 0; i < instructions.Count(); i++)
            {
                if (instructions[i].StartsWith("{") && instructions[i].EndsWith("}"))
                    VirtualMouse.MoveTo(instructions[i]);
                else if (instructions[i].StartsWith("[") && instructions[i].EndsWith("]"))
                {
                    int waitMS = Convert.ToInt32(instructions[i].Replace("[", "").Replace("]", ""));
                    Thread.Sleep(waitMS);
                }
                else if (instructions[i] == "LCD")
                    VirtualMouse.LeftDown();
                else if (instructions[i] == "RCD")
                    VirtualMouse.RightDown();
                else if (instructions[i] == "LCU")
                    VirtualMouse.LeftUp();
                else if (instructions[i] == "RCU")
                    VirtualMouse.RightUp();
                else if (instructions[i] == "MWU")
                    VirtualMouse.WheelUp();
                else if (instructions[i] == "MWD")
                    VirtualMouse.WheelDown();

                //Thread.Sleep(1);
            }

            this.WindowState = FormWindowState.Normal;
        }
    }
}
