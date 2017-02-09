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
    class ViewManager
    {

        //------------------------------------------------------------------------------
        private int _View;
        private view[] _Views; 
        //------------------------------------------------------------------------------
        // CTor
        public ViewManager()
        {
            view.Height = 1;
            view.Width = 1;
            _View = 0;
            _Views = new view[2]; 
            _Views[0] = new ViewPersp();
            _Views[1] = new viewQuad();

        }

        //------------------------------------------------------------------------------
        // ChangeView 
        public void CamMove( Vectors move)
        {
            view.Move(move);
        }
        //------------------------------------------------------------------------------
        public void Reposition(Vectors pos, Vectors VD, Vectors Up)
        {
            view.Reposition(pos,VD,Up);
        } 
        //------------------------------------------------------------------------------
        public void ChangeView()
        {  
            if (_View == 1)
            {
                _View = 0;
                view.ViewRestore();
            }
            else
            { 
                _View = 1;
                view.ViewSave();
            } 
        }

        //------------------------------------------------------------------------------
        // Init all views
        public void Init(int width, int height) 
        {
            view.Height = height;
            view.Width = width;
            foreach(view v in _Views )
            {
                v.Init();
            }
        }
         

        //------------------------------------------------------------------------------
        // Draw
        public void Draw(Scene s)
        {
            _Views[_View].Draw(s);
        }

        //------------------------------------------------------------------------------
        // Reshape
        public void Reshape(int width, int height)
        {
            view.Height = height;
            view.Width = width; 
        }

        //------------------------------------------------------------------------------
        // RotateX Camera 
        public void RotateX (double Angle)
        {
            view.RotateX(Angle);        	
        }

        //------------------------------------------------------------------------------
        // RotateY Camera 
        public void RotateY (double Angle)
        {
            view.RotateY(Angle);
        }

        //------------------------------------------------------------------------------
        // RotateZ Camera 
        public void RotateZ(double Angle)
        {
            view.RotateZ(Angle);
        }

        //------------------------------------------------------------------------------
        // MoveForward Camera 
        public void MoveForward(double Distance)
        {
            view.MoveForward(Distance);
        }

        //------------------------------------------------------------------------------
        // StrafeRight Camera 
        public void StrafeRight(double Distance)
        {
            view.StrafeRight(Distance);
        }

        //------------------------------------------------------------------------------
        // MoveUpward Camera 
        public void MoveUpward(double Distance)
        {
            view.MoveUpward(Distance);
        }

        //------------------------------------------------------------------------------
        // Move  Camera 
        public void OthoPosInc()
        {
            view.OrthoPos/=2;
        }
        //------------------------------------------------------------------------------
        // Move  Camera 
        public void OthoPosDec()
        {
            view.OrthoPos*=2;
        }
    }
}
