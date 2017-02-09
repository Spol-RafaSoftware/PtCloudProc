using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathsLib;
using DEFINES;

namespace Render
{
    public static class TrackBall
    {
        private static double _RadiusTrackball;
        private static Vectors _StartPtTrackball = new Vectors();
        private static Vectors _EndPtTrackball = new Vectors();
        private static long _XCenterTrackball = 0;
        private static long _YCenterTrackball = 0;
        private static Camera gCamera;  
        // single set of interaction flags and states
        private static int[] gDollyPanStartPoint = new int[] { 0, 0 };
        public static double[] gTrackBallRotation = new double[] { 0.0f, 0.0f, 0.0f, 0.0f };
        public static bool gDolly = false;
        public static bool gPan = false;
        public static bool gTrackball = false;
        
        static public void setCamera(Camera C)
        {
            gCamera = C;
        }
        // ---------------------------------
        static private void mouseDolly(int x, int y)
        {
            double newz = gCamera.vPosition._Z;
            double dolly = (gDollyPanStartPoint[1] - y) * -newz / 300.0f;
            newz += dolly;
            if ((newz < CONST.EPSILON) && (newz > -CONST.EPSILON)) // do not let z = 0.0
                newz = 0.0001;
            gDollyPanStartPoint[0] = x;
            gDollyPanStartPoint[1] = y;

            gCamera.vPosition._Z = newz;
        }

        // ---------------------------------
        // move camera in x/y plane
        static private void mousePan(int x, int y)
        {
            double panX = (gDollyPanStartPoint[0] - x) / (900.0f / -gCamera.vPosition._Z);
            double panY = (gDollyPanStartPoint[1] - y) / (900.0f / -gCamera.vPosition._Z);
            gCamera.vPosition._X -= panX;
            gCamera.vPosition._Y -= panY;
            gDollyPanStartPoint[0] = x;
            gDollyPanStartPoint[1] = y;
        }


        //-----------------------------------------------------------------------------
        static public void mouseDown(int mousex, int mousey) // trackball
        {
            if (gDolly ) // send to pan
                MouseDollyStart(mousex, mousey);
            else if (gPan) // send to dolly
                MousePanStart(mousex, mousey);
            else
            {
                // y = camera.viewHeight - y;
                gDolly = false; // no dolly
                gPan = false; // no pan
                gTrackball = true;
                TrackBall.startTrackball(mousex, mousey, 0, 0,view.Width,view.Height);
            }
        }

        static public void mouseDragged(int button,int x, int y)
        {
	        //y = camera.viewHeight - y;
            if (button > 0)
            {
                if (TrackBall.gTrackball)
                {
                    TrackBall.rollToTrackball(x, y, ref gTrackBallRotation);
                }
                else if (TrackBall.gDolly)
                {
                    mouseDolly(x, y); 
                }
                else if (TrackBall.gPan)
                {
                    mousePan(x, y);
                }
            }
        }
 
        // ---------------------------------
        static private void  MousePanStart(int x, int y)
        {
            //MOUSE_END TRACK BALL
            MouseEndTrackBall(  x,   y);
            gDolly = false; // no dolly
            gPan = true;
            gTrackball = false; // no trackball
            gDollyPanStartPoint[0] = x;
            gDollyPanStartPoint[1] = y;
        }
        
        // ---------------------------------
        static private void MouseEndTrackBall(int x, int y)
        {
            // y = camera.viewHeight - y;
            if (gTrackball)
            { // if we are currently tracking, end trackball
                if (gTrackBallRotation[0] != 0.0)
                    TrackBall.addToRotationTrackball(gTrackBallRotation, view.worldRotation);
                gTrackBallRotation[0] = gTrackBallRotation[1] = gTrackBallRotation[2] = gTrackBallRotation[3] = 0.0f;
            }
        }

        // ---------------------------------
        static private void MouseDollyStart(int x, int y)
        {
            //MOUSE_END TRACK BALL
            MouseEndTrackBall(x, y);
            gDolly = true;
            gPan = false; // no pan
            gTrackball = false; // no trackball
            gDollyPanStartPoint[0] = x;
            gDollyPanStartPoint[1] = y;
        }

        // ---------------------------------
        static public void mouseUp()
        {
            if (gDolly)
            { // end dolly
                gDolly = false;
            }
            else if (gPan)
            { // end pan
                gPan = false;
            }
            else if (gTrackball)
            { // end trackball
                gTrackball = false;
                if (gTrackBallRotation[0] != 0.0)
                    TrackBall.addToRotationTrackball(gTrackBallRotation, view.worldRotation);
                gTrackBallRotation[0] = gTrackBallRotation[1] = gTrackBallRotation[2] = gTrackBallRotation[3] = 0.0f;
            }
        }

    


        //------------------------------------------------------
        // mouse positon and view size as inputs
        private static void startTrackball(long x, long y, long originX, long originY, long width, long height)
        {
            double xxyy;
            double nx, ny; 
            nx = width;
            ny = height;
            if (nx > ny)
                _RadiusTrackball = (double)ny * 0.5d;
            else
                _RadiusTrackball = (double)nx * 0.5d; 
            _XCenterTrackball = (long)(originX + width * 0.5d);
            _YCenterTrackball = (long)(originY + height * 0.5d); 
            _StartPtTrackball._X = x - _XCenterTrackball;
            _StartPtTrackball._Y = y - _YCenterTrackball;
            xxyy = _StartPtTrackball._X * _StartPtTrackball._X + _StartPtTrackball._Y * _StartPtTrackball._Y;
            if (xxyy > _RadiusTrackball * _RadiusTrackball)
            {
                // Outside the sphere.
                _StartPtTrackball._Z = 0.0d;
            }
            else
                _StartPtTrackball._Z = Math.Sqrt(_RadiusTrackball * _RadiusTrackball - xxyy);

        }

        // update to new mouse position, output rotation angle
        private static void rollToTrackball(long x, long y, ref double[] rot) // rot is output rotation angle
        {
            double xxyy;
            double cosAng, sinAng;
            double ls, le, lr;

            _EndPtTrackball._X = x - _XCenterTrackball;
            _EndPtTrackball._Y = y - _YCenterTrackball;
            if (Math.Abs(_EndPtTrackball._X - _StartPtTrackball._X) < CONST.kTol && Math.Abs(_EndPtTrackball._Y - _StartPtTrackball._Y) < CONST.kTol)
                return; // Not enough change in the vectors to have an action.

            // Compute the ending vector from the surface of the ball to its center.
            xxyy = _EndPtTrackball._X * _EndPtTrackball._X + _EndPtTrackball._Y * _EndPtTrackball._Y;
            if (xxyy > _RadiusTrackball * _RadiusTrackball)
            {
                // Outside the sphere.
                _EndPtTrackball._Z = 0.0f;
            }
            else
                _EndPtTrackball._Z = Math.Sqrt(_RadiusTrackball * _RadiusTrackball - xxyy);

            // Take the cross product of the two vectors. r = s X e
            rot[1] = _StartPtTrackball._Y * _EndPtTrackball._Z - _StartPtTrackball._Z * _EndPtTrackball._Y;
            rot[2] = -_StartPtTrackball._X * _EndPtTrackball._Z + _StartPtTrackball._Z * _EndPtTrackball._X;
            rot[3] = _StartPtTrackball._X * _EndPtTrackball._Y - _StartPtTrackball._Y * _EndPtTrackball._X;

            // Use atan for a better angle.  If you use only cos or sin, you only get
            // half the possible angles, and you can end up with rotations that flip around near
            // the poles.

            // cos(a) = (s . e) / (||s|| ||e||)
            cosAng = _StartPtTrackball._X * _EndPtTrackball._X + _StartPtTrackball._Y * _EndPtTrackball._Y + _StartPtTrackball._Z * _EndPtTrackball._Z; // (s . e)
            ls = Math.Sqrt(_StartPtTrackball._X * _StartPtTrackball._X + _StartPtTrackball._Y * _StartPtTrackball._Y + _StartPtTrackball._Z * _StartPtTrackball._Z);
            ls = 1.0d / ls; // 1 / ||s||
            le = Math.Sqrt(_EndPtTrackball._X * _EndPtTrackball._X + _EndPtTrackball._Y * _EndPtTrackball._Y + _EndPtTrackball._Z * _EndPtTrackball._Z);
            le = 1.0d / le; // 1 / ||e||
            cosAng = cosAng * ls * le;

            // sin(a) = ||(s X e)|| / (||s|| ||e||)
            sinAng = lr = Math.Sqrt(rot[1] * rot[1] + rot[2] * rot[2] + rot[3] * rot[3]); // ||(s X e)||;
            // keep this length in lr for normalizing the rotation vector later.
            sinAng = sinAng * ls * le;
            rot[0] = (float)Math.Atan2(sinAng, cosAng) * CONST.RAD2DEG;   

            // Normalize the rotation axis.
            lr = 1.0d / lr;
            rot[1] *= lr; rot[2] *= lr; rot[3] *= lr;

            // returns rotate
        }

        public static void rotation2Quat(ref double[] A, ref  double[] q)
        {
            double ang2;  // The half-angle
            double sinAng2; // sin(half-angle)

            // Convert a GL-style rotation to a quaternion.  The GL rotation looks like this:
            // {angle, x, y, z}, the corresponding quaternion looks like this:
            // {{v}, cos(angle/2)}, where {v} is {x, y, z} / sin(angle/2).

            ang2 = A[0] * CONST.DEG2RAD * 0.5d;  // Convert from degrees ot radians, get the half-angle.
            sinAng2 = Math.Sin(ang2);
            q[0] = A[1] * sinAng2; q[1] = A[2] * sinAng2; q[2] = A[3] * sinAng2;
            q[3] = Math.Cos(ang2);
        }

        public static void addToRotationTrackball(double[] dA, double[] A)
        {
            double[] q0, q1, q2;
            q0 = new double[4];
            q1 = new double[4];
            q2 = new double[4];
            double theta2, sinTheta2;

            // Figure out A&apos; = A . dA
            // In quaternions: let q0 <- A, and q1 <- dA.
            // Figure out q2 = q1 + q0 (note the order reversal!).
            // A&apos; <- q3.

            rotation2Quat(ref A, ref q0);
            rotation2Quat(ref dA, ref q1);

            // q2 = q1 + q0;
            q2[0] = q1[1] * q0[2] - q1[2] * q0[1] + q1[3] * q0[0] + q1[0] * q0[3];
            q2[1] = q1[2] * q0[0] - q1[0] * q0[2] + q1[3] * q0[1] + q1[1] * q0[3];
            q2[2] = q1[0] * q0[1] - q1[1] * q0[0] + q1[3] * q0[2] + q1[2] * q0[3];
            q2[3] = q1[3] * q0[3] - q1[0] * q0[0] - q1[1] * q0[1] - q1[2] * q0[2];
            // Here&apos;s an excersize for the reader: it&apos;s a good idea to re-normalize your quaternions
            // every so often.  Experiment with different frequencies.

            // An identity rotation is expressed as rotation by 0 about any axis.
            // The "angle" term in a quaternion is really the cosine of the half-angle.
            // So, if the cosine of the half-angle is one (or, 1.0 within our tolerance),
            // then you have an identity rotation.
            if (Math.Abs(Math.Abs(q2[3] - 1.0d)) < 1.0e-7)
            {
                // Identity rotation.
                A[0] = 0.0f;
                A[1] = 1.0f;
                A[2] = A[3] = 0.0f;
                return;
            }

            // If you get here, then you have a non-identity rotation.  In non-identity rotations,
            // the cosine of the half-angle is non-0, which means the sine of the angle is also
            // non-0.  So we can safely divide by sin(theta2).

            // Turn the quaternion back into an {angle, {axis}} rotation.
            theta2 = (double)Math.Acos(q2[3]);
            sinTheta2 = (double)(1.0d / Math.Sin((double)theta2));
            A[0] = theta2 * 2.0f * CONST.RAD2DEG;
            A[1] = q2[0] * sinTheta2;
            A[2] = q2[1] * sinTheta2;
            A[3] = q2[2] * sinTheta2;
        } 

    }
}
