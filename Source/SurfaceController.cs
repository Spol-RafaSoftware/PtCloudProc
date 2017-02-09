using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
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
    public class SurfaceController
    {

        //------------------------------------------------------------------------------
        // STATIC and Enum 
        public static int _MINNODESURF = 5;
        public static int _MSSETSIZE = 5;
        public static int _RANSACPNUM = 750;                             // max number of iterations for the ransac
        public static double _RANSACPROB = 0.95d;                          // the probabillity before we consider a
                                                                          // A candidate Worthy

        //------------------------------------------------------------------------------
        // Private Data Members
        private List<Surface> _Surfaces;
        private List<ModelBase> _Models;
        private List<int> _displaylist; 
        private Dictionary<OctreeLeaf, Surface> _NodeSufDict;
        private int _SurfSel;
        private int _ModelSel;
        private int _usedCount; 


        //------------------------------------------------------------------------------
        public SurfaceController()
        {
            _Surfaces = new List<Surface>();
            _displaylist = new List<int>();
            _SurfSel = -1;
            _ModelSel = -1;
            _Models = new List<ModelBase>();
            _NodeSufDict = new Dictionary<OctreeLeaf, Surface>();
        }
 

        //------------------------------------------------------------------------------
        public int SurfCOUNT 
        {
            get
            {
                return _Surfaces.Count;
            }
        }

        //------------------------------------------------------------------------------
        public int ModelCOUNT
        {
            get
            {
                return _Models.Count;
            }
        }


        //------------------------------------------------------------------------------
        public int ModelSel
        {
            get
            {
                return _ModelSel;
            }
        }
        //------------------------------------------------------------------------------
        public int SurfSel
        {
            get
            {
                return _SurfSel;
            }
        }
        //------------------------------------------------------------------------------
        //  
        public void drawID(ref int i, Scene.eDrawMode dm)
        {
            if ((i >= -1) && (SurfCOUNT > i))
            {
                _SurfSel = i;

            }
            else
            {
                if (i > SurfCOUNT)
                {
                    _SurfSel = -1;
                }
                else
                {
                    if (i < -1)
                    {
                        _SurfSel = (SurfCOUNT - 1);
                    } 
                }
             
            }
            i = _SurfSel;
            ReSetDrawList(dm);
        } 

        //------------------------------------------------------------------------------
        public void AddSurface(Surface s)
        {
            _Surfaces.Add(s);
        }

        //------------------------------------------------------------------------------
        private void createdisplaylistAll()
        {

            foreach (Surface s in _Surfaces)
            {
                s.CreateDisplaylist(ref _displaylist,_NodeSufDict);
            }
        }

        //------------------------------------------------------------------------------
        public void ReSetDrawList(Scene.eDrawMode dm)
        {
            removeDisplayLists(); 
            if ( Scene.eDrawMode.eDM_LEAFSURFACE <= dm)
            {
                switch(dm)
                {
                    case Scene.eDrawMode.eDM_LEAFSURFACE:
                        {
                            if (_SurfSel == -1)
                            {
                                createdisplaylistAll();
                            }
                            else
                            {
                                Surface s = getSurfaceID(_SurfSel);
                                if (s != null)
                                    s.CreateDisplaylist(ref _displaylist, _NodeSufDict);
                            }
                        }
                        break;
                    case Scene.eDrawMode.eDM_MODELALL: 
                        break;
                    case Scene.eDrawMode.eDM_MODELPLANES: 
                        break;
                    case Scene.eDrawMode.eDM_MODELSPHERE: 
                        break;
                    case Scene.eDrawMode.eDM_MODELCYLINDER: 
                        break;
                    case Scene.eDrawMode.eDM_MODELCONE:
                        break;
                    case Scene.eDrawMode.eDM_MODELNUMS: 
                        break;
                }
            } 
        }

        //------------------------------------------------------------------------------
        private Surface getSurfaceID(int ID)
        {
            Surface ret = null; 
            foreach (Surface s in _Surfaces)
            {
                if (s.ID == ID)
                {
                    ret = s;
                }
            } 
            return ret;
        }
        //------------------------------------------------------------------------------
        private ModelBase getModelID( int ID)
        {
            ModelBase ret = null; 

            foreach (ModelBase m in _Models)
            {
                if (ID == m.ID)
                {
                    ret = m;
                }
            } 
            
            return ret; 
        }

        //-----------------------------------------------------------------------------
        public void DrawModelNums(List<int> drawme, bool ModelColourType, int sel)
        {
            if (drawme != null)
            {
                if (drawme.Count > 0)
                {
                    foreach (int i in drawme)
                    {
                        foreach (ModelBase m in _Models)
                        {
                            if (i == m.ID)
                            {
                                m.CreateDisplayList(_displaylist, ModelColourType);
                            } 
                        }


                    }
                }
            }
            else
            {
                foreach (ModelBase m in _Models)
                {
                    switch (sel)
                    {

                        case (int)Scene.eDrawMode.eDM_MODELPLANES:
                            {
                                if (m is ModelPlane)
                                {
                                    m.CreateDisplayList(_displaylist, ModelColourType);

                                }
                            }
                            break;
                        case (int)Scene.eDrawMode.eDM_MODELCYLINDER:
                            {
                                if (m is ModelCylinder)
                                {
                                    m.CreateDisplayList(_displaylist, ModelColourType);

                                }
                            }
                            break;
                        case (int)Scene.eDrawMode.eDM_MODELCONE:
                            {
                                if (m is ModelCone)
                                {
                                    m.CreateDisplayList(_displaylist, ModelColourType);

                                }
                            }
                            break;
                        case (int)Scene.eDrawMode.eDM_MODELSPHERE:
                            {
                                if (m is ModelSphere)
                                {
                                    m.CreateDisplayList(_displaylist, ModelColourType);

                                }
                            }
                            break;
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        //  Draw all the surfaces
        public void Draw()
        { 
            foreach (int i in _displaylist)
            {
                Gl.glCallList(i);
            }
        }

        //------------------------------------------------------------------------------
        public void removeDisplayLists()
        {

            foreach (Surface s in _Surfaces)
            {
                s.removeDisplayLists();
            }
            foreach (ModelBase m in _Models)
            {
                m.DestroyDisplayList();
            }
            _displaylist = new List<int>();
        }
        //------------------------------------------------------------------------------
        public void GetAllSurfaceData(out string[] surfacedata)
        {
            if(_Surfaces.Count>0) 
            {
                surfacedata = new string[_Surfaces.Count];
                int i = 0;
                foreach (Surface s in _Surfaces)
                {
                    string str="";
                    str += s.ID.ToString();
                    str += " ";
                    str += _Surfaces[i].GetType();
                    surfacedata[i] = str;
                    i++;
                }


            }else
            {
                surfacedata = new string[1];
                surfacedata[0] = "";
            }

        }
        //------------------------------------------------------------------------------
        public void GetAllModelData(out string[] surfacedata)
        {
            if (_Models.Count > 0)
            {
                surfacedata = new string[_Models.Count];
                int i = 0;
                foreach (ModelBase m in _Models)
                {
                    string str = "";
                    str += m.ID.ToString(); 
                    str += " "; 
                    str += m.type();
                    surfacedata[i] = str;
                    i++;
                }


            }
            else
            {
                surfacedata = new string[1];
                surfacedata[0] = "";
            }

        }
     
        //------------------------------------------------------------------------------
        public void SimpleModelData(string[] surfacedata5 ) 
        {
            surfacedata5[0] = "Models " + _Models.Count;

            int cyl = 0, sph = 0, ple = 0, cne = 0;
            foreach (ModelBase m1 in _Models)
            {
                if (m1.type() == ModelCylinder.stype())
                {
                    cyl++;
                }
                else if (m1.type() == ModelSphere.stype())
                {
                    sph++;
                }
                else if (m1.type() == ModelCone.stype())
                {
                    cne++;
                }
                else if (m1.type() == ModelPlane.stype())
                {
                    ple++;
                }
            }

            surfacedata5[1] = ModelPlane.stype() + ":" + ple;
            surfacedata5[2] = ModelSphere.stype() + ":" + sph;
            surfacedata5[3] = ModelCylinder.stype() + ":" + cyl;
            surfacedata5[4] = ModelCone.stype() + ":" + cne;
               
        }
   
        //------------------------------------------------------------------------------
        public void ModelData(out string[] surfacedata,ref int id)
        { 
            ModelBase m = null;
            m = getModelID(id);
            if (m != null)
            {
                m.ModelData(out surfacedata);
                id = m.ID;
            }
            else
            {
                surfacedata = new string[5];
                surfacedata[0] = "Models " + _Models.Count;

                int cyl = 0, sph = 0, ple = 0, cne = 0;
                foreach (ModelBase m1 in _Models)
                {
                    if (m1.type() == ModelCylinder.stype())
                    {
                        cyl++;
                    }
                    else if (m1.type() == ModelSphere.stype())
                    {
                        sph++;
                    }
                    else if (m1.type() == ModelCone.stype())
                    {
                        cne++;
                    }
                    else if (m1.type() == ModelPlane.stype())
                    {
                        ple++;
                    }
                }

                surfacedata[1] = ModelPlane.stype() + ":" + ple;
                surfacedata[2] = ModelSphere.stype() + ":" + sph;
                surfacedata[3] = ModelCylinder.stype() + ":" + cyl;
                surfacedata[4] = ModelCone.stype() + ":" + cne;
                id = 0;
            } 
        }
        
            
        //------------------------------------------------------------------------------
        public void SurfaceData(out string[] surfacedata,ref int id)
        { 
            Surface s = null;
            s = getSurfaceID(id);
            if (s != null)
            {
                s.SurfaceData(out surfacedata);
                id=s.ID;
            }
            else
            {
                //s = getSurfaceID(0);
                surfacedata = new string[2];
                surfacedata[0] = "Surfaces " + _Surfaces.Count;
                surfacedata[1] = "Used:" + _usedCount;
                id = 0;
            }
        }

        //------------------------------------------------------------------------------
        public void SimpleSurfaceData( string[] surfacedata2)
        { 
                //s = getSurfaceID(0); 
            surfacedata2[0] = "Surfaces " + _Surfaces.Count;
            surfacedata2[1] = "Used:" + _usedCount;
            } 

        //------------------------------------------------------------------------------
        public void SurfaceTests(List<OctreeLeaf> leafs,ref double PP,int OctreeCount)
        {
            if (_Surfaces.Count > 0)
            {
                
                DateTime startTime = DateTime.Now;   
                Trace.WriteLine("_SC-SurfaceTests-Start"+ startTime.ToString());
                ModelBase.set_POSERROR(leafs[0].R);  
                _usedCount = 0;
                int i = 0;
                // we calculate the  new surface here so the 
                // CalculateStats creates new surfaces 
                // limit is we dont want more surfaces than OctreeCells
                for (; (i < _Surfaces.Count) &&(_Surfaces.Count<OctreeCount); i++)
                {
                    if (_Surfaces[i].leafCount < _MINNODESURF)
                    {
                        _Surfaces[i].RemoveSurfIDLeafs();
                        _Surfaces.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        Trace.WriteLine("_SC-CalculateStats:" + i.ToString());
                        _Surfaces[i].CalculateStats(ref _Surfaces,ref _Models);
                    }
                    
                }
                PP = 20.0d;

                // reset ID
                for (i=0; i < _Surfaces.Count; i++)
                {
                    _Surfaces[i].ID = i;
                }

                for (i = 0; i < _Models.Count; i++)
                {
                    _Models[i].ID = i;
                    _Models[i].Sort_l();
                }
                

                for (i = 0; i < _Surfaces.Count; i++)
                {
                    for (int j = 0; j < _Surfaces[i].leafCount; j++)
                    {
                        try
                        {
                            _NodeSufDict.Add(_Surfaces[i].Leafs[j], _Surfaces[i]);
                        } 
                        catch (ArgumentException)
                        {
                            Trace.WriteLine("_SC-An element with Key =" + _Surfaces[i].Leafs[j].ID + "  already exists.");
                        }
                    }
                }

                PP = 60.0d;
                // Calculate the leafs unUsed 
                foreach(OctreeLeaf l in leafs)
                {
                    if (!_NodeSufDict.ContainsKey(l))
                    {
                        _NodeSufDict.Add(l, null);
                        _usedCount++;
                    }
                }
                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                Trace.WriteLine("_SC-SurfaceTests-End:" + duration.ToString());
            }
        }


        //------------------------------------------------------------------------------
        public void SurfacedrawColour(double R, double G, double B)
        {
            Surface s;
            if (_SurfSel < 0)
            {
                s = getSurfaceID(0);

            }
            else
            {
                s = getSurfaceID(_SurfSel);

            }

            if (s != null)
            {
                s.SurfColour._R = R;
                s.SurfColour._G = G;
                s.SurfColour._B = B;
            } 
        }
                
        //------------------------------------------------------------------------------
        public void WriteXmlData(ref XmlTextWriter outs)
        { 
            outs.WriteStartElement("SurfaceController");
                outs.WriteStartElement("MINNODESURF");
                outs.WriteValue(_MINNODESURF);
                outs.WriteFullEndElement();
                outs.WriteStartElement("RANSACPNUM");
                outs.WriteValue(_RANSACPNUM);
                outs.WriteFullEndElement();
                outs.WriteStartElement("RANSACPROB");
                outs.WriteValue(_RANSACPROB);
                outs.WriteFullEndElement();
                outs.WriteStartElement("POSERROR");
                outs.WriteValue(ModelBase.get_POSERROR());
                outs.WriteFullEndElement();
                outs.WriteStartElement("PCNTERROR");
                outs.WriteValue(ModelBase.get_PCNTERROR());
                outs.WriteFullEndElement();
                outs.WriteStartElement("ANGLECRIT");
                outs.WriteValue(ModelBase._ANGLECRIT);
                outs.WriteFullEndElement(); 
                
           outs.WriteStartElement("Surfaces");  
                foreach (Surface o in _Surfaces)
                {
                  o.WriteXmlData(ref outs);
                }
           outs.WriteFullEndElement(); 
           outs.WriteStartElement("Models");  
                foreach (ModelBase o in _Models)
                {
                  o.WriteXmlData(ref outs);
                } 
     
           outs.WriteFullEndElement();
           outs.WriteStartElement("Pairs");
                foreach (var pair in _NodeSufDict)
                {
                    if ((pair.Key != null) && (pair.Value != null))
                    {
                        outs.WriteValue(" "+pair.Key.ID + " ") ;
                        outs.WriteValue(" "+pair.Value.ID+" "); 
                    }
                }
            outs.WriteFullEndElement(); 
            outs.WriteStartElement("usedCount");
               outs.WriteValue(_usedCount);
            outs.WriteFullEndElement();
            outs.WriteFullEndElement();
        } 
        //------------------------------------------------------------------------------
    }
}
