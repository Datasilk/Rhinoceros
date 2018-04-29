using System;
using System.Windows.Forms;
using System.Drawing;

namespace Rhinoceros
{
    public partial class Browser : Form
    {
        //window properties
        private int normalTop = 0;
        private int normalLeft = 0;
        private int normalWidth = 0;
        private int normalHeight = 0;
        private bool isMaximized = false;

        //dragging properties
        private int grip = 4;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseCapture();

        public void MaximizeWindow()
        {
            if (isMaximized == false)
            {
                normalTop = Top;
                normalLeft = Left;
                normalWidth = Width;
                normalHeight = Height;
                isMaximized = true;
                WindowState = FormWindowState.Normal;
                container.Padding = new Padding(0);
                Left = Top = 0;
                Width = Screen.FromControl(this).WorkingArea.Width;
                Height = Screen.FromControl(this).WorkingArea.Height;
            }
        }

        public void NormalizeWindow()
        {
            WindowState = FormWindowState.Normal;
            container.Padding = Grip;
            Top = normalTop;
            Left = normalLeft;
            Width = normalWidth;
            Height = normalHeight;
            isMaximized = false;
        }

        public void MinimizeWindow()
        {
            WindowState = FormWindowState.Minimized;
            container.Padding = Grip;
            Top = normalTop;
            Left = normalLeft;
            Width = normalWidth;
            Height = normalHeight;
            isMaximized = false;
        }

        public void DragWindow()
        {
            Width = normalWidth;
            Height = normalHeight;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        public void BorderColor(int r, int g, int b)
        {
            container.BackColor = Color.FromArgb(r, g, b);
        }

        public void ToolbarColor(int r, int g, int b)
        {
            toolbar.BackColor = Color.FromArgb(r, g, b);
        }

        public void ToolbarFontColor(int r, int g, int b)
        {
            toolbar.ForeColor = Color.FromArgb(r, g, b);
        }

        public void DefaultTheme()
        {
            container.BackColor = options.borderColor;
            toolbar.BackColor = options.toolbar.backgroundColor;
            toolbar.ForeColor = options.toolbar.fontColor;
            toolbar.Font = new Font(Font, options.toolbar.font);
        }

        public void ChangeGripSize(int size)
        {
            grip = size;
            if (isMaximized == false)
            {
                container.Padding = Grip;
            }
        }

        public void Exit()
        {
            Application.Exit();
        }
    }
}
