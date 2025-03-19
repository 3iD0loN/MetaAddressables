using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class GenericComparer<T> : ObjectComparer<T>
    {
        #region Properties
        public Func<T, T, bool> Equality { get; }

        public Func<T, int> Hash { get; }
        #endregion

        #region Methods
        public GenericComparer(IEqualityComparer<T> comparer) :
            this(comparer.Equals, comparer.GetHashCode)
        {
        }

        public GenericComparer(Func<T, T, bool> equality = null, Func<T, int> hash = null)
        {
            Equality = (equality != null) ? equality : (x, y) => base.Equals(x, y);
            Hash = (hash != null) ? hash : x => base.GetHashCode(x);
        }

        #region IEqualityComparer<T>
        public override int GetHashCode(T target)
        {
            return Hash != null ? Hash.Invoke(target) : default;
        }

        public override bool Equals(T leftHand, T rightHand)
        {
            return Equality != null ? Equality.Invoke(leftHand, rightHand) : false;
        }
        #endregion
        #endregion
    }
}
