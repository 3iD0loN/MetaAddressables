using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace USP.MetaAddressables
{
    public class PropertyComparer : EnumerableComparer
    {
        #region Methods
        public PropertyComparer(params PropertyComparerPair[] children) :
            base(new PropertyComparerPairFactory(children))
        {
        }
        #endregion
    }

    /*/
    public class PropertyComparer<T> : PropertyComparer, IItemComparer<T>
    {
        #region Methods
        public PropertyComparer(params PropertyComparerPair<T>[] children) :
            base(children)
        {
        }

        public bool Equals(T leftHand, T rightHand)
        {
            var itemComparer = this as IItemComparer;
            return itemComparer.Equals(leftHand, rightHand);
        }

        public int GetHashCode(T target)
        {
            var itemComparer = this as IItemComparer;
            return itemComparer.GetHashCode(target);
        }
        #endregion
    }
    //*/
}
