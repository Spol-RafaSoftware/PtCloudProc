//------------------------------------------------------------------------------
// using definition
using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using Render; 

//------------------------------------------------------------------------------
// namespace definition
namespace MathsLib
{
    //------------------------------------------------------------------------------
    // class PCA 
    class Eigen
    {
        //------------------------------------------------------------------------------
        // Constructors
        public Eigen(double[,] m)
        {
            m_kMat = new Matrix();
            for (int r = 0; r <= 2; r++)
            {
                for (int c = 0; c <= 2; c++)
                {
                    m_kMat.at(r, c, m[r, c]);
                }
            }
            m_iSize = 3;
            m_bIsRotation = false;
        }


        //----------------------------------------------------------------------------
        public void EigenStuff()
        {
            // do the work
            m_afDiag = new double[m_iSize];
            m_afSubd = new double[m_iSize];
            Tridiagonal3();
            QLAlgorithm();
            IncreasingSort();
            GuaranteeRotation();
        }
        //----------------------------------------------------------------------------
        public double GetEigenvalue(int i)
        {
            return m_afDiag[i];
        }

        //----------------------------------------------------------------------------
        public Vectors GetEigenvector(int i)
        {
            Vectors v;
            v = new Vectors(m_kMat.at(0, i), m_kMat.at(1, i), m_kMat.at(2, i));
            return v;
        }
        //---------------------------------------------------------------------------- 
        public void Tridiagonal3()
        {

            double fM00 = m_kMat.at(0, 0);
            double fM01 = m_kMat.at(0, 1);
            double fM02 = m_kMat.at(0, 2);
            double fM11 = m_kMat.at(1, 1);
            double fM12 = m_kMat.at(1, 2);
            double fM22 = m_kMat.at(2, 2);

            m_afDiag[0] = fM00;
            m_afSubd[2] = 0.0d;
            //if (Math.Abs(fM02) >  double.MinValue) 
            if (C.CONST.definitelyGreaterThan(Math.Abs(fM02), C.CONST.EPSILON)) 
            {
                double fLength = Math.Sqrt(fM01 * fM01 + fM02 * fM02);
                double fInvLength = 1.0d / fLength;
                fM01 *= fInvLength;
                fM02 *= fInvLength;
                double fQ = 2.0d * fM01 * fM12 + fM02 * (fM22 - fM11);
                m_afDiag[1] = fM11 + fM02 * fQ;
                m_afDiag[2] = fM22 - fM02 * fQ;
                m_afSubd[0] = fLength;
                m_afSubd[1] = fM12 - fM01 * fQ;
                m_kMat.at(0, 0, 1.0f);
                m_kMat.at(0, 1, 0.0f);
                m_kMat.at(0, 2, 0.0f);
                m_kMat.at(1, 0, 0.0f);
                m_kMat.at(1, 1, fM01);
                m_kMat.at(1, 2, fM02);
                m_kMat.at(2, 0, 0.0f);
                m_kMat.at(2, 1, fM02);
                m_kMat.at(2, 2, -fM01);
                m_bIsRotation = false;
            }
            else
            {
                m_afDiag[1] = fM11;
                m_afDiag[2] = fM22;
                m_afSubd[0] = fM01;
                m_afSubd[1] = fM12;
                m_kMat.at(0, 0, 1.0d);
                m_kMat.at(0, 1, 0.0d);
                m_kMat.at(0, 2, 0.0d);
                m_kMat.at(1, 0, 0.0d);
                m_kMat.at(1, 1, 1.0d);
                m_kMat.at(1, 2, 0.0d);
                m_kMat.at(2, 0, 0.0d);
                m_kMat.at(2, 1, 0.0d);
                m_kMat.at(2, 2, 1.0d);
                m_bIsRotation = true;
            }
        }
        //----------------------------------------------------------------------------
        public Boolean QLAlgorithm()
        {
            int iMaxIter = 32;
            for (int i0 = 0; i0 < 3; i0++)
            {
                int i1;
                for (i1 = 0; i1 < iMaxIter; i1++)
                {
                    int i2;
                    for (i2 = i0; i2 <= m_iSize - 2; i2++)
                    {
                        double fTmp = Math.Abs(m_afDiag[i2]) + Math.Abs(m_afDiag[i2 + 1]);

                        if (Math.Abs(m_afSubd[i2]) + fTmp == fTmp)  //wierd?
                            break;
                    }
                    if (i2 == i0)
                        break;

                    double fG = (m_afDiag[i0 + 1] - m_afDiag[i0]) / (2.0d * m_afSubd[i0]);
                    double fR = Math.Sqrt(fG * fG + 1.0d);
                    if (C.CONST.definitelyLessThan(fG, 0.0d))
                        fG = m_afDiag[i2] - m_afDiag[i0] + m_afSubd[i0] / (fG - fR);
                    else
                        fG = m_afDiag[i2] - m_afDiag[i0] + m_afSubd[i0] / (fG + fR);
                    double fSin = 1.0d;
                    double fCos = 1.0d;
                    double fP = 0.0d;
                    for (int i3 = i2 - 1; i3 >= i0; i3--)
                    {
                        double fF = fSin * m_afSubd[i3];
                        double fB = fCos * m_afSubd[i3];
                        if (!C.CONST.definitelyLessThan(Math.Abs(fF), Math.Abs(fG)))
                        {
                            fCos = fG / fF;
                            fR = Math.Sqrt(Math.Abs(fCos * fCos + 1.0d));
                            m_afSubd[i3 + 1] = fF * fR;
                            fSin = 1.0f / fR;
                            fCos *= fSin;
                        }
                        else
                        {
                            fSin = fF / fG;
                            fR = Math.Sqrt(Math.Abs(fSin * fSin + 1.0f));
                            m_afSubd[i3 + 1] = fG * fR;
                            fCos = 1.0f / fR;
                            fSin *= fCos;
                        }
                        fG = m_afDiag[i3 + 1] - fP;
                        fR = (m_afDiag[i3] - fG) * fSin + 2.0f * fB * fCos;
                        fP = fSin * fR;
                        m_afDiag[i3 + 1] = fG + fP;
                        fG = fCos * fR - fB;

                        for (int i4 = 0; i4 < m_iSize; i4++)
                        {
                            fF = m_kMat.at(i4, i3 + 1);
                            m_kMat.at(i4, i3 + 1, (fSin * m_kMat.at(i4, i3) + fCos * fF));
                            m_kMat.at(i4, i3, (fCos * m_kMat.at(i4, i3) - fSin * fF));
                        }
                    }
                    m_afDiag[i0] -= fP;
                    m_afSubd[i0] = fG;
                    m_afSubd[i2] = 0.0f;
                }
                if (i1 == iMaxIter)
                {
                    return false;
                }
            }

            return true;
        }

        //----------------------------------------------------------------------------
        public void IncreasingSort()
        {
            // sort eigenvalues in increasing order, e[0] <= ... <= e[iSize-1]
            for (int i0 = 0, i1; i0 <= m_iSize - 2; i0++)
            {
                // locate minimum eigenvalue
                i1 = i0;
                double fMin = m_afDiag[i1];
                int i2;
                for (i2 = i0 + 1; i2 < m_iSize; i2++)
                {
                    if (C.CONST.definitelyLessThan(m_afDiag[i2], fMin))
                    {
                        i1 = i2;
                        fMin = m_afDiag[i1];
                    }
                }

                if (i1 != i0)
                {
                    // swap eigenvalues
                    m_afDiag[i1] = m_afDiag[i0];
                    m_afDiag[i0] = fMin;

                    // swap eigenvectors
                    for (i2 = 0; i2 < m_iSize; i2++)
                    {
                        double fTmp = m_kMat.at(i2, i0);
                        m_kMat.at(i2, i0, m_kMat.at(i2, i1));
                        m_kMat.at(i2, i1, fTmp);
                        m_bIsRotation = !m_bIsRotation;
                    }
                }
            }
        }
        //----------------------------------------------------------------------------
        public void GuaranteeRotation()
        {
            if (!m_bIsRotation)
            {
                // change sign on the first column
                for (int iRow = 0; iRow < m_iSize; iRow++)
                {
                    m_kMat.at(iRow, 0, -m_kMat.at(iRow, 0));
                }
            }
        }

        //----------------------------------------------------------------------------
        static public void eigentest()
        {
            double[,] m = new double[3, 3];
            m[0, 0] = 2;
            m[0, 1] = 1;
            m[0, 2] = 1;
            m[1, 0] = 1;
            m[1, 1] = 2;
            m[1, 2] = 1;
            m[2, 0] = 1;
            m[2, 1] = 1;
            m[2, 2] = 2;
            Eigen e = new Eigen(m);
            e.EigenStuff();
            for (int i = 0; i <= 2; i++)
            {
                Vectors v = e.GetEigenvector(i);
                Debug.WriteLine("" +i+ ", eigenvalue=" +   e.GetEigenvalue(i)+ ";" );
                Debug.WriteLine("Eigenvector(" +  v._X+ "," + v._Y + "," +  v._Z + ")" );
            }
        }

        private int m_iSize;
        private Matrix m_kMat;
        private double[] m_afDiag;
        private double[] m_afSubd;
        private Boolean m_bIsRotation;
        // private double normal, eigenval, dcent, dmax, dmin; //pca results

    }

    #region LICENCE
    // find eigen values/vectors for a symmetric 3x3 matrix (only)
    // intended for pca 
    // adapted from:
    // Wild Magic Source Code
    // David Eberly
    // http://www.geometrictools.com
    // Copyright (c) 1998-2008
    //
    // This library is free software; you can redistribute it and/or modify it
    // under the terms of the GNU Lesser General Public License as published by
    // the Free Software Foundation; either version 2.1 of the License, or (at
    // your option) any later version.  The license is available for reading at
    // either of the locations:
    //     http://www.gnu.org/copyleft/lgpl.html
    //     http://www.geometrictools.com/License/WildMagicLicense.pdf
    //
    // Version: 4.0.0 (2006/06/28) 
    // For odd size matrices, the Householder reduction involves an odd
    // number of reflections.  The product of these is a reflection.  The
    // QL algorithm uses rotations for further reductions.  The final
    // orthogonal matrix whose columns are the eigenvectors is a reflection,
    // so its determinant is -1.  For even size matrices, the Householder
    // reduction involves an even number of reflections whose product is a
    // rotation.  The final orthogonal matrix has determinant +1.  Many
    // algorithms that need an eigendecomposition want a rotation matrix.
    // We want to guarantee this is the case, so m_bRotation keeps track of
    // this.  The DecrSort and IncrSort further complicate the issue since
    // they swap columns of the orthogonal matrix, causing the matrix to
    // toggle between rotation and reflection.  The value m_bRotation must
    // be toggled accordingly.
    #endregion LICENCE 
}
