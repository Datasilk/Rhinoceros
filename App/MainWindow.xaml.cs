using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp.Wpf;

namespace Rhinoceros
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            browser.JavascriptObjectRepository.Register("Rhino", new JsCallback(this), true);
        }

    }

    public class JsCallback
    {
        MainWindow main;
        public JsCallback(MainWindow main) { this.main = main; }

        #region "Callback Methods used via Javascript"
        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void Maximize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        public void Minimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void Toolbar(bool show)
        {
            if(show == true)
            {
                main.WindowStyle = WindowStyle.ToolWindow;
            }
            else
            {
                main.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
        #endregion
    }
}
