using System;
using System.Collections.Generic; 


//------------------------------------------------------------------------------
// namespace definition
namespace Model
{
    //------------------------------------------------------------------------------
    // class OctreeLeafComparer
    class OctreeLeafComparer:IComparer<OctreeLeaf>
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
                    if (C.CONST.approximatelyEqual(x.Sigma._X, y.Sigma._X))
                    {
                        if (C.CONST.approximatelyEqual(x.Sigma._Y, y.Sigma._Y))
                        {
                            if (C.CONST.approximatelyEqual(x.Sigma._Z, y.Sigma._Z))
                            { 
                                return 0; //<= THERE EQUAL
                            }
                            else
                            {
                                if (C.CONST.definitelyGreaterThan(x.Sigma._Z, y.Sigma._Z))
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
                        else
                        {
                            if (C.CONST.definitelyGreaterThan(x.Sigma._Y, y.Sigma._Y))
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
                    else
                    {
                        if (C.CONST.definitelyGreaterThan(x.Sigma._X, y.Sigma._X))
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

    }//OctreeLeafComparer
}//namespace mOdel
