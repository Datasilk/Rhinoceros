using System;
using System.Windows.Forms;

namespace Rhinoceros
{
    public class BrowserEvents
    {
        private Browser window;
        private int grip = 4;
        public BrowserEvents(Browser window) { this.window = window; }

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseCapture();


        public void Maximize()
        {
            window.WindowState = FormWindowState.Maximized;
            window.container.Margin = new Padding(0);
        }

        public void Normalize()
        {
            window.WindowState = FormWindowState.Normal;
            window.container.Padding = new Padding(grip, 0, grip, grip);
        }

        public void Minimize()
        {
            window.WindowState = FormWindowState.Minimized;
            window.container.Padding = new Padding(grip, 0, grip, grip);
        }

        public void DragWindow()
        {
            ReleaseCapture();
            SendMessage(window.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        public void ChangeGrip(int grip) { this.grip = grip; }
    }
}
