//------------------------------------------------------------------------------
// using definition
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Xml.Serialization;
using Tao.OpenGl; 
using MathsLib;
using Render;

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class definition 
    public class Scene
    {

        //------------------------------------------------------------------------------
        //Private Members for the Class
        private Octree _Ot;
        private SurfaceController _Sc;
        private Vectors _c;
        private Vectors _Offeset;
        private Vectors _OffesetOrtho;
        private Ray _ray;
        private double[] _modelMatrix;
        private Scene.eDrawMode _drawMode;
        public enum eDrawMode { eDM_BRANCH_PTS, eDM_BRANCH_BB, eDM_BRANCH_BBPTS, eDM_CENTERS, eDM_LEAFCENTERS, eDM_LEAFSIGMA, eDM_LEAFSURFACE, eDM_MODELALL, eDM_MODELPLANES, eDM_MODELSPHERE, eDM_MODELCYLINDER, eDM_MODELCONE, eDM_MODELTORUS, eDM_MODELNUMS };

        private string _dataSrc;

        //------------------------------------------------------------------------------
        public Scene()
        {
            _Ot = new Octree();
            _Sc = new SurfaceController();
            _ray = new Ray();
            _c = new Vectors();
            _Offeset = new Vectors(); 
            _OffesetOrtho = new Vectors(0.0, 0.0, 0.0);
           
            _modelMatrix = new double[16];
            _drawMode = Scene.eDrawMode.eDM_CENTERS;
            _dataSrc = "";
        }


        //------------------------------------------------------------------------------
        // Update
        public int intersects(Ray r, double NearFarLength)
        {
            _ray = r;
           return  _Ot.intersects(r, NearFarLength);
        }


        //------------------------------------------------------------------------------
        //  
        public int drawID
        { 
            get
            {
                return _Ot.DRAWID;
            }
        } 
        //------------------------------------------------------------------------------
        //  
        public string DataSrc
        {
            set
            {
                _dataSrc = value;
            }
            get
            {
                return _dataSrc;
            }
        }
 
        //------------------------------------------------------------------------------
        //  
        public Scene.eDrawMode drawMode
        {
            set
            {
                _drawMode = value;
                _Ot.ReSetDrawList(_drawMode); 
                _Sc.ReSetDrawList(_drawMode);
                
            }
            get
            {
                return _drawMode;
            }
        }
        //------------------------------------------------------------------------------
        //  drawModeInc
        private void drawModeInc()
        {
            switch (_drawMode)
            {
                case Scene.eDrawMode.eDM_BRANCH_PTS:
                    _drawMode = Scene.eDrawMode.eDM_BRANCH_BB;
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BB:
                    _drawMode = Scene.eDrawMode.eDM_BRANCH_BBPTS;
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BBPTS:
                    _drawMode = Scene.eDrawMode.eDM_CENTERS;
                    break;
                case Scene.eDrawMode.eDM_CENTERS:
                    _drawMode = Scene.eDrawMode.eDM_LEAFCENTERS;
                    break;
                case Scene.eDrawMode.eDM_LEAFCENTERS:
                    _drawMode = Scene.eDrawMode.eDM_LEAFSIGMA;
                    break;
                case Scene.eDrawMode.eDM_LEAFSIGMA:
                    _drawMode = Scene.eDrawMode.eDM_LEAFSURFACE;
                    break;
                case Scene.eDrawMode.eDM_LEAFSURFACE:
                    _drawMode = Scene.eDrawMode.eDM_BRANCH_PTS;
                    break;
            }
 
        }

        //------------------------------------------------------------------------------
        //  drawModeDec
        private void drawModeDec()
        {
            switch (_drawMode)
            {
                case Scene.eDrawMode.eDM_BRANCH_PTS:
                    _drawMode = Scene.eDrawMode.eDM_LEAFSURFACE;
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BB:
                    _drawMode = Scene.eDrawMode.eDM_BRANCH_PTS;
                    break;
                case Scene.eDrawMode.eDM_BRANCH_BBPTS:
                    _drawMode = Scene.eDrawMode.eDM_BRANCH_BB;
                    break;
                case Scene.eDrawMode.eDM_CENTERS:
                    _drawMode = Scene.eDrawMode.eDM_BRANCH_BBPTS;
                    break;
                case Scene.eDrawMode.eDM_LEAFCENTERS:
                    _drawMode = Scene.eDrawMode.eDM_CENTERS;
                    break;
                case Scene.eDrawMode.eDM_LEAFSIGMA:
                    _drawMode = Scene.eDrawMode.eDM_LEAFCENTERS;
                    break;
                case Scene.eDrawMode.eDM_LEAFSURFACE:
                    _drawMode = Scene.eDrawMode.eDM_LEAFSIGMA;
                    break;
            }
 
        } 
        //------------------------------------------------------------------------------
        public int OctreeID_inc()
        {
            return _Ot.drawIDInc(_drawMode);
        }
        
        //------------------------------------------------------------------------------
        public int OctreeID_dec()
        {
            return _Ot.drawIDDec(_drawMode);
        }
        
        //------------------------------------------------------------------------------
        public void OctreeDrawID(ref int i)
        {
             _Ot.drawID(ref i, _drawMode);
        }
 
        //------------------------------------------------------------------------------
        public void SurfacedrawID(ref int i)
        {
            _Sc.drawID(ref i, _drawMode);
        }

        //------------------------------------------------------------------------------
        public void SurfacedrawColour(double R,double G, double B)
        {
            _Sc.SurfacedrawColour(R,G,B);
            this.drawMode = (Scene.eDrawMode.eDM_LEAFSURFACE);
        }

        //------------------------------------------------------------------------------
        public int getSurfaceSel()
        {
            return _Sc.SurfSel;
        }
        //------------------------------------------------------------------------------
        public int getModelSel()
        {
            return _Sc.SurfSel;
        }
        //------------------------------------------------------------------------------
        public int OctreeCount()
        {
            return _Ot.Count;
        }

        //------------------------------------------------------------------------------
        public void OctreeData(out string[] octreedata,int id)
        { 
            // if id <0 use drawid 
                _Ot.OctreeData(out octreedata, id); 
        }

        //------------------------------------------------------------------------------
        public void DrawModelNums(List<int> drawme,bool colByType,int selType)
        {
            _Sc.DrawModelNums(drawme,colByType,selType);
        }

        //------------------------------------------------------------------------------
        public void SurfaceData(out string[] surfdata,ref int id)
        {
            _Sc.SurfaceData(out surfdata, ref id);
        }

        //------------------------------------------------------------------------------
        public void ModelData(out string[] modeldata,ref int id)
        {
            _Sc.ModelData(out modeldata,ref  id);
        } 

        //------------------------------------------------------------------------------
        public void SimpleModelData(string[] modeldata5)
        { 
            _Sc.SimpleModelData(modeldata5);
        }

        //------------------------------------------------------------------------------
        public void SimpleSurfaceData(string[] surfdata2)
        {
            _Sc.SimpleSurfaceData(surfdata2);
        }

        //------------------------------------------------------------------------------
        public void SimpleOctData(string[] octreedata)
        {
            // if id <0 use drawid 
            _Ot.SimpleOctData(octreedata);
        }

        //------------------------------------------------------------------------------
        public void WriteOctreeData(string filename)
        {
            _Ot.WriteOctreeData(filename);
        }

        //------------------------------------------------------------------------------
        public void Showhighlight(bool b )
        {
            _Ot.Showhighlight = b;
        }

        //------------------------------------------------------------------------------
        public int MoveAdjhighlight(int x, int y, int z)
        {
            return _Ot.MoveAdjhighlight(x,y,z);
        }  

        //-----------------------------------------------------------------------------
        public int screenRay( int MouseX, int MouseY )
        {
            double nx, ny, nz, fz, fy, fx,nearFar;
            if (unProject(MouseX, MouseY, 0.1d, out nx, out ny, out nz) == true)
            {
                if (unProject(MouseX, MouseY, 1.0d, out fx, out fy, out fz) == true)
                {

                    _ray = new Ray(new Vectors(nx, ny, nz), (new Vectors(fx - nx ,  fy-ny , fz - nz).normalize()));//nx - fx, ny - fy, nz - fz)).normalize()); //
                    nearFar =Math.Abs( fz - nz);
                    return _Ot.intersects(_ray, nearFar);
                }
            }
            return -1;
        }

        //------------------------------------------------------------------------------
        // Update
        private bool unProject(int x, int y, double NrFar, out double worldNearX, out double worldNearY, out double worldNearZ)
        {
            double[] projMatrix = new double[16]; 
            int[] viewport = new int[4];
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projMatrix);
            if (Glu.gluUnProject((double)x, (double)(viewport[1] + viewport[3] - y), NrFar, _modelMatrix, projMatrix, viewport, out worldNearX, out worldNearY, out worldNearZ) == Gl.GL_TRUE)
            {
                return true;
            }
            else
                return false;
        }

        //------------------------------------------------------------------------------
        public string InitScene(ref double ProcProg)
        { 
            Vectors Avg;
            DateTime startTime = DateTime.Now, stopTime; 
            // build Octree 
            _Ot.Init(); 
            _Ot.OctreeSurface(ref _Sc,ref ProcProg);
            stopTime = DateTime.Now;
            //_Ot.ReSetDrawList(_drawMode);
            TimeSpan duration = stopTime - startTime;
            PtData.ptCloud_avg(out Avg); 
            _Offeset = Avg; 
           return duration.ToString();
        }

        //------------------------------------------------------------------------------
        // Move_OffesetOrtho() 
        public void Move_OffesetOrtho(double x, double y, double z)
        {
            _OffesetOrtho.addXYZ(x, y, z);
        }
        
        //------------------------------------------------------------------------------
        // reset ortho transform
        public void Reset_OffesetOrtho()
        { 
            _OffesetOrtho.set(0.0d, 0.0d, 0.0d, 0.0d);
        }

        //------------------------------------------------------------------------------
        // DrawNotRotated() 
        public void DrawNotRotated()
        {
            Gl.glPushMatrix();
              Gl.glTranslated( -_OffesetOrtho._X,  -_OffesetOrtho._Y,  -_OffesetOrtho._Z);
             Gl.glScaled(view.OrthoPos, view.OrthoPos, view.OrthoPos);
            _Ot.Draw();
            _Sc.Draw();
            Gl.glPopMatrix();
         }

        //------------------------------------------------------------------------------
        // Draw() 
        public void Draw()
        {
           Gl.glPushMatrix();
           Gl.glTranslated(-_Offeset._X, -_Offeset._Y, -_Offeset._Z);
           Gl.glDisable(Gl.GL_LIGHTING);
           Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, _modelMatrix);
           _Ot.Draw();
           _Sc.Draw();
           GL_Draw.glLine(_ray.position(), _ray.position() + 10.0 * _ray.direction(),Colour.Indian_Red(),Colour.Indian_Red());
           Gl.glEnable(Gl.GL_LIGHTING);
           Gl.glPopMatrix();
        }

        //------------------------------------------------------------------------------
        //  get Current OCtree Centre and Max and Radius
        public void getViewData(out Vectors max, out Vectors cen, out Vectors sigma, ref double radius)
        {
            _Ot.getViewData(out max, out cen, out sigma, ref radius);
            _Offeset = cen;
        }


        //------------------------------------------------------------------------------
        public void GetAllSurfaceData(out string[] Surfdata)
        {
            _Sc.GetAllSurfaceData(out Surfdata);
        }

        //------------------------------------------------------------------------------
        public void GetAllModelData(out string[] Surfdata)
        {
            _Sc.GetAllModelData(out Surfdata);
        }
        
        //------------------------------------------------------------------------------
        public void WriteXmlData(ref XmlTextWriter outs) 
        { 
            outs.WriteStartElement("Scene");
                outs.WriteStartElement("DataSrc");
                outs.WriteValue(_dataSrc);
                outs.WriteFullEndElement();
                outs.Flush(); 
                    _Ot.WriteXmlData(ref outs); 
                outs.Flush(); 
                    _Sc.WriteXmlData(ref outs); 
                outs.Flush();
                outs.WriteStartElement("SceneCentre");
                    _c.write4(ref outs);
                outs.WriteFullEndElement();
            outs.WriteFullEndElement(); 
        }    
    
        //------------------------------------------------------------------------------
        public void ReadXmlData(ref XmlTextReader ins)
        {
            bool finished = false;
            while (!finished)
            {
                ins.Read();

                if (ParseXml(ref ins, ref finished))
                { 
                    while (ins.Read()&&ParseXml(ref ins,ref finished))
                    { 
                    }
                }
            }
 
        }

        //-------------------------------------------------------------------------------
        private bool ParseXml(ref XmlTextReader ins, ref bool finished)
        {

               bool ret = false; // have we delt with it?
                switch (ins.NodeType)
                {
                    case XmlNodeType.Element:
                        if (ins.Name == "Scene")
                        {
                            ret = true;
                        }
                        else
                        {
                            if (ins.Name == "Centre")
                            {
                                Debug.Write(ins.Name);
                                _c.read(ref ins);
                                ins.Read();
                                ret = true;
                            }
                            else
                            {

                                if (ins.Name == "DataSrc")
                                {
                                    ins.Read();

                                    _dataSrc = ins.Value;
                                    ins.Read();
                                    ret = true;
                                }
                                else
                                {
                                    if (_Ot.ParseXml(ref ins))
                                    {
                                        while (ins.Read() && _Ot.ReadXmlData(ref ins))
                                        {
                                        }
                                    }
                                    else
                                    {
                                        //
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
                        if (ins.Name == "Scene")
                        {
                            finished = true;
                            ret = true;
                        }
                        break;
                }

                return ret;
        }
        //-------------------------------------------------------------------------------
    }
}
