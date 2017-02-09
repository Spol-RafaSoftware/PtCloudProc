using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MathsLib;

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class OctreeNode
    public static class LMA
    {
        // Levenberg- Marquardt Algorithm 
        static private double lambda1 = 0.0001d;
        static private double termepsilon = 0.001d;
        static private int maxiter = 10;
         
        // Calculate the current sum sq'd error
        static public double Objective(List<Vectors> x, double[] a, ModelBase m)
        {
            double t = 0.0d;
            double sum =0.0d;
            int szX = x.Count;
            for(int i = 0; i<szX;i++)
            {
                t = m.DistLMA(x[i], a);
                sum += t * t; 
            }

            return sum;
        }

        static public double solve(List<Vectors> x, double[] a, ModelBase m)
        { 
              int npts = x.Count;
              int n = a.GetLength(0);
            m.Normalise(a); // normalise P(0)
            double e0 = Objective(x, a, m);
            double lambda = lambda1;
            bool done = false;
            // g = gradient,H =hessian,d= step to min
            // H d = -g solve for d

            double [][] H = new double [n][];
            //setup H
                for (int i = 0; i < n; i++)
                {
                    H[i] = new double[n];
                }
            // 
            double[] g = new double[n];


            int iter = 0;
            int term = 0; 

            do
            {
                ++iter;
                // hessian approximation
                for (int r = 0; r < n; r++)
                {
                    for (int c = 0; c <n; c++)
                    {
                       
                        for (int i = 0; i < npts; i++)
                        {
                            if (i==0)
                                H[r][c] = 0.0d ;

                            H[r][c]+=m.grad(x[i],a,r)*m.grad(x[i],a, c);
                        } // for all points 
                    }
                }

                //set diagonal elements 
                for (int r = 0; r < n; r++)
                {
                    H[r][r] *= (1 + lambda);
                    H[r][r] += lambda;
                }
                
                //gradient 
                for (int r = 0; r < n; r++)
                { 
                    for (int i = 0; i < npts; i++)
                    {
                        if (i == 0)
                            g[r] = 0.0d;

                        g[r]=g[r] - m.DistLMA(x[i], a) * m.grad(x[i], a, r);
                    } // for all points  
                }
 
                Choleski ch = new Choleski();
                double[] d = new double[a.GetLength(0)];
                ch.solve(H, d, g);
                double[] na = new double[n];
                for(int i=0;i<n; i++)
                    na[i] = a[i] + d[i];

                m.Normalise(na);
                double e1 = Objective(x, na, m);

                if (Math.Abs(e1 - e0) > termepsilon)
                {
                    term = 0;
                }
                else
                {
                    term++;
                    if (termepsilon ==4)
                    {
                        done = true;
                    }
                }
                if(iter>=maxiter)
                    done = true;

                if (e1 > e0)
                {
                    lambda *= 10.0d;
                }
                else
                {
                    lambda *= 0.04d;
                    e0 = e1;
                    for(int i=0; i<n; i++)
                        a[i] = na[i];
                }


            }
            while (!done);

            return lambda;
        } 

    }//LM
    class Choleski
    {
        public bool decomp(double[][] A, double[] p)
            {
                int n = A.GetLength(0);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++ )
                    {
                        double sum = A[i][j];
                        for(int k=i-1; k>=0;k--)
                            sum -=(A[i][k]*A[j][k]);

                        if (i == j)
                        {
                            if (sum < 0.0d)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            A[i][j] = sum / p[i];
                        }

                    }
                }

                return true;
            }


        public void Solve1(double[][] A, double[] p, double[] b, double[] x)
        {
            int n = A.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                double sum = b[i];
                for (int k = i - 1; k >= 0; k--)
                {
                    sum -= (A[i][k] * x[k] );
                }
                x[i]= sum / p[i];
            }
        }

        public bool solve(double[][] A, double[] x, double[] b)
        {
            double[] p = new double[A.GetLength(0)];
            if (!decomp(A, p))
            {
                return false;
            }
            else
            {
                Solve1(A, p, b, x);
                return true;
            }
        }

     }

}
