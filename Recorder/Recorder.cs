using System;
using System.Collections.Concurrent;
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
using WindowsInput;

namespace Recorder
{
    public partial class Recorder : Form
    {
        public static StringBuilder LoadedSequence;
        public static string OpenFilePath;
        public static bool bCapRun, bThreadRunning, isDirty;
        public static ConcurrentQueue<string> CaptureQueue;
        BackgroundWorker CaptureWatcher = new BackgroundWorker();
        public static AutoResetEvent endKeyCaught = new AutoResetEvent(false);
        SaveFileDialog sfd;

        public Recorder()
        {
            CaptureQueue = new ConcurrentQueue<string>();
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

                RecordBtn_Click(null, new EventArgs());
            }
        }

        public void RecordBtn_Click(object sender, EventArgs e)
        {
            OpenFilePath = string.Empty;
            if (bCapRun)
            {
                bCapRun = false;
                InterceptMouse.StopCapture();
                InterceptKeyboard.StopCapture();
                RecordBtn.Image = Properties.Resources.offIcon;
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
                CaptureQueue = new ConcurrentQueue<string>();
                InterceptKeyboard.StartCapture();
                InterceptMouse.StartCapture();
                isDirty = true;
                RecordBtn.Image = Properties.Resources.recordIcon;
            }
        }

        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            string fileContents = string.Empty;
            if (CaptureQueue.Count > 0 && isDirty)
            {
                DialogResult saveDialogResult = MessageBox.Show(
                    "You have a previous recording stored. Would you like to save that recording before opening a new one?",
                    "Previous Recording Stored", MessageBoxButtons.YesNo);
                if (saveDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveRecordingBtn_Click(this, new EventArgs());
                }
                else if (saveDialogResult == DialogResult.Cancel || saveDialogResult == DialogResult.Abort)
                {
                    return;
                }
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Recording|*.rcd";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader reader = new StreamReader(ofd.FileName))
                    fileContents = reader.ReadToEnd();
                OpenFilePath = ofd.FileName;
                CaptureQueue = new ConcurrentQueue<string>();
                if (!string.IsNullOrEmpty(fileContents))
                    LoadIntoCaptureQueue(fileContents);
                isDirty = false;
            }
            
        }

        private void LoadIntoCaptureQueue(string fileContents)
        {
            string[] instructions = fileContents.Split(';');
            foreach (string instr in instructions)
            {
                CaptureQueue.Enqueue(instr + ';');
            }
        }

        private void PlayBackBtn_Click(object sender, EventArgs e)
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

            DialogResult dr =
                MessageBox.Show("This will take over your mouse and perform the recording. Are you ready?", "Playback", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                this.WindowState = FormWindowState.Minimized;
                Playback();
            }
        }

        private void Playback()
        {
            bool shiftDown = false, ctrlDown = false, altDown = false;
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
                else if (instructions[i].StartsWith("(") && instructions[i].EndsWith(")"))
                {
                    string enteredKey = instructions[i].Replace("(", "").Replace(")", "");
                    if (enteredKey.Contains("SHIFT"))
                    {
                        shiftDown = true;
                        enteredKey = enteredKey.Replace("SHIFT&", "");
                    }
                    if (enteredKey.Contains("CONTROL"))
                    {
                        ctrlDown = true;
                        enteredKey = enteredKey.Replace("CONTROL&", "");
                    }
                    if (enteredKey.Contains("ALT"))
                    {
                        altDown = true;
                        enteredKey = enteredKey.Replace("ALT&", "");
                    }

                    int keystroke = Convert.ToInt32(enteredKey);
                    Console.WriteLine((VirtualKeyCode)keystroke);
                    switch(keystroke)
                    {
                        case (int)Keys.Enter:
                            InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
                            break;
                        default:
                            List<VirtualKeyCode> vkc = new List<VirtualKeyCode>();

                            if (shiftDown)
                            {
                                vkc.Add(VirtualKeyCode.LSHIFT);
                                shiftDown = false;
                            }

                            if (ctrlDown)
                            {
                                vkc.Add(VirtualKeyCode.LCONTROL);
                                ctrlDown = false;
                            }

                            if (altDown)
                            {
                                vkc.Add(VirtualKeyCode.LMENU);
                                altDown = false;
                            }

                            if (vkc.Count > 0)
                            {
                                InputSimulator.SimulateModifiedKeyStroke(vkc.ToArray(), (VirtualKeyCode) keystroke);
                            }
                            else
                            {
                                InputSimulator.SimulateKeyPress((VirtualKeyCode) keystroke);
                            }
                            break;

                    }
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

        private void SaveRecordingBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(OpenFilePath))
            {
                if (CaptureQueue.Count > 0 && isDirty)
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
                        isDirty = false;
                        MessageBox.Show("Saved recording!");
                    }
                }
                else
                {
                    MessageBox.Show("No recording to save!");
                }
            }
            else
            {
                if (CaptureQueue.Count > 0)
                {
                    if (
                        MessageBox.Show("This will overwrite the recording file. Are you sure?", "Overwrite Recording",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (StreamWriter file = new StreamWriter(OpenFilePath))
                        {
                            foreach (string instr in CaptureQueue)
                            {
                                file.Write(instr);
                            }
                        }
                        isDirty = false;
                        MessageBox.Show("Saved recording!");
                    }
                }
            }
            
        }

        private void SaveAsBtn_Click(object sender, EventArgs e)
        {
            if (CaptureQueue.Count > 0)
            {
                sfd = new SaveFileDialog();
                sfd.Filter = "Recording | *.rcd";
                if (!string.IsNullOrEmpty(OpenFilePath))
                {
                    sfd.InitialDirectory = OpenFilePath.Substring(0, OpenFilePath.LastIndexOf("\\"));
                    sfd.FileName = OpenFilePath.Substring(OpenFilePath.LastIndexOf("\\") + 1);
                }
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter file = new StreamWriter(sfd.FileName))
                    {
                        foreach (string instr in CaptureQueue)
                        {
                            file.Write(instr);
                        }
                    }
                    isDirty = false;
                    MessageBox.Show("Saved recording!");
                }
            }
            else
            {
                MessageBox.Show("No recording to save!");
            }
        }

        private void Recorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            CaptureWatcher.CancelAsync();
        }

        private void EditRecordingBtn_Click(object sender, EventArgs e)
        {
            EditRecording form = new EditRecording(CaptureQueue);
            form.Show();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int MapVirtualKey(int uCode, int uMapType);
    }
}
