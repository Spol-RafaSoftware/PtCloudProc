using System;
using System.Xml;
using System.Collections.Generic; 
using MathsLib;
using Tao.OpenGl;
using Render;
using Model;
//------------------------------------------------------------------------------
// namespace definition
namespace Render
{
    //------------------------------------------------------------------------------
    // class Surface 
    public class Surface
    {

        //------------------------------------------------------------------------------
        // Private Data Members
        private List<OctreeLeaf> _leafs;              // a list of all of the leaf nodes in the system which have a normal whose
                                                      // vnormal points in the same direction 
        protected Vectors _vnorm;                  // vertex normal
        protected Vectors _eigenvec2;              // from pca  
        protected Vectors _eigenvec3;              // from pca       
        protected Vectors _rg;                     // raddi of gyration from pca
        protected Vectors _sigma;                  // sum of all points in descendent nodes
        protected double _tand;                    // normal distance of tangent plane from origin
        
        private int _displaylist;
        private int _id; 
        private Colour _g1;
        //------------------------------------------------------------------------------
        // Constructors
        public Surface( int id)
        {
            _id = id; 
            _leafs = new List<OctreeLeaf>();
             
            _displaylist = -1;
            _g1 = Colour.TABLECOLOUR((int)C.CONST.RANDOM(0.0d, 129.0d));
            _vnorm = new Vectors();
            _eigenvec2 = new Vectors();
            _eigenvec3 = new Vectors();
            _sigma = new Vectors();
            _rg = new Vectors(); 

        }
        //------------------------------------------------------------------------------

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        //------------------------------------------------------------------------------
        public int leafCount
        {
            get
            {
                return _leafs.Count;
            }
        }

        //------------------------------------------------------------------------------
        public List<OctreeLeaf> Leafs
        {
            get
            {
                return _leafs;
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
        public Vectors vNorm
        {
            get
            { 
                    return _vnorm;   
            }
        }

        //------------------------------------------------------------------------------
        public Colour SurfColour
        {
            get
            { 
                return _g1;
            }
        }

        //------------------------------------------------------------------------------
        public Vectors Centre
        { 
            get
            {
                if (_leafs.Count>2)
                {
                    Vectors bbMin = _leafs[0].vMin;
                    
                    Vectors bbMax = _leafs[leafCount-1].vMax;
                    return new Vectors( bbMin._X + ((bbMax._X - bbMin._X) * 0.5), 
                                        bbMin._Y + ((bbMax._Y - bbMin._Y) * 0.5), 
                                        bbMin._Z + ((bbMax._Z - bbMin._Z) * 0.5));
                }else{
                    return new Vectors();
                }
            } 
        }

        //------------------------------------------------------------------------------
        public void add(int id, OctreeLeaf lf)
        { 
            _leafs.Add(lf);

        }

        private void SurfaceAdj(OctreeLeaf l, Dictionary<OctreeLeaf, Surface> nodesuf, int i, int j, int k)
        {

            OctreeNode n = l.getAdjacentNOde(i - 1, j - 1, k - 1);
            if (n != null)
            {
                Surface s;
                if (nodesuf.TryGetValue((OctreeLeaf)n, out s))
                {
                    if (s != null)
                        Render.GL_Draw.glLine(l.Sigma, n.Sigma, _g1, s.SurfColour);
                    else
                        Render.GL_Draw.glLine(l.Sigma, n.Sigma, _g1, Colour.Blue());
                }
            }
        }
        //------------------------------------------------------------------------------
        public void CreateDisplaylist(ref List<int> dl, Dictionary<OctreeLeaf, Surface> nodesuf)
        {
            if (_leafs.Count > 0)
            {  
                _displaylist = Gl.glGenLists(1);
                Gl.glNewList(_displaylist, Gl.GL_COMPILE);  
                float[] g;
                _g1.tofloat3f(out g);

                foreach (OctreeLeaf l in _leafs)
                {

                    SurfaceAdj(l, nodesuf, 2, 1, 1);
                    SurfaceAdj(l, nodesuf, 0, 1, 1);
                    SurfaceAdj(l, nodesuf, 1, 2, 1);
                    SurfaceAdj(l, nodesuf, 1, 0, 1);
                    SurfaceAdj(l, nodesuf, 1, 1, 2);
                    SurfaceAdj(l, nodesuf, 1, 1, 0); 

                }// for each leaf in leafs

                Gl.glPointSize(5.0f);
                Gl.glEnable(Gl.GL_POINT_SMOOTH);
                Gl.glBegin(Gl.GL_POINTS);
                for (int i = 0; i < _leafs.Count; i++)
                {
                  Gl.glColor3f(g[0], g[1], g[2]);
                  Gl.glVertex3d(_leafs[i].Sigma._X, _leafs[i].Sigma._Y, _leafs[i].Sigma._Z);  
                }
                Gl.glEnd();
               // displayEigen();
                Gl.glDisable(Gl.GL_POINT_SMOOTH); 
                Gl.glEndList();
 
                dl.Add(_displaylist);
            }
        }

        //------------------------------------------------------------------------------ 
        public  void removeDisplayLists()
        {
            if (_displaylist != -1)
            {
                Gl.glDeleteLists(_displaylist, 1);
                _displaylist = -1;
            }
        }

        //-------------------------------------------------------------------------------- 
        public void CalculateStats(ref List<Surface>   SC,ref List<ModelBase> m)
        {
            // first lets sort our list
            OctreeLeafComparer lc= new OctreeLeafComparer();
            _leafs.Sort(lc);
            assignSurIDLeafs();
            DoPCATest();

             Segment(ref  SC, ref m) ;

        }
        //------------------------------------------------------------------------------ 
        public void RemoveSurfIDLeafs()
        {
            for (int i = 0; i < leafCount; i++)
            {
                _leafs[i].SurfaceNum = -1;
                _leafs[i] = null;
            }
        }

        //------------------------------------------------------------------------------ 
        private void assignSurIDLeafs()
        {
            for (int i = 0; i < leafCount; i++)
            {
                _leafs[i].SurfaceNum = ID;
            }
        }

        //------------------------------------------------------------------------------ 
        protected bool DoPCATest()
        {
            bool result = true;

            // create a list of vectors for the suface;
            List<Vectors> vcList = new List<Vectors>(leafCount);

            for (int k = 0; k < leafCount ; k++)
            {
                vcList.Add(_leafs[k].Sigma);
            }

            PCA calc = new PCA(vcList, vcList.Count);
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
            if (result)
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
            if (leafCount > SurfaceController._MINNODESURF)
            {
                Vectors c = new Vectors();
                Vectors s = new Vectors();
                Vectors from = new Vectors();
                Vectors to = new Vectors();
                c.multiply(_vnorm, _rg._X);
                s = _sigma;
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
        void Segment(ref List<Surface> SC, ref List<ModelBase> m)
        {
            if (leafCount > SurfaceController._MINNODESURF)
            {
                int trails =1;

                ModelBase[] candidateLst = new ModelBase[4];
                ModelBase TopModel = null;

                int nm = 0; // number of models under test
                int topsz = 0;
                int topflag = 0;
                int[] contiq;
                int topValidCount=0;
                double prob =0.0d;

                while(
                    (prob<SurfaceController._RANSACPROB)
                    &&(Surface.RansacPEqN(trails,leafCount)<SurfaceController._RANSACPROB)
                    &&(trails<SurfaceController._RANSACPNUM)
                    )
                {

                    Vectors[] pts = new Vectors[SurfaceController._MSSETSIZE];
                    Vectors[] ptsN = new Vectors[SurfaceController._MSSETSIZE];
                    for (int i = 0; i < SurfaceController._MSSETSIZE; i++)
                    {
                        int k =C.CONST.iRANDOM(0,leafCount-1);
                        pts[i] = _leafs[k].Sigma;
                        ptsN[i] = _leafs[k].Norm;
                    }

                    candidateLst[0] = new ModelPlane(pts, ptsN);
                     //candidateLst[0].Initialize() ;
                    if (candidateLst[0].Initialize())
                    {
                        // plane could fit the  data
                        nm++;
                    }
                    if(nm==0)
                    { 
                        // create other canidates
                        candidateLst[nm] = new ModelCylinder(pts, ptsN);
                         if(candidateLst[nm].Initialize())
                             nm++;
                         candidateLst[nm] = new ModelCone(pts, ptsN);
                         if (candidateLst[nm].Initialize())
                             nm++;
                         candidateLst[nm] = new ModelSphere(pts, ptsN);
                         if (candidateLst[nm].Initialize())
                             nm++;
                    }

                    // test models
                    while (nm > 0)
                    {
                        nm--;
                        int ValidCount= candidateLst[nm].Eval(_leafs);
                        if (ValidCount > SurfaceController._MINNODESURF)
                        {
                            if (ValidCount > topsz)
                            {
                                candidateLst[nm].refine(_leafs, true);
                                ValidCount=candidateLst[nm].Eval(_leafs);
                                FlagSubset(out contiq, null);
                                if (contiq[contiq[0]] > topsz)
                                {
                                    topValidCount = ValidCount;
                                    TopModel = candidateLst[nm];
                                    topflag = contiq[0];
                                    topsz = contiq[topflag];
                                }

                            }//if ValidCount>topsz
                        } 
                    }//while nm > 0



                    trails++;

                    if(TopModel!=null)
                    {
                        prob = TopModel.probabillity4(trails, topsz, topValidCount, this._leafs.Count);
                    }

                }// while nm > 0

                if (TopModel != null)
                {
                    // recompute largest subset
                    TopModel.Eval(Leafs);
                    
                     FlagSubset(out contiq, null);
                     int flag = contiq[0];
                  
                     split(flag,ref SC);

                    TopModel.refine(Leafs, false);
                    m.Add(TopModel);
                }// if TopModel
            }// if leafscount> minsurf


        }

        //------------------------------------------------------------------------------    
        private  void FlagSubset(out int [] contig, Surface Join)
        {
            List<int> res = new List<int>();
            res.Add(0);
            int fmax = 0;
            int pmax = leafCount;
            int iq=0, dq=0;
            int joinid =-1;
            if (Join != null)
            {
                joinid = Join.ID;
                pmax += Join.leafCount;
            }
            List<OctreeLeaf> q = new List<OctreeLeaf>(pmax);
            for (int k = 0; k < pmax; k++)
                q.Add(null);

            Surface s = this;
            
            while (s != null)
            {
                List<OctreeLeaf> nl = s.Leafs;

                for (int i = 0; i < s.leafCount; i++)
                {
                    OctreeLeaf l = nl[i];
                    if (l.Flag == 0)
                    {
                        //start new sub set flag it and enqueue
                        fmax++;
                        l.Flag = fmax;
                        res.Add(1);
                        iq = dq = 0;
                        q[iq] = l;
                        iq++;

                        // deque nodes process adj
                        while (dq < iq )
                        {
                            if (dq < q.Count)
                            {
                                l = q[dq];
                                for (int x = -1; x < 2; x++)
                                {
                                    for (int y = -1; y < 2; y++)
                                    {

                                        for (int z = -1; z < 2; z++)
                                        {
                                            OctreeLeaf adj = (OctreeLeaf)l.getAdjacentNOde(x, y, z);
                                            if (adj != null)
                                            {
                                                if ((adj.SurfaceNum == ID) || (adj.SurfaceNum == joinid))
                                                {
                                                    if (adj.Flag == 0)
                                                    {
                                                        adj.Flag = fmax;
                                                        res[fmax]++;
                                                        if (iq < q.Count)
                                                            q[iq] = adj;
                                                        iq++;
                                                    }
                                                }// surf 
                                            }
                                        }// for z -1,0,1 
                                    }// for y -1,0,1 
                                }// for x -1,0,1 
                            }
                            dq++;

                        }//queque

                    }// al unflagged nodes
 
                }// for all leafs
                if (s == this)
                {
                    s = Join;
                }
                else
                {
                    s = null;
                }
            }//while s!= null


            // find largest contiguous segment
            int mm = 0;
            for (int i=0;i<= fmax; i++)
            {

                if (res[i] > mm)
                {
                    mm = res[i];
                    res[0]=i;
                }
            }

            //contig
            contig = new int [res.Count];
            for (int i = 0; i < res.Count; i++)
            {
                contig[i] = res[i];
            }


        }
        //------------------------------------------------------------------------------    
        private void split(int flag,ref List<Surface> SC)
        {
            List<OctreeLeaf> newLst = new List<OctreeLeaf>(leafCount);

            for (int i = 0; i < leafCount; i++)
            {
                if (_leafs[i].Flag == flag)
                {
                    newLst.Add(_leafs[i]);
                    _leafs[i].Flag = -1;
                }
                else
                    _leafs[i].Flag = 0;
            }

            int[] f;
            FlagSubset(out f, null); 
            Surface[] Frag = new Surface[f.Length];
            int j =1;
            for (int i = 0; i < f.Length; i++)
            {
                if(f[i]>= SurfaceController._MINNODESURF)
                { 
                    Surface s = new Surface(SC.Count+j);
                    j++;
                    Frag[j - 1] = s;
                }
            }

            // distribute the remaining modes
            for (int i = 0; i < leafCount; i++)
            {
                int nflag = Leafs[i].Flag;
                if (nflag > 0)
                {
                    if (Frag[nflag] != null)
                    {
                        Frag[nflag].add(Leafs[i].ID,Leafs[i]);
                    }
                    else
                        Leafs[i].SurfaceNum = -1;
                    Leafs[i].Flag = 0;
                }
            }

            // finalise surfaces
            for (int i = 0; i < f.Length; i++)
            {
                // 
                if (Frag[i] != null)
                {
                    if (Frag[i].leafCount > SurfaceController._MINNODESURF)
                    {
                        Frag[i].assignSurIDLeafs();
                        Frag[i].DoPCATest();
                        SC.Add(Frag[i]);
                    }
                    else
                    {

                        Frag[i].RemoveSurfIDLeafs();
                        Frag[i]=null; 
                    }
                }
            }

            _leafs = newLst;
            assignSurIDLeafs();
            DoPCATest();
        }


        //------------------------------------------------------------------------------    
        public static double RansacPEqN(int trails, int nodeCount)
        {
            double C = 1.0 -Math.Pow(((double)SurfaceController._MINNODESURF/(double)nodeCount),4); 
            return (1.0d - Math.Pow(C, (double)trails));
        }

        //-------------------------------------------------------------------------------
        public void SurfaceData(out string[] surfacedata)
        {

            surfacedata = new string[leafCount + 3];
            surfacedata[0] = "Surface: " + ID;
            int i = 1;
            surfacedata[i] = "Norm: " + vNorm.ToString();
            i++;//i = 2
            surfacedata[i] = "Leafs: " + leafCount;
            i++;//i = 3 
            if (leafCount != 0)
            {
                for (int j = 0; j < leafCount; j++)
                {
                    surfacedata[j + i] = "l: " + Leafs[j].ID + ": " + vNorm.dot(Leafs[j].Norm) + ": " + Leafs[j].Norm.ToString();
                }
            }
            else
                surfacedata[i] = "l:NO LEAFS";
        }
  
        //------------------------------------------------------------------------------
        public void WriteXmlData(ref XmlTextWriter outs)
        {
            outs.WriteStartElement("Surface");
            outs.WriteStartElement("Model_ID");
            outs.WriteValue(_id);
            outs.WriteFullEndElement();
            _g1.WriteXmlData(ref outs);
            outs.WriteStartElement("SurfaceNorm");
            _vnorm.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement("SurfaceEigen2");
            _eigenvec2.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement("SurfaceEigen3");
            _eigenvec3.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement("SurfaceRadii");
            _rg.write4(ref outs);
            outs.WriteFullEndElement();
            outs.WriteStartElement("SurfaceSigma");
            _sigma.write4(ref outs);
            outs.WriteFullEndElement();
            outs.Flush();
            outs.WriteStartElement("LeafsInSurface");
            foreach (OctreeLeaf L in _leafs)
            { 
                outs.WriteValue(" "+L.ID+" "); 
            }
            outs.WriteFullEndElement();
            outs.WriteFullEndElement();
        }

    }
} 