using System;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

namespace Rhinoceros
{
    public class JsEvents
    {
        private Browser _window;
        private Browser Window {
            get {
                //we want to have full control of how JavaScript accesses the Rhinoceros Browser window,
                //so we bound JavaScript to this class instead of the Browser Form class, and then we'll
                //obtain a reference to the Browser Form securely from here
                if(_window == null) { _window = Application.OpenForms.OfType<Browser>().First(); }
                return _window;
            }
        }
        public void maximize()
        {
            Window.Invoke(Window.maximize);
        }

        public void normalize()
        {
            Window.Invoke(Window.maximize);
        }

        public void minimize()
        {
            Window.Invoke(Window.maximize);
        }

        public void dragwindow()
        {
            Window.Invoke(Window.drag);
        }

        public void bordercolor(int r, int g, int b)
        {
            Window.Invoke(Window.borderColor, r, g, b);
        }

        public void toolbarcolor(int r, int g, int b)
        {
            Window.Invoke(Window.toolbarColor, r, g, b);
        }

        public void toolbarfontcolor(int r, int g, int b)
        {
            Window.Invoke(Window.toolbarFontColor, r, g, b);
        }

        public void toolbarbuttoncolors(int bg, int bghover, int bgmousedown, int font, int fonthover, int fontmousedown)
        {
            var colors = new MenuButtonColorOptions();
            colors.backgroundColor = Color.FromArgb(bg);
            colors.backgroundHoverColor = Color.FromArgb(bghover);
            colors.backgroundMouseDownColor = Color.FromArgb(bgmousedown);
            colors.fontColor = Color.FromArgb(font);
            colors.fontHoverColor = Color.FromArgb(fonthover);
            colors.fontMouseDownColor = Color.FromArgb(fontmousedown);
            Window.Invoke(Window.toolbarButtonColors, colors);
        }

        public void defaulttheme()
        {
            Window.Invoke(Window.defaultTheme);
        }

        public void bordersize(int grip)
        {
            Window.Invoke(Window.changeGripSize, grip);
        }

        public void drag()
        {
            Window.Invoke(Window.drag);
        }

        public void exit()
        {
            Window.Invoke(Window.exit);
        }
    }
}
