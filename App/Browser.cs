using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using CefSharp;
using CefSharp.WinForms;

namespace Rhinoceros
{
    class AppOptions
    {
        //internal application options
        public string url = "http://localhost:7000/";
        public string title = "Rhinoceros";
        public Color borderColor = Color.FromArgb(50, 50, 50);
        public int borderThickness = 5;
        public bool showDevTools = false;
        public ToolbarOptions toolbar = new ToolbarOptions();
        public MenuButtonOptions menuButton = new MenuButtonOptions();
        public class ToolbarOptions
        {

            public Color backgroundColor = Color.FromArgb(50, 50, 50);
            public Color fontColor = Color.FromArgb(255, 255, 255);
            public Padding padding = new Padding(8, 4, 8, 4);
            public int height = 45;
            public float fontSize = 12;
            public string fontFamily = "Segoe UI";
        }

        public class MenuButtonOptions : MenuButtonColorOptions
        {
            public int width = 50;
            public int height = 36;
        }
    }

    public partial class Browser : Form
    {
        //controls
        private ChromiumWebBrowser browser;
        private Container container;
        private Panel toolbar;
        private Label title;
        private MenuButton buttonClose;
        private MenuButton buttonMinimize;
        private MenuButton buttonMaximize;

        //Javascript-bound class
        private JsEvents events;

        //cross-thread delegates
        public delegate void Command();
        public Command maximize;
        public Command minimize;
        public Command normalize;
        public Command toggleMaximize;
        public Command drag;
        public Command exit;
        public Command useToolbar;
        public Command defaultTheme;

        public delegate void CommandColor(int r, int g, int b);
        public CommandColor borderColor;
        public CommandColor toolbarColor;
        public CommandColor toolbarFontColor;

        public delegate void CommandInt(int num);
        public CommandInt changeGripSize;

        public delegate void CommandStr(string str);
        public CommandStr changeTitle;

        //properties
        private AppOptions options = new AppOptions();
        private Padding Grip { get { return new Padding(grip, grip, grip, grip); } }
        private int _dblClickedToolbar = 0;
        private bool _mouseDownToolbar = false;

        //constructor
        public Browser()
        {
            InitializeComponent();

            //set up browser options from config file
            grip = options.borderThickness;
            gripCorner = grip * 4;

            //create container for chromium browser
            container = new Container
            {
                Dock = DockStyle.Fill,
                Padding = Grip,
                BackColor = options.borderColor
            };
            Controls.Add(container);

            //create draggable toolbar
            toolbar = new Panel()
            {
                Dock = DockStyle.Top,
                BackColor = options.toolbar.backgroundColor,
                ForeColor = options.toolbar.fontColor,
                Height = options.toolbar.height
            };
            toolbar.DoubleClick += Toolbar_DoubleClick;
            toolbar.MouseDown += Toolbar_MouseDown;
            toolbar.MouseMove += Toolbar_MouseMove;
            toolbar.MouseUp += Toolbar_MouseUp;
            Controls.Add(toolbar);

            //create controls for toolbar
            title = new Label();
            title.Dock = DockStyle.Left;
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Height = options.toolbar.height;
            title.Padding = options.toolbar.padding;
            title.Text = options.title;
            title.Font = new Font(options.toolbar.fontFamily, options.toolbar.fontSize, FontStyle.Regular);
            title.AutoSize = true;
            toolbar.Controls.Add(title);

            //create maximize button
            buttonMinimize = new MenuButton(ButtonType.minimize, options.menuButton);
            buttonMinimize.Width = options.menuButton.width;
            buttonMinimize.Height = options.menuButton.height;
            buttonMinimize.Dock = DockStyle.Right;
            buttonMinimize.BackColor = options.toolbar.backgroundColor;
            buttonMinimize.Click += ButtonMinimize_Click;
            toolbar.Controls.Add(buttonMinimize);

            //create maximize button
            buttonMaximize = new MenuButton(ButtonType.maximize, options.menuButton);
            buttonMaximize.Width = options.menuButton.width;
            buttonMaximize.Height = options.menuButton.height;
            buttonMaximize.Dock = DockStyle.Right;
            buttonMaximize.BackColor = options.toolbar.backgroundColor;
            buttonMaximize.Click += ButtonMaximize_Click;
            toolbar.Controls.Add(buttonMaximize);

            //create close button
            buttonClose = new MenuButton(ButtonType.close, options.menuButton);
            buttonClose.Width = options.menuButton.width;
            buttonClose.Height = options.menuButton.height;
            buttonClose.Dock = DockStyle.Right;
            buttonClose.BackColor = options.toolbar.backgroundColor;
            buttonClose.Click += ButtonClose_Click;
            toolbar.Controls.Add(buttonClose);

            //set up browser settings
            var paths = Application.LocalUserAppDataPath.Split('\\');
            var dataPath = string.Join("\\", paths.Take(paths.Length - 1)) + "\\";
            var settings = new CefSettings()
            {
                CachePath = dataPath + "Profile\\",
                LogSeverity = LogSeverity.Disable
            };
            var browserSettings = new BrowserSettings()
            {
                BackgroundColor = Utility.ColorToUInt(Color.FromArgb(35, 35, 35))
            };

            //delete cache (optional)
            if (Directory.Exists(dataPath + "Profile\\Cache"))
            {
                FileSystem.DeleteDirectory(dataPath + "Profile\\Cache");
            }

            //add command line arguments when creating web browser
            var cmdArgs = settings.CefCommandLineArgs;
            cmdArgs.Add("disable-plugin-discovery", "1");
            cmdArgs.Add("disable-direct-write", "1");

            Cef.Initialize(settings);
            Cef.EnableHighDPISupport();
            browser = new ChromiumWebBrowser(options.url)
            {
                Dock = DockStyle.Fill,
                BackColor = options.borderColor,
                MenuHandler = new CustomMenuHandler()
            };
            browser.BrowserSettings = browserSettings;
            container.Controls.Add(browser);

            //set up browser internal events
            browser.IsBrowserInitializedChanged += (object sender, IsBrowserInitializedChangedEventArgs e) =>
            {
                if (e.IsBrowserInitialized == true)
                {
                    //show DevTools
                    if (options.showDevTools == true)
                    {
                        browser.ShowDevTools();
                    }
                }
            };
            browser.TitleChanged += (object sender, TitleChangedEventArgs e) =>
            {
                Invoke(changeTitle, e.Title);
            };

            //set up form window
            events = new JsEvents();
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);

            //bind delegates for cross-thread purposes
            maximize = new Command(MaximizeWindow);
            normalize = new Command(NormalizeWindow);
            minimize = new Command(MinimizeWindow);
            drag = new Command(DragWindow);
            borderColor = new CommandColor(BorderColor);
            toolbarColor = new CommandColor(ToolbarColor);
            toolbarFontColor = new CommandColor(ToolbarFontColor);
            changeGripSize = new CommandInt(ChangeGripSize);
            exit = new Command(Exit);
            defaultTheme = new Command(DefaultTheme);
            changeTitle = new CommandStr(ChangeTitle);

            //bind to JavaScript
            browser.JavascriptObjectRepository.Register("Rhino", events, false);
        }

        //events
        #region "Events"
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            normalTop = Top;
            normalLeft = Left;
            normalWidth = Width;
            normalHeight = Height;
        }

        protected override void OnMove(EventArgs e)
        {
            if (isMaximized == true && _dblClickedToolbar == 0)
            {
                isMaximized = false;
                container.Padding = Grip;
                Width = normalWidth;
                Height = normalHeight;
            }
            else
            {
                _dblClickedToolbar -= 1;
            }
        }

        private void Toolbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 1)
            {
                _mouseDownToolbar = true;
                var max = false;
                if (isMaximized == true) { max = true; }
                var cursor = PointToClient(Cursor.Position);
                NormalizeWindow();
                if (max == true)
                {
                    Top = 0;
                    Left = cursor.X - (normalWidth / 2);
                    Debug.WriteLine(cursor.X + ", " + normalWidth);
                }
            }
        }

        private void Toolbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownToolbar == true)
            {
                Invoke(drag);
            }
        }

        private void Toolbar_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDownToolbar = false;
            normalTop = Top;
            normalLeft = Left;
        }

        private void Toolbar_DoubleClick(object sender, EventArgs e)
        {
            _dblClickedToolbar = 2;
            _mouseDownToolbar = false;
            ButtonMaximize_Click(null, null);
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void ButtonMinimize_Click(object sender, EventArgs e)
        {
            MinimizeWindow();
        }

        private void ButtonMaximize_Click(object sender, EventArgs e)
        {
            if (isMaximized == false)
            {
                MaximizeWindow();
            }
            else
            {
                NormalizeWindow();
            }
        }
        #endregion

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

        //Rectangle ResizeTop { get { return new Rectangle(0, 0, ClientSize.Width, grip); } }
        Rectangle ResizeLeft { get { return new Rectangle(0, 0, grip, ClientSize.Height); } }
        Rectangle ResizeBottom { get { return new Rectangle(0, ClientSize.Height - grip, ClientSize.Width, grip); } }
        Rectangle ResizeRight { get { return new Rectangle(ClientSize.Width - grip, 0, grip, ClientSize.Height); } }
        //Rectangle ResizeTopLeft { get { return new Rectangle(0, 0, grip, grip); } }
        //Rectangle ResizeTopRight { get { return new Rectangle(ClientSize.Width -grip, 0, grip, grip); } }
        Rectangle ResizeBottomLeft { get { return new Rectangle(0, ClientSize.Height - gripCorner, gripCorner, gripCorner); } }
        Rectangle ResizeBottomRight { get { return new Rectangle(ClientSize.Width - gripCorner, ClientSize.Height - gripCorner, gripCorner, gripCorner); } }

        protected override void WndProc(ref Message m)
        {
            //Debug.WriteLine(m.Msg + ": " + ((Native.WindowMessages)m.Msg).ToString());
            base.WndProc(ref m);
            if (m.Msg == (int)Native.WindowMessages.WM_NCHITTEST)
            {
                //trap WM_NCHITTEST
                var cursor = PointToClient(Cursor.Position);

                //if (ResizeTopLeft.Contains(cursor))
                //{
                //    m.Result = (IntPtr)HTTOPLEFT;
                //}
                //else if (ResizeTopRight.Contains(cursor))
                //{
                //    m.Result = (IntPtr)HTTOPRIGHT;
                //}
                if (ResizeBottomLeft.Contains(cursor))
                {
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                }
                else if (ResizeBottomRight.Contains(cursor))
                {
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                }
                //else if (ResizeTop.Contains(cursor))
                //{
                //    m.Result = (IntPtr)HTTOP;
                //}
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
                normalWidth = Width;
                normalHeight = Height;
            }else if(m.Msg == (int)Native.WindowMessages.WM_QUERYOPEN)
            {
                if(isMaximized == true)
                {
                    isMaximized = false;
                    MaximizeWindow();
                }
            }else if(m.Msg == (int)Native.WindowMessages.WM_LBUTTONUP ||
                m.Msg == (int)Native.WindowMessages.WM_NCLBUTTONUP ||
                m.Msg == (int)Native.WindowMessages.WM_MOUSEFIRST ||
                m.Msg == (int)Native.WindowMessages.WM_EXITSIZEMOVE)
            {
                Toolbar_MouseUp(null, null);
            }
        }

        #endregion
    }
}
