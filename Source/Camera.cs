        //------------------------------------------------------------------------------
// using definition
using System;
using MathsLib;
using Tao.OpenGl;

//------------------------------------------------------------------------------
// namespace definition
namespace Render
{
    //------------------------------------------------------------------------------
    // class definition
    class Camera
    {

        //-----------------------------------------------------------------------------
        private Vectors _ViewDir;
        private Vectors _RightVector;
        private Vectors _UpVector;
        private Vectors _Position;
        private Vectors _Rotated;

        //-----------------------------------------------------------------------------
        private Vectors _ViewDirSave;
        private Vectors _RightVectorSave;
        private Vectors _UpVectorSave;
        private Vectors _PositionSave;
        private Vectors _RotatedSave; 

        //-----------------------------------------------------------------------------
        //Constructor
        public Camera()
        {  
	        _Position = new Vectors (0.0, 0.0,	2.0);
            _ViewDir = new Vectors(0.0, 0.0, -1.0);
            _RightVector = new Vectors(1.0, 0.0, 0.0);
            _UpVector = new Vectors(0.0, 1.0, 0.0);
            _Rotated = new Vectors(0.0, 0.0, 0.0);

            _PositionSave = new Vectors(0.0, 0.0, 2.0);
            _ViewDirSave = new Vectors(0.0, 0.0, -1.0);
            _RightVectorSave = new Vectors(1.0, 0.0, 0.0);
            _UpVectorSave = new Vectors(0.0, 1.0, 0.0);
            _RotatedSave = new Vectors(0.0, 0.0, 0.0);
        }

        //-----------------------------------------------------------------------------
        public void Move(Vectors Direction)
        { 
            _Position = _Position + Direction; 
        }


        //-----------------------------------------------------------------------------
        public void Reposition(Vectors pos, Vectors VD, Vectors Up)
        { 
            if(pos!=null)
                _Position = pos;
            if (VD != null)
                _ViewDir = VD; 
            if(Up!= null) 
                _UpVector = Up;
        }
        //-----------------------------------------------------------------------------
        public void RotateX(double Angle)
        {
	        _Rotated.addX(Angle);
            _ViewDir = (_ViewDir * Math.Cos(Angle * C.CONST.DEG_TO_RAD) + _UpVector * Math.Sin(Angle * C.CONST.DEG_TO_RAD));
            _ViewDir.normalize(); 
	        _UpVector=_ViewDir.cross(_RightVector);
            _UpVector.negate();
        }

        //-----------------------------------------------------------------------------
        public void RotateY(double Angle)
        {
	        _Rotated.addY( Angle);
            _ViewDir = (_ViewDir * Math.Cos(Angle * C.CONST.DEG_TO_RAD) - _RightVector * Math.Sin(Angle * C.CONST.DEG_TO_RAD));
            _ViewDir.normalize(); 
             _RightVector= _ViewDir.cross(_UpVector);
        }

        //-----------------------------------------------------------------------------
        public void RotateZ(double Angle)
        {
	        _Rotated.addZ(Angle);
            _RightVector = (_RightVector * Math.Cos(Angle * C.CONST.DEG_TO_RAD) + _UpVector * Math.Sin(Angle * C.CONST.DEG_TO_RAD));
            _UpVector = _ViewDir.cross(_RightVector);
            _UpVector.negate();
        }

        //-----------------------------------------------------------------------------
        public void Draw()
        {
	        Vectors ViewPoint = _Position+_ViewDir; 
            Glu.gluLookAt(_Position._X, _Position._Y, _Position._Z, ViewPoint._X, ViewPoint._Y, ViewPoint._Z, _UpVector._X, _UpVector._Y, _UpVector._Z);
        }
 
        //-----------------------------------------------------------------------------
        public void MoveForward(double Distance)
        {
            _Position = _Position + (_ViewDir * -Distance);
        }
 
        //-----------------------------------------------------------------------------
        public void StrafeRight(double Distance)
        {
            _Position = _Position + (_RightVector * Distance);
        }
 
        //-----------------------------------------------------------------------------
        public void MoveUpward(double Distance)
        {
            _Position = _Position + (_UpVector * Distance);

        }
 
        //-----------------------------------------------------------------------------

        #region Accessor
        public Vectors vViewDir
        {
            get
            {
                return _ViewDir;
            }
            set
            {
                _ViewDir = value;
            }
        }

        public Vectors vRightVector
        {
            get
            {
                return _RightVector;
            }
            set
            {
                _RightVector = value;
            }
        }
        public Vectors vUpVector
        {
            get
            {
                return _UpVector;
            }
            set
            {
                _UpVector = value;
            }
        }
        public Vectors vPosition
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        public Vectors vRotated
        {
            get
            {
                return _Rotated;
            }
            set
            {
                _Rotated = value;
            
            }
        }

        public int SaveRestore
        {
 
            set
            {
                if (value == 1)
                {
                  _ViewDirSave =_ViewDir;
                  _RightVectorSave =_RightVector;
                  _UpVectorSave =_UpVector;
                  _PositionSave =_Position;
                  _RotatedSave =_Rotated; 

                }
                else
                {
                     _ViewDir     = _ViewDirSave;
                     _RightVector = _RightVectorSave;
                     _UpVector    = _UpVectorSave;
                     _Position    = _PositionSave;
                     _Rotated     = _RotatedSave;  
                }

            }
        }

        #endregion

 
    }
}
