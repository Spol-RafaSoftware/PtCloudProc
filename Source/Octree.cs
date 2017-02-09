//------------------------------------------------------------------------------
// using definition
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using MathsLib;
using System.Xml;
using Tao.OpenGl;
using Render;
//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class Octree
    public class Octree
    {
        //------------------------------------------------------------------------------
        private OctreeBranch _Tree;
        private bool _showhghlight;
        private int _drawID;
        private int _Count;
        private int _DisplayListHiglight; 
        private OctreeNode _HghLghtCell;
        private List<int> _displaylist;
        private List<OctreeLeaf> _leafs;

        //------------------------------------------------------------------------------
        // Constructors
        public Octree()
        {
            _drawID = 0; 
            _DisplayListHiglight = -1;
            _HghLghtCell = null;  
            _displaylist = new List<int>();
            _leafs = new List<OctreeLeaf>();
        }
        
        //------------------------------------------------------------------------------
        // Init
        public void Init()
        {
            Trace.WriteLine("_Octree-Init-Start");
            Vectors bbmin, bbmax;
            int c=PtData.ptCloud_Xcount();
            List<int> pts = new List<int>(c);
            PtData.ptCloud_min(out bbmin);
            PtData.ptCloud_max(out bbmax);
            int i = 0;
            for (; i < c; i++)
            {
                pts.Add(i); 
            
            }
            double xr = Math.Abs(bbmax._X - bbmin._X);
            double yr = Math.Abs(bbmax._Y - bbmin._Y);
            double zr = Math.Abs(bbmax._Z - bbmin._Z);
            double r = xr;

            if (r < yr)
            {
                    r = yr;
            }
            if (r < zr)
            {
                r = zr;
            }
            //make our bounding box bigger than the data
            r += C.CONST.EPSILON;
            bbmin.addXYZ(-C.CONST.EPSILON, -C.CONST.EPSILON, -C.CONST.EPSILON);
            bbmax.addXYZ(C.CONST.EPSILON, C.CONST.EPSILON, C.CONST.EPSILON);

            Vectors newbbmax= new Vectors(bbmin._X+r,bbmin._Y+r,bbmin._Z+r);
            _Tree = new OctreeBranch(0, 0, bbmin, newbbmax, pts, r);

            DateTime startTime = DateTime.Now; 

            Trace.WriteLine("_Octree-Split-Start" + startTime.ToString() );
            Split();
            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Trace.WriteLine("_Octree-Split-End:"+duration.ToString());
            Vectors idx = new Vectors(_Tree.vMin);
            double minr = (r / (Math.Pow(2.0d,(double)(OctreeNode._CUROCTDEPTH))));//((double)(OctreeNode._CUROCTDEPTH) * (OctreeNode._CUROCTDEPTH) * (OctreeNode._CUROCTDEPTH)));
            //idx._X+= minr; 
            //idx._Y+= minr; 
            //idx._Z+= minr; 
            // calculate its gobal position
            _Tree.GlobalPosition(minr , idx);
            // Find the Adjcent cells
            startTime = DateTime.Now;
            Trace.WriteLine("_Octree-Adjacent-Start" + startTime.ToString());
            AdjacentLeaf();

            //AdjacentTree();
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Trace.WriteLine("_Octree-Adjacent-End:" + duration.ToString());
            Trace.WriteLine("_Octree-Init-End");
        }

        //------------------------------------------------------------------------------
        // create the octree and call the data
        public void Split()
        {
            int currDepth=0;
            _Count = 1; // count =1 as we already Created _Tree
            // we have a node to split
            _Tree.Create(ref currDepth, ref _Count);
            //Debug.WriteLine("_Count={0}", _Count);
            //Debug.WriteLine("{0}", _Tree.ToString());
            _Tree.BalanceOctree(ref _Count); 
        }

        //------------------------------------------------------------------------------
        //  Draw all the 
        public void Draw()
        {

            //for(int i =0; i< _displaylist.Count;i++)
            foreach (int i in _displaylist)
            {
                Gl.glCallList(i);
            }
            if (_DisplayListHiglight != -1)
            {
                Gl.glCallList(_DisplayListHiglight);
            }
        }


        //------------------------------------------------------------------------------
        //  
        public int drawIDInc(Scene.eDrawMode dm)
        {
            if (_drawID < _Count)
                _drawID++;
            else
                _drawID = 0;

            ReSetDrawList(dm);

            return _drawID;
        }

        //------------------------------------------------------------------------------
        //  
        public int drawIDDec(Scene.eDrawMode dm)
        {
            if (_drawID > 0)
                _drawID--;
            else
                _drawID = _Count;

            ReSetDrawList(dm);
            return _drawID;
        }

        //------------------------------------------------------------------------------
        //  
        public void drawID(ref int i, Scene.eDrawMode dm)
        {
            if ((i >= 0) && (_Count > i))
            {
                    _drawID = i;

            }
            else
            {
                if (i > _Count)
                {
                    _drawID = _Count;
                }
                else
                {
                    if (i < 0)
                    {
                        _drawID = 0;
                    }
                }
            }
            i = _drawID;
            ReSetDrawList(dm);
        }

     

        //------------------------------------------------------------------------------
        public void ReSetDrawList(Scene.eDrawMode dm)
        { 
            _displaylist = new List<int>();
            if (dm < Scene.eDrawMode.eDM_LEAFSURFACE)
            {
                if (_drawID == 0)
                {
                    _Tree.CreateDraw((int)dm, ref _displaylist);
                }
                else
                {
                    OctreeNode o = null;
                    _Tree.getOctNode(_drawID, ref o);
                    if (o != null)
                        o.CreateDraw((int)dm, ref _displaylist);
                }
            }
        }

        //------------------------------------------------------------------------------
        //  get Current Centre and Max
        public void getViewData(out Vectors max, out Vectors cen, out Vectors sigma, ref double radius)
        {
                OctreeNode o = null;
                if (_drawID == 0)
                {
                    o = _Tree;
                }
                else
                {
                    _Tree.getOctNode(_drawID, ref o);
                }

                if (o == null)
                {
                    o = _Tree;
                }

                max = new Vectors(o.vMax);
                cen = o.Centre;
                sigma = new Vectors(o.Sigma);
                radius = o.R;  
        }

  

        //------------------------------------------------------------------------------
        // iterate through the oct tree
        public void getNode( ref OctreeNode ret)
        {
            if (_drawID == 0)
            {
                ret=_Tree; 
            }
            else
            {
                _Tree.getOctNode(_drawID, ref ret); 
            }
        }
        
        //------------------------------------------------------------------------------
        public int Count
        {
            get
            {
                return _Count;
            }

        }

        //------------------------------------------------------------------------------
        public int DRAWID
        {
            get
            {
                return _drawID;
            }

        }
 

        //------------------------------------------------------------------------------ 
        public override string ToString()
        {
            return _Tree.ToString();
        }

        //------------------------------------------------------------------------------
        public void OctreeData(out string[] octreedata , int id)
        {
            OctreeNode o = null;
            if (id < 0)
            {
                //if (_drawID == 0)
                //{
                //    o = _Tree;
                //}
                //else
                //{
                //    _Tree.getOctNode(_drawID, ref o);
                //} 
                 o= _HghLghtCell;
            }
            else
            {
                _Tree.getOctNode(id, ref o);
 
            }

            if (o == null)
            {
                o = _Tree;
            }
            octreedata = new string[39];
            int i = 0;
            octreedata[i] = "Depth:" + o.Depth.ToString(); i++;
            octreedata[i] = "ID:" + o.ID.ToString(); i++;
            if(o.IsBranch==true)
                octreedata[i] = "Branch:";  
            else
                octreedata[i] = "Leaf:"; 
                i++;
            if (o.ParentNode != null)
            {
                octreedata[i] = "PatentID:" + o.ParentNode.ID.ToString();
            }else
                octreedata[i] = "PatentID:NULL";
            i++;
            octreedata[i] = "NumPts:" + o.NumPts.ToString(); i++;
            octreedata[i] = "Centre"; i++;
            octreedata[i] = o.Centre.ToString(); i++;
            octreedata[i] = "IJK"; i++;
            octreedata[i] = "(" + ((int)o.Ipos).ToString() + "," + ((int)o.Jpos).ToString() + "," + ((int)o.Kpos).ToString() + ")"; i++;

            octreedata[i] = "Sigma"; i++;
            octreedata[i] = o.Sigma.ToString(); i++;

            octreedata[i] = "Norm"; i++;
            octreedata[i] = o.Norm.ToString(); i++;
            octreedata[i] = "EigenVec2"; i++;
            octreedata[i] = o.EigenVec2.ToString(); i++;
            octreedata[i] = "EigenVec3"; i++;
            octreedata[i] = o.EigenVec3.ToString(); i++;
            octreedata[i] = "vMax"; i++;
            octreedata[i] = o.vMax.ToString(); ; i++;
            octreedata[i] = "vMin"; i++;
            octreedata[i] = o.vMin.ToString(); i++;
            octreedata[i] = "RadiiGyration"; i++;
            octreedata[i] = o.RadiiGyration.ToString(); i++;
            octreedata[i] = "Radius"; i++;
            octreedata[i] = o.R.ToString(); i++;
 

            octreedata[i] = "Adj(1,0,0)="  + o.getAdjacentID( 1,  0,  0); i++;
            octreedata[i] = "Adj(-1,0,0)=" + o.getAdjacentID(-1,  0,  0); i++;
            octreedata[i] = "Adj(0,1,0)="  + o.getAdjacentID( 0,  1,  0); i++;
            octreedata[i] = "Adj(0,-1,0)=" + o.getAdjacentID( 0, -1,  0); i++;
            octreedata[i] = "Adj(0,0,1)="  + o.getAdjacentID( 0,  0,  1); i++;
            octreedata[i] = "Adj(0,0,-1)=" + o.getAdjacentID( 0,  0, -1); i++;

            octreedata[i] = "Adj(1,1,0)="    + o.getAdjacentID( 1,  1,  0); i++;
            octreedata[i] = "Adj(1,0,1)="    + o.getAdjacentID( 1,  0,  1); i++;
            octreedata[i] = "Adj(-1,-1,0)="  + o.getAdjacentID(-1, -1,  0); i++;
            octreedata[i] = "Adj(-1,0,-1)="  + o.getAdjacentID(-1,  0, -1); i++; 
            octreedata[i] = "Adj(0,1,1)="    + o.getAdjacentID( 0,  1,  1); i++;
            octreedata[i] = "Adj(0,-1,-1)="  + o.getAdjacentID( 0, -1, -1); i++; 
            octreedata[i] = "Adj(1,1,1)="    + o.getAdjacentID( 1,  1,  1); i++;
            octreedata[i] = "Adj(-1,-1,-1)=" + o.getAdjacentID(-1, -1, -1); i++;

        }
        //------------------------------------------------------------------------------
        public void SimpleOctData(string[] surfacedata)
        {  
              surfacedata[0] = "Octree " + this._Count;
              if(this._Tree!=null)
                  surfacedata[1] = "c:" + this._Tree.Centre;
              else  
                 surfacedata[1] = "c:"; 
        }

        //------------------------------------------------------------------------------
        public void WriteOctreeData(string filename)
        {
            OctreeNode o = null;
            if (_drawID == 0)
            {
                o = _Tree;
            }
            else
            {
                _Tree.getOctNode(_drawID, ref o);
            }

            if (o == null)
            {
                o = _Tree;
            } 

            int count = 0;
            try
            {
                string[] celldata;
                OctreeData(out celldata,-1);
                FileStream file = File.Create(filename + _drawID.ToString() + "_data.txt");
                using (StreamWriter sw = new StreamWriter(file))
                {
                    foreach(string s in celldata)
                        sw.WriteLine("{0}", s);
                    sw.Close();
                }

                FileStream fs = File.Create(filename +_drawID.ToString()+ "_points.XYZ");
                using (StreamWriter sw = new StreamWriter(fs))
                { 
                    double x = 0.0f, y = 0.0f, z = 0.0f;
                    while (count<o.PtsIdx.Count)
                    {
                        PtData.Pt(o.PtsIdx[count], ref x, ref y, ref z);
                        bool good = true;
                        if (double.IsInfinity(x) || double.IsNaN(x))
                            good = false;
                        if (double.IsInfinity(y) || double.IsNaN(y))
                            good = false;
                        if (double.IsInfinity(z) || double.IsNaN(z))
                            good = false;

                        if (good == true)
                        {
                            sw.WriteLine("{0} {1} {2}",x,y,z);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error : Bad Data ln = " + count);
                            throw new SystemException("Error : Bad Data ln = " + count);
                        }
                        count++;
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
        public int intersects(Ray r, double nearFarDist)
        {
            double length = nearFarDist;
            bool res=false;

            if (_HghLghtCell != null)
            {
                _HghLghtCell.ISHighlighted = false;
                _HghLghtCell = null;
                DestroyDisplayLists_Highlight();
            }
            double dist = 0.0d;
            res = intersectLeafs(r, ref length, ref dist);
            if (res)
            { 

                if (  ((int)length)== 0)
                {
                    _HghLghtCell = _Tree;
                }
                else
                {
                    _Tree.getOctNode((int)length, ref _HghLghtCell);
                }
                if (_HghLghtCell != null)
                {
                    buildDisplayLists_Highlight();
                }
                else
                {
                    DestroyDisplayLists_Highlight();
                }
            }

            return (int)length;
           
            
        }

        //------------------------------------------------------------------------------ 
        private bool intersectLeafs(Ray r, ref double length, ref double Dist)
        {
            bool res =false;
            int tID = 0;
            double d = double.PositiveInfinity;

            foreach (OctreeLeaf l in _leafs)
            {
                if (l.intersect(r, ref length, ref  Dist))
                {
                    res = true;
                    if (Dist < d)
                    {
                        d = Dist;
                        tID = l.ID;
                    }
                }
            }

            // if res
            if (res == true)
            {
                length = tID;

            }

            return res;
        }

        //------------------------------------------------------------------------------
        public int MoveAdjhighlight(int x, int y, int z)
        { 
            OctreeNode n =null;
            if (_HghLghtCell != null)
                 n = _HghLghtCell.getAdjacentNOde(x,y,z);
            _HghLghtCell = n; 
            if (_HghLghtCell != null)
            {
                buildDisplayLists_Highlight();
                return _HghLghtCell.ID;
            }
            else
            {
                DestroyDisplayLists_Highlight();
                return -1;
            }
        } 
        //------------------------------------------------------------------------------ 
        private void buildDisplayLists_Highlight()
        {
            _DisplayListHiglight = Gl.glGenLists(1);
            Gl.glNewList(_DisplayListHiglight, Gl.GL_COMPILE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPointSize(2.5f);
            buildDisplayLists_HighlightCube(_HghLghtCell.Centre, _HghLghtCell.vMax, _HghLghtCell.vMin);
            Gl.glEnd();
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEndList();
        }

        //------------------------------------------------------------------------------ 
        private void buildDisplayLists_HighlightCube(Vectors Centre, Vectors vMax, Vectors vMin)
        {
            Vectors V1 = new Vectors(Centre);
            Vectors V2 = new Vectors(vMax-vMin);
            V2 *= 0.5d;
            float[] g;
            Colour.Aquamarine().tofloat3f(out g);
            Gl.glColor4f(g[0], g[1], g[2],0.2f); 
                Gl.glBegin(Gl.GL_QUAD_STRIP);
                    //Quads 1 2 3 4
                    Gl.glVertex3d(V1._X + V2._X, V1._Y + V2._Y, V1._Z + V2._Z);    //V2
                    Gl.glVertex3d(V1._X + V2._X, V1._Y - V2._Y, V1._Z + V2._Z);    //V1
                    Gl.glVertex3d(V1._X + V2._X, V1._Y + V2._Y, V1._Z - V2._Z);    //V4
                    Gl.glVertex3d(V1._X + V2._X, V1._Y - V2._Y, V1._Z - V2._Z);    //V3
                    Gl.glVertex3d(V1._X - V2._X, V1._Y + V2._Y, V1._Z - V2._Z);    //V6
                    Gl.glVertex3d(V1._X - V2._X, V1._Y - V2._Y, V1._Z - V2._Z);    //V5
                    Gl.glVertex3d(V1._X - V2._X, V1._Y + V2._Y, V1._Z + V2._Z);    //V8
                    Gl.glVertex3d(V1._X - V2._X, V1._Y - V2._Y, V1._Z + V2._Z);    //V7
                    Gl.glVertex3d(V1._X + V2._X, V1._Y + V2._Y, V1._Z + V2._Z);    //V2
                    Gl.glVertex3d(V1._X + V2._X, V1._Y - V2._Y, V1._Z + V2._Z);    //V1
                Gl.glEnd();
                Gl.glBegin(Gl.GL_QUADS);
                    //Quad 5
                    Gl.glVertex3d(V1._X - V2._X, V1._Y + V2._Y, V1._Z - V2._Z);    //V6
                    Gl.glVertex3d(V1._X - V2._X, V1._Y + V2._Y, V1._Z + V2._Z);    //V8
                    Gl.glVertex3d(V1._X + V2._X, V1._Y + V2._Y, V1._Z + V2._Z);    //V2
                    Gl.glVertex3d(V1._X + V2._X, V1._Y + V2._Y, V1._Z - V2._Z);    //V4
                    //Quad 6 
                    Gl.glVertex3d(V1._X - V2._X, V1._Y - V2._Y, V1._Z + V2._Z);    //V7
                    Gl.glVertex3d(V1._X - V2._X, V1._Y - V2._Y, V1._Z - V2._Z);    //V5
                    Gl.glVertex3d(V1._X + V2._X, V1._Y - V2._Y, V1._Z - V2._Z);    //V3
                    Gl.glVertex3d(V1._X + V2._X, V1._Y - V2._Y, V1._Z + V2._Z);     //V1
                Gl.glEnd();
        }

        //------------------------------------------------------------------------------ 
        private void DestroyDisplayLists_Highlight()
        {

            if (_DisplayListHiglight != -1)
            {
                Gl.glDeleteLists(_DisplayListHiglight, 1);
                _DisplayListHiglight = -1;
            }
        }
        
        //------------------------------------------------------------------------------
        public bool Showhighlight 
        {
            get
            {
                return _showhghlight;
            }
            set
            {
                _showhghlight = value;
            }
        }

           
        //------------------------------------------------------------------------------
        public void build_leafs()
        {
            _Tree.buildLeafList(ref _leafs);
        }

        //------------------------------------------------------------------------------
        // create the octree ajacent cells
        public void AdjacentTree()
        {
            _Tree.findAdjacent(_Tree);
            build_leafs();
        }

        //------------------------------------------------------------------------------
        // create the octree ajacent cells
        public void AdjacentLeaf()
        {
            build_leafs(); 
            foreach (OctreeLeaf l in _leafs)
            { 
                l.findAdjacent(_leafs); 
            }
            
        }


        //------------------------------------------------------------------------------
        // create the octree ajacent cells
        public void OctreeSurface(ref SurfaceController SC, ref double PP)
        {
            Trace.WriteLine("_Octree-CreatingSurf-Start");
            // create first surface
            Surface s = null; 
            // list of ID's of nodes which have been added to a surface
            List<int> used = new List<int>(_leafs.Count); 
            // loop through all leafs 
            foreach (OctreeLeaf l in _leafs)
            {
                // if the leaf has point
                if (l.NumPts > 0)
                {   
                    // have we used this leaf in a surface
                    if (!used.Contains(l.ID))
                    {
                            s = new Surface( SC.SurfCOUNT); 
                            inAdjplane(l, ref s, ref used);
                            used.Add(l.ID);
                            SC.AddSurface(s);
                    }

                    if (used.Count == _leafs.Count)
                    {
                        break;// we have assigned all the leaves
                    }

                }// NumPts
            }// for each

            SC.SurfaceTests(_leafs, ref PP, _Count);
            Trace.WriteLine("_Octree-CreatingSurf-End");
        }

        //------------------------------------------------------------------------------
        private bool testFLatNess( OctreeNode candidate)
        {
            if (candidate != null)
            {
                if (candidate.Bulge < OctreeNode._FLATNESS)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        //------------------------------------------------------------------------------
        private void ChooseFirstBestCandidate(ref OctreeLeaf candidate, List<OctreeLeaf> leafs)
        {
            foreach (OctreeLeaf l in leafs)
            {
                if (l.Bulge < OctreeNode._SURFCRIT)
                {
                    candidate = l;
                    break;
                }
            }
        }

        //------------------------------------------------------------------------------
        private void inAdjplane(OctreeLeaf l, ref Surface s, ref List<int> used)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {

                    for (int z = -1; z < 2; z++)
                    {
                        OctreeNode n = l.getAdjacentNOde(x, y, z);

                        if (testFLatNess(n))
                        {
                            if ((l.inplane(n) == true) && (!used.Contains(n.ID)))
                            {
                                used.Add(n.ID);
                                s.add(n.ID, (OctreeLeaf)n);
                                inAdjplane((OctreeLeaf)n, ref  s, ref used);
                            }
                        }
                    }// for z -1,0,1 
                }// for y -1,0,1 
            }// for x -1,0,1 
        }
        
        //------------------------------------------------------------------------------
        public void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.WriteStartElement("Octree");
            outs.WriteStartElement("MAXDRAWPOINTS");
            outs.WriteValue(OctreeNode._MAXDRAWPOINTS);
            outs.WriteFullEndElement();
            outs.WriteStartElement("DRAWRATIO");
            outs.WriteValue(OctreeNode._DRAWRATIO);
            outs.WriteFullEndElement();
            outs.WriteStartElement("MINPTS");
            outs.WriteValue(OctreeNode._MINPTS);
            outs.WriteFullEndElement();
            outs.WriteStartElement("MINPTSPCA");
            outs.WriteValue(OctreeNode._MINPTSPCA);
            outs.WriteFullEndElement(); 
            outs.WriteStartElement("MAXDEPTH");
            outs.WriteValue(OctreeNode._MAXDEPTH);
            outs.WriteFullEndElement(); 
            outs.WriteStartElement("CUROCTDEPTH");
            outs.WriteValue(OctreeNode._CUROCTDEPTH);
            outs.WriteFullEndElement(); 
            outs.WriteStartElement("SURFCRIT");
            outs.WriteValue(OctreeNode._SURFCRIT);
            outs.WriteFullEndElement();
            outs.WriteStartElement("FLATNESS");
            outs.WriteValue(OctreeNode._FLATNESS);
            outs.WriteFullEndElement(); 
            outs.WriteStartElement("ANGLECRIT");
            outs.WriteValue(OctreeNode._ANGLECRIT);
            outs.WriteFullEndElement();
            outs.WriteStartElement("MINR");
            outs.WriteValue(OctreeNode._MINR);
            outs.WriteFullEndElement(); 
            _Tree.WriteXmlData(ref outs);
            outs.WriteFullEndElement();

        }

        //------------------------------------------------------------------------------
        public bool ParseXml(ref XmlTextReader ins)
        {
            bool ret = false; // have we delt with it?
            switch (ins.NodeType)
            {
                case XmlNodeType.Element:
                    if (ins.Name == "Octree")
                    { 
                        ret = true;
                    }
                    break;
                case XmlNodeType.EndElement:
                    if (ins.Name == "Octree")
                    {
                        ret = false;
                        ins.Read();
                    }
                    break;

            }
            return ret;
        }

        //------------------------------------------------------------------------------
        public bool ReadXmlData(ref XmlTextReader ins)
        {
            bool ret = false; // have we delt with it?

            switch (ins.NodeType)
            {
                case XmlNodeType.Element:
                    if (ins.Name == "MAXDRAWPOINTS")
                    {
                        ins.Read();
                        OctreeNode._MAXDRAWPOINTS = int.Parse(ins.Value);
                        ins.Read(); 
                        ret = true;
                    }
                    else
                    {
                        if (ins.Name == "DRAWRATIO")
                        {
                            ins.Read();
                            OctreeNode._DRAWRATIO = double.Parse(ins.Value);
                            ins.Read(); 
                            ret = true;
                        }
                        else
                        {
                            if (ins.Name == "MINPTS")
                            {
                                ins.Read();
                                OctreeNode._MINPTS = int.Parse(ins.Value);
                                ins.Read();
                                ret = true;
                            }
                            else
                            {
                                if (ins.Name == "MINPTSPCA")
                                {
                                    ins.Read(); 
                                    OctreeNode._MINPTSPCA = int.Parse(ins.Value);
                                    ins.Read();
                                    ret = true;
                                }
                                else
                                {

                                    if (ins.Name == "MAXDEPTH")
                                    {
                                        ins.Read();
                                        OctreeNode._MAXDEPTH = int.Parse(ins.Value);
                                        ins.Read();
                                        ret = true;
                                    }
                                    else
                                    {

                                        if (ins.Name == "CUROCTDEPTH")
                                        {
                                            ins.Read(); 
                                            OctreeNode._CUROCTDEPTH = int.Parse(ins.Value);
                                            ins.Read();
                                            ret = true;
                                        }
                                        else
                                        {
                                            if (ins.Name == "SURFCRIT")
                                            {
                                                ins.Read(); 
                                                OctreeNode._SURFCRIT = double.Parse(ins.Value);
                                                ins.Read();
                                                ret = true;
                                            }
                                            else
                                            {

                                                if (ins.Name == "FLATNESS")
                                                {
                                                    ins.Read(); 
                                                    OctreeNode._FLATNESS = double.Parse(ins.Value);
                                                    ins.Read();
                                                    ret = true;
                                                }
                                                else
                                                {

                                                    if (ins.Name == "ANGLECRIT")
                                                    {
                                                        ins.Read(); 
                                                        OctreeNode._ANGLECRIT = double.Parse(ins.Value); 
                                                        ret = true;
                                                        ins.Read();
                                                    }
                                                    else
                                                    {

                                                        if (ins.Name == "MINR")
                                                        {
                                                            ins.Read(); 
                                                            OctreeNode._MINR = double.Parse(ins.Value);
                                                            ins.Read();
                                                            ret = true;
                                                        }
                                                        else
                                                        {

                                                            if (_Tree.ParseXml(ref ins))
                                                            {
                                                                while (ins.Read() && _Tree.ReadXmlData(ref ins))
                                                                {
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
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

                case XmlNodeType.EndElement:
                    if (ins.Name == "Octree")
                    {
                        ret = false;
                    }
                    break;
            }
            return ret;


          //  _Tree.WriteXmlData(ref outs); 
            
        }
        //------------------------------------------------------------------------------

    }//class
 
}//namespace
