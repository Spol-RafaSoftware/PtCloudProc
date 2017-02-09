using System;
using System.Collections.Generic; 

//------------------------------------------------------------------------------
// namespace definition
namespace Render
{
    //------------------------------------------------------------------------------
    // class Surface 
    class SurfaceComparer:IComparer<Surface>
    {
        public int Compare(Surface x, Surface y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0; //<= THERE EQUAL
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {

                    // ...and y is not null, compare the 
                    // lengths of the two Leaf.
                    //
                    int retval = x.Leafs.Count.CompareTo(y.Leafs.Count);

                    if (retval != 0)
                    {
                        // If the leafs are not of equal length,
                        // the longer string is greater.
                        //
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them by ID
                        return x.ID.CompareTo(y.ID);
                    }
                }
            }
        }

    }// class
}// Render
