using System;
using System.Collections.Generic;
using System.Reflection;

namespace USP.MetaAddressables
{ 
    public class PropertyComparer<T> : IPropertyComparer<T>
    {
        #region Static Methods
        private static void ThrowExceptionIfNull(IEnumerable<PropertyComparerPair> children)
        {
            if (children == null)
            {
                return;
            }

            foreach (PropertyComparerPair child in children)
            {
                if (child.PropertyExpression == null || child.PropertyComparer == null)
                {
                    throw new NullReferenceException("Property expression cannot be null.");
                }

                if (child.MemberExpression.Member is not (PropertyInfo or FieldInfo))
                {
                    throw new NullReferenceException($"Member expression {child.MemberExpression} for {typeof(T)} is {child.MemberExpression.GetType()}, not a property or field expression.");
                }

                ThrowExceptionIfNull(child.PropertyComparer.Children);
            }
        }
        #endregion

        #region Properties
        public IEnumerable<PropertyComparerPair> Children => TypedChildren;

        public IEnumerable<PropertyComparerPair<T, object>> TypedChildren { get; }
        #endregion

        #region Methods
        public PropertyComparer(params PropertyComparerPair<T, object>[] children)
        {
            this.TypedChildren = children;

            ThrowExceptionIfNull(children);
        }

        public new bool Equals(object leftHand, object rightHand)
        {
            return Equals((T)leftHand, (T)rightHand);
        }

        public virtual int GetHashCode(object target)
        {
            return GetHashCode((T)target);
        }

        public virtual bool Equals(T leftHand, T rightHand)
        {
            bool result = true;

            if (Children != null)
            {
                foreach (PropertyComparerPair<T, object> child in TypedChildren)
                {
                    var childLeftHand = child.Access(leftHand);
                    var childRightHand = child.Access(rightHand);

                    var childValue = child.PropertyComparer.Equals(childLeftHand, childRightHand);

                    result = result & childValue;
                }
            }

            return result;
        }

        public virtual int GetHashCode(T target)
        {
            if (Children == null)
            {
                throw new InvalidOperationException("No children identified.");
            }

            int result = 17;

            foreach (PropertyComparerPair<T, object> child in TypedChildren)
            {
                var childTarget = child.Access(target);

                var childValue = child.PropertyComparer.GetHashCode(childTarget);

                result = result * 31 ^ childValue;
            }

            return result;
        }
        #endregion
    }
}
