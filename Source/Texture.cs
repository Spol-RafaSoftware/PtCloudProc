//------------------------------------------------------------------------------
// namespace
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO; 
using System.Collections.Generic;
using Tao.OpenGl;
 
//------------------------------------------------------------------------------
// namespace definition

namespace Render {

    //------------------------------------------------------------------------------
    // class definition

    class Texture {
 

        private string _filename;
        private int _texture;
        private bool _mipmap;
        private bool _replace; 

        public Texture() {
            _texture = 0;
             _mipmap = true;
            _replace = false;
            _filename = "../Data/BUZZ.bmp";
        }
        public void delTexture() {
            Gl.glDeleteTextures(1,ref _texture);
        }
         

        //------------------------------------------------------------------------------
        // read texture
        public void  read() {
            Gl.glGenTextures(1,  out _texture);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, _texture);
            if (Gl.glIsTexture( _texture)==0) {
		        throw new Exception("Error : Failed texture creation");
	        }
            Bitmap bitmap = new Bitmap(_filename,true);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);     
                // Rectangle For Locking The Bitmap In Memory
                Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                // Get The Bitmap's Pixel Data From The Locked Bitmap
           BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

          
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, bitmap.Width, bitmap.Height, 0, 
                Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE,bitmapData.Scan0);
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB8, bitmap.Width , bitmap.Height ,
                Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            if (_mipmap) {
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
            }
            else {
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            }
        }

        //------------------------------------------------------------------------------
        // render texture
        public void render()
        {
           Gl.glBindTexture(Gl.GL_TEXTURE_2D, _texture);
            if (_replace) {
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
            }
            else {
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            }
        } 

    };

} // ComputerGraphics
 

//------------------------------------------------------------------------------
