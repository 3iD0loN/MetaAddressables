using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace USP.MetaAddressables
{
    /*/
    /// <summary>
    /// An equality comparer used to compare two corresponding objects in a class hierarchy.
    /// </summary>
    public interface IPropertyComparer : IEqualityComparer
    {
        #region Properties
        /// <summary>
        /// The comparer that correspond to child properties of the object instances.
        /// </summary>
        IEnumerable<PropertyComparerPair> Children { get; }
        #endregion
    }

    /// <summary>
    /// An equality comparer used to compare two corresponding objects in a class hierarchy.
    /// </summary>
    /// <typeparam name="T">The parent class type.</typeparam>
    public interface IPropertyComparer<T> : IPropertyComparer, IEqualityComparer<T>
    {
        #region Properties
        /// <summary>
        /// The comparers for child properties of the targets.
        /// </summary>
        IEnumerable<PropertyComparerPair<T, object>> TypedChildren { get; }
        #endregion
    }
    //*/
}
