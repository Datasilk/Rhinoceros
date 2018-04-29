using System.Windows.Forms;
using System.Linq;

namespace Rhinoceros
{
    public class JsEvents
    {
        public void maximize()
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.maximize);
        }

        public void normalize()
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.maximize);
        }

        public void minimize()
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.maximize);
        }

        public void dragwindow()
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.drag);
        }

        public void bordercolor(int r, int g, int b)
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.borderColor, r, g, b);
        }

        public void toolbarcolor(int r, int g, int b)
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.toolbarColor, r, g, b);
        }

        public void toolbarfontcolor(int r, int g, int b)
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.toolbarFontColor, r, g, b);
        }

        public void defaulttheme()
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.defaultTheme);
        }

        public void bordersize(int grip)
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.changeGripSize, grip);
        }

        public void exit()
        {
            Browser window = Application.OpenForms.OfType<Browser>().First();
            window.Invoke(window.exit);
        }
    }
}
