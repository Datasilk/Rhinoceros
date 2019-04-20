using System.Drawing;

namespace Rhinoceros
{
    class Utility
    {
        public static uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }
    }
}
