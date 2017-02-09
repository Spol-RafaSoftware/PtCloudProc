//------------------------------------------------------------------------------
// using definition
using System;
using System.Collections.Generic;
using Tao.OpenGl; 
using MathsLib;
using Render;

//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class definition 
    class Triangle
    {

 
       public int[]     vert= new int[3];		// ID Numbers of the 3 verts.  vert[i] is opposite
   			                                    // adj[i].  The vertices around a triangle are ordered in
				                                // the positive direction.  Vertices are not also stored
		        		                        // in the list of points in this triangle.

       public int[]     adj= new int[3];		     // ID Numbers of the 3 adjacent triangles, or -1 if
		                        		             // that side is exterior.
       public  int     worst_pt=-1;	                 // ID of the worst point.  This is its index in the
				                                     // global points array.   
       LinkedList<int>   l= new LinkedList<int>();   // Linked list of the ID numbers of the points inside this
		                		                     // triangle.
       public  double   maxe= 0.0d;		             // Its error from the plane of the triangle,
				                                     // measured vertically.  Type is not short int ob possible
				                                     // overflow.
     

    }
}
