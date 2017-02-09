//------------------------------------------------------------------------------
// namespace
using System;
using System.IO;
using System.Collections.Generic; 
using System.Text;
using MathsLib;
using Tao.OpenGl;
 

namespace Render {

    //------------------------------------------------------------------------------
    // class definition

    class Patch3D { 
        private int _u;
        private int _v;
        private int _tiles;
        private bool _valid;
        private Vectors[][] _matrix;
        private Vectors[][] _vertex;
        private Vectors[][] _normal;
 

        //------------------------------------------------------------------------------
        // constructor
        public Patch3D(int u, int v, int tiles)
        { 
            _u = u;
            _v = v;
            _tiles = tiles;
            _valid = false;
            _matrix = new Vectors[C.CONST.ORDER][];
            for (int i = 0; i < C.CONST.ORDER; i++) {
                _matrix[i] = new Vectors[C.CONST.ORDER]; 
                for (int j = 0; j < C.CONST.ORDER; j++) {
                    _matrix[i][j] =new Vectors();
                }
            }
            create();
        }
         
   
        //------------------------------------------------------------------------------
        // copy constructor
        public Patch3D(  Patch3D  patch3D) {
            copy(patch3D);
        }
 

        //------------------------------------------------------------------------------
        // create
        private void create() {
            _vertex = new Vectors [(_tiles + 1)][];
            _normal = new Vectors [(_tiles + 1)][];
            for (int v = 0; v < (_tiles + 1); v++) {
                _vertex[v] = new Vectors[(_tiles + 1)];
                _normal[v] = new Vectors[(_tiles + 1)];
            }
        }

        //------------------------------------------------------------------------------
        // erase
        private void erase() {
            for (int v = 0; v < (_tiles + 1); v++) {
                _vertex[v] = null;
                _normal[v]= null;
            }
            _vertex= null;
            _normal =null;
        }

        //------------------------------------------------------------------------------
        // copy
        private void copy(  Patch3D patch3D)
        {
            _u = patch3D._u;
            _v = patch3D._v;
            _valid = patch3D._valid;
            _tiles = patch3D._tiles;
            for (int i = 0; i < C.CONST.ORDER; i++) {
                for (int j = 0; j < C.CONST.ORDER; j++) {
                    _matrix[i][j] = patch3D._matrix[i][j];
                }
            }
            create();
            for (int v = 0; v < (_tiles + 1); v++) {
                for (int u = 0; u < (_tiles + 1); u++) {
                    _vertex[v][u] = patch3D._vertex[v][u];
                    _normal[v][u] = patch3D._normal[v][u];
                }
            }
        }

        //------------------------------------------------------------------------------
        // tiles
        public void tiles(int tiles) {
            _valid = false;
            erase();
            _tiles = tiles;
            create();
        }

        //------------------------------------------------------------------------------
        // modify
        public void modify() {
            _valid = false;
        }

        //------------------------------------------------------------------------------
        // render
        public void render(Vectors[][] point, int m, int n, bool texture) {
            if (!_valid) {
                _valid = true;
                matrices(point, m, n);
                vertices();
                normals();
            }
            double r = (double)_u / (double)(m - 2);
            double g = (double)_v / (double)(n - 2);
            double b = (1.0 - r) * (1.0 - g);
            float[] white = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] ambient = { (float)r, (float)g, (float)b, 1.0f };
            float[] diffuse = { (float)r, (float)g, (float)b, 1.0f };
            float[] specular = { 0.5f, 0.5f, 0.5f, 1.0f };
            float shininess = 10.0f;
            float[] emission = { 0.0f, 0.0f, 0.0f, 0.0f };
            if (texture) {
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, white);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, white);
            }
            else {
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, ambient);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, diffuse);
            }
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, specular);
            Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, shininess);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, emission);
            if (texture) {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
            }
            Gl.glBegin(Gl.GL_QUADS);
            for (int i = 0; i < _tiles; i++) {
                for (int j = 0; j < _tiles; j++) {
                    int u = _u * _tiles + j;
                    int v = _v * _tiles + i;
                    double[] point1;
                    Gl.glTexCoord2d((double)u / (double)((m - 1) * _tiles),
                        (double)v / (double)((n - 1) * _tiles));
                    Gl.glNormal3dv(_normal[i][j].toDouble3(out point1));
                    Gl.glVertex3dv(_vertex[i][j].toDouble3(out point1));
                    u++;
                    Gl.glTexCoord2d((double)u / (double)((m - 1) * _tiles),
                        (double)v / (double)((n - 1) * _tiles));
                    Gl.glNormal3dv(_normal[i][j + 1].toDouble3(out point1));
                    Gl.glVertex3dv(_vertex[i][j + 1].toDouble3(out point1));
                    v++;
                    Gl.glTexCoord2d((double)u / (double)((m - 1) * _tiles),
                        (double)v / (double)((n - 1) * _tiles));
                    Gl.glNormal3dv(_normal[i + 1][j + 1].toDouble3(out point1));
                    Gl.glVertex3dv(_vertex[i + 1][j + 1].toDouble3(out point1));
                    u--;
                    Gl.glTexCoord2d((double)u / (double)((m - 1) * _tiles),
                        (double)v / (double)((n - 1) * _tiles));
                    Gl.glNormal3dv(_normal[i + 1][j].toDouble3(out point1));
                    Gl.glVertex3dv(_vertex[i + 1][j].toDouble3(out point1));
                    v--;
                }
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        //------------------------------------------------------------------------------
        // matrices
        public void matrices(Vectors [][]point, int m, int n) {
            int i;
            int j;
            int k;
            Vectors[][] Z = new Vectors[C.CONST.ORDER][];
            for (i = 0; i < C.CONST.ORDER; i++) {
                Z[i] = new Vectors[C.CONST.ORDER];
                for (j = 0; j < C.CONST.ORDER; j++) {
                    int v = _v + j - 1;
                    v = (v < 0) ? 0 : ((v > n - 1) ? n - 1 : v);
                    int u = _u + i - 1;
                    u = (u < 0) ? 0 : ((u > m - 1) ? m - 1 : u);
                    Z[i][j] = point[v][u];
                }
            }
            double[][] B = new double [][]{
                new double []{ -1.0 / 6.0,  3.0 / 6.0, -3.0 / 6.0, 1.0 / 6.0 },
                new double []{  3.0 / 6.0, -6.0 / 6.0,  3.0 / 6.0, 0.0 / 6.0 },
                new double []{ -3.0 / 6.0,  0.0 / 6.0,  3.0 / 6.0, 0.0 / 6.0 },
               new double [] {  1.0 / 6.0,  4.0 / 6.0,  1.0 / 6.0, 0.0 / 6.0 }};

            Vectors[][] BZ= new Vectors[C.CONST.ORDER][];
            for (i = 0; i < C.CONST.ORDER; i++) {
                BZ[i] = new Vectors[C.CONST.ORDER];
                for (j = 0; j < C.CONST.ORDER; j++) {
                    BZ[i][j] = new Vectors();
                    for (k = 0; k < C.CONST.ORDER; k++) {
                        BZ[i][j] += B[i][k] * Z[k][j];
                    }
                }
            }
            for (i = 0; i < C.CONST.ORDER; i++) {
                for (j = 0; j < C.CONST.ORDER; j++) {
                    _matrix[i][j] =new  Vectors();
                    for (k = 0; k < C.CONST.ORDER; k++) {
                        _matrix[i][j] += BZ[i][k] * B[j][k];
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        // vertices
        public void vertices() {
            for (int v = 0; v < (_tiles + 1); v++) {
                double y = (double)v / (double)_tiles;
                for (int u = 0; u < (_tiles + 1); u++) {
                    double x = (double)u / (double)_tiles;
                    Vectors []T = new Vectors[C.CONST.ORDER];
                    for (int j = 0; j < C.CONST.ORDER; j++) {
                        T[j] = _matrix[0][j];
                        for (int k = 1; k < C.CONST.ORDER; k++) {
                            T[j] = x * T[j] + _matrix[k][j];
                        }
                    }
                    _vertex[v][u] = T[0];
                    for (int k = 1; k < C.CONST.ORDER; k++) {
                        _vertex[v][u] = y * _vertex[v][u] + T[k];
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        // normals
        public void normals() {
            for (int v = 0; v < (_tiles + 1); v++) {
                double y = (double)v / (double)_tiles;
                for (int u = 0; u < (_tiles + 1); u++) {
                    double x = (double)u / (double)_tiles;
                    int j;
                    int k;
                    Vectors[] T= new Vectors[C.CONST.ORDER];
                    // d/du tangent
                    for (j = 0; j < C.CONST.ORDER; j++) {
                        T[j] = (double)(C.CONST.ORDER - 1) * _matrix[0][j];
                        for (k = 1; k < C.CONST.ORDER - 1; k++) {
                            T[j] = x * T[j] + (double)(C.CONST.ORDER - 1 - k) * _matrix[k][j];
                        }
                    }
                    Vectors du = T[0];
                    for (k = 1; k < C.CONST.ORDER; k++) {
                        du = y * du + T[k];
                    }
                    // d/dv tangent
                    for (j = 0; j < C.CONST.ORDER; j++) {
                        T[j] = _matrix[0][j];
                        for (k = 1; k < C.CONST.ORDER; k++) {
                            T[j] = x * T[j] + _matrix[k][j];
                        }
                    }
                    Vectors dv = (double)(C.CONST.ORDER - 1) * T[0];
                    for (k = 1; k < C.CONST.ORDER - 1; k++) {
                        dv = y * dv + (double)(C.CONST.ORDER - 1 - k) * T[k];
                    }
                    // cross product
                    _normal[v][u] = du.cross(dv).unit();
                }
            }
        }

        //------------------------------------------------------------------------------
        // write
        public void write(StreamWriter sw, int m, int n) 
        {
            sw.WriteLine("Group {" );
            sw.WriteLine("Intersection {" );
            int u;
            int v;
            double xMin = double.PositiveInfinity;
            double xMax = -double.PositiveInfinity;
            double yMin = double.PositiveInfinity;
            double yMax = -double.PositiveInfinity;
            double zMin = double.PositiveInfinity;
            double zMax = -double.PositiveInfinity;
            for (v = 0; v < (_tiles + 1); v++) {
                for (u = 0; u < (_tiles + 1); u++) {
                    if (_vertex[v][u]._X < xMin) {
                        xMin = _vertex[v][u]._X;
                    }
                    if (_vertex[v][u]._X > xMax)
                    {
                        xMax = _vertex[v][u]._X;
                    }
                    if (_vertex[v][u]._Y < yMin)
                    {
                        yMin = _vertex[v][u]._Y;
                    }
                    if (_vertex[v][u]._Y > yMax)
                    {
                        yMax = _vertex[v][u]._Y;
                    }
                    if (_vertex[v][u]._Z < zMin) {
                        zMin = _vertex[v][u]._Z;
                    }
                    if (_vertex[v][u]._Z > zMax)
                    {
                        zMax = _vertex[v][u]._Z;
                    }
                }
            }
            const double BORDER = 0.001;
            sw.WriteLine("Plane {" );
            sw.WriteLine("Normal -1.0 0.0 0.0" );
            sw.WriteLine("Up 0.0 1.0 0.0" );
            sw.WriteLine("Centre {0} 0.0 0.0", xMin - BORDER  );
            sw.WriteLine("Visible false" );
            sw.WriteLine("}" );
            sw.WriteLine("Plane {" );
            sw.WriteLine("Normal 1.0 0.0 0.0" );
            sw.WriteLine("Up 0.0 1.0 0.0" );
            sw.WriteLine("Centre {0} 0.0 0.0" ,xMax + BORDER );
            sw.WriteLine("Visible false" );
            sw.WriteLine("}" );
            sw.WriteLine("Plane {" );
            sw.WriteLine("Normal 0.0 -1.0 0.0" );
            sw.WriteLine("Up 1.0 0.0 0.0" );
            sw.WriteLine("Centre 0.0 {0}  0.0" , yMin - BORDER  );
            sw.WriteLine("Visible false" );
            sw.WriteLine("}" );
            sw.WriteLine("Plane {" );
            sw.WriteLine("Normal 0.0 1.0 0.0" );
            sw.WriteLine("Up 1.0 0.0 0.0" );
            sw.WriteLine("Centre 0.0 {0}  0.0", yMax + BORDER  );
            sw.WriteLine("Visible false" );
            sw.WriteLine("}" );
            sw.WriteLine("Plane {" );
            sw.WriteLine("Normal 0.0 0.0 -1.0" );
            sw.WriteLine("Up 1.0 0.0 0.0" );
            sw.WriteLine("Centre 0.0 0.0 {0}" , zMin - BORDER );
            sw.WriteLine("Visible false" );
            sw.WriteLine("}" );
            sw.WriteLine("Plane {" );
            sw.WriteLine("Normal 0.0 0.0 1.0" );
            sw.WriteLine("Up 1.0 0.0 0.0" );
            sw.WriteLine("Centre 0.0 0.0 {0}" , zMax + BORDER );
            sw.WriteLine("Visible false" );
            sw.WriteLine("}" );
            sw.WriteLine("Union {" );
            sw.WriteLine("Mesh {" );
            double r = (double)_u / (double)(m - 2);
            double g = (double)_v / (double)(n - 2);
            double b = (1.0 - r) * (1.0 - g);
            sw.WriteLine("Material {" );
            sw.WriteLine("Ambient {0} {1} {2} ", r, g , b );
            sw.WriteLine("Diffuse {0} {1} {2} " ,r , g , b );
            sw.WriteLine("Specular 0.5 0.5 0.5" );
            sw.WriteLine("Shininess 10.0" );
            sw.WriteLine("Reflection 0.2 0.2 0.2" );
            sw.WriteLine("}" );
            sw.WriteLine("Vertex {" );
            for (v = 0; v < (_tiles + 1); v++) {
                for (u = 0; u < (_tiles + 1); u++) {
                    sw.WriteLine("Vertex {0}", _vertex[v][u].ToString() );
                }
            }
            sw.WriteLine("}" );
            sw.WriteLine("Normal {" );
            for (v = 0; v < (_tiles + 1); v++) {
                for (u = 0; u < (_tiles + 1); u++) {
                    sw.WriteLine("Normal {0} " , _normal[v][u].ToString() );
                }
            }
            sw.WriteLine("}" );
            sw.WriteLine("Face {" );
            for (v = 0; v < _tiles; v++) {
                for (u = 0; u < _tiles; u++) {
                    sw.WriteLine("Face  {0} {1} {2}" ,
                        (v * (_tiles + 1) + u) ,
                        (v * (_tiles + 1) + u + 1) ,
                        ((v + 1) * (_tiles + 1) + u + 1) );
                    sw.WriteLine("Face {0} {1} {2}" ,
                        (v * (_tiles + 1) + u) ,
                        ((v + 1) * (_tiles + 1) + u + 1) ,
                        ((v + 1) * (_tiles + 1) + u) );
                }
            }
            sw.WriteLine("}" );
            sw.WriteLine("Smooth true" );
            sw.WriteLine("}" );
            sw.WriteLine("}" );
            sw.WriteLine("}" );
            sw.WriteLine("}" );
  
 
        }
          


    };

} // ComputerGraphics
 

//------------------------------------------------------------------------------
