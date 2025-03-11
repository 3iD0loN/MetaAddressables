using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace USP.MetaAddressables
{
    public class EnumerableComparer : IEqualityComparer<IEnumerable>, IPropertyComparer
    {
        #region Properties
        public IPropertyComparer ItemComparer { get; }

        public IEnumerable<W> Children => null;
        #endregion

        #region Methods
        public EnumerableComparer(IPropertyComparer itemComparer)
        {
            this.ItemComparer = itemComparer;
        }

        public virtual bool Equals(IEnumerable leftHand, IEnumerable rightHand)
        {
            bool result = true;

            IEnumerator lhs = leftHand.GetEnumerator();
            IEnumerator rhs = rightHand.GetEnumerator();

            while (lhs.MoveNext() && rhs.MoveNext())
            {
                result &= ItemComparer.Equals(lhs.Current, rhs.Current);
            }

            return result;
        }

        public virtual int GetHashCode(IEnumerable target)
        {
            int hash = 17;

            foreach (var item in target)
            {
                hash = hash * 31 ^ ItemComparer.GetHashCode(item);
            }

            return hash;
        }

        public new bool Equals(object leftHand, object rightHand)
        {
            return Equals(leftHand as IEnumerable, rightHand as IEnumerable);
        }

        public virtual int GetHashCode(object target)
        {
            return GetHashCode(target as IEnumerable);
        }
        #endregion
    }

    public class EnumerableComparer<T> : EnumerableComparer, IEqualityComparer<IEnumerable<T>>
    {
        #region Methods
        public EnumerableComparer(IPropertyComparer itemComparer) :
            base(itemComparer)
        {
        }

        public virtual bool Equals(IEnumerable<T> leftHand, IEnumerable<T> rightHand)
        {
            return base.Equals(leftHand as IEnumerable, rightHand as IEnumerable);
        }

        public virtual int GetHashCode(IEnumerable<T> target)
        {
            return base.GetHashCode(target as IEnumerable);
        }
        #endregion
    }
}
