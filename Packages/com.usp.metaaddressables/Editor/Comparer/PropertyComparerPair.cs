using System;
using System.CodeDom;
using System.Linq.Expressions;
using System.Reflection;

namespace USP.MetaAddressables
{
    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    public class PropertyComparerPair : Tuple<LambdaExpression, IPropertyComparer>
    {
        #region Static Methods
        private static MemberExpression ExtractMemberExpression(Expression bodyExpression)
        {
            if (bodyExpression is UnaryExpression unaryExpression)
            {
                // This usually occurs as a result of implicit boxing of a simple native type to System.Object with Convert()
                bodyExpression = unaryExpression.Operand;
            }

            if (bodyExpression is not MemberExpression memberExpression)
            {
                throw new NullReferenceException($"Expression {bodyExpression} is a {bodyExpression.GetType()}, not a member expression {typeof(MemberExpression)}.");
            }

            return memberExpression;
        }
        #endregion

        #region Properties
        
        public IPropertyComparer PropertyComparer => Item2;

        /// <summary>
        /// The expression used to access a property of an object instance.
        /// </summary>
        public LambdaExpression PropertyExpression => Item1;

        /// <summary>
        /// A property comparer used to compare two corresponding objects in a class hierarchy.
        /// </summary>
        public readonly MemberExpression MemberExpression;

        public readonly MethodCallExpression MethodCallExpression;

        public readonly Delegate Accessor;
        #endregion

        #region Methods
        public PropertyComparerPair(LambdaExpression item1, IPropertyComparer item2) 
            : base(item1, item2)
        {
            Expression bodyExpression = item1.Body;
            if (bodyExpression is not MethodCallExpression methodCallExpression)
            {
                MemberExpression = ExtractMemberExpression(bodyExpression);
            }
            else
            {
                MethodCallExpression = methodCallExpression;
            }

            Accessor = PropertyExpression?.Compile();
        }

        public T GetMemberInfo<T>() where T : MemberInfo
        {
            if (MemberExpression == null)
            {
                return null;
            }

            return MemberExpression.Member as T;
        }

        public MethodInfo GetMethodInfo()
        {
            if (MethodCallExpression != null)
            {
                return MethodCallExpression.Method;
            }

            var propertyInfo = GetMemberInfo<PropertyInfo>();
            if (propertyInfo != null)
            {
                return propertyInfo.GetMethod;
            }

            return null;
        }

        public T GetMethodInfo<T>() where T : MethodInfo
        {            
            return GetMethodInfo() as T;
        }

        public virtual object Access(object value)
        {
            return Accessor?.DynamicInvoke(value);
        }
        #endregion
    }

    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    /// <typeparam name="E">The type of the expression, which inherits from <see cref="LambdaExpression"/></typeparam>
    public class PropertyComparerPair<E> : PropertyComparerPair where E : LambdaExpression
    {
        #region Properties
        public E TypedPropertyExpression => PropertyExpression as E;
        #endregion

        #region Methods
        public PropertyComparerPair(E item1, IPropertyComparer item2) :
            base(item1, item2)
        {
        }
        #endregion
    }

    /// <summary>
    /// A comparer associated with an expression to retrieve a property in a class hierarchy.
    /// </summary>
    /// <typeparam name="T">The parent class type.</typeparam>
    /// <typeparam name="U">The type that the property is converted after access.</typeparam>
    public class PropertyComparerPair<T, U> : PropertyComparerPair<Expression<Func<T, U>>>
    {
        #region Static Methods
        public static implicit operator PropertyComparerPair<T, U>((Expression<Func<T, U>>, IPropertyComparer) x)
        {
            return new PropertyComparerPair<T, U>(x.Item1, x.Item2);
        }

        public static implicit operator (Expression<Func<T, U>>, IPropertyComparer<T>)(PropertyComparerPair<T, U> x)
        {
            return (x.Expression, x.TypedPropertyComparer);
        }
        #endregion

        #region Properties
        public Expression<Func<T, U>> Expression => TypedPropertyExpression;

        public IPropertyComparer<T> TypedPropertyComparer => PropertyComparer as IPropertyComparer<T>;

        private readonly Func<T, U> @delegate;
        #endregion

        #region Methods
        public PropertyComparerPair(Expression<Func<T, U>> item1, IPropertyComparer item2) :
            base(item1, item2)
        {
            @delegate = Expression?.Compile();
        }

        public override object Access(object value)
        {
            return Access((T)value);
        }

        public virtual U Access(T value)
        {
            if (@delegate == null)
            {
                return default;
            }

            return @delegate.Invoke(value);
        }
        #endregion
    }
}
