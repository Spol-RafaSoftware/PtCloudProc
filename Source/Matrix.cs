//------------------------------------------------------------------------------
// using definition
using System; 
using System.Diagnostics;
using System.Xml;
using System.IO;

//------------------------------------------------------------------------------
// namespace definition
namespace MathsLib
{
    //------------------------------------------------------------------------------
    // class Matrix
    public class Matrix
    {

        //------------------------------------------------------------------------------
        // Constructors
        public Matrix() 
        {
            _m = new double[Vectors.DIM, Vectors.DIM];
            identity();
        }

        //------------------------------------------------------------------------------
        // Constructors
        public Matrix(Matrix m)
        {
            _m = new double[Vectors.DIM, Vectors.DIM];

            for (int i = 0; i < Vectors.DIM; ++i)
            {
                for (int j = 0; j < Vectors.DIM; ++j)
                {
                    m._m[i, j] = m._m[j, i];
                }
            }
               
        }

        //------------------------------------------------------------------------------
        // pre-multiply this matrix by another matrix
        public Matrix preMultiply(Matrix m)
        {
            Matrix p = new Matrix();
            for (int i = 0; i < Vectors.DIM; ++i)
            {
                for (int j = 0; j < Vectors.DIM; ++j)
                {
                    p._m[i, j] = m._m[i, Vectors.X] * _m[Vectors.X, j] + m._m[i, Vectors.Y] * _m[Vectors.Y, j]
                               + m._m[i, Vectors.Z] * _m[Vectors.Z, j] + m._m[i, Vectors.W] * _m[Vectors.W, j];
                }
            }
            return p;
        }

        //------------------------------------------------------------------------------
        // post-multiply this matrix by another matrix
        public Matrix postMultiply(Matrix m)
        {
            Matrix p= new Matrix();
            for (int i = 0; i < Vectors.DIM; ++i)
            {
                for (int j = 0; j < Vectors.DIM; ++j)
                {
                    p._m[i, j] = _m[i, Vectors.X] * m._m[Vectors.X, j] + _m[i, Vectors.Y] * m._m[Vectors.Y, j]
                               + _m[i, Vectors.Z] * m._m[Vectors.Z, j] + _m[i, Vectors.W] * m._m[Vectors.W, j];
                }
            }
            return p;
        }


        //------------------------------------------------------------------------------
        public void at(int i, int j, double k)
        {
            _m[i, j] = k;
        }

        //------------------------------------------------------------------------------
        public double at(int i, int j)
        {
           return _m[i, j];
        }

        //------------------------------------------------------------------------------
        // set this matrix to the identity matrix
        public Matrix identity() 
        {
            for (int i = 0; i < Vectors.DIM; ++i)
            {
                for (int j = 0; j < Vectors.DIM; ++j)
                {
                    _m[i,j] = 0.0;
                }
            }
            for (int k = 0; k < Vectors.DIM; ++k)
            {
                _m[k,k] = 1.0;
            }
            return this;
        }

        //------------------------------------------------------------------------------
        // return the transpose of this matrix
        public Matrix transpose() 
        {
            Matrix m = new Matrix();
            for (int i = 0; i < Vectors.DIM; ++i)
            {
                for (int j = 0; j < Vectors.DIM; ++j)
                {
                     m._m[i,j] = _m[j,i];
                }
            }
            return m;
        }

        //------------------------------------------------------------------------------
        // multiply this matrix by a vector for an affine transformation
        public Vectors multiply(Vectors v)
        {
            double vx = _m[Vectors.X, Vectors.X] * v._X + _m[Vectors.X, Vectors.Y] * v._Y + _m[Vectors.X, Vectors.Z] * v._Z + _m[Vectors.X, Vectors.W] * v._W;
            double vy = _m[Vectors.Y, Vectors.X] * v._X + _m[Vectors.Y, Vectors.Y] * v._Y + _m[Vectors.Y, Vectors.Z] * v._Z + _m[Vectors.Y, Vectors.W] * v._W;
            double vz = _m[Vectors.Z, Vectors.X] * v._X + _m[Vectors.Z, Vectors.Y] * v._Y + _m[Vectors.Z, Vectors.Z] * v._Z + _m[Vectors.Z, Vectors.W] * v._W;
            double vw = v._W;
            return  new Vectors(vx, vy, vz, vw);
        }

        //------------------------------------------------------------------------------
        // multiply this matrix by a ray
        public Ray multiply(Ray r) {
            return new Ray(this.multiply(r.position()), this.multiply(r.direction()));
        }


        //------------------------------------------------------------------------------
        public override string ToString()
        {
            string str;
            str = "{(" + _m[Vectors.X, Vectors.X] + "," + _m[Vectors.X, Vectors.Y] + "," + _m[Vectors.X, Vectors.Z] + "," + _m[Vectors.X, Vectors.W] + ")" +
                   "(" + _m[Vectors.Y, Vectors.X] + "," + _m[Vectors.Y, Vectors.Y] + "," + _m[Vectors.Y, Vectors.Z] + "," + _m[Vectors.Y, Vectors.W] + ")" +
                   "(" + _m[Vectors.Z, Vectors.X] + "," + _m[Vectors.Z, Vectors.Y] + "," + _m[Vectors.Z, Vectors.Z] + "," + _m[Vectors.Z, Vectors.W] + ")" +
                   "(" + _m[Vectors.W, Vectors.X] + "," + _m[Vectors.W, Vectors.Y] + "," + _m[Vectors.W, Vectors.Z] + "," + _m[Vectors.W, Vectors.W] + ")" + "}";
                   

            return str;
        }

        //------------------------------------------------------------------------------
        public void write3x3(ref StreamWriter outs)
        { 
            outs.WriteLine("Matrix = (");
            for (int i = 0; i < Vectors.DIM3; ++i)
            {
                outs.WriteLine("(");
                for (int j = 0; j < Vectors.DIM3; ++j)
                {
                    outs.WriteLine(_m[i, j] + (j < Vectors.DIM3 - 1 ? ", " : ")"));
                }
                outs.WriteLine(i < Vectors.DIM3 - 1 ? "," : ")");
            }
            
        }
 
        //------------------------------------------------------------------------------
        // write this matrix as a 4 x 4
    /*    void write4x4(ref SteamWriter outs) {
            out << "Matrix = (";
            for (int i = 0; i < DIM4; ++i) {
                out << endl << "(";
                for (int j = 0; j < DIM4; ++j) {
                    out << _m[i,j] << (j < DIM4 - 1 ? ", " : ")");
                }
                out << (i < DIM4 - 1 ? "," : ")");
            }
        }*/

        private double[,] _m;
        
        #region Testing
        static public void testClass()
        {

            int REPEATS = 100000;
            {
                Debug.WriteLine("+++++++++Test Matrix Constructors+++++++++++++++++");
                Matrix mA = new Matrix();
                Matrix mB = new Matrix(mA);
                Matrix mC = new Matrix();
                mC = mB;

                Debug.WriteLine("+++++++++Test Matrix Arithemtic+++++++++++++++++");
                int i = 0;
                double t = 0.0f;
                double res = 0.0f;
                res = t;
                Vectors vA = new Vectors(1, 2, 3, 1);
                Vectors vB = new Vectors(9, 8, 7, 1);
                Vectors vC = new Vectors(vB);
                Stopwatch st = Stopwatch.StartNew();
                {
                    st.Start();
                    for (; i < REPEATS; ++i)
                    {
                        mC = mA.preMultiply(mB);
                    }
                    st.Stop();
                    t = (double)(st.ElapsedMilliseconds);
                    st.Reset();
                    Debug.WriteLine("Time Taken ms:" + t + ":" + REPEATS + ": Result:" + mC + "=preMultiply(" + mA + "," + mB + ")");
                }

            }
        }
        #endregion
    }
}
