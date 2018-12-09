using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace ColorProfiles
{
    public static class Utilities
    {
        public static Color ColorFromRgbSafely(Vector<double> vec)
        {
            int r = (int)vec[0];
            int g = (int)vec[1];
            int b = (int)vec[2];

            return Color.FromArgb(
                r <= 255 ? r >= 0 ? r : 0 : 255,
                g <= 255 ? g >= 0 ? g : 0 : 255,
                b <= 255 ? b >= 0 ? b : 0 : 255
            );
        }
    }
}
