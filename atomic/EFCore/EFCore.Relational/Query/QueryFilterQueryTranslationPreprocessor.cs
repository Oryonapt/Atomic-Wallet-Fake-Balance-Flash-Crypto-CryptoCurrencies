namespace Microsoft.EntityFrameworkCore.Query;

public class QueryFilterQueryTranslationPreprocessor(
    QueryTranslationPreprocessorDependencies dependencies,
    QueryCompilationContext queryCompilationContext,
    QueryTranslationPreprocessor innerQueryTranslationPreprocessor)
        : QueryTranslationPreprocessor(dependencies, queryCompilationContext)
{
    public override Expression Process(Expression query)
    {
        query = new QueryFilterExpressionVisitor(
            QueryCompilationContext, 
            Dependencies.EvaluatableExpressionFilter)
                .ApplyStoredQueryFilter(query);

        return innerQueryTranslationPreprocessor.Process(query);
    }
}
