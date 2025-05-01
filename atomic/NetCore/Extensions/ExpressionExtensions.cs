namespace System.Linq.Expressions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) where T : class => first.Body switch
    {
        ConstantExpression { NodeType: ExpressionType.Constant, Value: false } => first,
        ConstantExpression { NodeType: ExpressionType.Constant, Value: true } => second,
        _ => first.Compose(second, Expression.AndAlso),
    };

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) where T : class => first.Body switch
    {
        ConstantExpression { NodeType: ExpressionType.Constant, Value: false } => second,
        ConstantExpression { NodeType: ExpressionType.Constant, Value: true } => first,
        _ => first.Compose(second, Expression.OrElse),
    };

    private static Expression<TDelegate> Compose<TDelegate>(this Expression<TDelegate> first, Expression<TDelegate> second, Func<Expression, Expression, Expression> merge) where TDelegate : class
    {
        var visitor = new ParameterVisitor(second.Parameters[0], first.Parameters[0]);
        var expression = merge(visitor.Visit(first.Body)!, visitor.Visit(second.Body)!);
        return Expression.Lambda<TDelegate>(expression, first.Parameters);
    }

    sealed class ParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression parameter)
            => parameter == oldParameter ? newParameter : base.VisitParameter(parameter);
    }
}