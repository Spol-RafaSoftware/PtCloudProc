//------------------------------------------------------------------------------
// using definition
using System;
using Tao.OpenGl; 
using Model;
using MathsLib; 

//------------------------------------------------------------------------------
// namespace definition
namespace Render
{
    //------------------------------------------------------------------------------
    // class definition
    abstract class view
    { 
        //------------------------------------------------------------------------------
        // Consts and static data for class 
        static protected Camera _Cam;
        static protected int _width;
        static protected int _height;
        static protected double _pos; 

        //------------------------------------------------------------------------------
        // Constructor
        static view()
        {
             _Cam = new Camera();
             _pos = 1.0d; 
        }

        public static void ViewSave()
        {
            _Cam.SaveRestore = 1;
        }

        public static void ViewRestore()
        { 
            _Cam.SaveRestore = 0;
        }

        public abstract void Init(); 
        public abstract void Draw(Scene s);  
        protected abstract void Projection();
 
        //-----------------------------------------------------------------------------
        static public void Move(Vectors Direction)
        {
            _Cam.Move(Direction);
        }
        //-----------------------------------------------------------------------------
        static public void Reposition(Vectors pos, Vectors VD, Vectors Up)
        {
            _Cam.Reposition(pos, VD,Up);
        }


        //-----------------------------------------------------------------------------
        static public void RotateX(double Angle)
        {
            _Cam.RotateX(Angle);
        }

        //-----------------------------------------------------------------------------
        static public void RotateY(double Angle)
        {
            _Cam.RotateY(Angle);
        }

        //-----------------------------------------------------------------------------
        static public void RotateZ(double Angle)
        {
            _Cam.RotateZ(Angle);
        }

        //-----------------------------------------------------------------------------
        static public void Draw() 
        {
            _Cam.Draw();
        }

        //-----------------------------------------------------------------------------
        static public void MoveForward(double Distance)
        {
            _Cam.MoveForward(Distance);
        }

        //-----------------------------------------------------------------------------
        static public void StrafeRight(double Distance)
        {
            _Cam.StrafeRight(Distance);
        }

        //-----------------------------------------------------------------------------
        static public void MoveUpward(double Distance)
        {
            _Cam.MoveUpward(Distance);
        }
        

        //-----------------------------------------------------------------------------
        static public int Width
        {
            set
            {
                _width = value;
            }
            get
            {
                return _width;
            } 
        }


        //-----------------------------------------------------------------------------
        static public int Height
        {
            set
            {
                _height = value;
            } 
            get
            {
                return _height;
            } 
        }
        //-----------------------------------------------------------------------------
        static public double OrthoPos
        {
            set
            {
                _pos = value;
            }
            get
            {
                return _pos; 
            }
        }



    }
}
