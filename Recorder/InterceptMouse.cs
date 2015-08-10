using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    class InterceptMouse
    {
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static long firstTime, secondTime;
        private static bool measureTime;
        private static Stopwatch stpWtch;

        public static void StartCapture()
        {
            stpWtch = new Stopwatch();
            measureTime = false;
            stpWtch.Start();
            _hookID = SetHook(_proc);
        }

        public static void StopCapture()
        {
            UnhookWindowsHookEx(_hookID);
            measureTime = true;
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            MSLLHOOKSTRUCT hookStruct;

            int deltaTime = 0;
            stpWtch.Stop();
            Recorder.CaptureQueue.Enqueue("[" + (stpWtch.ElapsedMilliseconds + 1) + "];");
            measureTime = true;
            stpWtch.Reset();
            stpWtch.Start();

            if (nCode >= 0)
            {
                hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                switch ((MouseMessages)wParam)
                {
                    case (MouseMessages.WM_MOUSEMOVE):
                        Recorder.CaptureQueue.Enqueue("{" + hookStruct.pt.x + ", " + hookStruct.pt.y + "};");
                        break;
                    case (MouseMessages.WM_LBUTTONDOWN):
                        Recorder.CaptureQueue.Enqueue("LCD;");
                        break;
                    case (MouseMessages.WM_RBUTTONDOWN):
                        Recorder.CaptureQueue.Enqueue("RCD;");
                        break;
                    case (MouseMessages.WM_LBUTTONUP):
                        Recorder.CaptureQueue.Enqueue("LCU;");
                        break;
                    case (MouseMessages.WM_RBUTTONUP):
                        Recorder.CaptureQueue.Enqueue("RCU;");
                        break;
                    case (MouseMessages.WM_MOUSEWHEEL):
                        long rotation = Convert.ToInt64(hookStruct.mouseData)/65536;
                        long delta = rotation/120;
                        if (delta == 1)
                            Recorder.CaptureQueue.Enqueue("MWU;");
                        else
                            Recorder.CaptureQueue.Enqueue("MWD;");
                        break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
