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
    class viewQuad: view
    {
        private int _perspective;
        private Vectors _cen;
        private Vectors _max;
        private double _radius;
        private Vectors _cmPos;
        private Vectors _cmUp;
        private Vectors _cmDir;

        //------------------------------------------------------------------------------
        // Init
        public override void Init()
        {  
            _cmPos = new Vectors();
            _cmUp  = new Vectors();
            _cmDir = new Vectors();
            _cen = new Vectors();
        }
        //-----------------------------------------------------------------------------
        protected override void Projection()
        {
            double ratio = (double)view._width /(double) view._height;
 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            if (_perspective > 0)
            {
                Glu.gluPerspective(60, ratio, 1, 100000);
            }
            else
            {
                double left = -_cen._X - (_radius * 2.0d  );
                double right =-_cen._X + (_radius * 2.0d  );

                double bottom = -_cen._Y - (_radius * 2.0d );
                double top = -_cen._Y + (_radius * 2.0d  );

                double znear = -_cen._Z - (_radius * 2.0d  );
                double zfar = -_cen._Z + (_radius * 2.0d );

                //double left =  - _radius;
                //double right =  _radius; 
                //double bottom =  - _radius;
                //double top =  _radius;
                if (ratio < 1.0)
                { // window taller than wide
                    bottom /= ratio;
                   top /= ratio;
               }
               else
               {
                   left *= ratio;
                   right *= ratio;
               }

                Gl.glOrtho(left, right, bottom, top,znear,zfar); 
            }
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity(); 
            Draw();
        } 
        
        private int Perspective
        { 
          set
          {
              _perspective = value;
              if (value == 0)
              {
                  // not using perspective we need to 
                  // store cam data
                  _cmPos = view._Cam.vPosition;
                  _cmUp = view._Cam.vUpVector;
                  _cmDir = view._Cam.vViewDir;
                  view._Cam.Reposition(_cen, null,null);
                  view._Cam.vViewDir=new Vectors(-1.0d,0.0d, 0.0d);// = _max - _cen;
                  view._Cam.vUpVector=new Vectors(0.0d,1.0d, 0.0d);
              }
              else
              {

                   view._Cam.vPosition= _cmPos;
                   view._Cam.vUpVector = _cmUp;
                   view._Cam.vViewDir= _cmDir;
              }
          }
        }

        //------------------------------------------------------------------------------
        // Draw
        public override void Draw(Scene s)
        {
            { 
                int width = view._width; 
                int height = view._height; 
                Vectors a;
                s.getViewData(out _max, out _cen, out a, ref _radius);
                Gl.glViewport(0, 0, width, height);
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();
                Glu.gluOrtho2D(0, width, 0, height);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glColor3ub(255, 255, 255);
                Gl.glBegin(Gl.GL_LINES);
                    Gl.glVertex2i(width / 2, 0);
                    Gl.glVertex2i(width / 2, height);
                    Gl.glVertex2i(0, height / 2);
                    Gl.glVertex2i(width, height / 2);
                Gl.glEnd();
                Gl.glEnable(Gl.GL_LIGHTING);

                width = (width + 1) / 2;
                height = (height + 1) / 2;

                Gl.glEnable(Gl.GL_SCISSOR_TEST);

                // Bottom Left SCREEN
                Gl.glViewport(0, 0, width, height); 
                Gl.glScissor(0, 0, width, height);
                // Front
                Perspective = 0;
                Projection();

                //Draw Scene for Front Viewport  
                s.DrawNotRotated(); 

                // Bottom Right
                Gl.glViewport(width, 0, width, height);
                Gl.glScissor(width, 0, width, height);

                // Right 
                Projection();
                Gl.glPushMatrix();
                Gl.glRotated(-90.0d, 0.0d, 1.0d, 0.0d);
                //Draw Scene for Right Viewport   
                s.DrawNotRotated();
                Gl.glPopMatrix(); 

                // Top Left Screen
                Gl.glViewport(0, height, width, height);
                Gl.glScissor(0, height, width, height);

                // Top 
                Projection();
                Gl.glPushMatrix(); 
                    Gl.glRotatef(90.0f, 1.0f, 0.0f, 0.0f); 
                    //Draw Scene for Top Viewport   
                    s.DrawNotRotated();
                Gl.glPopMatrix();
                

                // Top Right Screen
                Gl.glViewport(width, height, width, height);
                Gl.glScissor(width, height, width, height);

                // Perspective
                Perspective = 1;
                Projection();
                Gl.glPushMatrix(); 
                    Gl.glRotatef(30.0f, 0.0f, 1.0f, 0.0f);
                    Gl.glRotatef(20.0f, 1.0f, 0.0f, 0.0f); 
                    //Draw Scene for Perspective Viewport  
                    s.Draw();
                Gl.glPopMatrix();

                Gl.glDisable(Gl.GL_SCISSOR_TEST);

                 
            }
        }

    }
}
