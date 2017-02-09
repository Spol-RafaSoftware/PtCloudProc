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
    public class ModelSphere : ModelBase
    {
        private Vectors _Centre; 
        private double _radius;
        //------------------------------------------------------------------------------
        //constructor
        public ModelSphere(Vectors[] a, Vectors[] aN)
            : base(a, aN)
        {
            _Centre = new Vectors(); 
            _radius = 0.0d;
        }

        //------------------------------------------------------------------------------
        public static string stype()
        {
            return "Sphere";
        }

        //------------------------------------------------------------------------------
        public override Colour ModelColour
        {
            get
            {
                return Colour.Lawn_Green();
            }
        }
 
        //------------------------------------------------------------------------------
        public override string type()
        {
            return stype();
        }

        //------------------------------------------------------------------------------
        public override bool Initialize()
        {
            bool res = false;
            double best = 0.0d;
            int bi = 0, bj = 0; // best estimation pair
            Vectors x1 = new Vectors(); 

            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 4; j++)
                {  
                    double m = _CPtsN[i].dot(_CPtsN[j]);
                    if (m > best)
                    {
                        bi = i;
                        bj = j;
                        best = m;
                    }
                }
            }
            //TODO test this 
            Ray r1 = new Ray(_CPts[bi], _CPtsN[bi]);
            Ray r2 = new Ray(_CPts[bj], _CPtsN[bj]);
            x1 = r1.intersection(r2); 
            x1 /= 2.0d;
            _Centre = x1;
 

            // estimate radius 
            Vectors q = new Vectors();
            _radius = 0.0d;
            for (int i = 0; i < 4; i++)
            {
                Vectors ap = new Vectors(_CPts[i]);
                ap -= _Centre; 
                _radius += ap.magnitude();
            }
            _radius /= 4.0d;

            res = Good(); 
            return res; 
        }


        //------------------------------------------------------------------------------
        public override double Dist(Vectors pt)
        {
            Vectors a = new Vectors(pt);
            a -= _Centre; 
            return a.magnitude() - _radius;
        }

        //------------------------------------------------------------------------------
        public override double DistLMA(Vectors x, double[] a)
        { 
            //x-a
            Vectors dx = new Vectors(x._X - a[0], x._Y - a[1], x._Z - a[2]);
            return dx.magnitude() - a[3];
        }

        //------------------------------------------------------------------------------
        public override double Angle(Vectors pt, Vectors n)
        { 
            // normal at x
            Vectors perp = new Vectors(pt); 
            perp -= _Centre;
            perp.normalize();
            return Math.Abs(perp.dot(n));
        }

        //------------------------------------------------------------------------------
        public override void Update(double[] a)
        {
           // Normalise(a); //TODO:: do we need this since we do nothing in Normalise?
            _Centre = new Vectors(a[0], a[1], a[2]); 
            _radius = a[3];
        }

        //------------------------------------------------------------------------------
        public override double grad(Vectors xi, double[] a, int ak)
        {
            Vectors apos = new Vectors(a[0], a[1], a[2]);
            Vectors xyz = new Vectors(xi);
            xyz -= apos; 
             double n = xyz.magnitude();
             if (n < C.CONST.EPSILON)
             {
                 return 0.0d;
             }  

            switch (ak)
            {
                case 0:
                    //df/dx
                    return  -xyz._X / n;
                case 1:
                    //df/dy
                    return  - xyz._Y  / n;
                case 2:
                    //df/dz
                    return  - xyz._Z  / n;
                case 3: 
                    //dd/dr
                    return -1.0d;
            }

            return 0.0d;
        }

        //------------------------------------------------------------------------------
        public override double[] initial()
        {
            double[] a = new double[4];
            a[0] = _Centre._X;
            a[1] = _Centre._Y;
            a[2] = _Centre._Z; 
            a[3] = _radius;

            return a;

        }

        //------------------------------------------------------------------------------
        //public override double normalCurvature(Vectors pt, Vectors ptN)
        //{ 
        //    return 1.0 / _radius;
        //}

        //------------------------------------------------------------------------------
        public override void Normalise(double[] a)
        { 

        }

        //------------------------------------------------------------------------------
        public override void ModelData(out string[] surfacedata)
        {
            surfacedata = new string[_CPts.GetLength(0) + _CPtsN.GetLength(0) + 3+_l.Count];
            surfacedata[0] = "Model: " + type();
            int i = 1;
            surfacedata[i] = "Centre: " + _Centre.ToString();
            i++;//i = 2
            surfacedata[i] = "Radius: " + _radius.ToString();
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
            outs.WriteStartElement(type() + "_Centre");
            _Centre.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement(type() + "_Radius");
            outs.WriteValue(_radius);
            outs.WriteFullEndElement();
            outs.WriteFullEndElement();

        }
        //------------------------------------------------------------------------------
    }
}


 