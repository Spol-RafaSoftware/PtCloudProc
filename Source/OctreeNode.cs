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
    public abstract class OctreeNode
    {
        //------------------------------------------------------------------------------
        // STATIC and Enum
        public static int _MAXDRAWPOINTS = 2400000;
        public static double _DRAWRATIO = 1.0d;                  //
        public static int _MINPTS = 80;
        public static int _MINPTSPCA = 12;
        public static int _MAXDEPTH = 4; 
        public static int _CUROCTDEPTH = -1;
        public static int _OCT = 8;
        public static double _SURFCRIT = 0.1d;
        public static double _FLATNESS = 0.1d;
        public static double _ANGLECRIT = 0.85;//0.9961d;  // 5 deg crit angle cos(theta)//0.95d;//ADD to interface
        public static double _MINR = 0.1d;
        public enum eOctPos { eBBL,eBFL,eBBR,eBFR,eTBL,eTFL,eTBR,eTFR,eRoot};
  
        //------------------------------------------------------------------------------
        //
        private int _id;                           // ID for the Node
        private int _depth;                        // level this node of Octree  
        protected eOctPos _ePos;                   // 
        protected double _radius;                  // half side length of cell
        protected double _tand;                    // normal distance of tangent plane from origin
        protected int _Ipos;
        protected int _Jpos;
        protected int _Kpos; 
        protected Vectors _bbMin;                  // Min Vector
        protected Vectors _bbMax;                  // Max Vector
        protected Vectors _vnorm;                  // vertex normal
        protected Vectors _eigenvec2;              // from pca  
        protected Vectors _eigenvec3;              // from pca       
        protected Vectors _rg;                     // raddi of gyration from pca
        protected Vectors _sigma;                  // sum of all points in descendent nodes
        protected OctreeNode _parent;              // 
        protected List<int> _pts;                  // 
        protected OctreeNode[][][] _adjacent;       // connected nodes on same level
 

        //------------------------------------------------------------------------------
        //constructor
        public OctreeNode(int mID, int mDepth, Vectors min, Vectors max, double r,OctreeBranch parent,eOctPos p)
        {
            _id = mID;
            _depth = mDepth;
            _bbMax = new Vectors(max);
            _bbMin = new Vectors(min);
            _vnorm = new Vectors();
            _eigenvec2 = new Vectors();
            _eigenvec3 = new Vectors();
            _sigma = new Vectors();
            _Ipos = -1;
            _Jpos = -1;
            _Kpos = -1; 
            _rg = new Vectors(); 
            _adjacent = new OctreeNode[3][][];
            for (int i = 0; i < 3; i++)
            {
                _adjacent[i] = new OctreeNode[3][];
                for (int j = 0; j < 3; j++)
                {

                    _adjacent[i][j] = new OctreeNode[3];
                    for (int k = 0; k < 3; k++)
                    {
                        _adjacent[i][j][k] = null;
                    }
                }
            }
            _radius=r;
            _parent = parent;
            _ePos = p;
        }

        #region MUTATOR
        //------------------------------------------------------------------------------
        public void Min(double x, double y, double z)
        {
            _bbMin.setPosition(x, y, z);
        }

        //------------------------------------------------------------------------------
        public void Max(double x, double y, double z)
        {
            _bbMax.setPosition(x, y, z);
        }

        //------------------------------------------------------------------------------
        public Vectors vMin
        {
            get
            {
                return  _bbMin;
            }
            set
            {
                _bbMin.setPosition(value._X, value._Y, value._Z); 
            }
        }

        //------------------------------------------------------------------------------
        public Vectors vMax
        {
            get
            {
                return _bbMax;
            }
            set
            {
                _bbMax.setPosition(value._X, value._Y, value._Z);
            }
        }

        //------------------------------------------------------------------------------
        public int Ipos
        {
            get
            {
                return _Ipos;
            }
            set
            {
                _Ipos=value;
            }
        }

        //------------------------------------------------------------------------------
        public int Jpos
        {
            get
            {
                return _Jpos;
            }
            set
            {
                _Jpos = value;
            }
        }

        //------------------------------------------------------------------------------
        public int Kpos
        {
            get
            {
                return (int)_Kpos;
            }
            set
            {
                _Kpos = value;
            }
        }

        //------------------------------------------------------------------------------
        public void setIJKPosition(int I, int J, int K)
        {
            _Ipos = I;
            _Jpos = J;
            _Kpos = K;
        }
        
        //------------------------------------------------------------------------------
        public static int MAXDEPTH
        {
            get
            {
                return _MAXDEPTH;
            }
            set
            {
                _MAXDEPTH = value;
            }
        }
        //------------------------------------------------------------------------------ 
        protected bool DoPCATest()
        {
            bool result = true;
            PCA calc = new PCA(_pts, NumPts);
            Eigen e = calc.eigen;

            _vnorm = calc.Norm;
            _vnorm.normalize();
            _tand = calc.dcent;
            _eigenvec2 = e.GetEigenvector(1); _eigenvec2.normalize();
            _eigenvec3 = e.GetEigenvector(2); _eigenvec3.normalize();
            _rg._X = Math.Sqrt(Math.Abs(e.GetEigenvalue(0))); //TODO::Check if we need Abs here? or check for NaN
            _rg._Y = Math.Sqrt(Math.Abs(e.GetEigenvalue(1))); 
            _rg._Z = Math.Sqrt(Math.Abs(e.GetEigenvalue(2)));
            _rg._W = _rg._X / (_rg._X + _rg._Y + _rg._Z);

            result = C.CONST.IS_NUM(_vnorm);
            if(result)
                result = C.CONST.IS_NUM(_tand);
            if (result)
                result = C.CONST.IS_NUM(_eigenvec2);
            if (result)
                result = C.CONST.IS_NUM(_eigenvec3);
            if (result)
                result = C.CONST.IS_NUM(_rg);

            if (result != true)
            {
                // some thing gone wrong ?
                // set values to default
                _vnorm = new Vectors();
                _eigenvec2 = new Vectors();
                _eigenvec3 = new Vectors(); 
                _rg = new Vectors();
                _tand = 0.0d;
            }

            return result;

        }


        //------------------------------------------------------------------------------    
        protected void displayEigen()
        {
            //draw points  
            if (NumPts > OctreeNode._MINPTS)
            {
                Vectors c = new Vectors();
                Vectors s = new Vectors();
                Vectors from = new Vectors();
                Vectors to = new Vectors();
                c.multiply(_vnorm, _rg._X);
                s= _sigma ; 
                //from= s- c;
                //to =s +c;
                from = Centre - c;
                to = Centre + c;
                GL_Draw.glLine(from, to, Colour.Blue(), Colour.Blue_Violet());

                c.multiply(_eigenvec2, _rg._Y);
                from = Centre - c;
                to = Centre + c;
                //from = s - c;
                //to = s + c;
                GL_Draw.glLine(from, to, Colour.Gold(), Colour.Green_Yellow());

                c.multiply(_eigenvec3, _rg._Z);
                from = Centre - c;
                to = Centre + c;
                //from = s - c;
                //to = s + c; 
                GL_Draw.glLine(from, to, Colour.Red(), Colour.Salmon());
            }
        }


        //------------------------------------------------------------------------------    
        public void displayAdjacent(bool centriod)
        {
            if (centriod)
            {
                displayAdjacent(centriod, true, Colour.Blanched_Almond(), Colour.Blanched_Almond());
            }else
                displayAdjacent(centriod, false, Colour.Light_Gray(),Colour.Light_Gray());
        }

        //------------------------------------------------------------------------------    
        public void displayAdjacent(bool centriod,bool C6, Colour c1, Colour c2)
        {
            if (C6 == false)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {

                        for (int k = 0; k < 3; k++)
                        {
                            drawAdjLine(centriod, c1,c2, i, j, k);
                        }
                    }
                }
            }
            else
            {
                drawAdjLine(centriod, c1, c2, 2, 1, 1);
                drawAdjLine(centriod, c1, c2, 0, 1, 1);
                drawAdjLine(centriod, c1, c2, 1, 2, 1);
                drawAdjLine(centriod, c1, c2, 1, 0, 1);
                drawAdjLine(centriod, c1, c2, 1, 1, 2);
                drawAdjLine(centriod, c1, c2, 1, 1, 0); 
            }
           
            
        }
        //------------------------------------------------------------------------------    
        private void drawAdjLine(bool centriod, Colour Surfcol1, Colour Surfcol2, int i, int j, int k)
        {
            OctreeNode n = _adjacent[i][j][k];
            if (n != null)
            {
                if (centriod)
                {
                    Render.GL_Draw.glLine(Sigma, n.Sigma, Surfcol1, Surfcol2);
                }
                else
                    Render.GL_Draw.glLine(Centre, n.Centre, Surfcol1, Surfcol2);
            }
        }

        #endregion 
        #region ACCESSORS_READONLY
      
        //------------------------------------------------------------------------------
        public int Depth
        {
            get
            {
                return _depth;
            }
        }

        //------------------------------------------------------------------------------
        public int ID
        {
            get
            {
                return _id;
            }
        }
        //------------------------------------------------------------------------------
        public eOctPos ePos
        {
            get
            {
                return _ePos;
            }
        }
        //------------------------------------------------------------------------------
        public OctreeNode ParentNode
        {
            get
            {
                return _parent;
            }
        }
        
        //------------------------------------------------------------------------------
        public Vectors Centre
        {
            get
            {
                return new Vectors( _bbMin._X + ((_bbMax._X - _bbMin._X) * 0.5), 
                                    _bbMin._Y + ((_bbMax._Y - _bbMin._Y) * 0.5), 
                                    _bbMin._Z + ((_bbMax._Z - _bbMin._Z) * 0.5));
            }
        }
        //------------------------------------------------------------------------------
        public Vectors Norm 
        {

            get
            {
                return _vnorm;
            } 
        }

        //------------------------------------------------------------------------------
        public List<int> PtsIdx
        {
            get
            {
                return _pts;
            }
        }

        //------------------------------------------------------------------------------
        public Vectors EigenVec2
        {
            get
            {
                return _eigenvec2;
            }
        }

        //------------------------------------------------------------------------------
        public Vectors EigenVec3
        {
            get
            {
                return _eigenvec3;
            }
        } 

        //------------------------------------------------------------------------------
        public  double R
        {
            get
            {
                return _radius;
            }
        }
        
        //------------------------------------------------------------------------------
        public Vectors Sigma
        {
            get
            {
                return _sigma;
            }
        }

        //------------------------------------------------------------------------------
        public double Bulge
        {
            get
            {
                return _rg._W;
            }
        }

        //------------------------------------------------------------------------------
        public Vectors RadiiGyration 
        {
            get
            {
                return _rg;
            }
        }
 
        //------------------------------------------------------------------------------
        public double Tand
        {
            get
            {
                return _tand;
            }
        }
        //------------------------------------------------------------------------------
        public bool testMinPts
        {
            get
            {
                if(NumPts>_MINPTS)
                    return true;
                else 
                    return false;
            }
        }
        //------------------------------------------------------------------------------
 
        #endregion 
        #region ABSTRACT_FN

        //------------------------------------------------------------------------------
        public abstract bool IsBranch
        {
            get;
        }
        //------------------------------------------------------------------------------
        public abstract bool ISHighlighted
        {
            get;
            set;
        }

        //------------------------------------------------------------------------------
        public abstract int NumPts
        {
            get;
        }
 
        //------------------------------------------------------------------------------
        //  Drawing Functions for the System
        public abstract void CreateDraw(int m, ref List<int> drawlist);
        public abstract void removeDisplayLists();
        //------------------------------------------------------------------------------
        public abstract bool Split();
        public abstract bool EdgeTest();  
        public abstract bool getOctNode(int getID, ref OctreeNode ret);
        public abstract bool intersect(Ray ray, ref double length, ref double Dist);
        public abstract void buildLeafList(ref List<OctreeLeaf> leafs);
        public abstract void BalanceOctree(ref int NodeCount);
 
        //------------------------------------------------------------------------------
        #endregion
        // Calculate Current Global position
        public abstract void GlobalPosition(double minstep, Vectors min);

        public abstract OctreeNode findIJK(int I, int J, int K);

        //------------------------------------------------------------------------------
        public OctreeNode getAdjacentNOde(int x,int y,int z)
        {
            return _adjacent[x + 1][y + 1][z + 1];
        }

        //------------------------------------------------------------------------------
        public void setAdjacentNOde(int x, int y, int z,OctreeNode n)
        {
             _adjacent[x + 1][y + 1][z + 1]=n;
        }

        //------------------------------------------------------------------------------
        public int getAdjacentID(int x, int y, int z)
        {
            OctreeNode n= _adjacent[x + 1][y + 1][z + 1];
            if (n != null)
            {
                return n.ID;
            }
            else
            {
                return -1;
            }
        }
        //------------------------------------------------------------------------------
        public bool inplane(OctreeNode n)
        {
            // test whether proposed link to n is compatible with tangent plane
            if(n==null)
                return false;
           // if(n is OctreeBranch) // we only call it from leaf list now
           //     return false;
           // if(n.R < R) // All leafs should be same level now
           //     return false;
 
            if(Math.Abs( this.Norm.dot(n.Norm))<OctreeNode._ANGLECRIT) 
               return false;
 
            
            if(Math.Abs(n.Sigma.dot(Norm)-this.Tand) > 2*n.R)
                return false;

            if (Math.Abs(this.Sigma.dot( n.Norm) - n.Tand) > 2 * n.R) 
                 return false;
            return true;
        }

        public abstract OctreeNode getChild(int x, int y, int z);
 
        //------------------------------------------------------------------------------ 
        public abstract void findAdjacent(OctreeBranch _TreeRoot);
        
        //------------------------------------------------------------------------------
        public virtual void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.Flush();
               outs.WriteStartElement("ID");
                    outs.WriteValue(_id);
                    outs.WriteFullEndElement();  
               outs.WriteStartElement("Depth");
                    outs.WriteValue( _depth);
                    outs.WriteFullEndElement();   
               outs.WriteStartElement("Pos");
                    outs.WriteValue( _ePos.ToString());
                    outs.WriteFullEndElement();      
               outs.WriteStartElement("Radius");
                    outs.WriteValue( _radius);
                    outs.WriteFullEndElement();      
               outs.WriteStartElement("Tand");
                outs.WriteValue( _tand);
                outs.WriteFullEndElement();   
               outs.WriteStartElement("I");
                    outs.WriteValue( _Ipos);
                    outs.WriteFullEndElement(); 
               outs.WriteStartElement("J");
                    outs.WriteValue( _Jpos);
                    outs.WriteFullEndElement();
               outs.WriteStartElement("K");
                    outs.WriteValue(_Kpos);
                    outs.WriteFullEndElement();  
               outs.WriteStartElement("Min");
                    _bbMin.write4(ref outs );
                    outs.WriteFullEndElement();
               outs.WriteStartElement("Max");          
                    _bbMax.write4(ref outs ); 
               outs.WriteFullEndElement();
               outs.WriteStartElement("Norm");
                    _vnorm.write4(ref outs);
                    outs.WriteFullEndElement();
               outs.WriteStartElement("Eigen2");
                    _eigenvec2.write4(ref outs);
                    outs.WriteFullEndElement();
               outs.WriteStartElement("Eigen3");
                    _eigenvec3.write4(ref outs); 
               outs.WriteFullEndElement();
               outs.WriteStartElement("Radii");
                    _rg.write4(ref outs);
                    outs.WriteFullEndElement();
               outs.WriteStartElement("Sigma");
                     _sigma.write4(ref outs);
                     outs.WriteFullEndElement();

                     if (_id > 0)
                     {
                         outs.Flush();
                         outs.WriteStartElement("Pts");
                         for (int i = 0, j = 0; i < _pts.Count; i++, j++)
                         { 
                             outs.WriteValue(" "+_pts[i]+" "); 
                             if (j == 100)// periodically flush
                             {
                                 j = 0;
                                 outs.Flush();
                             }
                         }
                         outs.WriteFullEndElement();
                         outs.Flush();
                       outs.WriteStartElement("Parent");
                            outs.WriteValue(_parent.ID);
                            outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(1, 0, 0));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(-1, 0, 0));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(0, 1, 0));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(0, -1, 0));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(0, 0, 1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(0, 0, -1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(1, 1, 0));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(1, 0, 1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(-1, -1, 0));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(-1, 0, -1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(0, 1, 1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(0, -1, -1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(1, 1, 1));
                       outs.WriteFullEndElement();
                       outs.WriteStartElement("Adjacent");
                       outs.WriteValue(getAdjacentID(-1, -1, -1));
                       outs.WriteFullEndElement();
                     }
               outs.Flush();
          // OctreeNode[][][] _adjacent;   
        }
        //------------------------------------------------------------------------------
        public abstract bool ParseXml(ref XmlTextReader ins);
        //------------------------------------------------------------------------------
        public virtual bool ReadXmlData(ref XmlTextReader ins)
        {

            bool ret = false;
            return ret;
        }       
        //------------------------------------------------------------------------------

      
    }
} 