namespace Microsoft.EntityFrameworkCore.Query;

public class QueryTranslationPreprocessorFactory<TQueryTranslationPreprocessorFactory>(
    QueryTranslationPreprocessorDependencies dependencies,
    TQueryTranslationPreprocessorFactory innerQueryTranslationPreprocessorFactory)
        : IQueryTranslationPreprocessorFactory
            where TQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new QueryFilterQueryTranslationPreprocessor(
            dependencies,
            queryCompilationContext,
            innerQueryTranslationPreprocessorFactory.Create(queryCompilationContext));
}
