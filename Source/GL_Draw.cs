using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Model;
using MathsLib;

namespace Render
{
    static class GL_Draw
    {

        //-----------------------------------------------------------------------------
        static public int ptCloudDisplaylist()
        {
            int ptList;
            float[] Ambient1;
            float[] Ambient2;
            Vectors min, max;
            Colour.Deep_Pink().tofloat3f(out Ambient1);
            Colour.Dark_Orange().tofloat3f(out Ambient2);
            ptList = Gl.glGenLists(1);
            Gl.glNewList(ptList, Gl.GL_COMPILE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LINE_SMOOTH_HINT);
            PtData.ptCloud_min(out min);
            PtData.ptCloud_max(out max);
            drawBBCube(min._X, max._X,min._Y, max._Y,  min._Z, max._Z, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Gl.glDisable(Gl.GL_LINE_SMOOTH_HINT);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEndList();
            return ptList;
        }

        //-----------------------------------------------------------------------------
        static private void GLdrawPtsCloud()
        {
            double x=0.0d, y=0.0d, z=0.0d;
            Gl.glPointSize(10.0f);
            Gl.glBegin(Gl.GL_POINTS); 
            int pt = 0;
            for (; pt < PtData.ptCloud_Xcount(); pt++)
            {
                PtData.Pt(pt, ref x, ref y, ref z);
                Gl.glColor3f(1.0f, 1.0f, 1.0f);
                Gl.glVertex3d(x,y,z);
            }
            Gl.glEnd();
        }
        //-----------------------------------------------------------------------------
        static public void GLBBCube(Vectors Min, Vectors Max, Colour C)
        {

            drawBBCube(Min._X, Max._X, Min._Y, Max._Y, Min._Z, Max._Z, C.at(Colour.R), C.at(Colour.G), C.at(Colour.B));
        }

        //-----------------------------------------------------------------------------
        static private void drawBBCube(double minX, double maxX, double minY, double maxY, double minZ, double maxZ, double R, double G, double B)
        { 
            Gl.glDisable(Gl.GL_LIGHTING);
            // bottom square
            Gl.glBegin(Gl.GL_LINE_LOOP);
                Gl.glColor3d(R, G, B);
                Gl.glVertex3d(minX, minY, minZ);
                Gl.glVertex3d(minX, maxY, minZ);
                Gl.glVertex3d(maxX, maxY, minZ);
                Gl.glVertex3d(maxX, minY, minZ);
                Gl.glVertex3d(minX, minY, minZ);
            Gl.glEnd();
            // top square
            Gl.glBegin(Gl.GL_LINE_LOOP);
                Gl.glColor3d(R, G, B);
                Gl.glVertex3d(minX, minY, maxZ);
                Gl.glVertex3d(minX, maxY, maxZ);
                Gl.glVertex3d(maxX, maxY, maxZ);
                Gl.glVertex3d(maxX, minY, maxZ);
                Gl.glVertex3d(minX, minY, maxZ);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(R, G, B);
                Gl.glVertex3d(minX, minY, minZ);
                Gl.glVertex3d(minX, minY, maxZ);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(R, G, B);
                Gl.glVertex3d(maxX, maxY, minZ);
                Gl.glVertex3d(maxX, maxY, maxZ);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(R, G, B);
                Gl.glVertex3d(minX, maxY, minZ);
                Gl.glVertex3d(minX, maxY, maxZ);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(R, G, B);
                Gl.glVertex3d(maxX, minY, minZ);
                Gl.glVertex3d(maxX, minY, maxZ);
           Gl.glEnd();
           Gl.glEnable(Gl.GL_LIGHTING);
        }

        //-----------------------------------------------------------------------------
        static public void glLine(Vectors from, Vectors to, Colour c1, Colour c2)
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LINE_SMOOTH_HINT);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3d(c1._R, c1._G, c1._B);
            Gl.glVertex3d(from._X, from._Y, from._Z);
            Gl.glColor3d(c2._R, c2._G, c2._B);
            Gl.glVertex3d(to._X, to._Y, to._Z);
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_SMOOTH_HINT);
            Gl.glEnable(Gl.GL_LIGHTING);
        }
    }
}
