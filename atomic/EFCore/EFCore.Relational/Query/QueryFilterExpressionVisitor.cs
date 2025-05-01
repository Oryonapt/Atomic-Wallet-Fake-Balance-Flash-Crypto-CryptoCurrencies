using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query;

public class QueryFilterExpressionVisitor : ExpressionVisitor
{
    private bool _ignoredQueryFilter = false;
    private readonly Parameters _parameters = new();
    private readonly IList<string> _ignoredQueryFilterNames = [];
    private readonly Dictionary<IEntityType, Dictionary<object, LambdaExpression>> _parameterizedQueryFilterPredicateCache = [];

    private readonly MethodInfo _ignoreQueryFiltersMethodInfo =
        typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethod(nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters))!;

    private readonly QueryCompilationContext _queryCompilationContext;

#if NET8_0
    private readonly ParameterExtractingExpressionVisitor _parameterExtractingExpressionVisitor;
#elif NET9_0
    private readonly ExpressionTreeFuncletizer _expressionTreeFuncletizer;
#endif

    public QueryFilterExpressionVisitor(QueryCompilationContext queryCompilationContext, IEvaluatableExpressionFilter evaluatableExpressionFilter)
    {
        _queryCompilationContext = queryCompilationContext;

#if NET8_0
        _parameterExtractingExpressionVisitor = new(evaluatableExpressionFilter, _parameters, queryCompilationContext.ContextType, queryCompilationContext.Model, queryCompilationContext.Logger, false, true);
#elif NET9_0
        _expressionTreeFuncletizer = new(_queryCompilationContext.Model, evaluatableExpressionFilter, _queryCompilationContext.ContextType, generateContextAccessors: true, _queryCompilationContext.Logger);
#endif
    }

    public Expression ApplyStoredQueryFilter(Expression query)
    {
        var queryFilter = Visit(query);

        var dbContextOnQueryContextPropertyAccess =
            Expression.Convert(
                Expression.Property(
                    QueryCompilationContext.QueryContextParameter,
                    typeof(QueryContext).GetTypeInfo().GetDeclaredProperty(nameof(QueryContext.Context))!),
                _queryCompilationContext.ContextType);

        foreach (var (key, value) in _parameters.ParameterValues)
        {
            var lambda = (LambdaExpression)value!;
            var remappedLambdaBody = ReplacingExpressionVisitor.Replace(
                lambda.Parameters[0],
                dbContextOnQueryContextPropertyAccess,
                lambda.Body);

            _queryCompilationContext.RegisterRuntimeParameter(
                key,
                Expression.Lambda(
                    remappedLambdaBody.Type.IsValueType
                        ? Expression.Convert(remappedLambdaBody, typeof(object))
                        : remappedLambdaBody,
                    QueryCompilationContext.QueryContextParameter));
        }

        return queryFilter;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        var genericMethodDefinition = methodCallExpression.Method.GetGenericMethodDefinition();

        if (genericMethodDefinition == _ignoreQueryFiltersMethodInfo)
        {
            _ignoredQueryFilter = true;
            return base.VisitMethodCall(methodCallExpression);
        }
        if (genericMethodDefinition == RelationalEntityFrameworkCoreQueryableExtensions.StringIgnoreQueryFiltersMethodInfo)
        {
            foreach (var item in (IEnumerable<string>)((ConstantExpression)methodCallExpression.Arguments[1]).Value!)
            {
                _ignoredQueryFilterNames.Add(item);
            }
            return base.Visit(methodCallExpression.Arguments[0]);
        }

        return base.VisitMethodCall(methodCallExpression);
    }

    protected override Expression VisitExtension(Expression expression)
    {
        if (expression is EntityQueryRootExpression entityQueryRootExpression
            && entityQueryRootExpression.EntityType.GetStoredQueryFilter() is { } storedQueryFilter)
        {
            var rootEntityType = entityQueryRootExpression.EntityType;

            Expression queryRootExpression = entityQueryRootExpression;
            foreach (var queryFilter in storedQueryFilter)
            {
                if (_ignoredQueryFilter ||
                    _ignoredQueryFilterNames.Contains(queryFilter.Key))
                {
                    continue;
                }

                if (!_parameterizedQueryFilterPredicateCache.TryGetValue(rootEntityType, out var storedFilterPredicate))
                {
                    _parameterizedQueryFilterPredicateCache[rootEntityType] = storedFilterPredicate ??= [];
                }
                if (!storedFilterPredicate.TryGetValue(queryFilter.Key, out var filterPredicate))
                {
#if NET8_0
                    filterPredicate = (LambdaExpression)_parameterExtractingExpressionVisitor.ExtractParameters(queryFilter.Value, false);
#elif NET9_0
                    filterPredicate = (LambdaExpression)_expressionTreeFuncletizer.ExtractParameters(queryFilter.Value, _parameters, false, false);
#endif

                    storedFilterPredicate[queryFilter.Key] = filterPredicate;
                }

                queryRootExpression = Expression.Call(
                    method: QueryableMethods.Where.MakeGenericMethod(entityQueryRootExpression.EntityType.ClrType),
                    arg0: queryRootExpression ?? entityQueryRootExpression,
                    arg1: filterPredicate);
            }
            return queryRootExpression;
        }

        return base.VisitExtension(expression);
    }

    private sealed class Parameters : IParameterValues
    {
        private readonly Dictionary<string, object?> _parameterValues = [];

        public IReadOnlyDictionary<string, object?> ParameterValues
            => _parameterValues;

        public void AddParameter(string name, object? value)
            => _parameterValues.Add(name, value);
    }
}
