
//------------------------------------------------------------------------------
// using definition
using System;
using System.IO;
using System.Xml; 

//------------------------------------------------------------------------------
// namespace definition
namespace Render
{

    //------------------------------------------------------------------------------
    // class definition
     public class Colour
     {

        //------------------------------------------------------------------------------
        // constants
        public static int R = 0;
        public static int G = 1;
        public static int B = 2;
        public static int A = 3;
        public static int COLOURS = 4;

        //------------------------------------------------------------------------------
        // CTor
        public Colour():this( 0.0,0.0, 0.0, 1.0) {}
        public Colour(Colour c) : this(c.at(Colour.R), c.at(Colour.G), c.at(Colour.B), c.at(Colour.A)) { }
        public Colour(double r, double g, double b, double a )
        {
            _c= new double [COLOURS];
            _c[R] = r;
            _c[G] = g;
            _c[B] = b;
            _c[A] = a;
        }

        public double _R
        {
            get
            {
                return _c[R];
            }
            set
            {
                _c[R] = value;
            }
        }

        public double _G
        {
            get
            {
                return _c[G];
            }
            set
            {
                _c[G] = value;
            }
        }
        public double _B
        {
            get
            {
                return _c[B];
            }
            set
            {
                _c[B] = value;
            }
        }

        public double _A
        {
            get
            {
                return _c[A];
            }
            set
            {
                _c[A] = value;
            }
        } 
        public double at( int i)
        {
            return _c[i];
        } 
        
        public void at( int i, Double rgba)
        {
            _c[i]=rgba;
        }


        public float[] tofloat4f(out float[] f) 
        {
            f = new float[Colour.COLOURS];
            f[R] = (float)_c[R];
            f[G] = (float)_c[G];
            f[B] = (float)_c[B];
            f[A] = (float)_c[A];
            return f;
        }
        public float[] tofloat3f(out float[] f)
        {
            f = new float[Colour.COLOURS-1];
            f[R] = (float)_c[R];
            f[G] = (float)_c[G];
            f[B] = (float)_c[B]; 
            return f;
        }

        // add two colours and copy into this colour
        Colour add( Colour c1, Colour c2) {
            _c[R] = c1._c[R] + c2._c[R];
            _c[G] = c1._c[G] + c2._c[G];
            _c[B] = c1._c[B] + c2._c[B];
            _c[A] = c1._c[A] + c2._c[A];
            return this;
        } 

        public static Colour operator+(Colour c1, Colour c2)
        {
            return new Colour().add(c1, c2);
        }
        // subtract two colours and copy into this colour
        public Colour subtract(Colour c1, Colour c2)
        {
            _c[R] = c1._c[R] - c2._c[R];
            _c[G] = c1._c[G] - c2._c[G];
            _c[B] = c1._c[B] - c2._c[B];
            _c[A] = c1._c[A] - c2._c[A];
            return this;
        } 

        public static Colour operator-(Colour c1, Colour c2) 
        {
            return new Colour().subtract(c1, c2);
        }

        // multiply two colours and copy into this colour
        Colour multiply( Colour c1, Colour c2) {
            _c[R] = c1._c[R] * c2._c[R];
            _c[G] = c1._c[G] * c2._c[G];
            _c[B] = c1._c[B] * c2._c[B];
            _c[A] = c1._c[A] * c2._c[A];
            return this;
        } 

        public static Colour operator*(Colour c1, Colour c2) 
        {
            return new Colour().multiply(c1, c2);
        }

        // multiply colour by scalar and copy into this colour
        Colour multiply( Colour c, double s) {
            _c[R] = c._c[R] * s;
            _c[G] = c._c[G] * s;
            _c[B] = c._c[B] * s;
            _c[A] = c._c[A] * s;
            return this;
        } 

        public static Colour operator*( Colour c, double s) 
        {
            return new Colour().multiply(c, s);
        }

        // divide colour by scalar and copy into this colour
        Colour divide( Colour c, double s) 
        {
            double rs = (Math.Abs(s) < C.CONST.EPSILON) ? 0.0 : 1.0 / s;
            _c[R] = c._c[R] * rs;
            _c[G] = c._c[G] * rs;
            _c[B] = c._c[B] * rs;
            _c[A] = c._c[A] * rs;
            return this;
        }

        public static Colour operator/(Colour c, double s)
        {
            return new Colour().divide(c, s);
        }

        // negate this colour
        public Colour negate() {
            _c[R] = -_c[R];
            _c[G] = -_c[G];
            _c[B] = -_c[B];
            _c[A] = -_c[A];
            return this;
        }

        // negative operator-
        public static Colour operator-(Colour c)
        {
            return new Colour(-c.at(R), -c.at(G), -c.at(B), c.at(A));
        }
        void readRGB(ref StreamReader ins) {
            /*
             * string str;
               ins.Read(str); 
               _c.at(R,double.Parse(str));
               ins.Read(str); 
               _c.at(G,double.Parse(str));
               ins.Read(str); 
               _c.at(B,double.Parse(str));
                _c[A] = 1.0;
             * 
             */
        }

        void readRGBA(ref StreamReader ins) {
            /*
             * string str;
            ins.Read(str); 
            _c.at(R,double.Parse(str));
            ins.Read(str); 
            _c.at(G,double.Parse(str));
            ins.Read(str); 
            _c.at(B,double.Parse(str));
            ins.Read(str); 
            _c.at(A,double.Parse(str));
             * 
             */

        }

        public static Colour operator*(double s, Colour c)
        {
            return c * s;
        }
     
        public override string ToString()
        {
            string str;
            str = " " + at(R) + " " + at(G) + " " + at(B) + " " + at(A);
            return str;
        }
 
        private void read(ref StreamReader ins) {
            readRGB(ref ins);
        }

        private void write(ref StreamWriter outs)
        {
            outs.WriteLine("(" +_c[R] + ", " + _c[G] + ", " + _c[B] + ", " + _c[A] + ")");
        }
         
        //------------------------------------------------------------------------------
        public virtual void WriteXmlData(ref XmlTextWriter outs)
        {

            outs.WriteStartElement("Colour");
            outs.WriteStartElement("R");
            outs.WriteValue(_c[R]);
            outs.WriteFullEndElement();
            outs.WriteStartElement("G");
            outs.WriteValue(_c[G]);
            outs.WriteFullEndElement();
            outs.WriteStartElement("B");
            outs.WriteValue(_c[B]);
            outs.WriteFullEndElement();
            outs.WriteStartElement("A");
            outs.WriteValue(_c[A]);
            outs.WriteFullEndElement();
            outs.WriteFullEndElement();
        }

        #region PredefColours
        public static Colour TABLECOLOUR(int i)
        {
            switch(i)
            {
                  case 0: return Snow();  
                  case 1: return Ghost_White();  
                  case 2: return White_Smoke();  
                  case 3: return Gainsboro();  
                  case 4: return Floral_White();  
                  case 5: return Old_Lace();  
                  case 6: return Linen();  
                  case 7: return Antique_White();  
                  case 8: return Papaya_Whip();  
                  case 9: return Blanched_Almond();  
                  case 10: return Bisque();  
                  case 11: return Peach_Puff();  
                  case 12: return Navajo_White();  
                  case 13: return Moccasin();  
                  case 14: return Cornsilk();  
                  case 15: return Ivory();  
                  case 16: return Lemon_Chiffon();  
                  case 17: return Seashell();  
                  case 18: return Honeydew();  
                  case 19: return Mint_Cream();  
                  case 20: return Azure();  
                  case 21: return Alice_Blue();  
                  case 22: return Lavender();  
                  case 23: return Lavender_Blush();  
                  case 24: return Misty_Rose();  
                  case 25: return White();  
                  case 26: return Black();  
                  case 27: return DarkSlate_Gray();  
                  case 28: return DimGray();  
                  case 29: return Slate_Gray();  
                  case 30: return Light_Slate_Gray();  
                  case 31: return Gray();  
                  case 32: return Light_Gray();  
                  case 33: return Midnight_Blue();  
                  case 34: return Navy();  
                  case 35: return Cornflower_Blue();  
                  case 36: return DarkSlate_Blue();  
                  case 37: return Slate_Blue();  
                  case 38: return Medium_Slate_Blue();  
                  case 39: return Light_Slate_Blue();  
                  case 40: return Medium_Blue();  
                  case 41: return Royal_Blue();  
                  case 42: return Blue();  
                  case 43: return Dodger_Blue();  
                  case 44: return Deep_Sky_Blue();  
                  case 45: return Sky_Blue();  
                  case 46: return Light_Sky_Blue();  
                  case 47: return Steel_Blue();  
                  case 48: return Light_Steel_Blue();  
                  case 49: return Light_Blue();  
                  case 50: return Powder_Blue();  
                  case 51: return Pale_Turquoise();  
                  case 52: return Dark_Turquoise();  
                  case 53: return Medium_Turquoise();  
                  case 54: return Turquoise();  
                  case 55: return Cyan();  
                  case 56: return Light_Cyan();  
                  case 57: return Cadet_Blue();  
                  case 58: return Medium_Aquamarine();  
                  case 59: return Aquamarine();  
                  case 60: return Dark_Green();  
                  case 61: return Dark_OliveGreen();  
                  case 62: return Dark_Sea_Green();  
                  case 63: return Sea_Green();  
                  case 64: return Medium_Sea_Green();  
                  case 65: return Light_Sea_Green();  
                  case 66: return Pale_Green();  
                  case 67: return Spring_Green();  
                  case 68: return Lawn_Green();  
                  case 69: return Chartreuse();  
                  case 70: return Medium_Spring_Green();  
                  case 71: return Green_Yellow();  
                  case 72: return Lime_Green();  
                  case 73: return Yellow_Green();  
                  case 74: return Forest_Green();  
                  case 75: return Olive_Drab();  
                  case 76: return Dark_Khaki();  
                  case 78: return Khaki();  
                  case 79: return Pale_Goldenrod();  
                  case 80: return Light_Goldenrod_Yellow();  
                  case 81: return Light_Yellow();  
                  case 82: return Yellow();  
                  case 83: return Gold();  
                  case 84: return Light_Goldenrod();  
                  case 85: return Goldenrod();  
                  case 86: return Dark_Goldenrod();  
                  case 87: return Rosy_Brown();  
                  case 88: return Indian_Red();  
                  case 89: return Saddle_Brown();  
                  case 90: return Sienna();  
                  case 91: return Peru();  
                  case 92: return Burlywood();  
                  case 93: return Beige();  
                  case 94: return Wheat();  
                  case 95: return Sandy_Brown();  
                  case 96: return Tan();  
                  case 97: return Chocolate();  
                  case 98: return Firebrick();  
                  case 99: return Brown();  
                  case 100: return Dark_Salmon();  
                  case 101: return Salmon();  
                  case 102: return Light_Salmon();  
                  case 103: return Orange();  
                  case 104: return Dark_Orange();  
                  case 105: return Coral();  
                  case 106: return Light_Coral();  
                  case 107: return Tomato();  
                  case 108: return Orange_Red();  
                  case 109: return Red();  
                  case 110: return Hot_Pink();  
                  case 111: return Deep_Pink();  
                  case 112: return Pink();  
                  case 113: return Light_Pink();  
                  case 114: return Pale_Violet_Red();  
                  case 115: return Maroon();  
                  case 116: return Medium_Violet_Red();  
                  case 117: return Violet_Red();  
                  case 118: return Violet();  
                  case 119: return Plum();  
                  case 120: return Orchid();  
                  case 121: return Medium_Orchid();  
                  case 122: return Dark_Orchid();  
                  case 123: return Dark_Violet();  
                  case 124: return Blue_Violet();  
                  case 125: return Purple();  
                  case 126: return Medium_Purple();  
                  case 127: return Thistle();        
            }

            return Ghost_White();  
        }
 
 

        public static Colour Snow()           {        return new Colour(           1.0d, 250.0d / 255.0d, 250.0d / 255.0d, 1.0d);}
        public static Colour Ghost_White()    {        return new Colour(248.0d / 255.0d, 248.0d / 255.0d,            1.0d, 1.0d);}
        public static Colour White_Smoke()    {        return new Colour(245.0d / 255.0d, 245.0d / 255.0d, 245.0d / 255.0d, 1.0d);}
        public static Colour Gainsboro()      {        return new Colour(220.0d / 255.0d, 220.0d / 255.0d, 220.0d / 255.0d, 1.0d);}
        public static Colour Floral_White()   {        return new Colour(255.0d / 255.0d, 250.0d / 255.0d, 240.0d / 255.0d, 1.0d);}
        public static Colour Old_Lace()       {        return new Colour(253.0d / 255.0d, 245.0d / 255.0d, 230.0d / 255.0d, 1.0d);}
        public static Colour Linen()          {        return new Colour(240.0d / 255.0d, 240.0d / 255.0d, 230.0d / 255.0d, 1.0d);}
        public static Colour Antique_White () {        return new Colour(250.0d / 255.0d, 235.0d / 255.0d, 215.0d / 255.0d, 1.0d);}	  	 
        public static Colour Papaya_Whip ()   {        return new Colour(1.0d           , 239.0d / 255.0d, 213.0d / 255.0d, 1.0d);}	  	 
        public static Colour Blanched_Almond(){        return new Colour(1.0d           , 235.0d / 255.0d, 205.0d / 255.0d, 1.0d);}	  	 
        public static Colour Bisque ()        {        return new Colour(1.0d           , 228.0d / 255.0d, 196.0d / 255.0d, 1.0d);}  	     
        public static Colour  Peach_Puff 	(){        return new Colour(1.0d           , 218.0d / 255.0d, 185.0d / 255.0d, 1.0d);} 	  
        public static Colour Navajo_White ()  {        return new Colour(1.0d           , 222.0d / 255.0d, 173.0d / 255.0d, 1.0d);}  	 
        public static Colour Moccasin()       {        return new Colour(1.0d           , 228.0d / 255.0d, 181.0d / 255.0d, 1.0d);} 	  	 
        public static Colour Cornsilk ()      {        return new Colour(1.0d           , 248.0d / 255.0d, 220.0d / 255.0d, 1.0d);}
        public static Colour Ivory ()         {        return new Colour(1.0d           , 1.0d           , 240.0d / 255.0d, 1.0d);}	  	  
        public static Colour Lemon_Chiffon () {        return new Colour(1.0d           , 250.0d / 255.0d, 205.0d / 255.0d, 1.0d);}
        public static Colour Seashell()       {        return new Colour(1.0d           , 245.0d / 255.0d, 238.0d / 255.0d, 1.0d);}
        public static Colour Honeydew()       {        return new Colour(240.0d / 255.0d, 1.0d           , 240.0d / 255.0d, 1.0d);}
        public static Colour Mint_Cream()     {        return new Colour(245.0d / 255.0d, 1.0d           , 250.0d / 255.0d, 1.0d);}
        public static Colour Azure()          {        return new Colour(240.0d / 255.0d, 1.0d           ,            1.0d, 1.0d);}
        public static Colour Alice_Blue ()    {        return new Colour(240.0d / 255.0d, 248.0d / 255.0d,            1.0d, 1.0d);}
        public static Colour Lavender ()      {        return new Colour(230.0d / 255.0d, 230.0d / 255.0d, 250.0d / 255.0d, 1.0d);}	  	 
        public static Colour Lavender_Blush (){        return new Colour(1.0d           , 240.0d / 255.0d, 245.0d / 255.0d, 1.0d);}	  	 
        public static Colour Misty_Rose ()    {        return new Colour(1.0d           , 228.0d / 255.0d, 225.0d / 255.0d, 1.0d);}	  	 
        public static Colour White ()         {        return new Colour(1.0d           ,            1.0d,            1.0d, 1.0d);}	  	  
        public static Colour Black()          {        return new Colour(0.0d           , 0.0d           ,            0.0d, 1.0d);} 	 
        public static Colour DarkSlate_Gray (){        return new Colour( 49.0d / 255.0d,  79.0d / 255.0d,  79.0d / 255.0d, 1.0d);} 	 
        public static Colour DimGray()        {        return new Colour(105.0d / 255.0d, 105.0d / 255.0d, 105.0d / 255.0d, 1.0d);} 	 
        public static Colour Slate_Gray()     {        return new Colour(112.0d / 255.0d, 138.0d / 255.0d, 144.0d / 255.0d, 1.0d);} 	 
        public static Colour Light_Slate_Gray(){       return new Colour(119.0d / 255.0d, 136.0d / 255.0d, 153.0d / 255.0d, 1.0d);} 	 
        public static Colour Gray()            {       return new Colour(190.0d / 255.0d, 190.0d / 255.0d, 190.0d / 255.0d, 1.0d);} 	 
        public static Colour Light_Gray()      {       return new Colour(211.0d / 255.0d, 211.0d / 255.0d, 211.0d / 255.0d, 1.0d);}	  	  
        public static Colour Midnight_Blue ()  {       return new Colour( 25.0d / 255.0d,  25.0d / 255.0d, 112.0d / 255.0d, 1.0d);}  	 
        public static Colour Navy()            {       return new Colour(           0.0d,            0.0d, 128.0d / 255.0d, 1.0d);}  	 
        public static Colour Cornflower_Blue() {       return new Colour(100.0d / 255.0d, 149.0d / 255.0d, 237.0d / 255.0d, 1.0d);}	  	 
        public static Colour DarkSlate_Blue()  {       return new Colour( 72.0d / 255.0d,  61.0d / 255.0d, 139.0d / 255.0d, 1.0d);}	 
        public static Colour Slate_Blue()      {       return new Colour(106.0d / 255.0d,  90.0d / 255.0d, 205.0d / 255.0d, 1.0d);} 
        public static Colour Medium_Slate_Blue(){      return new Colour(123.0d / 255.0d, 104.0d / 255.0d, 238.0d / 255.0d, 1.0d);}	 	 
        public static Colour Light_Slate_Blue (){      return new Colour(132.0d / 255.0d, 112.0d / 255.0d,            1.0d, 1.0d);} 
        public static Colour Medium_Blue()      {      return new Colour(           0.0d,            0.0d, 205.0d / 255.0d, 1.0d);}
        public static Colour Royal_Blue()       {      return new Colour( 65.0d / 255.0d, 105.0d / 255.0d, 225.0d / 255.0d, 1.0d);} 	 
        public static Colour Blue ()            {      return new Colour(           0.0d,            0.0d,            1.0d, 1.0d);} 	 		 
        public static Colour Dodger_Blue ()     {      return new Colour( 30.0d / 255.0d, 144.0d / 255.0d,            1.0d, 1.0d);} 	 
        public static Colour Deep_Sky_Blue()    {      return new Colour(           0.0d, 191.0d / 255.0d,            1.0d, 1.0d);}
        public static Colour Sky_Blue()         {      return new Colour(135.0d / 255.0d, 206.0d / 255.0d, 250.0d / 255.0d, 1.0d);}
        public static Colour Light_Sky_Blue()   {      return new Colour(155.0d / 255.0d, 206.0d / 255.0d, 250.0d / 255.0d, 1.0d);}
        public static Colour Steel_Blue()       {      return new Colour( 70.0d / 255.0d, 130.0d / 255.0d, 180.0d / 255.0d, 1.0d);}
        public static Colour Light_Steel_Blue() {      return new Colour(176.0d / 255.0d, 196.0d / 255.0d, 222.0d / 255.0d, 1.0d);}	 
        public static Colour Light_Blue ()      {      return new Colour(173.0d / 255.0d, 216.0d / 255.0d, 230.0d / 255.0d, 1.0d);} 	 
        public static Colour Powder_Blue ()     {      return new Colour(176.0d / 255.0d, 224.0d / 255.0d, 230.0d / 255.0d, 1.0d);}	 
        public static Colour Pale_Turquoise()   {      return new Colour(175.0d / 255.0d, 238.0d / 255.0d, 238.0d / 255.0d, 1.0d);}	 
        public static Colour Dark_Turquoise()   {      return new Colour(           0.0d, 206.0d / 255.0d, 209.0d / 255.0d, 1.0d);} 	 
        public static Colour Medium_Turquoise (){      return new Colour( 72.0d / 255.0d, 209.0d / 255.0d, 204.0d / 255.0d, 1.0d);} 	 
        public static Colour Turquoise()        {      return new Colour( 64.0d / 255.0d, 224.0d / 255.0d, 208.0d / 255.0d, 1.0d);}	 
        public static Colour Cyan()             {      return new Colour(           0.0d,            1.0d,            1.0d, 1.0d);}	 	 
        public static Colour Light_Cyan()       {      return new Colour(224.0d / 255.0d,            1.0d,            1.0d, 1.0d);}	 	 
        public static Colour Cadet_Blue()       {      return new Colour( 95.0d / 255.0d, 158.0d / 255.0d, 160.0d / 255.0d, 1.0d);} 	 
        public static Colour Medium_Aquamarine(){      return new Colour(102.0d / 255.0d, 205.0d / 255.0d, 170.0d / 255.0d, 1.0d);}	 	 
        public static Colour Aquamarine()       {      return new Colour(127.0d / 255.0d,            1.0d, 212.0d / 255.0d, 1.0d);}	 	 
        public static Colour Dark_Green()       {      return new Colour(           0.0d, 100.0d / 255.0d,            0.0d, 1.0d);}	 	 
        public static Colour Dark_OliveGreen () {      return new Colour( 85.0d / 255.0d, 107.0d / 255.0d,  47.0d / 255.0d, 1.0d);}	 	 
        public static Colour Dark_Sea_Green()   {      return new Colour(143.0d / 255.0d, 188.0d / 255.0d, 143.0d / 255.0d, 1.0d);}	 	 
        public static Colour Sea_Green 	()      {      return new Colour( 46.0d / 255.0d, 139.0d / 255.0d,  87.0d / 255.0d, 1.0d);} 	 
        public static Colour Medium_Sea_Green() {      return new Colour( 60.0d / 255.0d, 179.0d / 255.0d, 113.0d / 255.0d, 1.0d);}	 	 
        public static Colour Light_Sea_Green()  {      return new Colour( 32.0d / 255.0d, 178.0d / 255.0d, 170.0d / 255.0d, 1.0d);}	 
        public static Colour Pale_Green ()      {      return new Colour(152.0d / 255.0d, 251.0d / 255.0d, 152.0d / 255.0d, 1.0d);} 	 
        public static Colour Spring_Green ()    {      return new Colour(           0.0d,            1.0d, 127.0d / 255.0d, 1.0d);}	 	 
        public static Colour Lawn_Green ()      {      return new Colour(124.0d / 255.0d, 252.0d / 255.0d,            0.0d, 1.0d);}	 	 
        public static Colour Chartreuse ()      {      return new Colour(127.0d / 255.0d,            1.0d,            0.0d, 1.0d);}
        public static Colour Medium_Spring_Green() {   return new Colour(           0.0d, 250.0d / 255.0d, 154.0d / 255.0d, 1.0d);} 	 	 
        public static Colour Green_Yellow()     {      return new Colour(173.0d / 255.0d,            1.0d,  47.0d / 255.0d, 1.0d);}	 	 
        public static Colour Lime_Green ()      {      return new Colour( 50.0d / 255.0d, 205.0d / 255.0d,  50.0d / 255.0d, 1.0d);} 	 
        public static Colour Yellow_Green()     {      return new Colour(154.0d / 255.0d, 205.0d / 255.0d,  50.0d / 255.0d, 1.0d);}	 	 
        public static Colour Forest_Green ()    {      return new Colour( 34.0d / 255.0d, 139.0d / 255.0d,  34.0d / 255.0d, 1.0d);} 	 
        public static Colour Olive_Drab ()      {      return new Colour(107.0d / 255.0d, 142.0d / 255.0d,  35.0d / 255.0d, 1.0d);}	 	 
        public static Colour Dark_Khaki ()      {      return new Colour(189.0d / 255.0d, 183.0d / 255.0d, 107.0d / 255.0d, 1.0d);}	 	 
        public static Colour Khaki ()           {      return new Colour(240.0d / 255.0d, 230.0d / 255.0d, 140.0d / 255.0d, 1.0d);} 
        public static Colour Pale_Goldenrod ()  {      return new Colour(238.0d / 255.0d, 232.0d / 255.0d, 170.0d / 255.0d, 1.0d);}	 	 
        public static Colour Light_Goldenrod_Yellow (){return new Colour(250.0d / 255.0d, 250.0d / 255.0d, 210.0d / 255.0d, 1.0d);}	 	 
        public static Colour Light_Yellow ()    {      return new Colour(           1.0d,            1.0d, 224.0d / 255.0d, 1.0d);} 
        public static Colour Yellow 	()      {      return new Colour(           1.0d,            1.0d,            0.0d,	1.0d);}	 
        public static Colour Gold ()            {      return new Colour(           1.0d, 215.0d / 255.0d,            0.0d, 1.0d);}
        public static Colour Light_Goldenrod () {      return new Colour(238.0d / 255.0d, 221.0d / 255.0d, 130.0d / 255.0d, 1.0d);}	 	 
        public static Colour Goldenrod ()       {      return new Colour(218.0d / 255.0d, 165.0d / 255.0d,  32.0d / 255.0d, 1.0d);}
        public static Colour Dark_Goldenrod ()  {      return new Colour(184.0d / 255.0d, 134.0d / 255.0d,  11.0d / 255.0d, 1.0d);}
        public static Colour Rosy_Brown ()      {      return new Colour(188.0d / 255.0d, 143.0d / 255.0d, 143.0d / 255.0d, 1.0d);}	 
        public static Colour Indian_Red ()      {      return new Colour(205.0d / 255.0d,  92.0d / 255.0d,  92.0d / 255.0d, 1.0d);}
        public static Colour Saddle_Brown ()    {      return new Colour(139.0d / 255.0d,  69.0d / 255.0d,  19.0d / 255.0d, 1.0d);}
        public static Colour Sienna ()          {      return new Colour(160.0d / 255.0d,  82.0d / 255.0d,  45.0d / 255.0d, 1.0d);}
        public static Colour Peru ()            {      return new Colour(205.0d / 255.0d, 133.0d / 255.0d,  63.0d / 255.0d, 1.0d);}
        public static Colour Burlywood ()       {      return new Colour(222.0d / 255.0d, 184.0d / 255.0d, 135.0d / 255.0d, 1.0d);}
        public static Colour Beige 	()          {      return new Colour(245.0d / 255.0d, 245.0d / 255.0d, 220.0d / 255.0d, 1.0d);}
        public static Colour Wheat 	()          {      return new Colour(245.0d / 255.0d, 222.0d / 255.0d, 179.0d / 255.0d, 1.0d);}
        public static Colour Sandy_Brown ()     {      return new Colour(244.0d / 255.0d, 164.0d / 255.0d,  96.0d / 255.0d, 1.0d);} 
        public static Colour Tan 	()          {      return new Colour(210.0d / 255.0d, 180.0d / 255.0d, 140.0d / 255.0d, 1.0d);}
        public static Colour Chocolate ()       {      return new Colour(210.0d / 255.0d, 105.0d / 255.0d,  30.0d / 255.0d, 1.0d);}
        public static Colour Firebrick 	()      {      return new Colour(178.0d / 255.0d,  34.0d / 255.0d,  34.0d / 255.0d, 1.0d);} 
        public static Colour Brown 	()          {      return new Colour(165.0d / 255.0d,  42.0d / 255.0d,  42.0d / 255.0d, 1.0d);} 
        public static Colour Dark_Salmon ()     {      return new Colour(233.0d / 255.0d, 150.0d / 255.0d, 122.0d / 255.0d, 1.0d);}	 	 
        public static Colour Salmon ()          {      return new Colour(250.0d / 255.0d, 128.0d / 255.0d, 114.0d / 255.0d, 1.0d);} 
        public static Colour Light_Salmon 	()  {      return new Colour(           1.0d, 160.0d / 255.0d, 122.0d / 255.0d, 1.0d);} 	 
        public static Colour Orange 	()      {      return new Colour(           1.0d, 165.0d / 255.0d,            0.0d, 1.0d);}
        public static Colour Dark_Orange ()     {      return new Colour(           1.0d, 140.0d / 255.0d,            0.0d, 1.0d);} 
        public static Colour Coral 	()          {      return new Colour(           1.0d, 127.0d / 255.0d,  80.0d / 255.0d, 1.0d);} 	 
        public static Colour Light_Coral 	()  {      return new Colour(240.0d / 255.0d, 128.0d / 255.0d, 128.0d / 255.0d, 1.0d);} 	 
        public static Colour Tomato ()          {      return new Colour(           1.0d,  99.0d / 255.0d,  71.0d / 255.0d, 1.0d);}
        public static Colour Orange_Red 	()  {      return new Colour(           1.0d,  69.0d / 255.0d,            0.0d, 1.0d);} 	 
        public static Colour Red 	()          {      return new Colour(           1.0d,            0.0d,            0.0d, 1.0d);}
        public static Colour Hot_Pink 	()      {      return new Colour(           1.0d, 105.0d / 255.0d, 180.0d / 255.0d, 1.0d);} 	 
        public static Colour Deep_Pink ()       {      return new Colour(           1.0d,  20.0d / 255.0d, 147.0d / 255.0d, 1.0d);} 
        public static Colour Pink ()            {      return new Colour(           1.0d, 192.0d / 255.0d, 203.0d / 255.0d, 1.0d);}
        public static Colour Light_Pink ()      {      return new Colour(           1.0d, 182.0d / 255.0d, 193.0d / 255.0d, 1.0d);} 	 
        public static Colour Pale_Violet_Red () {      return new Colour(219.0d / 255.0d, 112.0d / 255.0d, 147.0d / 255.0d, 1.0d);} 	 	 
        public static Colour Maroon ()          {      return new Colour(176.0d / 255.0d,  48.0d / 255.0d,  96.0d / 255.0d, 1.0d);}	 
        public static Colour Medium_Violet_Red 	(){    return new Colour(199.0d / 255.0d,  21.0d / 255.0d, 133.0d / 255.0d, 1.0d);} 
        public static Colour Violet_Red ()      {      return new Colour(208.0d / 255.0d,  32.0d / 255.0d, 144.0d / 255.0d, 1.0d);} 
        public static Colour Violet ()          {      return new Colour(238.0d / 255.0d, 130.0d / 255.0d, 238.0d / 255.0d, 1.0d);} 
        public static Colour Plum ()            {      return new Colour(221.0d / 255.0d, 160.0d / 255.0d, 221.0d / 255.0d, 1.0d);}
        public static Colour Orchid ()          {      return new Colour(218.0d / 255.0d, 112.0d / 255.0d, 214.0d / 255.0d, 1.0d);} 
        public static Colour Medium_Orchid ()   {      return new Colour(186.0d / 255.0d,  85.0d / 255.0d, 211.0d / 255.0d, 1.0d);} 
        public static Colour Dark_Orchid ()     {      return new Colour(153.0d / 255.0d,  50.0d / 255.0d, 204.0d / 255.0d, 1.0d);} 
        public static Colour Dark_Violet ()     {      return new Colour(148.0d / 255.0d,            0.0d, 211.0d / 255.0d, 1.0d);} 
        public static Colour Blue_Violet ()     {      return new Colour(138.0d / 255.0d,  43.0d / 255.0d, 226.0d / 255.0d, 1.0d);} 
        public static Colour Purple ()          {      return new Colour(160.0d / 255.0d,  32.0d / 255.0d, 240.0d / 255.0d, 1.0d);}
        public static Colour Medium_Purple ()   {      return new Colour(147.0d / 255.0d, 112.0d / 255.0d, 219.0d / 255.0d, 1.0d);}
        public static Colour Thistle ()         {      return new Colour(216.0d / 255.0d, 191.0d / 255.0d, 216.0d / 255.0d, 1.0d);}
        #endregion

        double[] _c;
    }
}