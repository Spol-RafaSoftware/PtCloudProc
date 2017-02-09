//------------------------------------------------------------------------------
// using definition
using System;
using System.IO;   
using System.Collections.Generic;
using Model;

//------------------------------------------------------------------------------
// namespace definition
namespace MathsLib
{
    //------------------------------------------------------------------------------
    // class PCA
    class PCA
    {
        // principle component analysis
        private Vectors _m; //mean coords, total weight
        private double _eigenval, _dcent; //pca results
        private Vectors _normal;
        private Eigen _e; //eigen extraction object;


        //------------------------------------------------------------------------------
        // Constructors
        public PCA(double[][] p)
        {
            _normal = new Vectors();
            _m = new Vectors();
            analyse(p, p.GetLength(0));
        }


        //------------------------------------------------------------------------------
        // Constructors
        public PCA(double[][] p, int number)
        {
            _normal = new Vectors();
            _m = new Vectors();
            analyse(p, number);
        }


        //------------------------------------------------------------------------------
        // Constructors
        public PCA(List<int> ptLst,int number)
        {
            _normal = new Vectors();
            _m = new Vectors();
            analyseLst(ptLst, number);
        }

        public PCA(List<Vectors> vecLst, int number)
        {
            _normal = new Vectors();
            _m = new Vectors();
            analyseVectors(vecLst, number);
        }

        public Vectors Mean
        {
            get
            {
                return _m;
            }
        }
        public double eigenval
        {
            get
            {
                return _eigenval;
            }
        }
        
        public double dcent
        {
            get
            {
                return _dcent;
            }
        }

        public Vectors Norm
        {
            get
            {
                return _normal;
            }
 
        }

        public Eigen eigen
        {
            get{
               return _e; //eigen extraction object;
            }
        }
        //------------------------------------------------------------------------------ 
        public void analyseVectors(List<Vectors> p, int number)
        {
            // calculate  point count, centroid and
            // then plane by pca
            // p[0-2][] are coordinates
            // p[3][] if present, is weight
            int n = 0; //point count
            int i = 0;
            // means
            _m.set(0.0, 0.0, 0.0, 1.0d);
            double Ptx = 0.0d, Pty = 0.0d, Ptz = 0.0d;

            while (i < number)
            {
                _m._W = 1.0d;
                Ptx = p[i]._X;
                Pty = p[i]._Y;
                Ptz = p[i]._Z;
                _m._X += Ptx;
                _m._Y += Pty;
                _m._Z += Ptz;
                n++;
                i++;
            }
            _m /= n;
            // covariance
            double x, y, z;
            double xx, yy, zz;
            xx = yy = zz = 0.0d;
            double xy, yz, xz;
            xy = yz = xz = 0.0d;
            i = number - 1;
            while (i >= 0)
            {
                _m._W = 1.0;
                Ptx = p[i]._X;
                Pty = p[i]._Y;
                Ptz = p[i]._Z;
                x = Ptx - _m._X;//p[0][i] - _m._X;
                y = Pty - _m._Y;//p[1][i] - _m._Y;
                z = Ptz - _m._Z;//p[2][i] - _m._Z;
                xx += x * x;
                yy += y * y;
                zz += z * z;
                xy += x * y;
                yz += y * z;
                xz += x * z;
                i--;
            }
            // standardize
            xx /= (double)n;
            yy /= (double)n;
            zz /= (double)n;
            xy /= (double)n;
            yz /= (double)n;
            xz /= (double)n;
            // setup matrix
            double[,] cov = new double[3, 3];
            cov[0, 0] = xx;
            cov[0, 1] = xy;
            cov[0, 2] = xz;
            cov[1, 0] = xy;
            cov[1, 1] = yy;
            cov[1, 2] = yz;
            cov[2, 0] = xz;
            cov[2, 1] = yz;
            cov[2, 2] = zz;
            // do pca, smallest eigenvector is normal
            _e = new Eigen(cov);
            _e.EigenStuff();
            _normal = _e.GetEigenvector(0);
            _eigenval = _e.GetEigenvalue(0);
            _dcent = _normal._X * _m._X + _normal._Y * _m._Y + _normal._Z * _m._Z;
        }// analyse _pts



        //------------------------------------------------------------------------------ 
        public void analyseLst(List<int> p, int number)
        {
            // calculate  point count, centroid and
            // then plane by pca
            // p[0-2][] are coordinates
            // p[3][] if present, is weight
            int n = 0; //point count
            int i = 0;
            // means
            _m.set(0.0, 0.0, 0.0, 1.0d);
            double Ptx = 0.0d, Pty = 0.0d, Ptz = 0.0d;

            while (i <number)
            {
                _m._W = 1.0d;
                PtData.Pt(p[i], ref Ptx, ref Pty, ref Ptz);
                _m._X += Ptx;
                _m._Y += Pty;
                _m._Z += Ptz;
                n ++;
                i++;
            }
            _m /= n;
            // covariance
            double x, y, z;
            double xx, yy, zz;
            xx = yy = zz = 0.0d;
            double xy, yz, xz;
            xy = yz = xz = 0.0d;
            i = number - 1;
            while (i >= 0)
            {
                _m._W = 1.0;
                PtData.Pt(p[i], ref Ptx, ref Pty, ref Ptz);
                x = Ptx - _m._X;//p[0][i] - _m._X;
                y = Pty - _m._Y;//p[1][i] - _m._Y;
                z = Ptz - _m._Z;//p[2][i] - _m._Z;
                xx +=  x * x;
                yy +=  y * y;
                zz +=  z * z;
                xy +=  x * y;
                yz +=  y * z;
                xz +=  x * z;
                i--;
            }
            // standardize
            xx /= (double)n;
            yy /= (double)n;
            zz /= (double)n;
            xy /= (double)n;
            yz /= (double)n;
            xz /= (double)n;
            // setup matrix
            double[,] cov = new double[3, 3];
            cov[0, 0] = xx;
            cov[0, 1] = xy;
            cov[0, 2] = xz;
            cov[1, 0] = xy;
            cov[1, 1] = yy;
            cov[1, 2] = yz;
            cov[2, 0] = xz;
            cov[2, 1] = yz;
            cov[2, 2] = zz;
            // do pca, smallest eigenvector is normal
            _e = new Eigen(cov);
            _e.EigenStuff();
            _normal = _e.GetEigenvector(0);
            _eigenval = _e.GetEigenvalue(0);
            _dcent = _normal._X * _m._X + _normal._Y * _m._Y + _normal._Z * _m._Z;
        }// analyse _pts

        //------------------------------------------------------------------------------ 
        public void analyse(double[][] p, int number)
        {
            // calculate  point count, centroid and
            // then plane by pca
            // p[0-2][] are coordinates
            // p[3][] if present, is weight
            int n = 0; //point count
            int i = number - 1;
            // means
            _m.set( 0.0,0.0,0.0,0.0);
            while (i >= 0)
            {
                _m._W = 1.0d;
                if (p.GetLength(0) == 4)
                {
                    _m._W = p[3][i];
                }
                _m._X += p[0][i] * _m._W;
                _m._Y += p[1][i] * _m._W;
                _m._Z += p[2][i] * _m._W;
                n += (int)_m._W;
                i--;
            }
            _m /= n; 
            // covariance
            double x, y, z;
            double xx, yy, zz;
            xx = yy = zz = 0.0d;
            double xy, yz, xz;
            xy = yz = xz = 0.0d;
            i = number - 1;
            while (i >= 0)
            {
                _m._W = 1.0;
                if (p.GetLength(0) == 4)
                {
                    _m._W = p[3][i];
                }
                x = p[0][i] - _m._X;
                y = p[1][i] - _m._Y;
                z = p[2][i] - _m._Z;
                xx += _m._W * x * x;
                yy += _m._W * y * y;
                zz += _m._W * z * z;
                xy += _m._W * x * y;
                yz += _m._W * y * z;
                xz += _m._W * x * z;
                i--;
            }
            // standardize
            xx /= n;
            yy /= n;
            zz /= n;
            xy /= n;
            yz /= n;
            xz /= n;
            // setup matrix
            double[,] cov = new double[3, 3];
            cov[0, 0] = xx;
            cov[0, 1] = xy;
            cov[0, 2] = xz;
            cov[1, 0] = xy;
            cov[1, 1] = yy;
            cov[1, 2] = yz;
            cov[2, 0] = xz;
            cov[2, 1] = yz;
            cov[2, 2] = zz;
            // do pca, smallest eigenvector is normal
            _e = new Eigen(cov);
            _e.EigenStuff();
            _normal = _e.GetEigenvector(0);
            _eigenval = _e.GetEigenvalue(0);
            _dcent = _normal._X * _m._X + _normal._Y * _m._Y + _normal._Z * _m._Z;
        }// analyse

        //----------------------------------------------------------------------------
        static public void PCA_eigentestlst()
        {
            List<int> i = new List<int>() { 0,1,2};

            PtData.addPt(2, 1, 1);
            PtData.addPt(1, 2, 1);
            PtData.addPt(1, 1, 2);

            PCA p = new PCA(i, i.Count);
            System.Diagnostics.Debug.WriteLine(" normal = {0}", p._normal.ToString());
            System.Diagnostics.Debug.WriteLine(" eigenval = {0}", p.eigenval.ToString());
            System.Diagnostics.Debug.WriteLine(" decent = {0}", p.dcent.ToString());
            System.Diagnostics.Debug.WriteLine(" mean = {0}", p.Mean.ToString());
           
            for (int j = 0; j <= 2; j++)
            {
                Vectors v =  p.eigen.GetEigenvector(j);
                System.Diagnostics.Debug.WriteLine("" + j + ", eigenvalue=" + p.eigen.GetEigenvalue(j) + ";");
                System.Diagnostics.Debug.WriteLine("Eigenvector(" + v._X + "," + v._Y + "," + v._Z + ")"); 
 
            }

            // EXPECTED RESULTS FROM TEST
            //  Normal[ -0.57735026, -0.57735026, -0.57735026 ]
            //  eigenval-7.4505806E-9
            //  dcent  -2.309401
            //  mean   1.3333334:1.3333334:1.3333334:1.0
            //  0 eigenvalue= -7.4505806E-9 
            //  Eigenvector = -0.57735026,-0.57735026,-0.57735026
            //  1 eigenvalue= 0.3333333     
            //  Eigenvector = 0.81649655,-0.40824828,-0.40824828
            //  2 eigenvalue= 0.3333333     
            //  Eigenvector = 0.0,-0.70710677, 0.70710677
        }
        //----------------------------------------------------------------------------
        static public void PCA_eigentest()
        {
            double[ ][] m = new double[3][];
            m[0] = new double[3];
            m[1] = new double[3];
            m[2] = new double[3];
            m[0][0] = 2;
            m[0][1] = 1;
            m[0][2] = 1;
            m[1][0] = 1;
            m[1][1] = 2;
            m[1][2] = 1;
            m[2][0] = 1;
            m[2][1] = 1;
            m[2][2] = 2;

            PCA p = new PCA(m, m.GetLength(0));

            System.Diagnostics.Debug.WriteLine(" normal = {0}", p._normal.ToString());
            System.Diagnostics.Debug.WriteLine(" eigenval = {0}", p.eigenval.ToString());
            System.Diagnostics.Debug.WriteLine(" decent = {0}", p.dcent.ToString());
            System.Diagnostics.Debug.WriteLine(" mean = {0}", p.Mean.ToString());


            for (int j = 0; j <= 2; j++)
            {
                Vectors v = p.eigen.GetEigenvector(j);
                System.Diagnostics.Debug.WriteLine("" + j + ", eigenvalue=" + p.eigen.GetEigenvalue(j) + ";");
                System.Diagnostics.Debug.WriteLine("Eigenvector(" + v._X + "," + v._Y + "," + v._Z + ")"); 
    
            }
        }
    }//class PCA

}// mathlibs namespace
