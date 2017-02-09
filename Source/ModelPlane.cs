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
    public class ModelPlane: ModelBase
    {
        private Vectors _N;
        private double _d; 
        //------------------------------------------------------------------------------
        //constructor
        public ModelPlane(Vectors[] a, Vectors[] aN ):base(a,aN)
        {
            _d = 0.0d;
            _N = new Vectors(); 
        }
        
        //------------------------------------------------------------------------------
        public override Colour  ModelColour
        {
	        get {
                return Colour.Yellow();
            }
        } 
        //------------------------------------------------------------------------------
        public override bool Initialize()
        {
            bool res = false;
            if (_CPts != null)
            {
                if (_CPtsN != null)
                {
                    // first we need to find the best pair of points
                    BestNormalInSample(ref _N);
                    _N.normalize();
                    _d = 0.0d;
                    for (int i = 0; i < 4; i++)
                    {
                        _d += _N.dot(_CPts[i]); 
                    }
                    _d /=4.0d;
                    res = Good();
                    return res;
                }
            }

            return res;
        }

        //------------------------------------------------------------------------------
        public override double Dist(Vectors pt)
        { 
            return Math.Abs(_N.dot(pt) - _d);
        }

        //------------------------------------------------------------------------------
        public override double DistLMA(Vectors x, double[] a) 
        {
            // plane uses pca
            // return Math.Abs(_N.dot(x)-_d);
            return  0.0d;
        }

        //------------------------------------------------------------------------------
        public static string stype()
        {
            return "Plane";
        }
        //------------------------------------------------------------------------------
        public override string type()
        {
            return  stype();
        }

        //------------------------------------------------------------------------------
        public override void Normalise(double[] a)
        {
        }

        //------------------------------------------------------------------------------
        public override double Angle(Vectors pt, Vectors n)
        {
            return Math.Abs(_N.dot(n) );
        } 

       
        //------------------------------------------------------------------------------
        public override void refine(List<OctreeLeaf> leafs, bool btest)
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
            PCA p = new PCA(x, x.Count);
            _N = p.Norm;
            _d = p.dcent;
        }

        //------------------------------------------------------------------------------
        public override void Update(double[] p)
        {
            // do nothing am using PCA HERE NOT LMA
        }

        //------------------------------------------------------------------------------
        public override double grad(Vectors x, double[] a, int ak)
        {
            double t = 1.0;
            return t;
        }

        //------------------------------------------------------------------------------
        public override double[] initial()
        {
            double[] a = new double[3];
            // Dont care Am using pCA inital used by LMA SO
            return a;
        
        }

        //------------------------------------------------------------------------------
        //public override double normalCurvature(Vectors a, Vectors b)
        //{
        //    double t = 0.0;
        //    return t;
        //}

        //------------------------------------------------------------------------------
        public override void ModelData(out string[] surfacedata)
        {
            surfacedata = new string[_CPts.GetLength(0) + _CPtsN.GetLength(0) + 3 + _l.Count];
            surfacedata[0] = "Model: " + type();
            int i = 1;
            surfacedata[i] = "Norm: " + _N.ToString();
            i++;//i = 2
            surfacedata[i] = "dist: " + _d.ToString();
            i++;//i = 3  
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
            outs.WriteStartElement(type() + "_Norm");
            _N.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement(type() + "_dist"); 
            outs.WriteValue(_d);
            outs.WriteFullEndElement();
            outs.WriteFullEndElement();

        }
    }
}
