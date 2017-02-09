//------------------------------------------------------------------------------
// using definition
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using MathsLib;
using Render;

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class OctreeNode
    public class ModelCone : ModelBase
    {
        static double MAX_CONE_APEX = 1.4835298641951801403851371532153d;// 85 degrees
        private Vectors _pos;
        private Vectors _axis;
        private double _apex;   
        private double _n2d0;
        private double _n2d1;   
             
        //------------------------------------------------------------------------------
        //constructor
        public ModelCone(Vectors[] a, Vectors[] aN)
            : base(a, aN)
        {
            _pos = new Vectors();
            _axis = new Vectors();
            _apex = 0.0d;
        }

        //------------------------------------------------------------------------------
        public static string stype()
        {
            return "Cone";
        }

        //------------------------------------------------------------------------------
        public override string type()
        {
            return stype();
        }

        //------------------------------------------------------------------------------
        public override Colour ModelColour
        {
            get
            {
                return Colour.Sky_Blue();
            }
        } 

        //------------------------------------------------------------------------------
        public override bool Initialize()
        {
            bool res = false;
            if (Init(_CPts[0], _CPts[1], _CPts[2], _CPtsN[0], _CPtsN[1], _CPts[2]))
            {
                res = Good();
            }
            
            return res;
        }

        private void Normal_ang(ref double angle ,Vectors n)
        { 
            angle = _axis.dot(n);
            C.CONST.CLAMP(ref angle, -1.0d, 1.0d);// clamp angle to [-1, 1]

            if (angle < 0.0d)
                // angle = omega + 90 
                angle = Math.Acos(angle) - (Math.PI / 2.0d);
            else
                // angle = 90 - omega
                angle = (Math.PI / 2.0d) - Math.Acos(angle);
            
        }
 
        private bool Init(  Vectors  p1, Vectors    p2, Vectors    p3, Vectors  n1,   Vectors  n2, Vectors    n3)
        { 
            // compute center by intersecting the three planes given by (p1, n1)
            // (p2, n2) and (p3, n3)
            // set up linear system
            double[] a = new double[12];
            double d1 = p1.dot(n1);
            double d2 = p2.dot(n2);
            double d3 = p3.dot(n3);
            // column major
            a[0]  = n1._X;
            a[1]  = n2._X;
            a[2]  = n3._X;
            a[3]  = n1._Y;
            a[4]  = n2._Y;
            a[5]  = n3._Y;
            a[6]  = n1._Z;
            a[7]  = n2._Z;
            a[8]  = n3._Z;
            a[9]  = d1;
            a[10] = d2;
            a[11] = d3;
            if(dmat_solve(3, 1, a)>0)
	            return false;
            _pos = new Vectors( a[9],a[10],a[11]);

            // compute axisDir
            Vectors s1 = p1 - _pos;
            Vectors s2 = p2 - _pos;
            Vectors s3 = p3 - _pos;
            s1.normalize();
            s2.normalize();
            s3.normalize();

            Vectors n = new Vectors(((s2 + _pos) - (s1 + _pos)).cross(s3 + _pos - (s2 + _pos)));
           n.normalize();		
            _axis = n;
            // make sure axis points in direction of s1
            // this defines the side of the cone!!!
            if(_axis.dot(s1) < 0.0d)
	            _axis *= -1;
            _apex = 0.0d;
            double angle = 0.0d;
            Normal_ang(ref angle, n1);
            _apex += angle;
            Normal_ang(ref angle, n2);
            _apex += angle;  
            Normal_ang(ref angle, n3);
            _apex += angle; 
            _apex /= 3.0d;
            if ((_apex < C.CONST.EPSILON) || (_apex > ( (Math.PI / 2.0d) - C.CONST.EPSILON)) ||(C.CONST.IS_NUM(_apex)))
	            return false;
              if(_apex > MAX_CONE_APEX) 
	            return false; 
            _n2d0 = Math.Cos(_apex);
            _n2d1 = -Math.Sin(_apex); 
            return true;
        }

        //------------------------------------------------------------------------------
        public override double Dist(Vectors pt)
        { 
	        // this is for one sided cone!
	        Vectors s = pt - _pos;
	        double g = s.dot(_axis); // distance to plane orhogonal to
		                             // axisdir through center
	                                 // distance to axis
	        double sqrS = s.dot(s);
            double f = sqrS - (g * g);
	        if(f <= C.CONST.EPSILON)
		        f = 0.0d;
	        else
		        f = Math.Sqrt(f);

            double da = _n2d0 * f;
            double db = _n2d1 * g;
            if ((g < C.CONST.EPSILON) && ( (da - db) < C.CONST.EPSILON)) // is inside other side of cone -> disallow
		        return Math.Sqrt(sqrS);
	        return Math.Abs(da + db);
        }
          
        //------------------------------------------------------------------------------
        public override double DistLMA(Vectors x, double[] a)
        {   
            // x-a
            Vectors s = new Vectors(x._X - a[0],x._Y-a[1],x._Z-a[2]);
	        //
            double g = Math.Abs(s._X * a[3] + s._Y * a[4] + s._Z * a[5]);
            double f = s.dot(s) - (g * g);
	        if(f <= C.CONST.EPSILON)
		        f = 0.0d;
	        else
		        f = Math.Sqrt(f);
	        return Math.Cos(a[6]) * f - Math.Sin(a[6]) * g;

        }
          
        //------------------------------------------------------------------------------
        public override double Angle(Vectors pt, Vectors n)
        {
            // normal at x 
            Vectors a = new Vectors(pt);
            a -= _pos;
            // normal at x
            Vectors perp = new Vectors(a);
            double dt = a.dot(_axis);
            perp -= (_axis * dt);
            perp.normalize();

            return Math.Abs(perp.dot(n));
        } 

        //------------------------------------------------------------------------------
        public override void Update(double[] a)
        {
             Normalise(a);  
            _pos=new Vectors(a[0], a[1], a[2]);
            _axis = new Vectors(a[3], a[4], a[5]);
            _apex = a[6];
            _n2d0 = Math.Cos(_apex);
            _n2d1 = -Math.Sin(_apex); 
           
        }

        //------------------------------------------------------------------------------
        public override double grad(Vectors xi, double[] a, int ak)
        {  
            //double df = f(xi, a);
            //double dg = g(xi, a);  
            Ray ra = new Ray(new Vectors(a[0], a[1], a[2]), new Vectors(a[3], a[4], a[5]));

            Vectors s = new Vectors(xi._X-a[3], xi._Y-a[4], xi._Z - a[5]);

            double dg = Math.Abs(s._X * a[3] + s._Y * a[4] + s._Z * a[5]);
	        double df = s.dot(s) - (dg * dg);
	        if(df <= C.CONST.EPSILON)
		        df = 0.0d;
	        else
		        df = Math.Sqrt(df);
            double[] ggrad = new double[6]; 

            ggrad[0] =  - ra.direction()._X;
            ggrad[1] = -ra.direction()._Y;
            ggrad[2] = -ra.direction()._Z;
            ggrad[3] = s._X - ra.direction()._X * dg;
            ggrad[4] = s._Y - ra.direction()._Y * dg;
            ggrad[5] = s._Z - ra.direction()._Z * dg;

            double[] fgrad = new double[6];
	        if(df < C.CONST.EPSILON)
	        {
                fgrad[0] = Math.Sqrt(1 - ra.direction()._X * ra.direction()._X);
                fgrad[1] = Math.Sqrt(1 - ra.direction()._Y * ra.direction()._Y);
                fgrad[2] = Math.Sqrt(1 - ra.direction()._Z * ra.direction()._Z);
	        }
	        else
	        {
                fgrad[0] = (ra.direction()._X * dg - s._X) / df;
                fgrad[1] = (ra.direction()._Y * dg - s._Y) / df;
                fgrad[2] = (ra.direction()._Z * dg - s._Z) / df;
	        }
            fgrad[3] = dg * fgrad[0];
            fgrad[4] = dg * fgrad[1];
            fgrad[5] = dg * fgrad[2];
	        double sinPhi = -Math.Sin(a[6]);
	        double cosPhi = Math.Cos(a[6]);

            if(ak<6)
                return cosPhi * fgrad[ak] + sinPhi * ggrad[ak];
            else
                return df * sinPhi - dg * cosPhi;
 
        }

        //------------------------------------------------------------------------------
        public override double[] initial()
        { 
            double[] a = new double[7];
            a[0] = _pos._X;
            a[1] = _pos._Y;
            a[2] = _pos._Z;
            a[3] = _axis._X;
            a[4] = _axis._Y;
            a[5] = _axis._Z;
            a[6] = _apex; 
            //a[7] = _d;
            return a;
        }

        ////------------------------------------------------------------------------------
       // public override double normalCurvature(Vectors pt, Vectors ptN)
       // {
       //     double nc = 0.0d;
       //     return nc;
       // }

        //------------------------------------------------------------------------------
        public override void Normalise(double[] a)
        {
            // a[0],a[1],a[2] //x closest 
	        // normalize direction
	        double l = Math.Sqrt( (a[3] * a[3]) + (a[4] * a[4]) + (a[5] * a[5]) );
	        for(int i = 3; i < 6; ++i)
		        a[i] /= l;
	        // normalize angle
            a[6] -= Math.Floor(a[6] / (2 * Math.PI)) * (2 * Math.PI); // a[6] %= 2*Math.PI
	        if(a[6] > Math.PI)
	        {
                a[6] -= Math.Floor(a[6] / Math.PI) * Math.PI; // a[6] %= Math.PI
		        for(  int i = 3; i < 6; ++i)
			        a[i] *= -1;
	        }
	        if(a[6] > Math.PI / 2)    // 
		        a[6] = Math.PI - a[6];
        }

        //------------------------------------------------------------------------------
        public override void ModelData(out string[] surfacedata)
        {
            surfacedata = new string[_CPts.GetLength(0) + _CPtsN.GetLength(0) + 4 + _l.Count];
            surfacedata[0] = "Model: " + type();
            int i = 1;
            surfacedata[i] = "pos: " + _pos.ToString();
            i++;//i = 2
            surfacedata[i] = "axis: " + _axis.ToString();
            i++;//i = 3 
            surfacedata[i] = "apex: " + _apex.ToString(); 
            i++;//i = 4
            for (int I = 0; I < _CPts.GetLength(0); I++)
            {
                surfacedata[i] = _CPts[I].ToString();
                i++;
                surfacedata[i] = _CPtsN[I].ToString();
                i++;
            }
            for (int I = 0; I < _l.Count; I++)
            {
                surfacedata[i] = _l[I].ID.ToString();
                i++;
            }
        }
          
        //------------------------------------------------------------------------------
        public override void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.WriteStartElement("Model_" + type());
            base.WriteXmlData(ref outs); 
            
            outs.WriteStartElement(type()+"_Position");
            _pos.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement(type()+"_axis");
            _axis.write4(ref outs);
            outs.WriteFullEndElement();

            outs.WriteStartElement(type() + "_axis");
            outs.WriteValue(_apex);
            outs.WriteFullEndElement();
            
            outs.WriteFullEndElement();
        }

        //------------------------------------------------------------------------------
                
        int dmat_solve ( int n, int rhs_num, double[] a )
        //******************************************************************************
        //
        //  Purpose:
        //
        //    DMAT_SOLVE uses Gauss-Jordan elimination to solve an N by N linear system.
        //
        //  Discussion:
        //
        //    The doubly dimensioned array A is treated as a one dimensional vector,
        //    stored by COLUMNS.  Entry A(I,J) is stored as A[I+J*N]
        //
        //  Modified:
        //
        //    29 August 2003
        //
        //  Author:
        //
        //    John Burkardt
        //
        //  Parameters:
        //
        //    Input, int N, the order of the matrix.
        //
        //    Input, int RHS_NUM, the number of right hand sides.  RHS_NUM
        //    must be at least 0.
        //
        //    Input/output, double A[N*(N+RHS_NUM)], contains in rows and columns 1
        //    to N the coefficient matrix, and in columns N+1 through
        //    N+RHS_NUM, the right hand sides.  On output, the coefficient matrix
        //    area has been destroyed, while the right hand sides have
        //    been overwritten with the corresponding solutions.
        //
        //    Output, int DMAT_SOLVE, singularity flag.
        //    0, the matrix was not singular, the solutions were computed;
        //    J, factorization failed on step J, and the solutions could not
        //    be computed.
        //
        {
          double apivot;
          double factor;
          int i;
          int ipivot;
          int j;
          int k;
          double temp;

          for ( j = 0; j < n; j++ )
          {
        //
        //  Choose a pivot row.
        //
            ipivot = j;
            apivot = a[j+j*n];

            for ( i = j; i < n; i++ )
            {
              if ( Math.Abs ( apivot ) < Math.Abs ( a[i+j*n] ) )
              {
                apivot = a[i+j*n];
                ipivot = i;
              }
            }

            if ( apivot == 0.0 )
            {
              return j;
            }
        //
        //  Interchange.
        //
            for ( i = 0; i < n + rhs_num; i++ )
            {
              temp          = a[ipivot+i*n];
              a[ipivot+i*n] = a[j+i*n];
              a[j+i*n]      = temp;
            }
        //
        //  A(J,J) becomes 1.
        //
            a[j+j*n] = 1.0;
            for ( k = j; k < n + rhs_num; k++ )
            {
              a[j+k*n] = a[j+k*n] / apivot;
            }
        //
        //  A(I,J) becomes 0.
        //
            for ( i = 0; i < n; i++ )
            {
              if ( i != j )
              {
                factor = a[i+j*n];
                a[i+j*n] = 0.0;
                for ( k = j; k < n + rhs_num; k++ )
                {
                  a[i+k*n] = a[i+k*n] - factor * a[j+k*n];
                }
              }

            }

          }

          return 0;
        }
        //******************************************************************************

    }
}


