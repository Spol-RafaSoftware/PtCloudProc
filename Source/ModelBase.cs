//------------------------------------------------------------------------------
// using definition
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Render;
using Tao.OpenGl;
using MathsLib; 

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class OctreeNode
    public abstract class ModelBase
    {
        //------------------------------------------------------------------------------
        // STATIC and Enum
        #region static_Enum
        protected static double _POSERROR = 0.1d;
        protected static double _PCNTERROR = 2.0d;
        public static double _ANGLECRIT = 0.85d;

        public static void set_POSERROR(double s)
        {
            _POSERROR = s / _PCNTERROR;
        }

        public static double get_POSERROR()
        {
            return _POSERROR;
        }

        public static void set_PCNTERROR(double s)
        {
            _PCNTERROR = s;
        }

        public static double get_PCNTERROR()
        {
            return _PCNTERROR;
        }
        #endregion


        //------------------------------------------------------------------------------
        protected Vectors[] _CPts;            // the list of sigmas which we are using to estimate the shape
        protected Vectors[] _CPtsN;           // the normals 
        protected Colour _g1;
        protected List<OctreeLeaf> _l;
        private int _id;
        private int    _DisplayListCTN; 

        //------------------------------------------------------------------------------
        //constructor
        public ModelBase(Vectors[] CanditatePts, Vectors[] CanditatePtsN)
        {
            _CPts = CanditatePts;
            _CPtsN = CanditatePtsN;
            _g1 = Colour.TABLECOLOUR((int)C.CONST.RANDOM(0.0d, 129.0d));
            _l = new List<OctreeLeaf>(SurfaceController._MINNODESURF); 
            _DisplayListCTN = -1;
        }

        //------------------------------------------------------------------------------
        public abstract Colour ModelColour
        {
            get;
        }

        //------------------------------------------------------------------------------
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        
        //------------------------------------------------------------------------------
        public void Sort_l()
        {
            OctreeLeafIDComparer lc = new OctreeLeafIDComparer();
            _l.Sort(lc);
        }

        //------------------------------------------------------------------------------
        #region ABSTRACT_FN

        //------------------------------------------------------------------------------
        public abstract bool Initialize();
        public abstract string type();
        public abstract double Dist(Vectors pt);
        public abstract double DistLMA(Vectors x, double[] a);
        public abstract double Angle(Vectors pt,Vectors n);
      //  public abstract double normalCurvature(Vectors a, Vectors b);   
        public abstract double grad(Vectors x, double[] a, int ak);
        public abstract double[] initial();
        public abstract void Update(double[] p);
        public abstract void Normalise(double[] a);
        public abstract void ModelData(out string[] strlst);


        //------------------------------------------------------------------------------
        public double fit(OctreeLeaf l)
        {
            double ret = 0;
            Vectors p = l.Sigma;
            ret = 0.5 * (1 - Angle(p, l.Norm) + 0.5 * Math.Abs(Dist(p))) / ModelBase._POSERROR;
            return ret;
        }

        //------------------------------------------------------------------------------
        public  bool testPtN(Vectors pt, Vectors n)
        {
            bool ret = false;

            double d = Math.Abs(Dist(pt));
            double ang = Angle(pt, n);
            if (d < ModelBase._POSERROR)
            {
                if (ang > ModelBase._ANGLECRIT)
                {
                    ret = true;
                }
            }

            return ret;
        }

        //------------------------------------------------------------------------------
        public bool testLeaf(OctreeLeaf l)
        {
            return testPtN(l.Sigma, l.Norm);
        }

        //------------------------------------------------------------------------------
        public int Eval(List<OctreeLeaf> l)
        {
            int nodesValid = 0;
            _l.Clear();

            foreach (OctreeLeaf n in l)
            {
                // if the leaf has not enought points 
                // we wont have a PCA Value so pointless checking
                //TODOCHECK
                if (n.NumPts > OctreeNode._MINPTSPCA)
                {
                    if (testLeaf(n))
                    {
                        n.Flag = 0;
                       _l.Add(n);
                        nodesValid++;
                    }
                    else
                    {
                        n.Flag = -1;
                    }
                }

                //l[n].Flag = testLeaf(l[n]) ? 0 : -1; 
            }

            return nodesValid;
        }
        //------------------------------------------------------------------------------
        public virtual void refine(List<OctreeLeaf> leafs, bool btest) 
        {
            double[] Param = initial();
            List<Vectors> x = new List<Vectors>(leafs.Count);
            int j = 0;
            // for every 
            for(int i=0; i<leafs.Count;i++)
            {
                bool b=false;
                if ((btest == false))
                {
                    b = true;
                }
                else // check this else
                {

                    if (testLeaf(leafs[i]))
                    {
                        if (leafs[i].testMinPts)
                        {
                            if (leafs[i].planar())
                            {

                                b = true;
                            }
                        }
                    }
                }
            
                if (b)
                {
                    x.Add(leafs[i].Sigma);
                    j++;
                }
            }

            LMA.solve(x,   Param, this);
            Update(Param);
        }

        #endregion

        //------------------------------------------------------------------------------
        public double probabillity4(int samples, int n, int nodesValid, int nodesChecked)
        {
            double d = 0.0d;
            
            double C = 1.0 -Math.Pow(((double) n/(double)nodesChecked),4);
            d= (1.0d - Math.Pow(C,(double)samples));
      
            return d;
        }

        //------------------------------------------------------------------------------
        public double probabillity4(int s, int nodesValid, int nodesChecked)
        {
            double d = 0.0d;
            d = probabillity4(s, nodesValid, nodesChecked);
            return d;
        }


        //------------------------------------------------------------------------------
        protected bool Good()
        {
            bool ret = true;
            for (int i = 0; i < _CPts.GetLength(0); i++)
            {
                if (!testPtN(_CPts[i], _CPtsN[i]))
                {
                    ret = false;
                    i += _CPts.GetLength(0);
                }
            }

            return ret;
        }

        //------------------------------------------------------------------------------
        protected double BestNormalInSample(ref Vectors n)
        {
            double mag = 0.0d;
            double m  = 0.0d; 
            Vectors v1 = new Vectors(_CPts[0]-_CPts[1]);
            Vectors v2 = new Vectors(_CPts[2] - _CPts[3]);
            Vectors v3 = v1.cross(v2);
            // 
            m = v3.magnitude();
            if (m>mag)
            {
                n = v3;
                mag = m;
            }

            v1 = new Vectors(_CPts[0] - _CPts[2]);
            v2 = new Vectors(_CPts[1] - _CPts[3]);
            v3 = v1.cross(v2);
            m = v3.magnitude();
            if (m > mag)
            {
                n = v3;
                mag = m;
            }

            v1 = new Vectors(_CPts[0] - _CPts[3]);
            v2 = new Vectors(_CPts[1] - _CPts[2]);
            v3 = v1.cross(v2);
            m = v3.magnitude();
            if (m > mag)
            {
                n = v3;
                mag = m;
            }

            return mag;
        }

        //------------------------------------------------------------------------------
        public double f(Vectors xi, double[] a) // ray since pt normal define a plane
        {
            Ray ra = new Ray(new Vectors(a[0], a[1], a[2]), new Vectors(a[3], a[4], a[5]));
            Vectors xyz = xi - ra.position();
            Vectors abc = ra.direction()/ ra.direction().magnitude();

            abc.cross(xyz);
            return abc.magnitude();
        }

        //------------------------------------------------------------------------------
        public double g(Vectors xi, double[] a) // ray since pt normal define a plane
        {
            Ray ra = new Ray(new Vectors(a[0], a[1], a[2]), new Vectors(a[3], a[4], a[5]));
            Vectors xyz = xi - ra.position();
            Vectors abc = ra.direction() / ra.direction().magnitude(); 
            abc*= xyz;
            return abc.magnitude();
        }

        //------------------------------------------------------------------------------ 
        public void CreateDisplayList(List<int> drawlist, bool byModelType)
        {

            _DisplayListCTN = Gl.glGenLists(1);
            Gl.glNewList(_DisplayListCTN, Gl.GL_COMPILE); 
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glPointSize(5.0f);
            float[] g1;
            if (byModelType == true)
                ModelColour.tofloat3f(out g1);
            else
                _g1.tofloat3f(out g1);  
            Gl.glBegin(Gl.GL_POINTS); 

                Gl.glColor3f(g1[0], g1[1], g1[2]);
                foreach (OctreeLeaf L in _l)
                { 
                    Vectors S = L.Sigma;
                    Gl.glVertex3d(S._X, S._Y, S._Z);

                }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEndList();
            drawlist.Add(_DisplayListCTN);
        }

        //------------------------------------------------------------------------------ 
        public void DestroyDisplayList()
        {
            
            if (_DisplayListCTN != -1)
            {
                Gl.glDeleteLists(_DisplayListCTN, 1);
                _DisplayListCTN = -1;
            }
        }
  
        //------------------------------------------------------------------------------
        public virtual void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.WriteStartElement("Model_ID");
            outs.WriteValue(_id);
            outs.WriteFullEndElement();

            _g1.WriteXmlData(ref outs);
            for (int i = 0; i < _CPts.GetLength(0); i++)
            {
                outs.WriteStartElement("SamplePoint" + i);
                _CPts[i].write4(ref outs);
                outs.WriteFullEndElement();
                outs.WriteStartElement("SampleNormal"+i);
                _CPtsN[i].write4(ref outs);
                outs.WriteFullEndElement();
            }
            outs.Flush();
            
            outs.WriteStartElement("LeafsInModel"); 
            foreach (OctreeLeaf L in _l)
            {  
                outs.WriteValue(" "+L.ID+" ");
            }
            outs.WriteFullEndElement();
        }
        //------------------------------------------------------------------------------

    }
}
