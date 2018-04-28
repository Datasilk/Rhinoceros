using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace Rhinoceros
{
    public partial class Browser : Form
    {
        public ChromiumWebBrowser browser;
        public Container container;
        private Panel toolbar;
        private BrowserEvents events;

        public Browser()
        {
            InitializeComponent();

            //create container for chromium browser
            container = new Container
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(container);

            //create draggable toolbar
            toolbar = new Panel()
            {
                Dock = DockStyle.Top,
                BackColor = Color.PaleTurquoise,
                Height = 4
            };
            toolbar.MouseDown += Toolbar_MouseDown;
            Controls.Add(toolbar);
            
            //create chromium web browser
            var url = "http://markentingh.io";
            var paths = Application.LocalUserAppDataPath.Split('\\');
            var dataPath = string.Join("\\", paths.Take(paths.Length - 1)) + "\\";
            var settings = new CefSettings()
            {
                CachePath = dataPath + "Profile\\"
            };
            Cef.Initialize(settings);
            browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            container.Controls.Add(browser);

            //set up form window
            events = new BrowserEvents(this);
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            events.Normalize();
        }

        private void Toolbar_MouseDown(object sender, MouseEventArgs e)
        {
            events.DragWindow();
        }

        #region "borderless resizing"

        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;
        private const int grip = 10;

        Rectangle ResizeTop { get { return new Rectangle(0, 0, ClientSize.Width, grip); } }
        Rectangle ResizeLeft { get { return new Rectangle(0, 0, grip, ClientSize.Height); } }

        

        Rectangle ResizeBottom { get { return new Rectangle(0, ClientSize.Height - grip, ClientSize.Width, grip); } }
        Rectangle ResizeRight { get { return new Rectangle(ClientSize.Width - grip, 0, grip, ClientSize.Height); } }

        Rectangle ResizeTopLeft { get { return new Rectangle(0, 0, grip, grip); } }
        Rectangle ResizeTopRight { get { return new Rectangle(ClientSize.Width -grip, 0, grip, grip); } }
        Rectangle ResizeBottomLeft { get { return new Rectangle(0, ClientSize.Height - grip, grip, grip); } }
        Rectangle ResizeBottomRight { get { return new Rectangle(ClientSize.Width - grip, ClientSize.Height - grip, grip, grip); } }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x84)
            {
                //trap WM_NCHITTEST
                var cursor = PointToClient(Cursor.Position);

                if (ResizeTopLeft.Contains(cursor))
                {
                    m.Result = (IntPtr)HTTOPLEFT;
                }
                else if (ResizeTopRight.Contains(cursor))
                {
                    m.Result = (IntPtr)HTTOPRIGHT;
                }
                else if (ResizeBottomLeft.Contains(cursor))
                {
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                }
                else if (ResizeBottomRight.Contains(cursor))
                {
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                }
                else if (ResizeTop.Contains(cursor))
                {
                    m.Result = (IntPtr)HTTOP;
                }
                else if (ResizeLeft.Contains(cursor))
                {
                    m.Result = (IntPtr)HTLEFT;
                }
                else if (ResizeRight.Contains(cursor))
                {
                    m.Result = (IntPtr)HTRIGHT;
                }
                else if (ResizeBottom.Contains(cursor))
                {
                    m.Result = (IntPtr)HTBOTTOM;
                }
            }
        }

        #endregion
    }
}
 