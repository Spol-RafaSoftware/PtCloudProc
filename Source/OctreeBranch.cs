//------------------------------------------------------------------------------
// using definition
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using MathsLib;
using Tao.OpenGl; 
using Render;

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class Octree
    public class OctreeBranch : OctreeNode
    {

        //------------------------------------------------------------------------------
        private List<OctreeNode> _chld;
        private int _DispLstCentreNorm;

        //------------------------------------------------------------------------------
        // Constructors
        public OctreeBranch(int ID, int depth, Vectors min, Vectors max, List<int> parentpts, double R)
            : base(ID, depth, min, max, R, null, eOctPos.eRoot) 
        {
                _pts = parentpts;
                _chld = new List<OctreeNode>(OctreeNode._OCT);// the oct part
                // we dont care about it it is the root node
                _vnorm.set(0.0d, 0.0d, 0.0d, 1.0d);
                _eigenvec2.set(0.0d, 0.0d, 0.0d, 1.0d);
                _eigenvec3.set(0.0d, 0.0d, 0.0d, 1.0d);
                _rg.set(0.0d, 0.0d, 0.0d, 1.0d);
               PtData.ptCloud_avg(out _sigma);
               // we are creating the root node so need to calculate the Draw Ratio for points 
               if (_pts.Count > _MAXDRAWPOINTS)
               {
                   _DRAWRATIO = (double) _MAXDRAWPOINTS / (double) _pts.Count;
               }
        }
        // 
        public OctreeBranch(double r, OctreeLeaf l, OctreeBranch parent,eOctPos p)
            : base(l.ID, l.Depth, l.vMin, l.vMax, r,parent,p)
        {

            _pts = new List<int>(l.PtsIdx);

            _chld = new List<OctreeNode>(OctreeNode._OCT);// the oct part
            _DispLstCentreNorm = -1;
             
            // we dont need to do PCA for Branches they come from leaves
            // which Are already calculated   
            _vnorm = l.Norm;
            _eigenvec2 = l.EigenVec2;
            _eigenvec3 = l.EigenVec3;
            _rg = l.RadiiGyration;
            _sigma = l.Sigma;
        }

        //------------------------------------------------------------------------------
        // Accessors
        public override bool IsBranch
        {
            get
            {
                return true;
            }
        }

        //------------------------------------------------------------------------------
        public override int NumPts
        {
            get
            { 
                return this._pts.Count;
            }
        }

        //------------------------------------------------------------------------------ 
        private void buildDisplayLists_CentreNorm(bool centroid)
        {
            _DispLstCentreNorm = Gl.glGenLists(1);
            Gl.glNewList(_DispLstCentreNorm, Gl.GL_COMPILE);
            Gl.glDisable(Gl.GL_LIGHTING);
            GL_Draw.GLBBCube(_bbMin, _bbMax, Colour.Dark_Orchid());
            displayEigen();
            displayAdjacent(centroid);
            Gl.glPointSize(5.0f);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glColor3d(1.0d, 0.0d, 0.0d);
            Gl.glVertex3d(Centre._X, Centre._Y, Centre._Z);
            Gl.glEnd();
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEndList();
        }
        //------------------------------------------------------------------------------ 
        public override void removeDisplayLists()
        {
            if (_DispLstCentreNorm != -1)
            {
                Gl.glDeleteLists(_DispLstCentreNorm, 1);
                _DispLstCentreNorm = -1;
            }
        }

        //------------------------------------------------------------------------------
        //  CreateDraw - dreate the list for the oct tree to draw 
        public override void CreateDraw(int m, ref List<int> drawlist) 
        {
            removeDisplayLists();

            switch ( (Scene.eDrawMode)m)
            {
                case Scene.eDrawMode.eDM_BRANCH_PTS:
                    {
                        foreach (OctreeNode o in _chld)
                        {
                            if (o != null)
                                o.CreateDraw((int)Scene.eDrawMode.eDM_BRANCH_PTS, ref drawlist);
                        }
                    }
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BB:
                    {
                        //    drawlist.Add(_DisplayListBB);
                        foreach (OctreeNode o in _chld)
                        {
                            if (o != null)
                                o.CreateDraw((int)Scene.eDrawMode.eDM_BRANCH_BB, ref drawlist);
                        }
                    }
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BBPTS:
                    {
                        //    drawlist.Add(_DisplayListBB);
                        foreach (OctreeNode o in _chld)
                        {
                            if (o != null)
                                o.CreateDraw((int)Scene.eDrawMode.eDM_BRANCH_BBPTS, ref drawlist);
                        }
                    }
                    break;
                case Scene.eDrawMode.eDM_CENTERS:
                    {
                        buildDisplayLists_CentreNorm(false);
                        drawlist.Add(_DispLstCentreNorm);
                        foreach (OctreeNode o in _chld)
                        {
                            if (o != null)
                                o.CreateDraw((int)Scene.eDrawMode.eDM_CENTERS, ref drawlist);
                        }
                    }
                    break;
                case Scene.eDrawMode.eDM_LEAFSIGMA:
                    { 
                        foreach (OctreeNode o in _chld)
                        {
                            if (o != null)
                                o.CreateDraw((int)Scene.eDrawMode.eDM_LEAFSIGMA, ref drawlist);
                        }
                    }
                    break;
                case Scene.eDrawMode.eDM_LEAFCENTERS:
                    {
                        foreach (OctreeNode o in _chld)
                        {
                            if (o != null)
                                o.CreateDraw((int)Scene.eDrawMode.eDM_LEAFCENTERS, ref drawlist);
                        }
                    }
                    break;
            }
        }

        //------------------------------------------------------------------------------
        public override bool Split()
        {
            if (NumPts > OctreeNode._MINPTS)
            {
                return true;
            }
            else
               return false;
        } 

        //------------------------------------------------------------------------------
        private void CalculateNewBBCoord(int i, out Vectors lmin, out Vectors lmax)
        {
            Vectors C = new Vectors(_bbMax - _bbMin);
            C *= 0.5f;
            C += _bbMin;
            lmin = new Vectors(_bbMin);
            lmax = new Vectors(C);

            if ((i != 0) || (i != 7))
            {
                if ((i == 1) || (i == 3) || (i == 5))
                {
                    // odd numbers back of cube
                    lmin.addZ((_bbMax._Z - _bbMin._Z) * 0.5);
                    lmax.addZ((_bbMax._Z - _bbMin._Z) * 0.5);
                }
                if (i > 3)
                {
                    // top layer of cube add to y componet
                    lmin.addY((_bbMax._Y - _bbMin._Y) * 0.5);
                    lmax.addY((_bbMax._Y - _bbMin._Y) * 0.5);
                }

                if ((i == 2) || (i == 3) || (i == 6))
                {
                    // right layer of cube add to y componet 
                    lmin.addX((_bbMax._X - _bbMin._X) * 0.5);
                    lmax.addX((_bbMax._X - _bbMin._X) * 0.5);
                }
            }
            if (i == 7)
            {
                lmax = _bbMax;
                lmin = C;
            }
        } 
        //------------------------------------------------------------------------------
        public override bool getOctNode(int getID, ref OctreeNode ret)
        {
            if (getID == ID)
            {
                ret = this;
                return true;
            }
            else
            {
                bool res=false;
                foreach (OctreeNode o in _chld)
                {
                    if (o != null)
                    {
                        o.getOctNode(getID, ref ret);
                        if (res)
                            break;
                    }
                }
                return res;

            }
        }

        //------------------------------------------------------------------------------
        public override bool intersect(Ray ray, ref double length, ref double Dist)
        {
            bool ret = false;
            foreach (OctreeNode o in _chld)
            {
                if (o != null)
                {
                    ret = o.intersect(ray, ref length, ref Dist);
                    if (ret == true)
                        break;
                }
            }

            return ret;
        }

        //------------------------------------------------------------------------------
        public void Create(ref int currDepth, ref int NodeCount)
        {
            currDepth++;
            if (currDepth > _CUROCTDEPTH)
                _CUROCTDEPTH++;

            int i = 0; 
            double r = R / 2.0d;// Depth;
            List<bool> used = new List<bool>(_pts.Count);        // avoid testing points  
            for (int k = 0; k < used.Capacity; k++ )
               used.Add(false);
                OctreeLeaf l = null;
            //create the first cells for this branch
            for (i=0; i < (OctreeNode._OCT); i++)
            {
                Vectors lmin, lmax;
                CalculateNewBBCoord(i, out lmin, out lmax); 
                OctreeNode n= null;
                try
                {
                    l = new OctreeLeaf(NodeCount, currDepth, lmin, lmax, ref  used, r, this, (eOctPos)i);
                    n = l;
                    NodeCount++;
                    if (l.Split())
                    {
                        OctreeBranch b = new OctreeBranch(l.R, l, this, l.ePos);
                        b.Create(ref currDepth, ref NodeCount);
                        n = b;
                    }
                }

                catch (Exception s)
                {
                    n = null;
                    Debug.WriteLine(s.Message.ToString());
                }
                AddChild(n,i); 

            } 
 
            currDepth--;
        }
        
        //------------------------------------------------------------------------------ 
        public override void findAdjacent(OctreeBranch _Tree) 
        {  
         /*   if(ID!=0)
            { 
          * // TODO
            }*/

            foreach (OctreeNode o in _chld)
            {
                if(o!= null)
                    o.findAdjacent(_Tree);
            }
          
        } 

        //------------------------------------------------------------------------------
        public override OctreeNode findIJK(int I, int J, int K)
        { 
            // try child cells 
            OctreeNode ret = null;
            foreach (OctreeNode o in _chld)
            {
                if (o != null)
                {
                  ret=  o.findIJK(I, J, K);
                  if (ret != null)
                  {
                      break;
                  }
                }
            }

            return ret;

        }

        //------------------------------------------------------------------------------ 
        public override void BalanceOctree(ref int NodeCount) 
        {
            // test if this 
            //create the first cells for this branch
            for (int i = 0; i < (OctreeNode._OCT); i++)
            {
                if (_chld[i] != null)
                {
                    if (_chld[i].IsBranch)
                    {
                        _chld[i].BalanceOctree(ref NodeCount);
                    }
                    else
                    {
                        // we have a leaf
                        if (_chld[i].Depth != _CUROCTDEPTH)
                        {
                            double r = _chld[i].R;
                            OctreeLeaf l = (OctreeLeaf)_chld[i];
                            OctreeBranch b = new OctreeBranch(r, l, this, _chld[i].ePos);
                            int j = _chld[i].Depth;
                            b.Create(ref j, ref NodeCount);
                            _chld[i] = b;
                            i --;
                        }
                    }
                }
            }  
        }

       // {
       //     int i=0;
       //     for (i = 0; i < (OctreeNode._OCT); i++)
       //     {
       //         if(!_chld[i].IsBranch)
       //         {
       //             OctreeLeaf l = (OctreeLeaf)_chld[i];
       //             // have a Leaf for balancing we need 
       //            OctreeBranch b = new OctreeBranch(l.R, l, this,l.ePos);
       //             b.Create(ref currDepth, ref NodeCount);
       //             AddChild(b,i);
       //         }
       //    }
       // }
        //------------------------------------------------------------------------------  
        public override bool ISHighlighted
        {
            get
            {
                return false;
            }
            set
            {
                foreach (OctreeNode o in _chld)
                {
                    o.ISHighlighted = value;
                } 
            }
        }

        //------------------------------------------------------------------------------ 
        public void AddChild(OctreeNode n, int i)
        { 
            if(i<_chld.Count)
            { 
                _chld.RemoveAt(i);
                _chld.Insert(i, n);
            } else
                _chld.Add(n);
        }

        //------------------------------------------------------------------------------ 
        public override string ToString()
        {
            char[] ch = new char[] {':'};
            string ret = new string(ch); 
            ret += Depth;ret += ":Branch:";  ret +="ID:"; ret += ID; ret += ":pts:"; ret += NumPts;
            ret += ":CX:"; ret += Centre._X; ret += ":CY:"; ret += Centre._Y; ret += ":CZ:"; ret += Centre._Z; ret += ":";
            ret += ":SX:"; ret += _sigma._X; ret += ":SY:"; ret += _sigma._Y; ret += ":SZ:"; ret += _sigma._Z; ret += ":";
            ret += "\n";
            foreach (OctreeNode o in _chld)
            {
                if (o != null)
                    ret += o.ToString();
            } 
            //ret += "}";
            return ret;
        }

        //------------------------------------------------------------------------------ 
        public override bool EdgeTest()
        {
            return false;
        }

        //------------------------------------------------------------------------------ 
        public override void buildLeafList(ref List<OctreeLeaf> leafs)
        { 
            foreach (OctreeNode o in _chld)
            {
                if (o != null)
                    o.buildLeafList(ref leafs);
            }
        }

        //------------------------------------------------------------------------------ 
        public override OctreeNode getChild(int x, int y, int z)
        {
            /// look up child given coords which are +-1
            int i = (z + 1) / 2 + (y + 1) + (x + 1) * 2;
            return _chld[i];
        }
        //------------------------------------------------------------------------------ 
        // Calculate Current Global position
        public override void GlobalPosition(double minstep, Vectors min)
        {
            int i = 0;     int j = 0;
            int k = 0;
 
             double ii, jj, kk;
             ii = ((Centre._X - min._X) / minstep);
             jj = ((Centre._Y - min._Y) / minstep);
             kk = ((Centre._Z - min._Z) / minstep);
             i = (int)ii;
             j = (int)jj;
             k = (int)kk;

            setIJKPosition(i, j, k);
            foreach (OctreeNode o in _chld)
            {
                if (o != null)
                    o.GlobalPosition(minstep, min);
            }
        }

        //------------------------------------------------------------------------------
        public override void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.WriteStartElement("Branch"); 
            base.WriteXmlData(ref outs);
            foreach (OctreeNode o in _chld)
            {
                if (o != null)
                    o.WriteXmlData(ref outs);
            }
            outs.WriteFullEndElement(); 
        }
        //------------------------------------------------------------------------------
        public override bool ParseXml(ref XmlTextReader ins) 
        { 
            bool ret = false;
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