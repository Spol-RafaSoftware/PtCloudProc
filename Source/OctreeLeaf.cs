//------------------------------------------------------------------------------
// using definition
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Tao.OpenGl; 

using Render;
using MathsLib;

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class OctreeLeaf
    public class OctreeLeaf : OctreeNode
    {

        //------------------------------------------------------------------------------
        private int _DisplayListLN;
        private int _DisplayListPT;
        private int _DisplayListCTN;
        private bool _PCA_result;
        private bool _highlighted;
        private int _SurfaceNum;
        private int _Flag;

        //------------------------------------------------------------------------------
        // Constructors
        public OctreeLeaf(int ID, int depth, Vectors min, Vectors max, ref List<bool> used, double R, OctreeBranch parent,eOctPos p)
            : base(ID, depth, min, max,R, parent,p)
        {
            PtData.findptsCube(_bbMin, _bbMax,R,p, ref used, parent.PtsIdx, out _pts);
            
            if(_pts.Count>0)
            {
                    _DisplayListPT = -1;
                _DisplayListLN = -1;
                _DisplayListCTN = -1;
                _PCA_result = false;
                _SurfaceNum = -1;

                CalculateSigma();

                if (OctreeNode._MINPTSPCA <= NumPts)
                {    _PCA_result=DoPCATest();
                }
            }
            else{ 
                throw new Exception("Invalid Leaf pts: "+_pts.Count); 
            }
        }

        //------------------------------------------------------------------------------ 
        public int Flag
        {
            get
            {
                return _Flag;
            }
            set
            {
                _Flag = value;
            }
        }

        //------------------------------------------------------------------------------ 
        private void buildDisplayLists_LN()
        {
            // find all the point in the bbbox
            if (NumPts > 0)
            {
                _DisplayListLN = Gl.glGenLists(1);
                Gl.glNewList(_DisplayListLN, Gl.GL_COMPILE);

                // draw the box 
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glEnable(Gl.GL_LINE_SMOOTH_HINT); 
                GL_Draw.GLBBCube(_bbMin, _bbMax, Colour.Lemon_Chiffon());
                Gl.glDisable(Gl.GL_LINE_SMOOTH_HINT);
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEndList();
            }
        }
        
        //------------------------------------------------------------------------------ 
        private void buildDisplayLists_PT()
        {
                if (NumPts > 0)//OctreeNode._MINPTS) // always show points
                {
                    _DisplayListPT = Gl.glGenLists(1);
                    Gl.glNewList(_DisplayListPT, Gl.GL_COMPILE);
                    double x = 0.0d, y = 0.0d, z = 0.0d;
                    Gl.glDisable(Gl.GL_LIGHTING);
                    Gl.glPointSize(2.0f);
                    Gl.glBegin(Gl.GL_POINTS);
                    float[] g;
                    int p = 0;
                    Vectors c = new Vectors(_vnorm);
                    if (NumPts > OctreeNode._MINPTS)
                        Colour.Azure().tofloat3f(out g);
                    else
                        Colour.Light_Blue().tofloat3f(out g);
                    c.multiply(_vnorm, _rg._X);
                    c.normalize();
                    //for (; p < (_pts.Count * _DRAWRATIO); p++)
                    //{
                    //    Gl.glColor3f(g[0], g[1], g[2]);
                    //    //Gl.glNormal3d(c._X, c._Y, c._Z);
                    //    PtData.Pt(_pts[p], ref x, ref  y, ref z);
                    //    Gl.glVertex3d(x, y, z);
                    //}

                    double Avgx = 0.0d, Avgy = 0.0d, Avgz = 0.0d;
                    p = 0;
                    int j = 0;
                    int end = _pts.Count; 
                    for (; p < end; p++)
                    {
                        //PtData.Pt(_pts[p], ref x, ref  y, ref z); 
                        if (j < (1.0 / _DRAWRATIO))
                        {
                            j++;
                            PtData.Pt(_pts[p], ref x, ref  y, ref z);
                            Avgx += x;
                            Avgy += y;
                            Avgz += z;
                        }
                        else
                        {
                            Avgx /= j;
                            j = 0;
                            Gl.glColor3f(g[0], g[1], g[2]);
                            Gl.glVertex3d(x, y, z);
                            Avgx = 0.0d;
                            Avgy = 0.0d;
                            Avgz = 0.0d;
                        }
                    }
                    Gl.glEnable(Gl.GL_LIGHTING);
                    Gl.glEnd(); 
                    Gl.glEndList();
                } 

        }



        //------------------------------------------------------------------------------ 
        private void buildDisplayLists_CTN(bool centroid)
        {
            //if (NumPts > 0)
            {
                _DisplayListCTN = Gl.glGenLists(1);
                Gl.glNewList(_DisplayListCTN, Gl.GL_COMPILE);
                Gl.glDisable(Gl.GL_LIGHTING);
                if (_PCA_result)
                {
                    displayAdjacent(centroid);
                    displayEigen();
                }

                float[] g;
                float[] g1;
                Colour.Light_Sea_Green().tofloat3f(out g1);
                if (NumPts > OctreeNode._MINPTS)
                    Colour.Yellow().tofloat3f(out g);
                else
                    Colour.Light_Goldenrod_Yellow().tofloat3f(out g);
                Vectors C = Centre;
                Vectors S = Sigma;
                Gl.glPointSize(5.0f);
                Gl.glEnable(Gl.GL_POINT_SMOOTH);
                Gl.glBegin(Gl.GL_POINTS);
                    Gl.glColor3f(g[0], g[1], g[2]);
                    Gl.glVertex3d(C._X, C._Y, C._Z);
                    Gl.glColor3f(g1[0], g1[1], g1[2]);
                    Gl.glVertex3d(S._X, S._Y, S._Z);
                    Gl.glEnd();
                Gl.glDisable(Gl.GL_POINT_SMOOTH);
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEndList();
            }

        }
         
        //------------------------------------------------------------------------------ 
        public override void removeDisplayLists()
        {
            if (_DisplayListPT != -1) 
            {
                Gl.glDeleteLists(_DisplayListPT, 1);
                 _DisplayListPT = -1;

            }
            
            if(_DisplayListLN != -1)
            {
                Gl.glDeleteLists(_DisplayListLN, 1);
                _DisplayListLN = -1;
            }

            if (_DisplayListCTN != -1)
            {
                Gl.glDeleteLists(_DisplayListCTN, 1);
                _DisplayListCTN = -1;
            }
 
            
        }



        //------------------------------------------------------------------------------
        //  Draw
        public override void CreateDraw(int m, ref List<int> drawlist)
        {
            removeDisplayLists();
            //switch (m)
            switch ( (Scene.eDrawMode)m)
            {
                case Scene.eDrawMode.eDM_BRANCH_PTS:
                    {
                        buildDisplayLists_PT();
                        drawlist.Add(_DisplayListPT);
                    }
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BB:
                    {
                        buildDisplayLists_LN();
                        drawlist.Add(_DisplayListLN);
                    }
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BBPTS:
                    {
                        buildDisplayLists_PT();
                        buildDisplayLists_LN(); 
                        drawlist.Add(_DisplayListLN);
                        drawlist.Add(_DisplayListPT); 
                    }
                    break;
                case Scene.eDrawMode.eDM_LEAFCENTERS:
                    {
                        if (NumPts > 0)//OctreeNode._MINPTS)
                        {
                            buildDisplayLists_CTN(false);
                            drawlist.Add(_DisplayListCTN);
                        }
                    }
                    break;
                case Scene.eDrawMode.eDM_LEAFSIGMA:
                    {
                        if (NumPts > 0)//OctreeNode._MINPTS)
                        {
                            buildDisplayLists_CTN(true);
                            drawlist.Add(_DisplayListCTN);
                        }
                    }
                    break;  
            }
        }

        //------------------------------------------------------------------------------
        public override bool IsBranch
        {
            get
            {
                return false;
            }
        }

        //------------------------------------------------------------------------------
        public override int NumPts
        {
            get
            {
                return _pts.Count;
            }
        }
 
        //------------------------------------------------------------------------------
        public int SurfaceNum
        {
            get
            {
                return _SurfaceNum;
            }
            set
            {
                _SurfaceNum = value;
            }
        }

        //------------------------------------------------------------------------------
        public bool IsInSurf
        {
            get
            {
                if (_SurfaceNum == -1)
                {
                    return false;
                }
                else 
                    return true;
            }
        }

        //------------------------------------------------------------------------------
        public override bool Split()
        {
            if (_PCA_result==true)
            {
                // something didnt go wrong with PCA calculations
                if (NumPts > OctreeNode._MINPTS)
                {
                    if (Depth < OctreeNode._MAXDEPTH)
                    {
                        if (!C.CONST.definitelyLessThan(R, OctreeNode._MINR)) // greater than Min Radius
                        {
                            if (!C.CONST.definitelyLessThan(RadiiGyration._X, C.CONST.EPSILON))// RadiiGyration._X Greater than ZERO
                            {
                                if (!C.CONST.definitelyLessThan(Bulge, OctreeNode._SURFCRIT))// BULGE >_SURFCRIT
                                {
                                    return true;
                                }
                            }
                        }
                    } // depth < maxdepth
                }// number pts >0 
            }// PCA_Result
            return false;
        }

        //------------------------------------------------------------------------------
        public override string ToString()
        {
            char[] ch = new char[] {':'}; 
            string ret = new string(ch);  
            ret += Depth; ret += ":Leaf:";ret += "ID:"; ret += ID; ret += ":pts:"; ret += NumPts; 
            ret += ":CX:"; ret += Centre._X; ret += ":CY:"; ret += Centre._Y; ret += ":CZ:"; ret += Centre._Z; ret += ":";
            ret += ":SX:"; ret += _sigma._X; ret += ":SY:"; ret += _sigma._Y; ret += ":SZ:"; ret += _sigma._Z; ret += ":";
            ret += "\n";
            return ret;
        }
        
        //------------------------------------------------------------------------------
        public override bool getOctNode(int getID,ref OctreeNode ret)
        {
            if (getID == ID)
            {
                ret = this;
                return true;
            }else
              return false;
        }
        //------------------------------------------------------------------------------ 
        public override bool EdgeTest()
        {
            return false;
        }
        //------------------------------------------------------------------------------  
        public override void BalanceOctree(ref int NodeCount) 
        {
        }

        //------------------------------------------------------------------------------ 
        private void CalculateSigma() 
        {
            if (NumPts > 0)
            {
                double x = 0.0d;
                double y = 0.0d;
                double z = 0.0d;
                _sigma = new Vectors(0.0, 0.0, 0.0);
                //we need to calculate
                for (int pt=0; pt < NumPts; pt++)
                {
                    PtData.Pt(_pts[pt], ref x, ref y, ref z);
                    _sigma.addXYZ(x, y, z);
                }
                _sigma /= (double)NumPts;
            }
        }
        //------------------------------------------------------------------------------
        public override bool ISHighlighted
        {
            get
            {
                return _highlighted;
            }
            set
            {
               _highlighted = value;
            }
        }
            // we are going to use spheres for intersection         
//   Given the ray R:
//              X = Rorg.x + Rdir.x * lambda
//              Y = Rorg.y + Rdir.y * lambda
//              Z = Rorg.z + Rdir.z * lambda
//   and the sphere defined by
//              (X-CenterX)2 + (Y-CenterY)2 + (Z-CenterZ)2 = Radius2
//  the intersection test can fail (if the ray misses the sphere), 
//  return one single solution (if the ray touches the sphere tangentially), 
//  or return two points (for a general collision). 
//  Whichever the case, the preceding equations are easily combined to yield
//      A*t2 + B*t + C = 0
//  where
//      A = Rdir.x2 + Rdir.y2 + Rdir.z2
//      B = 2* (Rdir.x2*(Rorg.x-CenterX) + Rdir.y2*(Rorg.y-CenterY) + Rdir.z2*(Rorg.z-CenterZ))
//      C = (Rorg.x-CenterX)2 + (Rorg.y-CenterY)2 + Rorg.z-CenterZ)2 - Radius2
//  In the preceding equation, A usually equals one because the ray's direction vector is normalized, 
//  saving some calculations.
//  Because we have a quadratic equation, all we have to do is solve it with the classic formula
//      -b +- sqrt (b2 - 4AC) / 2a
//  The quantity
//      B2 - 4ac
//  is referred to as the determinant. If it is negative, the square root function cannot be evaluated,
//  and thus the ray missed the sphere. 
//  If it's zero, the ray touched the sphere at exactly one point. 
//  If it's greater than zero, the ray pierced the sphere, and we have two solutions, which are
//      -b + sqrt (b2 - 4AC) / 2a
//      -b - sqrt (b2 - 4AC) / 2a
    
        //------------------------------------------------------------------------------ 
        public  bool IntersetAABB_OLD(Ray ray, ref double length, ref double Dist)
        { 
               double  Radius= vMax._X-vMin._X;
               double   A = ((ray.direction()._X)*(ray.direction()._X)) +((ray.direction()._Y)*(ray.direction()._Y))+ ((ray.direction()._Z)*(ray.direction()._Z));
               double   B = 2* ((ray.direction()._X)*(ray.direction()._X)*(ray.position()._X-Centre._X) + (ray.direction()._Y)*(ray.direction()._Y))*(ray.position()._Y-Centre._Y) + ((ray.direction()._Z)*(ray.direction()._Z))*(ray.position()._Z-Centre._Z);
               double   Cc = ((ray.position()._X-Centre._X)*(ray.position()._X-Centre._X)) + ((ray.position()._Y-Centre._Y)*(ray.position()._Y-Centre._Y)) + ((ray.position()._Z-Centre._Z)*(ray.position()._Z-Centre._Z)) - (Radius*Radius);

               double det = (B * B) - (4 * A * Cc);

               if (C.CONST.definitelyGreaterThan( det, 0.0d))
               {

                   length = ID;
                   _highlighted = true;
                   Dist = -B-Math.Sqrt(det);

                   return true;
               }

               return false;
        }

        public override bool  intersect(Ray ray, ref double length, ref double Dist)
        {
	        Vectors rayDelta = ray.direction() * length;
	        bool inside = true;
            double xt, yt, zt;
            _highlighted = false;
            // we are only interested in leafs with pts
            if (NumPts == 0)
            {
                return false;
            }

	        // Test the X component of the ray's origin to see if we are inside or not
	        if(ray.position()._X < vMin._X)
	        {
                xt = vMin._X - ray.position()._X; 
		        if(xt > rayDelta._X) // If the ray is moving away from the AABB, there is no intersection 
			        return false;

                xt /= rayDelta._X; 
		        inside = false;
	        } 
	        else if(ray.position()._X > vMax._X)
	        {
                xt = vMax._X - ray.position()._X; 
                if (xt < rayDelta._X) // If the ray is moving away from the AABB, there is no intersection 
			        return false;

                xt /= rayDelta._X;
		        inside = false;
	        } 
	        else
	        {
		        xt = -1.0f; 
	        }

            if (ray.position()._Y < vMin._Y)
	        {
                yt = vMin._Y - ray.position()._Y; 
                if (yt > rayDelta._Y)
			        return false;

                yt /= rayDelta._Y;
		        inside = false;
	        }
            else if (ray.position()._Y > vMax._Y)
	        {
                yt = vMax._Y - ray.position()._Y; 
                if (yt < rayDelta._Y) 
			        return false;

                yt /= rayDelta._Y;
		        inside = false;
	        } 
	        else
	        {
		        yt = -1.0f;
	        }

            if (ray.position()._Z < vMin._Z)
	        {
                zt = vMin._Z - ray.position()._Z;

                if (zt > rayDelta._Z) 
			        return false;

                zt /= rayDelta._Z;
		        inside = false;
	        }
            else if (ray.position()._Z > vMax._Z)
	        {
                zt = vMax._Z - ray.position()._Z;

                if (zt < rayDelta._Z)
			        return false;

                zt /= rayDelta._Z;
		        inside = false;
	        } 
	        else
	        {
		        zt = -1.0f;
	        }

            if (inside)
            {
                length = ID;
                _highlighted = true;
                return true;
            }

	        double t = xt;
        	
	        if(yt > t)
		        t = yt;
        	
	        if(zt > t)
		        t = zt;

	        if(t == xt) 
	        {
                double y = ray.position()._Y + rayDelta._Y * t;
                double z = ray.position()._Z + rayDelta._Z * t;
        	
                if (y < vMin._Y || y > vMax._Y)
			        return false;
                else if (z < vMin._Z || z > vMax._Z)
			        return false;
	        }
	        else if(t == yt)
	        {
                double x = ray.position()._X + rayDelta._X * t;
                double z = ray.position()._Z + rayDelta._Z * t;
                if (x < vMin._X || x > vMax._X)
			        return false;
                else if (z < vMin._Z || z > vMax._Z) 
			        return false;
	        }
	        else 
	        {
                double x = ray.position()._X + rayDelta._X * t;
                double y = ray.position()._Y + rayDelta._Y * t;

                if (x < vMin._X || x > vMax._X)
			        return false;
                else if (y < vMin._Y || y > vMax._Y)
			        return false;
	        }

            length = ID;
            _highlighted = true;
            Dist = ray.distance(Centre);

	        return true;
        }

        //------------------------------------------------------------------------------ 
        public override void buildLeafList(ref List<OctreeLeaf> leafs)
        {
            leafs.Add(this);
        }

        //------------------------------------------------------------------------------ 
        public override OctreeNode getChild(int x, int y, int z)
        { 
            /// look up child given coords which are +-1
            return this;
        }
        //------------------------------------------------------------------------------ 
        public override void findAdjacent(OctreeBranch _Tree)
        {   
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    for (int k = 0; k < 3; k++)
                    {
                        if (_adjacent[i][j][k] == null)
                        {
                            // find the adj at my depth
                            // so global pos of adj would be 
                            int l = (i - 1); // i=0,1,2 => l=-1,0,1
                            int m = (j - 1);
                            int n = (k - 1);
                            OctreeNode f = _Tree.findIJK(Ipos + l, Jpos + m, Kpos + n);
                            if (f != null)
                            {
                                if (f.IsBranch == false)
                                {
                                    _adjacent[i][j][k] = f; 
                                    if (_adjacent[l + 1][m + 1][n + 1].getAdjacentNOde(l, m, n) == null)
                                    {
                                        _adjacent[l + 1][m + 1][n + 1].setAdjacentNOde(l * -1, m * -1, n * -1, this);
                                    }
                                }
                            }
                        }
                    }
                }
            } 
        }

        //------------------------------------------------------------------------------ 
        public void findAdjacent(List<OctreeLeaf> _leafs)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    for (int k = 0; k < 3; k++)
                    {
                        if (_adjacent[i][j][k] == null)
                        {
                            // find the adj at my depth
                            // so global pos of adj would be 
                            int l = (i - 1); // i=0,1,2 => l=-1,0,1
                            int m = (j - 1);
                            int n = (k - 1);
                            OctreeNode f = findIJK(_leafs,Ipos + l, Jpos + m, Kpos + n);
                            if (f != null)
                            {
                                if (f.IsBranch == false)
                                {
                                    _adjacent[i][j][k] = f;
                                    if (_adjacent[l + 1][m + 1][n + 1].getAdjacentNOde(l, m, n) == null)
                                    {
                                        _adjacent[l + 1][m + 1][n + 1].setAdjacentNOde(l * -1, m * -1, n * -1, this);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------------------------------ 
        // Calculate Current Global position
        public override void GlobalPosition(double minstep, Vectors min)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            Vectors C = Centre - min;
            C /= minstep;
            i =(int) C._X;
            j =(int) C._Y;
            k =(int) C._Z;
            setIJKPosition(i, j, k);
        }
        //------------------------------------------------------------------------------
        public OctreeNode findIJK(List<OctreeLeaf> leaflist,int I, int J, int K)
        {
            foreach (OctreeLeaf l in leaflist)
            {
                if (l.findIJK(I, J, K) != null)
                {
                    return l;
                }
            }
            return null;
        }

        //------------------------------------------------------------------------------
        public override OctreeNode findIJK(int I, int J, int K)
        {
            if (Ipos == I)
            {
                if (Jpos == J)
                {
                    if (Kpos == K)
                    {
                        return this;
                    }
                }
            }
            return null;
        }


        //------------------------------------------------------------------------------
        public bool planar()
        {
            //meaning suitable for use in surface estgimation
            return (this.EigenVec2 != null) && (Bulge <= OctreeNode._SURFCRIT);
        }

        //------------------------------------------------------------------------------
        public override void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.WriteStartElement("Leaf"); 
            base.WriteXmlData(ref outs);
                outs.WriteStartElement("SurfaceNum");
                    outs.WriteValue(_SurfaceNum); 
                outs.WriteFullEndElement(); 
            outs.WriteFullEndElement(); 
        }
        //------------------------------------------------------------------------------
        public override bool ParseXml(ref XmlTextReader ins)
        {
            bool ret = false; // have we delt with it?
            switch (ins.NodeType)
            {
               case XmlNodeType.Element:
                    if (ins.Name == "Leaf")
                   {
                       ret = true;
                   }
                   else
                   {
                   }
                    break;
               case XmlNodeType.Whitespace:
                    {
                        ret = true;
                    }
                    break;

               case XmlNodeType.Comment:
                    {
                        ret = true;
                    }
                    break;
            }
            return ret;
        }
        //------------------------------------------------------------------------------
        public override bool ReadXmlData(ref XmlTextReader ins)
        {
            bool ret = false;
            return ret;
        }
        //------------------------------------------------------------------------------
    }
}