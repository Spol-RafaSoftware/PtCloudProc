//------------------------------------------------------------------------------
// namespace
using System;
using System.IO;
using System.Collections.Generic; 
using MathsLib; 
using Tao.OpenGl;
 
//------------------------------------------------------------------------------
// namespace definition
namespace Render {

    //------------------------------------------------------------------------------
    // class definition

    class Surface3D
    {
        Vectors[][] _point;
        Patch3D[][] _patch;
        int _m;
        int _n;
        int _u;
        int _v;
        bool _spin;
        double _angleX;
        double _angleY;
        double _angleZ;
        bool _spinX;
        bool _spinY;
        bool _spinZ;
        int _tiles;
        Texture _texture;


        //------------------------------------------------------------------------------
        // create
        private void create()
        {
            int u;
            int v;
            _point = new Vectors[_n][];
            for (v = 0; v < _n; v++)
            {
                _point[v] = new Vectors[_m];
                for (u = 0; u < _m; u++)
                {
                    _point[v][u] = new Vectors((double)u / (double)(_m - 1) - 0.5,
                        (double)v / (double)(_n - 1) - 0.5, 0.0);
                }
            }
            _patch = new Patch3D[_n - 1][];
            for (v = 0; v < _n - 1; v++)
            {
                _patch[v] = new Patch3D[_m - 1];
                for (u = 0; u < _m - 1; u++)
                {
                    _patch[v][u] = new Patch3D(u, v, _tiles);
                }
            }
        }

        //------------------------------------------------------------------------------
        // erase
        private void erase()
        {
            int u;
            int v;
            for (v = 0; v < _n; v++)
            {
                _point[v] = null;
            }
            _point = null;
            for (v = 0; v < _n - 1; v++)
            {
                for (u = 0; u < _m - 1; u++)
                {
                    _patch[v][u] = null;
                }
                _patch[v] = null;
            }
            _patch = null;
        }

        //------------------------------------------------------------------------------
        // copy
        private void copy(Surface3D surface3D)
        {
            _m = surface3D._m;
            _n = surface3D._n;
            _u = surface3D._u;
            _v = surface3D._v;
            create();
            int u;
            int v;
            for (v = 0; v < _n; v++)
            {
                for (u = 0; u < _m; u++)
                {
                    _point[v][u] = surface3D._point[v][u];
                }
            }
            for (v = 0; v < _n - 1; v++)
            {
                for (u = 0; u < _m - 1; u++)
                {
                    _patch[v][u] = surface3D._patch[v][u];
                }
            }
        }


        //------------------------------------------------------------------------------
        // wrap coordinates
        private void wrap()
        {
            if (_u < 0)
            {
                _u = _m - 1;
            }
            else if (_u > _m - 1)
            {
                _u = 0;
            }
            if (_v < 0)
            {
                _v = _n - 1;
            }
            else if (_v > _n - 1)
            {
                _v = 0;
            }
        }

        //------------------------------------------------------------------------------
        // clip coordinates
        private void clip()
        {
            if (_u < 0)
            {
                _u = 0;
            }
            else if (_u > _m - 1)
            {
                _u = _m - 1;
            }
            if (_v < 0)
            {
                _v = 0;
            }
            else if (_v > _n - 1)
            {
                _v = _n - 1;
            }
        }

        //------------------------------------------------------------------------------
        // constructor 
        public Surface3D(int m, int n)
        {
            _m = (m);
            _n = (n);
            _u = (0);
            _v = (0);
            _spin = (false);
            _angleX = (0.0);
            _angleY = (0.0);
            _angleZ = (0.0);
            _spinX = (false);
            _spinY = (true);
            _spinZ = (false);
            _tiles = (16);
            _texture = new Texture();
            create();
        }

        public Surface3D() : this(2, 2) { }

        //------------------------------------------------------------------------------
        // copy constructor
        public Surface3D(Surface3D surface3D)
        {
            copy(surface3D);
        }


        //------------------------------------------------------------------------------
        // CreatePatch
        public void CreatePatch(int m, int n, Vectors Min, Vectors Max)
        {
            erase();
            _m = m;
            _n = n;
            clip();
            create();
            double x = 0.0d, y = 0.0d, z = 0.0d;
            for (int v = 0; v < _n; v++)
            {
                x = Min._X + (Max._X - Min._X) * ((double)v / (double)_n);
                for (int u = 0; u < _m; u++)
                {
                    y = Min._Y + (Max._Y - Min._Y) * ((double)u / (double)_m);
                    z=(Max._Z - Min._Z)/2.0d;
                    _point[v][u]._X = x;
                    _point[v][u]._Y = y;
                    _point[v][u]._Z = z;
                }
            }
        } 


        //------------------------------------------------------------------------------
        // read
        public void read(string filename)
        {
            UInt64 count = 0;
            try
            {
                if (!File.Exists(filename))
                {
                    System.Diagnostics.Debug.WriteLine("Error : Invalid file name = " + filename);
                    throw new SystemException("Error : Invalid file name = " + filename);
                }
                DateTime startTime = DateTime.Now;
                using (StreamReader sr = File.OpenText(filename))
                {
                    long length = sr.BaseStream.Length;
                    string t;

                    erase();
                    t = sr.ReadLine();
                    _m = int.Parse(t);
                    t = sr.ReadLine();
                    _n = int.Parse(t);
                    clip();
                    create();
                    for (int v = 0; v < _n; v++)
                    {
                        for (int u = 0; u < _m; u++)
                        {
                            double x = 0.0d, y = 0.0d, z = 0.0d;
                            string[] num = t.Split(' ');
                            x = double.Parse(num[0]);
                            y = double.Parse(num[1]);
                            z = double.Parse(num[2]);
                            count++;
                            _point[v][u]._X = x;
                            _point[v][u]._Y = y;
                            _point[v][u]._Z = z;
                        }
                    }

                }

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                System.Diagnostics.Debug.WriteLine("Text XYZ Read"+filename+" with"+count+" pts duration: "+duration.TotalMilliseconds+" milliseconds.");
            }
            catch (SystemException s)
            {
                System.Diagnostics.Debug.WriteLine("{0}", s.Message.ToString());
            }


        }

        //------------------------------------------------------------------------------
        // write
        public void write(string filename)
        {
         
            try
            { 
                FileStream fs = File.Create(filename);
                using (StreamWriter sw = new StreamWriter(fs))
                {  
                    // determine cubic values 
                    sw.WriteLine("{0},",_m);
                    sw.WriteLine("{0},", _n );
                     for (int v = 0; v < _n; v++) {
                         for (int u = 0; u < _m; u++) {
                             sw.WriteLine("{0} {1} {2}", _point[v][u]._X , _point[v][u]._Y , _point[v][u]._Z);
                         }
                     }
                    sw.Close();
                }
            }
            catch (SystemException s)
            {
                System.Diagnostics.Debug.WriteLine("{0}", s.Message.ToString());
            } 
        }

        //------------------------------------------------------------------------------
        // clear
        public void clear()
        {
            erase();
            _m = 2;
            _n = 2;
            clip();
            create();
        }

        //------------------------------------------------------------------------------
        // preset
        public void preset()
        {
            erase();
            _m = 4;
            _n = 4;
            clip();
            create();
        }

        //------------------------------------------------------------------------------
        // next coordinate
        public void next(bool u, bool v)
        {
            if (u)
            {
                _u++;
            }
            if (v)
            {
                _v++;
            }
            wrap();
        }

        //------------------------------------------------------------------------------
        // previous coordinate
        public void previous(bool u, bool v)
        {
            if (u)
            {
                _u--;
            }
            if (v)
            {
                _v--;
            }
            wrap();
        }

        //------------------------------------------------------------------------------
        // first coordinate
        public void first(bool u, bool v)
        {
            if (u)
            {
                _u = 0;
            }
            if (v)
            {
                _v = 0;
            }
        }

        //------------------------------------------------------------------------------
        // last coordinate
        public void last(bool u, bool v)
        {
            if (u)
            {
                _u = _m - 1;
            }
            if (v)
            {
                _v = _n - 1;
            }
        }


        //------------------------------------------------------------------------------
        // modify
        public void modify(double x, double y, double z)
        {
            _point[_v][_u] += new Vectors(x, y, z);
            for (int v = _v - 2; v < _v + 2; v++)
            {
                if (v >= 0 && v < _n - 1)
                {
                    for (int u = _u - 2; u < _u + 2; u++)
                    {
                        if (u >= 0 && u < _m - 1)
                        {
                            _patch[v][u].modify();
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        // insert
        public void insert()
        {
            if (_u < _m - 1 && _v < _n - 1)
            {
                int u;
                int v;
                // copy old z
                int nOld = _n;
                Vectors[][] pointOld = _point;
                for (v = 0; v < _n; v++)
                {
                    pointOld[v] = _point[v];
                    for (u = 0; u < _m; u++)
                    {
                        pointOld[v][u] = _point[v][u];
                    }
                }
                _m++;
                _n++;
                // create new z
                _point = new Vectors[_n][];
                for (v = 0; v < _n; v++)
                {
                    _point[v] = new Vectors[_m];
                    for ( u = 0; u < _m; u++)
                    {
                        if (v <= _v)
                        {
                            if (u <= _u)
                            {
                                _point[v][u] = pointOld[v][u];
                            }
                            else if (u == _u + 1)
                            {
                                _point[v][u] = 0.5d * (pointOld[v][u - 1] + pointOld[v][u]);
                            }
                            else
                            {
                                _point[v][u] = pointOld[v][u - 1];
                            }
                        }
                        else if (v == _v + 1)
                        {
                            if (u <= _u)
                            {
                                _point[v][u] = 0.5d * (pointOld[v - 1][u] + pointOld[v][u]);
                            }
                            else if (u == _u + 1)
                            {
                                _point[v][u] = 0.25d * (pointOld[v - 1][u - 1] + pointOld[v - 1][u]
                                    + pointOld[v][u - 1] + pointOld[v][u]);
                            }
                            else
                            {
                                _point[v][u] = 0.5d * (pointOld[v - 1][u - 1] + pointOld[v][u - 1]);
                            }
                        }
                        else
                        {
                            if (u <= _u)
                            {
                                _point[v][u] = pointOld[v - 1][u];
                            }
                            else if (u == _u + 1)
                            {
                                _point[v][u] = 0.5d * (pointOld[v - 1][u - 1] + pointOld[v - 1][u]);
                            }
                            else
                            {
                                _point[v][u] = pointOld[v - 1][u - 1];
                            }
                        }
                    }
                }
                // erase patch
                for (v = 0; v < _n - 2; v++)
                {
                    for (u = 0; u < _m - 2; u++)
                    {
                        _patch[v][u] = null;
                    }
                    _patch[v] = null;
                }
                _patch = null;

                // create patch
                _patch = new Patch3D[_n - 1][];
                for (v = 0; v < _n - 1; v++)
                {
                    _patch[v] = new Patch3D[_m - 1];
                    for (u = 0; u < _m - 1; u++)
                    {
                        _patch[v][u] = new Patch3D(u, v, _tiles);
                    }
                }
                // erase old z
                for (v = 0; v < nOld; v++)
                {
                    pointOld[v] = null;
                }
                pointOld = null;
                _u++;
                _v++;
            }
        }

        //------------------------------------------------------------------------------
        // remove
        public void remove()
        {
            if (_u > 0 && _u < _m - 1 && _v > 0 && _v < _n - 1 && _m > 2 && _n > 2)
            {
                int u;
                int v;
                // copy old z
                int nOld = _n;
                Vectors[][] pointOld = _point;
                for (v = 0; v < _n; v++)
                {
                    pointOld[v] = _point[v];
                    for (u = 0; u < _m; u++)
                    {
                        pointOld[v][u] = _point[v][u];
                    }
                }
                for (v = 0; v < _n - 1; v++)
                {
                    for (u = 0; u < _m - 1; u++)
                    {
                        _patch[v][u] = null;
                    }
                    _patch[v] = null;
                }
                _patch = null;
                _m--;
                _n--;
                // create new z
                _point = new Vectors[_n][];
                for (v = 0; v < _n; v++)
                {
                    _point[v] = new Vectors[_m];
                    for (u = 0; u < _m; u++)
                    {
                        if (v < _v)
                        {
                            if (u < _u)
                            {
                                _point[v][u] = pointOld[v][u];
                            }
                            else
                            {
                                _point[v][u] = pointOld[v][u + 1];
                            }
                        }
                        else
                        {
                            if (u < _u)
                            {
                                _point[v][u] = pointOld[v + 1][u];
                            }
                            else
                            {
                                _point[v][u] = pointOld[v + 1][u + 1];
                            }
                        }
                    }
                }
                _patch = new Patch3D[_n - 1][];
                for (v = 0; v < _n - 1; v++)
                {
                    _patch[v] = new Patch3D[_m - 1];
                    for (u = 0; u < _m - 1; u++)
                    {
                        _patch[v][u] = new Patch3D(u, v, _tiles);
                    }
                }
                // erase old z
                for (v = 0; v < nOld; v++)
                {
                    pointOld[v] = null;
                }
                pointOld = null;
                _u--;
                _v--;
            }
        }

        //------------------------------------------------------------------------------
        // render
        public void render(bool markers, bool wireframe, bool hull, bool solid, bool axes, bool texture, int w,int h) 
        {
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            int width = w;
            int height = h;
            Gl.glViewport(0, 0, width, height);
            Gl.glPushMatrix();
            Gl.glRotated(_angleZ, 0.0, 0.0, 1.0);
            Gl.glRotated(_angleX, 1.0, 0.0, 0.0);
            Gl.glRotated(_angleY, 0.0, 1.0, 0.0);
            const double INCREMENT = 1.0;
            if (_spin && _spinX) {
                _angleX += INCREMENT;
            }
            if (_spin && _spinY) {
                _angleY += INCREMENT;
            }
            if (_spin && _spinZ) {
                _angleZ += INCREMENT;
            }
            if (hull) {
                // highlight control point hull
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glPushMatrix();
                Gl.glTranslated(0.0, 0.0, 0.002);
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glColor3d(1.0, 1.0, 0.0);
                for (int v = 0; v < _n - 1; v++) {
                    for (int u = 0; u < _m - 1; u++) {
                        double[] point;
                        Gl.glVertex3dv(_point[v][u].toDouble3(out point));
                        u++;
                        Gl.glVertex3dv(_point[v][u].toDouble3(out point));
                        v++;
                        Gl.glVertex3dv(_point[v][u].toDouble3(out point));
                        u--;
                        Gl.glVertex3dv(_point[v][u].toDouble3(out point));
                        v--;
                    }
                }
                Gl.glEnd();
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                Gl.glPopMatrix();
                Gl.glEnable(Gl.GL_LIGHTING);
            }
            if (markers) {
                // highlight control points
                float[] ambient = { 1.0f, 1.0f, 0.0f, 1.0f };
                float[] diffuse = { 1.0f, 1.0f, 0.0f, 1.0f };
                float[] specular = { 1.0f, 1.0f, 1.0f, 1.0f };
                float shininess = 10.0f;
                float[] emission = { 0.0f, 0.0f, 0.0f, 0.0f };
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, ambient);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, diffuse);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, specular);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, shininess);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, emission);
                for (int v = 0; v < _n; v++) {
                    for (int u = 0; u < _m; u++) {
                        Gl.glPushMatrix();
                        Gl.glTranslated(_point[v][u]._X, _point[v][u]._Y, _point[v][u]._Z);
                        Glu.GLUquadric sphere = Glu.gluNewQuadric();
                        Glu.gluQuadricDrawStyle(sphere, Glu.GLU_FILL);
                        Glu.gluQuadricNormals(sphere, Glu.GLU_SMOOTH);
                        Glu.gluSphere(sphere, 0.1d, 4, 2);
                        Glu.gluDeleteQuadric(sphere);
                        Gl.glPopMatrix();
                    }
                }
                // highlight current control point
                float[] ambient1 = { 0.0f, 1.0f, 1.0f, 1.0f };
                float[] diffuse1 = { 0.0f, 1.0f, 1.0f, 1.0f };
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, ambient1);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, diffuse1);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, specular);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, shininess);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, emission);
                Gl.glPushMatrix();
                Gl.glTranslated(_point[_v][_u]._X, _point[_v][_u]._Y, _point[_v][_u]._Z);
                Glu.GLUquadric sphere1 = Glu.gluNewQuadric();
                Glu.gluQuadricDrawStyle(sphere1, Glu.GLU_FILL);
                Glu.gluQuadricNormals(sphere1, Glu.GLU_SMOOTH);
                Glu.gluSphere(sphere1, 0.02d, 20, 10);
                Glu.gluDeleteQuadric(sphere1);
                Gl.glPopMatrix();
            }
            if (axes) {
                // highlight current control point axes
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glPushMatrix();
                Gl.glTranslated(_point[_v][_u]._X, _point[_v][_u]._Y, _point[_v][_u]._Z);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(1.0, 0.0, 0.0);
                Gl.glVertex3d(0.0, 0.0, 0.0);
                Gl.glVertex3d(0.2, 0.0, 0.0);
                Gl.glColor3d(0.0, 1.0, 0.0);
                Gl.glVertex3d(0.0, 0.0, 0.0);
                Gl.glVertex3d(0.0, 0.2, 0.0);
                Gl.glColor3d(0.0, 0.0, 1.0);
                Gl.glVertex3d(0.0, 0.0, 0.0);
                Gl.glVertex3d(0.0, 0.0, 0.2);
                Gl.glEnd();
                Gl.glPopMatrix();
                Gl.glEnable(Gl.GL_LIGHTING);
            }
            _texture.render();
            if (solid) {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, !wireframe ? Gl.GL_FILL : Gl.GL_LINE);
                for (int v = 0; v < _n - 1; v++) {
                    for (int u = 0; u < _m - 1; u++) {
                        _patch[v][u].render(_point, _m, _n, texture);
                    }
                }
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            }
            Gl.glPopMatrix();
        }

        //------------------------------------------------------------------------------
        // rotate
        public void rotate(double x, double y, double z)
        {
            _angleX += x;
            _angleY += y;
            _angleZ += z;
        }

        //------------------------------------------------------------------------------
        // play spin
        public void play()
        {
            _spin = !_spin;
        }

        //------------------------------------------------------------------------------
        // set spin axis
        public void spin()
        {
            if (_spinX)
            {
                _spinX = false;
                _spinY = true;
                _spinZ = false;
            }
            else if (_spinY)
            {
                _spinX = false;
                _spinY = false;
                _spinZ = true;
            }
            else if (_spinZ)
            {
                _spinX = true;
                _spinY = false;
                _spinZ = false;
            }
        }

        //------------------------------------------------------------------------------
        // stop spin
        public void stop()
        {
            _spin = false;
            _angleX = 0.0;
            _angleY = 0.0;
            _angleZ = 0.0;
        }

        //------------------------------------------------------------------------------
        // double tiles
        public void doubleTiles()
        {
            _tiles *= 2;
            for (int v = 0; v < _n - 1; v++)
            {
                for (int u = 0; u < _m - 1; u++)
                {
                    _patch[v][u].tiles(_tiles);
                }
            }
        }

        //------------------------------------------------------------------------------
        // half tiles
        public void halfTiles()
        {
            _tiles /= 2;
            if (_tiles < 1)
            {
                _tiles = 1;
            }
            for (int v = 0; v < _n - 1; v++)
            {
                for (int u = 0; u < _m - 1; u++)
                {
                    _patch[v][u].tiles(_tiles);
                }
            }
        }

        //------------------------------------------------------------------------------
        // texture
        public void texture() {
            _texture.read();
        }

        //------------------------------------------------------------------------------
        // mesh
        public void mesh(string filename)
        {

            try
            {
                FileStream fs = File.Create(filename);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    // determine cubic values 
                    sw.WriteLine("Screen {");
                    sw.WriteLine("Width 600");
                    sw.WriteLine("Height 600");
                    sw.WriteLine("Background 0.75 0.75 0.75");

                    sw.WriteLine("Camera {");
                    sw.WriteLine("Rotation 0.0 0.0 0.0");
                    sw.WriteLine("Distance 0.0");
                    sw.WriteLine("Centre 0.0 0.0 -4.0");
                    sw.WriteLine("FieldOfView {0}", Math.Atan(1.0 / 4.0) * C.CONST.RAD_TO_DEG * 2.0);
                    sw.WriteLine("}");

                    sw.WriteLine("}");

                    sw.WriteLine("Group {");

                    sw.WriteLine("Light {");
                    sw.WriteLine("Infinite false");
                    sw.WriteLine("Centre 0.0 0.0 6.0");
                    sw.WriteLine("Ambient 0.5 0.5 0.5");
                    sw.WriteLine("Diffuse 0.2 0.2 0.2");
                    sw.WriteLine("Specular 0.5 0.5 0.5");
                    sw.WriteLine("}");

                    sw.WriteLine("Light {");
                    sw.WriteLine("Infinite false");
                    sw.WriteLine("Centre 0.0 10.0 -4.0");
                    sw.WriteLine("Ambient 0.0 0.0 0.0");
                    sw.WriteLine("Diffuse 0.2 0.2 0.2");
                    sw.WriteLine("Specular 0.5 0.5 0.5");
                    sw.WriteLine("}");

                    sw.WriteLine("Transformation {");
                    sw.WriteLine("Translate 0.0 0.0 -4.0");
                    sw.WriteLine("}");

                    sw.WriteLine("Transformation {");
                    sw.WriteLine("Axis 0.0 0.0 1.0");
                    sw.WriteLine("Angle {0}", _angleZ);
                    sw.WriteLine("}");

                    sw.WriteLine("Transformation {");
                    sw.WriteLine("Axis 1.0 0.0 0.0");
                    sw.WriteLine("Angle {0}", _angleX);
                    sw.WriteLine("}");

                    sw.WriteLine("Transformation {");
                    sw.WriteLine("Axis 0.0 1.0 0.0");
                    sw.WriteLine("Angle {0}", _angleY);
                    sw.WriteLine("}");

                    for (int v = 0; v < _n - 1; v++)
                    {
                        for (int u = 0; u < _m - 1; u++)
                        {
                            _patch[v][u].write(sw, _m, _n);
                        }
                    }

                    sw.WriteLine("}");
                }
            }
            catch (SystemException s)
            {
                System.Diagnostics.Debug.WriteLine("{0}", s.Message.ToString());
            }

        }
    }

 

} // ComputerGraphics
 

//------------------------------------------------------------------------------
