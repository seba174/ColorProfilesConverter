using System.Drawing;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ColorProfiles
{
    public static class ExtensionMethods
    {
        public static Vector ToVector(this Color color)
        {
            return DenseVector.OfArray(new double[] { color.R, color.G, color.B });
        }
    }
}
