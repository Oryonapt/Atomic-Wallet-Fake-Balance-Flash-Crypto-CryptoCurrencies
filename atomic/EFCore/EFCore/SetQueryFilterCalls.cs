namespace Microsoft.EntityFrameworkCore;

public class SetQueryFilterCalls
{
    internal SetQueryFilterCalls() { }

    internal Dictionary<object, LambdaExpression> Filters = [];

    public SetQueryFilterCalls SetFilter(
        object filterKey, LambdaExpression propertyExpression)
    {
        Filters.Add(filterKey, propertyExpression);
        return this;
    }

    public SetQueryFilterCalls SetFilter(LambdaExpression propertyExpression)
    {
        if (propertyExpression.Body is BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left is MemberExpression leftMemberExpression)
            {
                Filters.Add(leftMemberExpression.Member.Name, propertyExpression);
            }
            if (binaryExpression.Right is MemberExpression rightMemberExpression)
            {
                Filters.Add(rightMemberExpression.Member.Name, propertyExpression);
            }
        }
        else if (propertyExpression.Body is MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments[0] is MemberExpression leftMemberExpression)
            {
                if (propertyExpression.Parameters[0] == leftMemberExpression.Expression)
                {
                    Filters.Add(leftMemberExpression.Member.Name, propertyExpression);
                }
            }
            if (methodCallExpression.Arguments[1] is MemberExpression rightMemberExpression)
            {
                if (propertyExpression.Parameters[0] == rightMemberExpression.Expression)
                {
                    Filters.Add(rightMemberExpression.Member.Name, propertyExpression);
                }
            }
        }
        return this;
    }
}

public sealed class SetQueryFilterCalls<TSource> : SetQueryFilterCalls
{
    internal SetQueryFilterCalls() { }

    public SetQueryFilterCalls<TSource> SetFilter(
        object storedKey, Expression<Func<TSource, bool>> propertyExpression)
    {
        base.SetFilter(storedKey, propertyExpression);
        return this;
    }

    public SetQueryFilterCalls<TSource> SetFilter(Expression<Func<TSource, bool>> selector)
    {
        base.SetFilter(selector);
        return this;
    }
}