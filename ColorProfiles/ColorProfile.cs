using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ColorProfiles
{
    public class ColorProfile
    {
        public string Name { get; protected set; }

        public Vector RedVector => DenseVector.OfArray(new double[] { Red_X, Red_Y, Red_Z });
        public Vector GreenVector => DenseVector.OfArray(new double[] { Green_X, Green_Y, Green_Z });
        public Vector BlueVector => DenseVector.OfArray(new double[] { Blue_X, Blue_Y, Blue_Z });

        public double Red_X { get; set; }
        public double Red_Y { get; set; }
        public double Red_Z => 1 - Red_X - Red_Y;

        public double Green_X { get; set; }
        public double Green_Y { get; set; }
        public double Green_Z => 1 - Green_X - Green_Y;

        public double Blue_X { get; set; }
        public double Blue_Y { get; set; }
        public double Blue_Z => 1 - Blue_X - Blue_Y;

        public double White_X { get; set; }
        public double White_Y { get; set; }
        public double White_Z => 1 - White_X - White_Y;

        public double Gamma { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public Matrix XYZConverter()
        {
            Vector<double> SrSgSb = GetSrSgSb();
            return DenseMatrix.OfColumnVectors(RedVector * SrSgSb[0], GreenVector * SrSgSb[1], BlueVector * SrSgSb[2]);
        }

        private Vector<double> GetSrSgSb()
        {
            Vector<double> XwYwZw = GetXwYwZw();
            Matrix colors = DenseMatrix.OfColumnVectors(RedVector, GreenVector, BlueVector);
            return colors.Solve(XwYwZw);
        }

        private Vector<double> GetXwYwZw()
        {
            return DenseVector.OfArray(new double[]
            {
                White_X * (1 / White_Y),
                1,
                White_Z * (1 / White_Y)
            });
        }
    }
}
