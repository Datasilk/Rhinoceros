using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace Rhinoceros
{
    class AppOptions
    {
        //internal application options
        public string url = "http://localhost:7000/";
        public string title = "Rhinoceros";
        public Color borderColor = Color.FromKnownColor(KnownColor.WindowFrame);
        public int borderThickness = 4;
        public bool showDevTools = false;
        public ToolbarOptions toolbar = new ToolbarOptions();

        public class ToolbarOptions{
            
            public Color backgroundColor = Color.FromKnownColor(KnownColor.WindowFrame);
            public Color fontColor = Color.FromKnownColor(KnownColor.WindowText);
            public Padding padding = new Padding(4, 4, 15, 4);
            public int height = 27;
            public float fontSize = 12;
            public string fontFamily = "Arial";
        }
    }

    public partial class Browser : Form
    {
        //controls
        private ChromiumWebBrowser browser;
        private Container container;
        private Panel toolbar;
        private Label title;

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
        private Padding Grip { get { return new Padding(grip, 0, grip, grip); } }
        private int _dblClickedToolbar = 0;
        private bool _mouseDownToolbar = false;

        //constructor
        public Browser()
        {
            InitializeComponent();

            //set up browser options from config file
            grip = options.borderThickness;

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
                Padding = options.toolbar.padding,
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
            title.Text = options.title;
            title.Font = new Font(options.toolbar.fontFamily, options.toolbar.fontSize, FontStyle.Regular);
            title.AutoSize = true;
            toolbar.Controls.Add(title);

            //set up browser settings
            var paths = Application.LocalUserAppDataPath.Split('\\');
            var dataPath = string.Join("\\", paths.Take(paths.Length - 1)) + "\\";
            var settings = new CefSettings()
            {
                CachePath = dataPath + "Profile\\",
                LogSeverity = LogSeverity.Disable
            };

            //delete cache (optional)
            if(Directory.Exists(dataPath + "Profile\\Cache"))
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
                BackColor = options.borderColor
            };
            container.Controls.Add(browser);

            //set up browser internal events
            browser.IsBrowserInitializedChanged += (object sender, IsBrowserInitializedChangedEventArgs e) => {
                if(e.IsBrowserInitialized == true)
                {
                    //show DevTools
                    if(options.showDevTools == true)
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
            if(e.Clicks == 1)
            {
                _mouseDownToolbar = true;
            }
        }

        private void Toolbar_MouseMove(object sender, MouseEventArgs e)
        {
            if(_mouseDownToolbar == true)
            {
                Invoke(drag);
            }
        }

        private void Toolbar_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDownToolbar = false;
        }

        private void Toolbar_DoubleClick(object sender, EventArgs e)
        {
            _dblClickedToolbar = 2;
            _mouseDownToolbar = false;

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
                normalWidth = Width;
                normalHeight = Height;
            }
        }

        #endregion
    }
}
 