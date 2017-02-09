using System;
using System.Collections.Generic;


//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    class OctreeLeafIDComparer : IComparer<OctreeLeaf>
    { 
            public int Compare(OctreeLeaf x, OctreeLeaf y)
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
                        // test x
                        if (x.ID == y.ID)
                        {
                            return 0; //<= THERE EQUAL
                        }
                        else
                        {
                            if ( x.ID > y.ID )
                            {
                                // x is greater. 
                                return 1;
                            }
                            else
                            {
                                // y is greater. 
                                return -1;
                            }
                        }

                    }
                }
            }
 
    }//OctreeLeafIDComparer
}
