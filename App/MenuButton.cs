using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Svg;

namespace Rhinoceros
{
    public partial class MenuButton : UserControl
    {
        private SvgDocument svg = null;
        private MenuButtonColorOptions colors;
        private ButtonType type;

        public MenuButton(ButtonType type, MenuButtonColorOptions colors)
        {
            InitializeComponent();
            this.type = type;
            UpdateButton(type, colors);
        }

        public void UpdateButton(ButtonType type, MenuButtonColorOptions colors)
        {
            BackColor = colors.backgroundColor;
            DrawIcon(type, colors.fontColor);
            this.colors = colors;
        }

        public void DrawIcon(ButtonType type, Color color)
        {
            if(svg == null)
            {
                svg = SvgDocument.Open(Directory.GetCurrentDirectory() + "\\Assets\\button-" + type.ToString() + ".svg");
            }
            colorizeSvgNodes(svg.Descendants(), new SvgColourServer(color));
            BackgroundImage = svg.Draw();
        }

        private void colorizeSvgNodes(IEnumerable<SvgElement> nodes, SvgPaintServer colorServer)
        {
            foreach (var node in nodes)
            {
                if (node.Fill != SvgPaintServer.None) node.Fill = colorServer;
                if (node.Color != SvgPaintServer.None) node.Color = colorServer;
                if (node.StopColor != SvgPaintServer.None) node.StopColor = colorServer;
                if (node.Stroke != SvgPaintServer.None) node.Stroke = colorServer;

                colorizeSvgNodes(node.Descendants(), colorServer);
            }
        }

        private void MenuButton_MouseEnter(object sender, System.EventArgs e)
        {
            BackColor = colors.backgroundHoverColor;
            DrawIcon(type, colors.fontHoverColor);
        }

        private void MenuButton_MouseLeave(object sender, System.EventArgs e)
        {
            BackColor = colors.backgroundColor;
            DrawIcon(type, colors.fontColor);
        }

        private void MenuButton_MouseDown(object sender, MouseEventArgs e)
        {
            BackColor = colors.backgroundMouseDownColor;
            DrawIcon(type, colors.fontMouseDownColor);
        }

        private void MenuButton_MouseUp(object sender, MouseEventArgs e)
        {
            BackColor = colors.backgroundColor;
            DrawIcon(type, colors.fontColor);
        }
    }


    public enum ButtonType
    {
        close = 0,
        minimize = 1,
        maximize = 2
    }

    public class MenuButtonColorOptions
    {
        public Color backgroundColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color backgroundHoverColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color backgroundMouseDownColor { get; set; } = Color.FromArgb(0, 153, 255);
        public Color fontColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color fontHoverColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color fontMouseDownColor { get; set; } = Color.FromArgb(255, 255, 255);
    }
}
