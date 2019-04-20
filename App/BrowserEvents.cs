using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using CefSharp;

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
        private bool isMaximizing = false;

        //dragging properties
        private int grip = 4;
        private int gripCorner = 20;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseCapture();

        public void MaximizeWindow()
        {
            if (isMaximized == false && isMaximizing == false)
            {
                isMaximizing = true;
                WindowState = FormWindowState.Normal;
                normalTop = Top;
                normalLeft = Left;
                normalWidth = Width;
                normalHeight = Height;
                isMaximized = true;
                container.Padding = new Padding(0);
                Left = Top = 0;
                Width = Screen.FromControl(this).WorkingArea.Width;
                Height = Screen.FromControl(this).WorkingArea.Height;
                isMaximizing = false;
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
        }

        public void DragWindow()
        {   
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
            title.ForeColor = Color.FromArgb(r, g, b);
        }

        public void ToolbarButtonColors(MenuButtonColorOptions colors)
        {
            buttonClose.UpdateColors(colors);
            buttonMaximize.UpdateColors(colors);
            buttonMinimize.UpdateColors(colors);
        }

        public void DefaultTheme()
        {
            container.BackColor = options.borderColor;
            toolbar.BackColor = options.toolbar.backgroundColor;
            toolbar.ForeColor = options.toolbar.fontColor;
            title.Font = new Font(options.toolbar.fontFamily, options.toolbar.fontSize, FontStyle.Regular);
            title.ForeColor = options.toolbar.fontColor;
            ToolbarButtonColors(options.button);
        }

        public void ChangeGripSize(int size)
        {
            grip = size;
            if (isMaximized == false)
            {
                container.Padding = Grip;
            }
        }

        public void ChangeTitle(string str)
        {
            title.Text = str;
        }

        public void NewWindow()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = Directory.GetCurrentDirectory() + "\\Rhinoceros.exe";
            start.WindowStyle = ProcessWindowStyle.Normal;
            Process proc = Process.Start(start);
        }

        public void Exit()
        {
            Application.Exit();
        }

        public class CustomMenuHandler : IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {

                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }
    }
}
