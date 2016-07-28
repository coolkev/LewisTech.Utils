using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LewisTech.Utils.Linq
{
    public static class LinqExtensions
    {


        //from http://stackoverflow.com/questions/6180704/combine-several-similar-select-expressions-into-a-single-expression
        public static Expression<Func<TSource, TDestination>> Combine<TSource, TDestination>(this Expression<Func<TSource, TDestination>> expression, params Expression<Func<TSource, TDestination>>[] selectors)
        {
            var zeroth = ((MemberInitExpression)expression.Body);
            var param = expression.Parameters[0];
            var bindings = new List<MemberBinding>(zeroth.Bindings.OfType<MemberAssignment>());

            foreach (var selector in selectors)
            {
                var memberInit = (MemberInitExpression)selector.Body;
                var replace = new ParameterReplaceVisitor(selector.Parameters[0], param);
                foreach (var binding in memberInit.Bindings.OfType<MemberAssignment>())
                {
                    bindings.Add(Expression.Bind(binding.Member, replace.VisitAndConvert(binding.Expression, "Combine")));
                }
            }

            return Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(zeroth.NewExpression, bindings), param);
        }


        //from http://stackoverflow.com/questions/6180704/combine-several-similar-select-expressions-into-a-single-expression
        public static Expression<Func<TSource1, TDestination1>> Combine<TSource1, TDestination1, TSource2, TDestination2>(this Expression<Func<TSource1, TDestination1>> expression, params Expression<Func<TSource2, TDestination2>>[] selectors) where TSource1 : TSource2 where TDestination1 : TDestination2
        {
            var zeroth = ((MemberInitExpression)expression.Body);
            var param = expression.Parameters[0];
            var bindings = new List<MemberBinding>(zeroth.Bindings.OfType<MemberAssignment>());

            foreach (var selector in selectors)
            {
                var memberInit = (MemberInitExpression)selector.Body;
                var replace = new ParameterReplaceVisitor(selector.Parameters[0], param);
                foreach (var binding in memberInit.Bindings.OfType<MemberAssignment>())
                {
                    bindings.Add(Expression.Bind(binding.Member, replace.VisitAndConvert(binding.Expression, "Combine")));
                }
            }

            return Expression.Lambda<Func<TSource1, TDestination1>>(
                Expression.MemberInit(zeroth.NewExpression, bindings), param);
        }

        public class ParameterReplaceVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression from, to;
            public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
            {
                this.from = from;
                this.to = to;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == from ? to : base.VisitParameter(node);
            }
        }



        public static Expression<Action<TInstance, TProp>> ToFieldAssignExpression<TInstance, TProp>(this Expression<Func<TInstance, TProp>> fieldGetter)
        {
            if (fieldGetter == null)
                throw new ArgumentNullException("fieldGetter");

            if (fieldGetter.Parameters.Count != 1 || !(fieldGetter.Body is MemberExpression))
                throw new ArgumentException(
                    @"Input expression must be a single parameter field getter, e.g. g => g._fieldToSet  or function(g) g._fieldToSet");

            var parms = new[]
            {
                fieldGetter.Parameters[0],
                Expression.Parameter(typeof (TProp), "value")
            };

            Expression body = Expression.Call(AssignmentHelper<TProp>.MethodInfoSetValue,
                new[] { fieldGetter.Body, parms[1] });

            return Expression.Lambda<Action<TInstance, TProp>>(body, parms);
        }


        private class AssignmentHelper<T>
        {
            internal static readonly MethodInfo MethodInfoSetValue =
                typeof(AssignmentHelper<T>).GetMethod("SetValue", BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Static);

            [UsedImplicitly]
            private static void SetValue(ref T target, T value)
            {
                target = value;
            }
        }

    }
}
