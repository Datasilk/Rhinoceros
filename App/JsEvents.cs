using System.Windows.Forms;
using System.Linq;

namespace Rhinoceros
{
    public class JsEvents
    {
        private Browser _window;
        private Browser Window {
            get {
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
