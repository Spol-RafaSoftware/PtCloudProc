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
    public class ModelCylinder : ModelBase
    {
        private Vectors _axis;
        private Vectors _pos;
        private double _radius;
        //------------------------------------------------------------------------------
        //constructor
        public ModelCylinder(Vectors[] a, Vectors[] aN)
            : base(a, aN)
        {
            _axis = new Vectors();
            _pos = new Vectors();
            _radius = 0.0d;
        }

        //------------------------------------------------------------------------------
        public static string stype()
        {
            return "Cylinder";
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
                return Colour.Red();
            }
        } 

        //------------------------------------------------------------------------------
        public override bool Initialize()
        {
            bool res = false;
            double best = 0.0d, sw = 0.0d;
            int bi = 0, bj = 0;
            Vectors x1 = new Vectors();
            Vectors x2 = new Vectors();
            Vectors wt = new Vectors();

            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 4; j++)
                {
                    Vectors Est = _CPtsN[i].cross(_CPtsN[j]);

                    if (Est.dot(_axis) < 0.0d)
                    { 
                        Est *= -1.0d;
                    }
                    _axis += (Est);

                    double m = Est.dot(Est);
                    if (m > best)
                    {
                        bi = i;
                        bj = j;
                        best = m;
                    } 
                    Ray r1 = new Ray(_CPts[i], _CPtsN[i]);
                    Ray r2 = new Ray(_CPts[j], _CPtsN[j]);
                    x2 = r1.intersection(r2);
                    x1 += x2;
                    x1 /= 2.0d;
                    if (m > 0.5d)
                    {
                        x1 *= m;
                        wt += x1;
                        sw += m;
                    }
                }
            }
            _axis.normalize();

            // estimate poits on axis by projecting travervals of pairs of normals
            wt /= sw;
            x1 = wt;
            Ray r3 = new Ray(_CPts[bi], _CPtsN[bi]);
            Ray r4 = new Ray(_CPts[bj], _CPtsN[bj]);
            x2 = r3.intersection(r4);
            x1 += x2;
            x1 /= 2.0d;

            x1 -= _axis * _axis.dot(x1);

            _pos += x1;

            // estimate radius 
            Vectors q = new Vectors();
            _radius = 0.0d;
            for (int i = 0; i < 4; i++)
            {
                Vectors ap = new Vectors(_CPts[i]);
                ap -= _pos;
                ap = ap.cross(_axis);
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
            a -= _pos;
            a = a.cross(_axis);
            return a.magnitude() - _radius;
        }

        //------------------------------------------------------------------------------
        public override double DistLMA(Vectors x, double[] a)
        {
            // return 0.0d;
            if (a.GetLength(0) > 6)
                return f(x, a) - a[6];
            else
                return 0.0d;

        }

        //------------------------------------------------------------------------------
        public override double Angle(Vectors pt, Vectors n)
        {
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
            _pos = new Vectors(a[0], a[1], a[2]);
            _axis = new Vectors(a[3], a[4], a[5]);
            _radius = a[6];
        }

        //------------------------------------------------------------------------------
        public override double grad(Vectors xi, double[] a, int ak)
        {
            Vectors apos = new Vectors(a[0], a[1], a[2]);
            Vectors xyz = new Vectors(xi);
            xyz -= apos;
            Vectors abc = new Vectors(a[3], a[4], a[5]);

            double df = f(xi, a);
            double dg = g(xi, a);

            switch (ak)
            {
                case 0:
                    //df/dx
                    return (abc._X * dg - xyz._X) / df;
                case 1:
                    //df/dy
                    return (abc._Y * dg - xyz._Y) / df;
                case 2:
                    //df/dz
                    return (abc._Z * dg - xyz._Z) / df;
                case 3:
                    //df/da
                    return dg * (abc._X * dg - xyz._X) / df;
                case 4:
                    //df/db
                    return dg * (abc._Y * dg - xyz._Y) / df;
                case 5:
                    //df/dc
                    return dg * (abc._Z * dg - xyz._Z) / df;
                case 6:
                    //dd/dr
                    return -1.0d;
            }

            return 0.0d;
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
            a[6] = _radius;

            return a;

        }

       // //------------------------------------------------------------------------------
        //public override double normalCurvature(Vectors pt, Vectors ptN)
        //{
       //     double t = ptN.dot(_axis);
       //     return t * t / _radius;
       // }

        //------------------------------------------------------------------------------
        public override void Normalise(double[] a)
        {
            //double   n = 1.0d;
            Vectors abc = new Vectors(a[3], a[4], a[5]);
            abc.normalize();
            a[3] = abc._X;
            a[4] = abc._Y;
            a[5] = abc._Z;

        }

        //------------------------------------------------------------------------------
        public override void ModelData(out string[] surfacedata)
        {
            surfacedata = new string[_CPts.GetLength(0) + _CPtsN.GetLength(0) + 4 + _l.Count];
            surfacedata[0] = "Model: " + type();
            int i = 1;
            surfacedata[i] = "Pos: " + _pos.ToString();
            i++;//i = 2
            surfacedata[i] = "axis: " + _axis.ToString();
            i++;//i = 3 
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
            outs.WriteStartElement(type() + "_Position");
            _pos.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement(type() + "_axis");
            _axis.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement(type() + "_Radius");
            outs.WriteValue(_radius);
            outs.WriteFullEndElement();
            outs.WriteFullEndElement();

        }
        //------------------------------------------------------------------------------
    }
}
 