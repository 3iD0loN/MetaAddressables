using Castle.Components.DictionaryAdapter.Xml;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static UnityEngine.GraphicsBuffer;

namespace USP.MetaAddressables
{
    public class ExpressionCapture
    {
        #region Fields
        /// <summary>
        /// The expression used to access a property of an object instance.
        /// </summary>
        public readonly LambdaExpression LambdaExpression;

        public Expression Expression;

        protected readonly Delegate @delegate;
        #endregion

        #region Methods
        public ExpressionCapture(LambdaExpression lambdaExpression)
        {
            LambdaExpression = lambdaExpression;

            Expression = LambdaExpression.Body;

            @delegate = LambdaExpression?.Compile();
        }

        public virtual object Invoke(object value)
        {
            return @delegate.DynamicInvoke(value);
        }
        #endregion
    }

    public class ExpressionCapture<L, E> : ExpressionCapture
        where L : LambdaExpression
        where E : Expression
    {
        #region Fields
        /// <summary>
        /// The expression used to access a property of an object instance.
        /// </summary>
        public new readonly L LambdaExpression;

        public new E Expression;
        #endregion

        #region Methods
        public ExpressionCapture(L lambdaExpression) :
            base(lambdaExpression)
        {
            LambdaExpression = lambdaExpression;

            Expression = LambdaExpression.Body as E;
        }
        #endregion
    }

    public class ExpressionCapture<T, U, E> : ExpressionCapture<Expression<Func<T, U>>, E>
        where E : Expression
    {
        #region Fields
        protected new readonly Func<T, U> @delegate;
        #endregion

        #region Methods
        public ExpressionCapture(Expression<Func<T, U>> lambdaExpression) :
            base(lambdaExpression)
        {
            @delegate = LambdaExpression?.Compile();
        }

        public virtual U Invoke(T value)
        {
            return @delegate.Invoke(value);
        }
        #endregion
    }

    public class MethodCallExpressionCapture<I, O> : ExpressionCapture<I, O, MethodCallExpression>
    {
        #region Methods
        public MethodCallExpressionCapture(Expression<Func<I, O>> expression) :
            base(expression)
        {
        }

        public virtual MethodInfo GetMethodInfo()
        {
            return Expression?.Method;
        }
        #endregion
    }

    public abstract class MemberExpressionCapture<C, M> : ExpressionCapture<C, M, MemberExpression>
    {
        #region Methods
        public MemberExpressionCapture(Expression<Func<C, M>> lambdaExpression)
            : base(lambdaExpression)
        {
            Expression bodyExpression = LambdaExpression.Body;
            if (bodyExpression is UnaryExpression unaryExpression)
            {
                // This usually occurs as a result of implicit boxing of a simple native type to System.Object with Convert()
                bodyExpression = unaryExpression.Operand;
            }

            if (bodyExpression is not MemberExpression memberExpression)
            {
                throw new NullReferenceException($"Expression {bodyExpression} is a {bodyExpression.GetType()}, not a member expression {typeof(MemberExpression)}.");
            }
            
            Expression = memberExpression;
        }

        public MemberInfo GetMemberInfo()
        {
            return Expression?.Member;
        }

        public abstract M Get(C target);
        #endregion
    }

    public abstract class MemberExpressionCapture<T, U, V> : MemberExpressionCapture<T, U>
        where V : MemberInfo
    {
        #region Methods
        public MemberExpressionCapture(Expression<Func<T, U>> lambdaExpression)
            : base(lambdaExpression)
        {
            if (base.Expression != null)
            {
                return;
            }

            Expression bodyExpression = LambdaExpression.Body;
            if (bodyExpression is UnaryExpression unaryExpression)
            {
                // This usually occurs as a result of implicit boxing of a simple native type to System.Object with Convert()
                bodyExpression = unaryExpression.Operand;
            }

            if (bodyExpression is not MemberExpression memberExpression)
            {
                throw new NullReferenceException($"Expression {bodyExpression} is a {bodyExpression.GetType()}, not a member expression {typeof(MemberExpression)}.");
            }

            Expression = memberExpression;
        }

        public new V GetMemberInfo()
        {
            return base.GetMemberInfo() as V;
        }
        #endregion
    }

    public class PropertyGetExpressionCapture<T, U> : MemberExpressionCapture<T, U, PropertyInfo>
    {
        #region Fields
        public MethodCallExpressionCapture<T, U> GetMethodCapture;
        #endregion

        #region Methods
        public PropertyGetExpressionCapture(Expression<Func<T, U>> lambdaExpression)
            : base(lambdaExpression)
        {
            GetMethodCapture = new MethodCallExpressionCapture<T, U>(x => Get(x));
        }

        public override U Get(T target)
        {
            PropertyInfo propertyInfo = GetMemberInfo();

            return (U)propertyInfo.GetValue(target);
        }
        #endregion
    }

    public class FieldExpressionCapture<T, U> : MemberExpressionCapture<T, U, FieldInfo>
    {
        #region Fields
        public MethodCallExpressionCapture<T, U> GetMethodCapture;
        #endregion

        #region Methods
        public FieldExpressionCapture(Expression<Func<T, U>> lambdaExpression)
            : base(lambdaExpression)
        {
            GetMethodCapture = new MethodCallExpressionCapture<T, U>(x => Get(x));
        }

        public override U Get(T target)
        {
            FieldInfo fieldInfo = GetMemberInfo();

            return (U)fieldInfo.GetValue(target);
        }
        #endregion
    }

    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    /// <typeparam name="T">The parent class type.</typeparam>
    /// <typeparam name="U">The type that the property is converted after access.</typeparam>
    public class PropertyComparerPair<T, U, V> : EnumerableComparerPair<PropertyGetExpressionCapture<T, U>, V>
        where V : IItemComparer
    {
        #region Static Methods
        public static implicit operator PropertyComparerPair<T, U, V>((Expression<Func<T, U>> propertyGetExpression, V propertyComparer) tuple)
        {
            return new PropertyComparerPair<T, U, V>(new PropertyGetExpressionCapture<T, U>(tuple.propertyGetExpression), tuple.propertyComparer);
        }

        public static implicit operator (Expression<Func<T, U>>, V)(PropertyComparerPair<T, U, V> comparerPair)
        {
            return (comparerPair.ExpressionCapture.LambdaExpression, comparerPair.PropertyComparer);
        }
        #endregion

        #region Methods
        public PropertyComparerPair(Expression<Func<T, U>> item1, V item2) :
            this(new PropertyGetExpressionCapture<T, U>(item1), item2)
        {
        }

        public PropertyComparerPair(PropertyGetExpressionCapture<T, U> item1, V item2)
            : base(item1, item2)
        {
        }        
        #endregion
    }

    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    public class PropertyComparerPair : PropertyComparerPair<object, object, IItemComparer>
    {
        #region Methods
        public PropertyComparerPair(Expression<Func<object, object>> item1)
            : this(item1, ObjectComparer.Default)
        {
        }

        public PropertyComparerPair(Expression<Func<object, object>> item1, IItemComparer item2)
            : base(item1, item2)
        {
        }
        #endregion
    }
    /*/
    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    /// <typeparam name="T">The parent class type.</typeparam>
    public class PropertyComparerPair<T> : PropertyComparerPair
    {
        #region Methods
        public PropertyComparerPair(Expression<Func<T, object>> item1)
            : this(item1, ObjectComparer.Default)
        {
        }

        public PropertyComparerPair(Expression<Func<T, object>> item1, IItemComparer item2)
            : base(Expression.Lambda<Func<object, object>>(Expression.Convert(item1.Body, typeof(object)), item1.Parameters), item2)
        {
        }
        #endregion
    }

    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    /// <typeparam name="T">The parent class type.</typeparam>
    public class PropertyComparerPair<T, U> : PropertyComparerPair<T>
    {
        #region Methods
        public PropertyComparerPair(Expression<Func<T, U>> item1)
            : this(item1, ObjectComparer<U>.Default)
        {
        }

        public PropertyComparerPair(Expression<Func<T, U>> item1, IItemComparer<U> item2)
            : base(item1, item2)
        {
        }
        #endregion
    }
    //*/

    public class PropertyComparerPairFactory : IEnumerableComparerPairFactory
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
                if (child.ExpressionCapture.LambdaExpression == null || child.PropertyComparer == null)
                {
                    throw new NullReferenceException("Property expression cannot be null.");
                }

                if (child.ExpressionCapture.GetMemberInfo() == null)
                {
                    throw new NullReferenceException($"Member expression {child.ExpressionCapture.Expression} is {child.ExpressionCapture.Expression.GetType()}, not a property or field expression.");
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The comparers for child properties of the targets.
        /// </summary>
        public IEnumerable<PropertyComparerPair> Children { get; }
        #endregion

        #region Methods
        public PropertyComparerPairFactory(params PropertyComparerPair[] children)
        {
            this.Children = children;

            ThrowExceptionIfNull(children);
        }

        public IEnumerable<(IItemComparer, object)> GetElements(object target)
        {
            var result = new List<(IItemComparer, object)>();

            foreach (PropertyComparerPair child in Children)
            {
                var childTarget = child.ExpressionCapture.Invoke(target);

                result.Add((child.PropertyComparer, childTarget));
            }

            return result;
        }

        public IEnumerable<(IItemComparer, object, object)> GetElements(object leftTarget, object rightTarget)
        {
            var result = new List<(IItemComparer, object, object)>();

            foreach (PropertyComparerPair child in Children)
            {
                var leftChild = child.ExpressionCapture.Invoke(leftTarget);
                var rightChild = child.ExpressionCapture.Invoke(rightTarget);

                result.Add((child.PropertyComparer, leftChild, rightChild));
            }

            return result;
        }
        #endregion
    }

    public class PropertyComparerPairFactory<T> : PropertyComparerPairFactory
    {
        #region Properties
        /// <summary>
        /// The comparers for child properties of the targets.
        /// </summary>
        public new IEnumerable<PropertyComparerPair<T>> Children { get; }
        #endregion

        #region Methods
        public PropertyComparerPairFactory(params PropertyComparerPair<T>[] children) :
            base(children)
        {
            this.Children = children;
        }

        public IEnumerable<(IItemComparer, object)> GetElements(T target)
        {
            var result = new List<(IItemComparer, object)>();

            foreach (PropertyComparerPair<T> child in Children)
            {
                var childTarget = child.ExpressionCapture.Invoke(target);

                result.Add((child.PropertyComparer, childTarget));
            }

            return result;
        }

        public IEnumerable<(IItemComparer, object, object)> GetElements(T leftTarget, T rightTarget)
        {
            var result = new List<(IItemComparer, object, object)>();

            foreach (PropertyComparerPair<T> child in Children)
            {
                var leftChild = child.ExpressionCapture.Invoke(leftTarget);
                var rightChild = child.ExpressionCapture.Invoke(rightTarget);

                result.Add((child.PropertyComparer, leftChild, rightChild));
            }

            return result;
        }
        #endregion
    }
}
