//------------------------------------------------------------------------------
// using definition
using System;
using System.Diagnostics;
using System.IO; 

//------------------------------------------------------------------------------
// namespace definition
namespace MathsLib
{
    //------------------------------------------------------------------------------
    // class definition
    public class Ray
    {

        //------------------------------------------------------------------------------
        // CTor
        public Ray(Vectors position,Vectors direction)
        {
            _position = new Vectors();
            _direction = new Vectors();
            _position.setPosition(position);
            _direction.setDirection(direction);
        }

        public Ray():this(new Vectors(0.0, 0.0, 0.0, 1.0),new Vectors(0.0, 0.0, -1.0, 0.0)){
            
        }

        public Ray(Ray r) : this(r.position(), r.direction()){    }

        public Vectors position()  { return _position; }
        public Vectors direction() { return _direction; } 
        
        //------------------------------------------------------------------------------
        // distance from this ray to a point
        public double distance(Vectors vector) {
             return (vector - _position).cross(_direction).magnitude();
        }
 
        //------------------------------------------------------------------------------
        // distance from this ray to another ray 
        public double distance(Ray ray)
        {  
            // determine the perpendicular to and angle between this ray and another ray
            Vectors perp= new Vectors(_direction.cross(ray._direction));
            double sine = perp.magnitude();
            if (sine <= C.CONST.EPSILON) { // near parallel rays
                return (ray._position - _position).cross(_direction).magnitude();
            }
            else {
                return Math.Abs((ray._position - _position).dot(perp /= sine));
            }
        }

        //------------------------------------------------------------------------------
        // intersection of this ray with another ray
        public Vectors intersection(Ray ray) 
        {
            // determine the perpendicular to and angle between this ray and another ray
            Vectors perp = new Vectors(_direction.cross(ray._direction));
            double sine = perp.magnitude();
            if (sine <= C.CONST.EPSILON)
            { // near parallel rays
                return _position + 0.5 * (ray._position - _position);
            }
            else {
                // determine the ray coplanar to this ray and parallel to another ray
                double length = (ray._position - _position).dot(perp /= sine);
                Ray coplanar= new Ray(ray._position - length * perp, ray._direction);
                // project to determine the intersection of this ray and the coplanar ray
                Vectors project = new Vectors((coplanar._position - _position).
                                  cross(_direction).cross(_direction));
                double lenA = (coplanar._position - _position).dot(project);
                double lenB = (coplanar._position + coplanar._direction - _position).dot(project);
                return coplanar._position + lenA / (lenA - lenB) * coplanar._direction +
                       0.5 * length * perp;
            }
        }

        //------------------------------------------------------------------------------
        // normalize this ray
        public void normalize() 
        {
             _direction.normalize(); 
        }  


        //------------------------------------------------------------------------------
        // unit vector of this ray
        public Ray unit() 
        {
            Ray R = new Ray(_position, _direction.unit());
            return R;
        }

        //------------------------------------------------------------------------------
        // negate this ray
        public void negate()
        {
            _direction.negate(); 
        }

        //------------------------------------------------------------------------------
        // negative operator- 
        public static Ray operator-(Ray r1) 
        {

            Ray R = new Ray(r1._position, -r1._direction);
            return R; 
        }

        //------------------------------------------------------------------------------
        // stream out from this ray
        public void write(ref StreamWriter outs)
        {
            outs.WriteLine("("+ _position.ToString()+","+_direction.ToString()+")");

        }

        //------------------------------------------------------------------------------
        // stream out from this ray to string
        public override string ToString()
        {
            string str = "Pos(" + _position.ToString() + "),Dir(" + _direction.ToString() + ")";
            return str;
        }
 
        private Vectors _position;
        private Vectors _direction;

        #region testing

        static public void testClass()
        {

            int REPEATS = 100000;
            System.Diagnostics.Debug.WriteLine("+++++++++Test Constructors Ray+++++++++++++++++");
            Ray rA= new Ray( new Vectors (0,1,0,1),new Vectors(1,0,0,0));
            Ray rB= new Ray( new Vectors (1,0,0.5,1),new Vectors(1,1,0,0));
            Ray rC= new Ray( rB);
            System.Diagnostics.Debug.WriteLine("+++++++++Test Arithmetic Ray+++++++++++++++++");
            Vectors vB = new Vectors(9, 8, 7, 1);
            int i = 0;
            double t = 0.0f;
            double res = 0.0f; 
            Stopwatch st = Stopwatch.StartNew();
            {
                st.Start();
                for (i = 0; i < REPEATS; ++i)
                {
                    res=rA.distance( vB);
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds)/REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("ms:"+t+"::"+res+":rA->vB,distance)");
            }
            {
                st.Start();
                for (i=0; i < REPEATS; ++i)
                {
                    res = rA.distance(rB);
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds) / REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("ms:"+t+":"+res+":rA->rB,distance");
            }
            {
                st.Start();
                for (i = 0; i < REPEATS; ++i)
                {
                    vB = rA.intersection(rB);
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds) / REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("Time Taken ms: "+t+":"+vB+":"+rA+"intersection"+rB+")" );
            }
            {
                st.Start();
                for (i = 0; i < REPEATS; ++i)
                {
                    vB = new Vectors(rA,rB);
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds) / REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("ms:"+t+":"+REPEATS+": Result:"+ vB+"=new Vectors("+rA+","+rB+")");
            }
            {
                st.Start();
                for (i = 0; i < REPEATS; ++i)
                {
                    rC = rA;
                    rC.unit();
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds) / REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("ms:"+t+":"+REPEATS+": Result:"+res+"=rA("+rA+").Unit()");
            }
            {
                st.Start();
                for (i = 0; i < REPEATS; ++i)
                {
                    rC = rA;
                    rC.normalize();
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds) / REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("Time Taken ms:"+ t+":"+REPEATS+": Result:"+res+"=rA("+rA+").normalize()" );
            }
            {
                st.Start();
                for (i = 0; i < REPEATS; ++i)
                {
                    rC = rA;
                    rC.negate();
                }
                st.Stop();
                t = (double)(st.ElapsedMilliseconds) / REPEATS;
                st.Reset();
                System.Diagnostics.Debug.WriteLine("ms:"+t+":"+REPEATS+": Result:"+res+"=rA("+rA+").negate()");
            }
        
        
        }
        #endregion

    }
}