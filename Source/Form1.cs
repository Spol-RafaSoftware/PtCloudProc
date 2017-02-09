using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
using Tao.OpenGl;
using Render;
using Model;
using MathsLib;

namespace TaoComp1
{

    public partial class Form1 : Form
    {
        private static Color _c1;
        private Scene _Scene;  
        private ViewManager _VManage; 
        private double _ANG_INC = 5.0f;
        private double _POS_INC = 0.1;
        private int _CREATE_POINTS = 80000;
        private string _filename;
        private int _state = 0;
        private bool _mouseSelect = false;                                  // use mouse casting ray                
        private bool _loadScene = false;                                    // are we loading a scene
        private bool _gendata = false;                                     // are we  generating a scene pts
        private int _hglghtOC = 0; 
        static private double _ProcessingProgress = 0;
          
        private static fm_Processing _p;
        private Thread _Tr;
        //------------------------------------------------------------------------------
        //Ctor
        public Form1()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts(); 
            _p = null;
            _Tr = null;
            _loadScene = false;
         }

        //------------------------------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        { 
            timer1.Start();
            timer1.Interval = 30;
            NMUD_OCTCELL.Maximum = (decimal)_CREATE_POINTS;
            NMUD_OCTCELL.Minimum = -1; // we wont ever reach -1 but need to allow 
            NMUD_MINSURF.Value = SurfaceController._MINNODESURF;
            NMUD_MINPTCELL.Value = OctreeNode._MINPTS;
        }

        //------------------------------------------------------------------------------
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Dispose();                 // Permanently stop the timer
            this.simpleOpenGlControl1.DestroyContexts();
            this.simpleOpenGlControl1.Dispose();
            _Scene = null;   
            _VManage = null;   

        }


        //------------------------------------------------------------------------------
        public double ANG_INC
        {
            get
            {
                return _ANG_INC;
            }
            set
            {
                _ANG_INC = value;
            }
        }

        //------------------------------------------------------------------------------
        public double POS_INC
        {
            get
            {
                return _POS_INC;
            }
            set
            {
                _POS_INC = value;
            }
        } 
 
        //------------------------------------------------------------------------------
        private void ChangeOctCell(ref int i)
        {
            _Scene.OctreeDrawID(ref i);
            OctreeData_Show(-1);
        }

        //------------------------------------------------------------------------------
        public void sceneInit()
        { 
            string str;
            Trace.WriteLine("_Scene-InitScene-Start"); 
            str = _Scene.InitScene(  ref _ProcessingProgress);
            Message(str);
            Trace.WriteLine(str);
            Trace.WriteLine("_Scene-InitScene-End");

            _ProcessingProgress = 100.0d;
        }

        //------------------------------------------------------------------------------
        private void StartProcessing()
        {
            Reader._progress = 0;// end reader progressbar
            _ProcessingProgress = 0;      // start processing progressbar

            _p = new fm_Processing();
            Trace.WriteLine("Finished-Reading");
            _Scene = new Scene(); 
            _Scene.DataSrc = this.Text;
            _VManage = new ViewManager();
            _Tr = new Thread(new ThreadStart(sceneInit));
            _p.Show();
            _Tr.Start();
            // //sceneInit();
        }


        //------------------------------------------------------------------------------
        private void StillProcessing()
        {
            if (_p != null)
            {
                if (!_p.IsDisposed)
                {
                    string[] surfdata = new string[2];
                    string[] modeldata = new string[5];
                    string[] octdata = new string[2];
                    lock (_Scene) 
                    {
                        lock(octdata)
                        {
                             _Scene.SimpleOctData(octdata);
                            lock(modeldata)
                            {        
                                _Scene.SimpleModelData(modeldata);
                                lock (surfdata)
                                {        
                                    _Scene.SimpleSurfaceData(surfdata);
                                    _p.UpDateLsts(surfdata, modeldata, octdata);
                                }  
                            }
                        }
   
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        private void finishedProcessing()
        { 
            if (_p != null)
            {
                _p.Close();
                _Tr.Join();
                _Tr = null;
                _p = null;
            }
        }

        //------------------------------------------------------------------------------
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_state == 0)
            {
                if (Reader._progress == 100)
                {
                    StartProcessing();
                }

                if (_ProcessingProgress == 100)
                {
                    finishedProcessing();
                    _state = 1;
                    InitGL();
                }
                else if (_ProcessingProgress < 99.0d)
                {
                    StillProcessing();
                }

                simpleOpenGlControl1.Refresh();
            }
        }


        #region KEYBOARD_CMDS
        //------------------------------------------------------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;
            bool ret = false;

            if (((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) && simpleOpenGlControl1.Focused)
            {
                switch(_state)
                {
                    case 0:
                    {
                       ret= ProcessCmdKey_0(keyData);
                    }
                    break;
                    case 1:
                    {
                      ret=  ProcessCmdKey_1(keyData);
                    }
                    break;

                }
            }
            if (ret)
            {
                simpleOpenGlControl1.Refresh();
                return ret;
            }else
                return base.ProcessCmdKey(ref msg, keyData);

        }
        
        //------------------------------------------------------------------------------
        private bool ProcessCmdKey_0(Keys keyData)
        {
              bool      ret = false;
            switch (keyData)
            {
                case Keys.B:
                {
                        ret = true;
                }
                break;   
            }
            return ret;
        }
        

        //------------------------------------------------------------------------------
        private bool ProcessCmdKey_1(Keys keyData)
        {
            bool ret = false;
            switch (keyData)
            {  
                case  Keys.B:
                {
                    _VManage.ChangeView();
                    ret = true;
                }
                break;   
                case Keys.P:
                {
                    int i = ((int)NMUD_OCTCELL.Value) + 1; 
                    ChangeOctCell(ref i);
                    NMUD_OCTCELL.Value = i;
                    OctreeData_Show(i);
                    ret = true;
                }  
                break;   
                case Keys.L:
                {
                    int i = ((int)NMUD_OCTCELL.Value) - 1;
                    ChangeOctCell(ref i);
                    NMUD_OCTCELL.Value = i;
                    OctreeData_Show(i);
                    ret = true;
                }
                break;
                case Keys.O:
                { 
                    _Scene.drawMode++;
                    Update_Drawmode(_Scene.drawMode);
                    ret = true;
                }
                break;
                case Keys.K:
                {
                    _Scene.drawMode--;
                    Update_Drawmode(_Scene.drawMode);  
                    ret = true;
                } 
                break;
                  
                //case Keys.I:
                //{
                //    int i = _Scene.getSurfacedrawID();
                //    i++;
                //    _Scene.SurfacedrawID(ref i);
                //    SurfData_Show(i);
                //    if(i!=-1)
                //        NM_SURFNUM.Value = i;
                //    else
                //        NM_SURFNUM.Value = 0;
                //    ret = true;
                //}
                //break;
                //case Keys.J:
                //{
                //    int i = _Scene.getSurfacedrawID();
                //    i--;
                //    _Scene.SurfacedrawID(ref i);
                //    SurfData_Show(i);
                    
                //    if (i != -1)
                //        NM_SURFNUM.Value = i;
                //    else
                //        NM_SURFNUM.Value = 0;

                //    ret = true; 
                //} 
                //break;  
                case Keys.C:
                { 
                    _VManage.StrafeRight(-_POS_INC);
                    ret = true;
                }
                break;
                case Keys.V:
                { 
                     _VManage.StrafeRight(_POS_INC);
                     ret = true;
                }    
                break;
                case Keys.PageUp:
                {            
                    _VManage.RotateZ(_ANG_INC);
                    ret = true;
                }    
                break;
                case Keys.PageDown:
                {    
                    _VManage.RotateZ(-_ANG_INC);
                    ret = true;
                }
                break;
                case Keys.NumPad2:
                {    
                    _VManage.MoveUpward(-_POS_INC);
                    ret = true;
                }
                break;
                case Keys.NumPad8: 
                {
                    _VManage.MoveUpward(_POS_INC);
                    ret = true;
                }
                break;
                case Keys.NumPad6:
                {   
                    _VManage.RotateX(_ANG_INC);
                    ret = true;
                }
                break;
                case Keys.NumPad4:
                {  
                    _VManage.RotateX(-_ANG_INC);
                    ret = true;
                }
                break; 
                case Keys.W:
                { 
                    _VManage.MoveForward(-_POS_INC);
                    ret = true;
                }
                break;
                case Keys.S:
                {   
                    _VManage.MoveForward(_POS_INC);
                    ret = true;
                }
                break;
                case Keys.D:
                {   
                    _VManage.RotateY(_ANG_INC);
                    ret = true;
                }
                break;
                case Keys.A:
                { 
                   _VManage.RotateY(-_ANG_INC);
                   ret = true;
                }
                break;
                  /*
                case Keys.R:
                { 
                    ret = true;
                }
                break;
              
                 * case Keys.Right:
                {
                    int i=_Scene.MoveAdjhighlight(1, 0, 0);
                    if ((_highlighted != i) && (i > -1))
                    {
                        Message(i.ToString());
                        OctreeData_Show(i);
                        _highlighted = i; 
                    }
                    ret = true;
                }
                break;
                case Keys.Left:
                {
                    int i = _Scene.MoveAdjhighlight(-1, 0, 0);
                    if ((_highlighted != i) && (i > -1))
                    {
                        Message(i.ToString());
                        OctreeData_Show(i);
                        _highlighted = i; 
                    }
                    ret = true;
                }
                break;
                case Keys.Up:
                {
                    int i = _Scene.MoveAdjhighlight(0, 1, 0);
                    if ((_highlighted != i) && (i > -1))
                    {
                        Message(i.ToString());
                        OctreeData_Show(i);
                        _highlighted = i; 
                    }
                    ret = true;
                }
                break;
                case Keys.Down:
                {
                    int i = _Scene.MoveAdjhighlight(0, -1, 0);
                    if ((_highlighted != i) && (i > -1))
                    {
                        Message(i.ToString());
                        OctreeData_Show(i);
                        _highlighted = i; 
                    }
                    ret = true;
                }
                break;**/
                case Keys.Right:
                {
                    _Scene.Move_OffesetOrtho(1.0, 0.0, 0.0);
                    ret = true;
                }
                break;
                case Keys.Left:
                {
                    _Scene.Move_OffesetOrtho(-1.0, 0.0, 0.0);
                    ret = true;
                }
                break;
                case Keys.Up:
                {
                    _Scene.Move_OffesetOrtho(0.0, 1.0, 0.0);
                    ret = true;
                }
                break;
                case Keys.Down:
                {
                    _Scene.Move_OffesetOrtho(0.0, -1.0, 0.0);
                    ret = true;
                }
                break; 
                case Keys.Home:
                {
                    int i = _Scene.MoveAdjhighlight(0, 0, 1);
                    if ((_hglghtOC != i) && (i > -1))
                    {
                        Message(i.ToString());
                        OctreeData_Show(i);
                        _hglghtOC = i; 
                    }
                    ret = true;
                }
                break;
                case Keys.End:
                {
                    int i = _Scene.MoveAdjhighlight(0, 0, -1);
                    if ((_hglghtOC != i) && (i > -1))
                    {
                        Message(i.ToString());
                        OctreeData_Show(i);
                        _hglghtOC = i; 
                    }
                    ret = true;
                }
                break;
                case Keys.OemMinus:
                {
                    _VManage.OthoPosDec();
                    ret = true;
                }
                break;
                case Keys.Oemplus:
                {
                    _VManage.OthoPosInc();
                    ret = true;
                }
                break;
            }
            tMsg.Text = _Scene.drawID.ToString(); 
            return ret;
        }

        #endregion
        #region MOUSE_FN

        //------------------------------------------------------------------------------
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mouseSelect = !_mouseSelect;
        }

        //------------------------------------------------------------------------------
        private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
        { 
                labelMouseButton.Text = String.Format("delta{0}", e.Delta); 
        }

        //------------------------------------------------------------------------------
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = String.Format("x={0}  y={1}", e.X, e.Y);
            labelMouseButton.Text = String.Format("MMoveButton{0}", e.Button );
            if (_state != 0)
            { 
                    if (_mouseSelect)
                    {
                        MouseSelectCell(e.X, e.Y);
                    }
                    simpleOpenGlControl1.Refresh();
            }
            
            Refresh();// update the contol boxes
        }

        //------------------------------------------------------------------------------
        private void MouseSelectCell(int x,int y)
        {
            int i = _Scene.screenRay(x, y);
            if ((_hglghtOC != i) && (i > -1))
            {
                Message(i.ToString());
                OctreeData_Show(i);
                _hglghtOC = i;
                simpleOpenGlControl1.Refresh();
            }
        }
        //------------------------------------------------------------------------------
        private void simpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = String.Format("x={0}  y={1}", e.X, e.Y);
            if(!_mouseSelect)
            {
                if (e.Button == MouseButtons.Right)
                {
                    MouseSelectCell(e.X,e.Y);
                }
            }
        }
        //------------------------------------------------------------------------------

        #endregion MOUSE_FN

        #region OPENGL
        //------------------------------------------------------------------------------
        // Initialise the Scenario
        public void InitGL()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            float[] lightPosition = { 1.0f, 1.0f, 1.0f, 0.0f };
            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_AMBIENT, Gl.GL_TRUE);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_CULL_FACE);
            _VManage.Init(simpleOpenGlControl1.Width, simpleOpenGlControl1.Height); 
            Vectors c,s; 
            PtData.ptCloud_avg(out s);
            PtData.ptCloudCtr(out c); 
            _VManage.Reposition(c,  s-c,new Vectors(0,0,1) );
            _Scene.drawMode = Scene.eDrawMode.eDM_LEAFSURFACE;
            OctreeData_Show(-1);
            int i = -1;
            SurfData_Show(ref i);
        }


        //------------------------------------------------------------------------------
        private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
        {
            Reshape(simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);
            simpleOpenGlControl1.Refresh();
        }

        //------------------------------------------------------------------------------
        // Display CallBack
        private void Display()
        {
            _VManage.Draw(_Scene);
        }

        //------------------------------------------------------------------------------
        // Reshape
        private void Reshape(int width, int height)
        {
            if (_state != 0)
            {
                if (width < 1)
                {
                    width = 1;
                }
                if (height < 1)
                {
                    height = 1;
                }
                _VManage.Reshape(width, height);
            }
            else
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }

        //------------------------------------------------------------------------------
        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            if (_state != 0)
            {
                Display();
            }
            else
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }

        #endregion


        #region TAB_CREATE 
        //------------------------------------------------------------------------------
        // C Page 1 Controls 
        private void BTN_Sphere_Click(object sender, EventArgs e)
        {
            if (PtData.ptCloud_Xcount() < 1)
            {
                PtData.genSphere(200.0d, 20.0d, 0.001d, _CREATE_POINTS);
                Reader._progress = 100;
            }
        }


        //------------------------------------------------------------------------------
        // 
        private void BTN_Elbow_Click(object sender, EventArgs e)
        {
            if (PtData.ptCloud_Xcount() < 1)
            {
                PtData.genElbow(200.0d, 20.0d, 0.001d, _CREATE_POINTS);
                Reader._progress = 100;
            }
        }


        //------------------------------------------------------------------------------
        // 
        private void BTN_CreateTori_Click(object sender, EventArgs e)
        { 
            if(PtData.ptCloud_Xcount()<1)
            {
                PtData.genTorus(200.0d, 20.0d, 0.001d, _CREATE_POINTS);
                Reader._progress = 100;
            }
        }


        //------------------------------------------------------------------------------
        // 
        private void BTN_Cylind_Click(object sender, EventArgs e)
        {
            if (PtData.ptCloud_Xcount() < 1)
            {
                PtData.genCylinder(200.0d, 20.0d, 0.001d, _CREATE_POINTS);
                Reader._progress = 100;
            }
        }

        //------------------------------------------------------------------------------
        //  
        private void BTN_Cone_Click(object sender, EventArgs e)
        {
            if (PtData.ptCloud_Xcount() < 1)
            {
                PtData.genCone(200.0d, 20.0d,15.0d, 0.001d, _CREATE_POINTS);
                Reader._progress = 100;
            }
        }

        //------------------------------------------------------------------------------
        // 
        private void TXTBX_CellPts_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            try
            {
                
                i = int.Parse(TXTBX_CellPts.Text);
                _CREATE_POINTS = i;
                TXTBX_CellPts.Text = _CREATE_POINTS.ToString();
            }
            catch
            {
                this.TXTBX_CellPts.Text = _CREATE_POINTS.ToString();
            }
        }


        //------------------------------------------------------------------------------
        // 
        private void BTN_Clear_Click_1(object sender, EventArgs e)
        {

            if (_state > 0)
            {
                _state = 0;
                Reader._progress = 0;
                _ProcessingProgress = 0;
                _Scene = null;
                _VManage = null;
                OctreeNode._CUROCTDEPTH = -1;
                PtData.Clear();
                PtData.Initialize();
                _Scene = new Scene();
                _VManage = new ViewManager();
            }

        } 
        #endregion 

        #region TAB_OCTREE
        private void OctreeData_Show(int id)
        {
            if (LST_BX_Octtree.Items.Count > 0)
                LST_BX_Octtree.Items.Clear();
            string[] octdata;
            _Scene.OctreeData(out octdata,id);
            LST_BX_Octtree.Items.AddRange(octdata);
        }


        private void NMUD_OCTCELL_ValueChanged(object sender, EventArgs e)
        {  
            if (_Scene!=null)
            {
                if (NMUD_OCTCELL.Value != _Scene.drawID)
                {

                    int i = (int)NMUD_OCTCELL.Value;
                    ChangeOctCell(ref i);
                    NMUD_OCTCELL.Value = i;
                    Display();
                    simpleOpenGlControl1.Refresh();
                }
            }
        }

        private void LST_BX_Octtree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string message = "Print out point data for cell?";
            string caption = "File Output";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox. 
            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            { 
                _Scene.WriteOctreeData("test.txt");

            }
        }

        private void LST_BX_Octtree_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                ListBox objListBox = (ListBox)sender;
                int itemIndex = -1;

                if (LST_BX_Octtree.Items.Count > 0)
                {
                    if (objListBox.ItemHeight != 0)
                    {
                        itemIndex = e.Y / objListBox.ItemHeight;
                        itemIndex += objListBox.TopIndex;
                    }
                    if ((itemIndex >= 0) && (itemIndex < LST_BX_Octtree.Items.Count))
                    {
                        ListToolTip.SetToolTip(objListBox, LST_BX_Octtree.Items[itemIndex].ToString());
                    }
                    else
                    {
                        ListToolTip.Hide(objListBox);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
        }
           

        #endregion
        #region TAB_SURF

        private void SurfData_Show(ref int id)
        {
            if (LST_BX_SURFDATA.Items.Count > 0)
                LST_BX_SURFDATA.Items.Clear();

            string[] surfdata;
            _Scene.SurfaceData(out surfdata,ref id);
            LST_BX_SURFDATA.Items.AddRange(surfdata);
        }

        private void NMUD_SURFNUM_ValueChanged(object sender, EventArgs e)
        {
            if (_Scene != null)
            {
                if (NMUD_SURFNUM.Value != _Scene.getSurfaceSel())
                {

                    int i = (int)NMUD_SURFNUM.Value;
                    SurfData_Show(ref i);
                    NMUD_SURFNUM.Value = i; 
                }
            }

        }
  

        private void LST_BX_SURFDATA_MouseDoubleClick(object sender, MouseEventArgs e)
        { 
        }

        private void LST_BX_SURFDATA_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                ListBox objListBox = (ListBox)sender;
                int itemIndex = -1;

                if (LST_BX_SURFDATA.Items.Count > 0)
                {
                    if (objListBox.ItemHeight != 0)
                    {
                        itemIndex = e.Y / objListBox.ItemHeight;
                        itemIndex += objListBox.TopIndex;
                    }
                    if ((itemIndex >= 0) && (itemIndex < LST_BX_SURFDATA.Items.Count))
                    {
                        ListToolTip.SetToolTip(objListBox, LST_BX_SURFDATA.Items[itemIndex].ToString());
                    }
                    else
                    {
                        ListToolTip.Hide(objListBox);
                    } 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
        }

        #endregion
        #region TAB_RANSAC

        //------------------------------------------------------------------------------
        // 
        private void TXTBX_radii_TextChanged(object sender, EventArgs e)
        {

            double i = 0.0d;
            try
            {
                if (TXTBX_radii.Text != "")
                {
                    i = double.Parse(TXTBX_radii.Text);
                    ModelBase.set_PCNTERROR(i);
                }
                else
                { 
                      TXTBX_radii.Text = ModelBase.get_PCNTERROR().ToString();
                }

            }
            catch
            {
                  TXTBX_radii.Text = ModelBase.get_PCNTERROR().ToString();
            }

        }

        //------------------------------------------------------------------------------
        // 
        private void TXTBX_Flatness_TextChanged(object sender, EventArgs e)
        {

            double i = 0.0d;
            try
            {
                if (TXTBX_Flatness.Text != "")
                {
                    i = double.Parse(TXTBX_Flatness.Text);
                    OctreeNode._FLATNESS = i;
                }
                else
                {
                      TXTBX_Flatness.Text = OctreeNode._FLATNESS.ToString();
                }
            }
            catch
            {
                  TXTBX_Flatness.Text = OctreeNode._FLATNESS.ToString();
            }
        }
        
        //------------------------------------------------------------------------------
        // 
        private void TXTBX_MaxIterRansac_TextChanged(object sender, EventArgs e)
        {

            int i = 0;
            try
            {
                if (TXTBX_MaxIterRansac.Text != "")
                {
                    i = int.Parse(TXTBX_MaxIterRansac.Text);
                    SurfaceController._RANSACPNUM = i;
                }
                else
                {
                      TXTBX_MaxIterRansac.Text = SurfaceController._RANSACPNUM.ToString();
                }
            }
            catch
            {
                  TXTBX_MaxIterRansac.Text = SurfaceController._RANSACPNUM.ToString();
            }
        }

        //------------------------------------------------------------------------------
        //    
        private void NMUD_OCTLVLMAX_ValueChanged_1(object sender, EventArgs e)
        {
            NumericUpDown nm = (NumericUpDown)sender;
            if (nm != null)
            {
                OctreeNode._MAXDEPTH = (int)nm.Value;
            }
        }

        private void NMUD_MINSURF_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nm = (NumericUpDown)sender;
            if (nm != null)
            {
                SurfaceController._MINNODESURF = (int)nm.Value;
            }
        }

        private void NMUD_MINPTCELL_ValueChanged(object sender, EventArgs e)
        {

            NumericUpDown nm = (NumericUpDown)sender;
            if (nm != null)
            {
                OctreeNode._MINPTS = (int)nm.Value;
            }
        }

        #endregion

        #region TAB_MODEL

        private void ModelData_Show(ref int id)
        {
            if (LST_BX_MODELDATA.Items.Count > 0)
                LST_BX_MODELDATA.Items.Clear();

            string[] modeldata;
            _Scene.ModelData(out modeldata, ref id);
            LST_BX_MODELDATA.Items.AddRange(modeldata);
        }

        private void NMUD_ModelData_ValueChanged(object sender, EventArgs e)
        {
            if (_Scene != null)
            {
                if (NMUD_ModelData.Value != _Scene.getModelSel())
                {

                    int i = (int)NMUD_ModelData.Value;
                   
                    ModelData_Show(ref i);
                    NMUD_ModelData.Value = i; 
                }
            }
        }

        #endregion

        #region STATUSBAR


        #endregion

        #region ToolStripMenu

        //------------------------------------------------------------------------------
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "tda.xyz.bin";
            openFileDialog1.Filter = "Binary PtCloud file | *.bin|PtClouds| *.xyz|PtCloudsCSV| *.CSV|wavefront(OBJ)| *.obj|All Files| *.*";
            openFileDialog1.ShowDialog();
        }

        //------------------------------------------------------------------------------
        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Binary PtCloud file(Bin) | *.bin|PtClouds(XYZ)| *.xyz|All Files| *.*";
            saveFileDialog1.ShowDialog();
        }

        //------------------------------------------------------------------------------
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the dat in the appropriate Format based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        Reader.Write_BIN(saveFileDialog1.FileName);
                        break;
                    case 2:
                        Reader.Write_XYZ(saveFileDialog1.FileName);
                        break;
                    case 3:
                        break;
                }
            }
        }


        //------------------------------------------------------------------------------
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.Text = openFileDialog1.FileName;
            this._filename = openFileDialog1.FileName;
            Fm_Loading f3 = new Fm_Loading();
            f3.FileName = _filename;
            f3.Show();
        }

        //------------------------------------------------------------------------------
        private void colourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _c1 = colorDialog1.Color;
                _Scene.SurfacedrawColour((double)_c1.R / byte.MaxValue, (double)_c1.G / byte.MaxValue, (double)_c1.B / byte.MaxValue);
            }
        }

        //------------------------------------------------------------------------------
        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fm_Keyinc k = new fm_Keyinc(); 
            k.fromInc(_POS_INC,_ANG_INC);
            k.ShowDialog();
            _ANG_INC =  k.ANGINC;
            _POS_INC =  k.POSINC;
        }

        //------------------------------------------------------------------------------
        private void octreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDrawMode(Scene.eDrawMode.eDM_BRANCH_BB); 
        }

        //------------------------------------------------------------------------------
        private void pointsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SetNewDrawMode(Scene.eDrawMode.eDM_BRANCH_PTS);
                     
        }

        //------------------------------------------------------------------------------
        private void branchCtrToolStripMenuItem_Click(object sender, EventArgs e)
        {
 
            SetNewDrawMode(Scene.eDrawMode.eDM_CENTERS); 
             
        }

        //------------------------------------------------------------------------------
        private void pointsOctToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDrawMode(Scene.eDrawMode.eDM_BRANCH_BBPTS); 
        }

        //------------------------------------------------------------------------------
        private void leafCtrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDrawMode(Scene.eDrawMode.eDM_LEAFCENTERS);
 
        }

        //------------------------------------------------------------------------------
        private void leafCntdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDrawMode(Scene.eDrawMode.eDM_LEAFSIGMA); 
        }

        //------------------------------------------------------------------------------
        private void surfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNewDrawMode(Scene.eDrawMode.eDM_LEAFSURFACE);
        }

        //------------------------------------------------------------------------------
        private void modelToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SetNewDrawMode(Scene.eDrawMode.eDM_MODELALL);
        }

        //------------------------------------------------------------------------------
        private void numberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (_state != 0)
            {
  
                string[] Surfdata;
                _Scene.GetAllModelData(out Surfdata);
                if (Surfdata.GetLength(0) > 0)
                {
                    DLG_SHowSurface dlg = new DLG_SHowSurface();
                    dlg.addsurfacedata(Surfdata);
                    DialogResult dr = new DialogResult();
                    dr = dlg.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        List<int> draw = new List<int>();
                        dlg.getCheckedModels(ref draw);
                        if (draw.Count > 0)
                        {

                            SetNewDrawMode(Scene.eDrawMode.eDM_MODELNUMS);
                            // get the selected surfaces
                            _Scene.DrawModelNums(draw,false,-1);
                        }
                    }
                }
            }

        }

        //------------------------------------------------------------------------------
        private void allPlanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != 0)
            {

                SetNewDrawMode(Scene.eDrawMode.eDM_MODELPLANES);
                // get the selected surfaces
                List<int> draw = null;
                _Scene.DrawModelNums(draw, true, (int)Scene.eDrawMode.eDM_MODELPLANES);
            }
        }

        //------------------------------------------------------------------------------
        private void allCylindersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != 0)
            {

                SetNewDrawMode(Scene.eDrawMode.eDM_MODELCYLINDER);
                // get the selected surfaces
                List<int> draw = null;
                _Scene.DrawModelNums(draw, true, (int)Scene.eDrawMode.eDM_MODELCYLINDER);

            }
        }

        //------------------------------------------------------------------------------
        private void allTorusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != 0)
            {

                SetNewDrawMode(Scene.eDrawMode.eDM_MODELTORUS);
                // get the selected surfaces
                List<int> draw = null;
                _Scene.DrawModelNums(draw, true, (int)Scene.eDrawMode.eDM_MODELTORUS);

            }

        }

        //------------------------------------------------------------------------------
        private void allSphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != 0)
            {

                SetNewDrawMode(Scene.eDrawMode.eDM_MODELSPHERE);
                // get the selected surfaces
                List<int> draw = null;
                _Scene.DrawModelNums(draw, true, (int)Scene.eDrawMode.eDM_MODELSPHERE);

            }

        }

        //------------------------------------------------------------------------------
        private void SetNewDrawMode(Scene.eDrawMode dm)
        { 
            if (_state != 0)
            {
 
                if (_Scene.drawMode != dm)
                {
                    Update_Drawmode(dm);
                    Refresh();// update the contol boxes
                    _Scene.drawMode = dm;
                    Display();
                    simpleOpenGlControl1.Refresh();
                }  
            }
        }

        //------------------------------------------------------------------------------
        private void ResetDrawToolStripMenuItem()
        { 
            surfToolStripMenuItem.Checked = false;
            pointsToolStripMenuItem.Checked = false;
            octreeToolStripMenuItem.Checked = false;
            pointsOctToolStripMenuItem.Checked = false;
            leafCtrToolStripMenuItem.Checked = false;
            leafCntdToolStripMenuItem.Checked = false;
            branchCtrToolStripMenuItem.Checked = false;
            modelToolStripMenuItem.Checked = false;
        }

        //------------------------------------------------------------------------------
        private void Update_Drawmode(Scene.eDrawMode newMode)
        {
            ResetDrawToolStripMenuItem();
            switch (newMode)
            {
                case (Scene.eDrawMode.eDM_BRANCH_BB):
                    {
                        octreeToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_BRANCH_BBPTS):
                    {
                        pointsOctToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_CENTERS):
                    {
                        branchCtrToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_LEAFCENTERS):
                    {
                        leafCtrToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_LEAFSIGMA):
                    {
                        leafCntdToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_LEAFSURFACE):
                    {
                        surfToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_BRANCH_PTS):
                    {
                        pointsToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_MODELALL):
                    {
                        modelToolStripMenuItem.Checked = true;
                    }
                    break;
                case (Scene.eDrawMode.eDM_MODELNUMS):
                    {
                        modelToolStripMenuItem.Checked = true;
                    }
                    break;
            }

        }

        //------------------------------------------------------------------------------
        public void Message(string msg)
        {
            this.toolStripStatusLabel1.Text = msg;
        }

        //------------------------------------------------------------------------------
        private void toolStripSaveScn_Click(object sender, EventArgs e)
        {
            saveFileDialog2.Filter = "Scene file(XML) | *.xml|All Files| *.*";
            saveFileDialog2.ShowDialog();
        }

        //------------------------------------------------------------------------------
        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog2.FileName != "")
            {
                // Saves the dat in the appropriate Format based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog2.FilterIndex)
                {
                    case 1:
                        Reader.Write_XMLSCN(saveFileDialog2.FileName,_Scene);
                        break;
                    case 2: 
                        break; 
                }
            }
        }

        //------------------------------------------------------------------------------
        private void loadSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.FileName = "Scn.xml";
            openFileDialog2.Filter = "Scene xml file | *.xml|All Files| *.*";
            openFileDialog2.ShowDialog();
        }
        //------------------------------------------------------------------------------
        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            _loadScene = true;

            this.Text = openFileDialog2.FileName;
            this._filename = openFileDialog2.FileName;
            System.Xml.XmlTextReader r = Reader.Read_XMLSCN(_filename);
            _Scene = new Scene();
            _Scene.ReadXmlData(ref r);
            Reader.Read(_Scene.DataSrc, false);
        }

        //------------------------------------------------------------------------------
        private void compareSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion ToolStripMenu

   




 














    }
}
