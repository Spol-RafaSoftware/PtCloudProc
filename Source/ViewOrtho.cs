//------------------------------------------------------------------------------
// using definition
using System;
using Tao.OpenGl;
using MathsLib;
using Model; 

//------------------------------------------------------------------------------
// namespace definition
namespace Render
{
    //------------------------------------------------------------------------------
    // class definition
    class ViewOrtho : view
    {

        //------------------------------------------------------------------------------
        // Init
        public override void Init()
        { 
        }

        //-----------------------------------------------------------------------------
        protected override void Projection()
        {
            double ratio = (double)view._width / (double)view._height;

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity(); 
            Gl.glOrtho(-ratio, ratio, -ratio, ratio, 1, 100000); 
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Draw();
        } 

        //------------------------------------------------------------------------------
        // Draw
        public override void Draw(Scene s)
        {   
            Gl.glViewport(0, 0, _width, _height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(0, _width, 0, _height);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT); 


            // Perspective
            Projection();
            Gl.glRotatef(30.0f, 0.0f, 1.0f, 0.0f);
            Gl.glRotatef(20.0f, 1.0f, 0.0f, 0.0f); 

            // CallList
            s.Draw();
 
        }

     
    }
}
