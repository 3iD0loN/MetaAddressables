using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace USP.MetaAddressables
{
    public class W : Tuple<LambdaExpression, IPropertyComparer>
    {
        #region Properties
        public LambdaExpression LambdaExpression => Item1;

        public IPropertyComparer PropertyComparer => Item2;

        public MemberExpression MemberExpression
        {
            get
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

                return memberExpression;
            }
        }
        #endregion

        #region Methods
        public W(LambdaExpression item1, IPropertyComparer item2) 
            : base(item1, item2)
        {
        }

        public T GetMemberInfo<T>() where T : MemberInfo
        {
            return MemberExpression.Member as T;
        }

        public virtual object Access(object value)
        {
            var @delegate = LambdaExpression?.Compile();

            return @delegate?.DynamicInvoke(value);
        }
        #endregion
    }

    public class X<T> : W where T : LambdaExpression
    {
        #region Properties
        public T TypedLambdaExpression => LambdaExpression as T;
        #endregion

        #region Methods
        public X(T item1, IPropertyComparer item2) : base(item1, item2)
        {
        }
        #endregion
    }

    public class Y<T, U> : X<Expression<Func<T, U>>>
    {
        #region Static Methods
        public static implicit operator Y<T, U>((Expression<Func<T, U>>, IPropertyComparer) x)
        {
            return new Y<T, U>(x.Item1, x.Item2);
        }

        public static implicit operator (Expression<Func<T, U>>, IPropertyComparer<T>)(Y<T, U> x)
        {
            return (x.Expression, x.TypedPropertyComparer);
        }
        #endregion

        #region Properties
        public Expression<Func<T, U>> Expression => TypedLambdaExpression;

        public IPropertyComparer<T> TypedPropertyComparer => PropertyComparer as IPropertyComparer<T>;
        #endregion

        #region Methods
        public Y(Expression<Func<T, U>> item1, IPropertyComparer item2) : base(item1, item2)
        {
        }

        public override object Access(object value)
        {
            return Access((T)value);
        }

        public virtual U Access(T value)
        {
            var @delegate = Expression?.Compile();

            if (@delegate == null)
            {
                return default;
            }

            return @delegate.Invoke(value);
        }
        #endregion
    }

    public interface IPropertyComparer : IEqualityComparer
    {
        #region Properties
        IEnumerable<W> Children { get; }
        #endregion
    }

    public interface IPropertyComparer<T> : IPropertyComparer, IEqualityComparer<T>
    {
        #region Properties
        IEnumerable<Y<T, object>> TypedChildren { get; }
        #endregion
    }
}
