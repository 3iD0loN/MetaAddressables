using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace USP.MetaAddressables
{
    /// <summary>
    /// A comparer associated with an expression to retrieve a property.
    /// </summary>
    /// <typeparam name="T">The parent type that contains the child elements.</typeparam>
    /// <typeparam name="U">The lowest common denominator type that all child elements share.</typeparam>
    /// <typeparam name="V">The <see cref="IEqualityComparer"/> type associated with each child element.</typeparam>
    public abstract class EnumerableComparerPair<E, C> : Tuple<E, C>
        where E : ExpressionCapture
        where C : IItemComparer
    {
        #region Properties
        /// <summary>
        /// The expression used to access a property of an object instance.
        /// </summary>
        public E ExpressionCapture => Item1;

        /// <summary>
        /// A property comparer used to compare two corresponding objects in a class hierarchy.
        /// </summary>
        public C PropertyComparer => Item2;
        #endregion

        #region Methods
        public EnumerableComparerPair(E item1, C item2)
            : base(item1, item2)
        {
        }
        #endregion
    }

    public interface IEnumerableComparerPairFactory
    {
        #region Methods
        public IEnumerable<(IItemComparer, object)> GetElements(object value);

        public IEnumerable<(IItemComparer, object, object)> GetElements(object left, object right);
        #endregion
    }

    public interface IEnumerableComparerPairFactory<T, U, V> : IEnumerableComparerPairFactory
        where V : IItemComparer
    {
        #region Methods
        public IEnumerable<(V, U)> GetElements(T value);

        public IEnumerable<(V, U, U)> GetElements(T left, T right);
        #endregion
    }

    public class EnumerableComparerPairFactory<T, V> : IEnumerableComparerPairFactory<IEnumerable<T>, T, V>
        where V : IItemComparer
    {
        #region Properties
        public V ElementComparer { get; }
        #endregion

        #region Methods
        public EnumerableComparerPairFactory(V elementComparer)
        {
            this.ElementComparer = elementComparer;
        }

        public IEnumerable<(V, T)> GetElements(IEnumerable<T> elements)
        {
            var result = new List<(V, T)>();

            foreach (T element in elements)
            {
                result.Add((ElementComparer, element));
            }

            return result;
        }

        public IEnumerable<(V, T, T)> GetElements(IEnumerable<T> leftElements, IEnumerable<T> rightElements)
        {
            var result = new List<(V, T, T)>();

            IEnumerator<T> leftEnumerator = leftElements.GetEnumerator();
            IEnumerator<T> rightEnumerator = rightElements.GetEnumerator();

            while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
            {
                T leftElement = leftEnumerator.Current;
                T rightElement = rightEnumerator.Current;

                result.Add((ElementComparer, leftElement, rightElement));
            }

            return result;
        }

        public IEnumerable<(IItemComparer, object)> GetElements(object value)
        {
            return GetElements(value as IEnumerable<T>) as IEnumerable<(IItemComparer, object)>;
        }

        public IEnumerable<(IItemComparer, object, object)> GetElements(object left, object right)
        {
            return GetElements(left as IEnumerable<T>, right as IEnumerable<T>) as IEnumerable<(IItemComparer, object, object)>;
        }
        #endregion
    }

    public abstract class EnumerableComparer : IItemComparer
    {
        #region Properties
        public IEnumerableComparerPairFactory ElementComparerFactory { get; }
        #endregion

        #region IEqualityComparer
        public EnumerableComparer(IEnumerableComparerPairFactory factory)
        {
            this.ElementComparerFactory = factory;
        }

        bool IEqualityComparer.Equals(object leftHand, object rightHand)
        {
            bool result = true;

            IEnumerable<(IItemComparer, object, object)> elements = ElementComparerFactory.GetElements(leftHand, rightHand);

            foreach ((IItemComparer comparer, object leftElement, object rightElement) in elements)
            {
                bool value = comparer.Equals(leftElement, rightElement);

                result &= value;
            }

            return result;
        }

        int IEqualityComparer.GetHashCode(object target)
        {
            int hash = 17;

            IEnumerable<(IItemComparer, object)> elements = ElementComparerFactory.GetElements(target);

            foreach ((IItemComparer comparer, object element) in elements)
            {
                int value = comparer.GetHashCode(element);

                hash = hash * 31 ^ value;
            }

            return hash;
        }
        #endregion
    }

    public abstract class EnumerableComparer<T, U, V, W> : EnumerableComparer, IItemComparer<T>
        where V : IItemComparer
        where W : IEnumerableComparerPairFactory<T, U, V>
    {
        #region Properties
        public new W ElementComparerFactory { get; }
        #endregion

        #region Methods
        public EnumerableComparer(W factory) :
            base(factory)
        {
            this.ElementComparerFactory = factory;
        }

        #region IEqualityComparer<T>
        bool IEqualityComparer<T>.Equals(T leftHand, T rightHand)
        {
            bool result = true;

            IEnumerable<(V, U, U)> elements = ElementComparerFactory.GetElements(leftHand, rightHand);

            foreach ((V comparer, U leftElement, U rightElement) in elements)
            {
                bool value = comparer.Equals(leftElement, rightElement);

                result &= value;
            }

            return result;
        }

        int IEqualityComparer<T>.GetHashCode(T target)
        {
            int hash = 17;

            IEnumerable<(V, U)> elements = ElementComparerFactory.GetElements(target);

            foreach ((V comparer, U element) in elements)
            {
                int value = comparer.GetHashCode(element);

                hash = hash * 31 ^ value;
            }

            return hash;
        }
        #endregion

        #region IEqualityComparer
        bool IEqualityComparer.Equals(object leftHand, object rightHand)
        {
            var comparer = this as IEqualityComparer<T>;
            return comparer.Equals((T)leftHand, (T)rightHand);
        }

        int IEqualityComparer.GetHashCode(object target)
        {
            var comparer = this as IEqualityComparer<T>;
            return comparer.GetHashCode((T)target);
        }
        #endregion
        #endregion
    }

    public class EnumerableComparer<T, U, V> : EnumerableComparer<IEnumerable<U>, U, V, EnumerableComparerPairFactory<U, V>>
        where T : IEnumerable<U>
        where V : IItemComparer<U>
    {
        #region Methods
        public EnumerableComparer(V elementComparer) :
            base(new EnumerableComparerPairFactory<U, V>(elementComparer))
        {
        }
        #endregion
    }

    public class EnumerableComparer<U, V> : EnumerableComparer<IEnumerable<U>, U, V>
        where V : IItemComparer<U>
    {
        #region Methods
        public EnumerableComparer(V elementComparer) :
            base(elementComparer)
        {
        }
        #endregion
    }
}
