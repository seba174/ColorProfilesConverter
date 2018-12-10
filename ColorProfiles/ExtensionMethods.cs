using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ColorProfiles
{
    public static class ExtensionMethods
    {
        public static Vector ToVector(this Color color)
        {
            return DenseVector.OfArray(new double[] { color.R, color.G, color.B });
        }

        public static IEnumerable<T> FindAllChildrenByType<T>(this Control control)
        {
            IEnumerable<Control> controls = control.Controls.Cast<Control>();
            return controls.OfType<T>().Concat(controls.SelectMany(ctrl => FindAllChildrenByType<T>(ctrl)));
        }
    }
}
