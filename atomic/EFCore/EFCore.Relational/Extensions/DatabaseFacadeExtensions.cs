using System.Data;

namespace Microsoft.EntityFrameworkCore;

public static class DatabaseFacadeExtensions
{
    public static List<T> ExecuteSqlQueryRaw<T>(
        this DatabaseFacade databaseFacade,
        string sql,
        IEnumerable<object>? parameters,
        Func<IDataRecord, T> resultSelector)
    {
        var facadeDependencies = GetFacadeDependencies(databaseFacade);
        var concurrencyDetector = facadeDependencies.CoreOptions.AreThreadSafetyChecksEnabled
            ? facadeDependencies.ConcurrencyDetector
            : null;

        concurrencyDetector?.EnterCriticalSection();

        try
        {
            var rawSqlCommand = facadeDependencies.RawSqlCommandBuilder.Build(sql, parameters: parameters ?? []);

            using var reader = rawSqlCommand.RelationalCommand.ExecuteReader(
                new RelationalCommandParameterObject(
                    facadeDependencies.RelationalConnection,
                    rawSqlCommand.ParameterValues,
                    null,
                    ((IDatabaseFacadeDependenciesAccessor)databaseFacade).Context,
                    facadeDependencies.CommandLogger, CommandSource.ExecuteSqlRaw));

            return reader.DbDataReader.Cast<IDataRecord>().Select(resultSelector).ToList();
        }
        finally
        {
            concurrencyDetector?.ExitCriticalSection();
        }
    }

    public static List<T> ExecuteSqlQuery<T>(
        this DatabaseFacade databaseFacade,
        FormattableString sql,
        Func<IDataRecord, T> resultSelector)
        => databaseFacade.ExecuteSqlQueryRaw(sql.Format, sql.GetArguments()!, resultSelector);

    public static async Task<List<T>> ExecuteSqlQueryRawAsync<T>(
        this DatabaseFacade databaseFacade,
        string sql,
        IEnumerable<object>? parameters,
        Func<IDataRecord, T> resultSelector,
        CancellationToken cancellationToken = default)
    {
        var facadeDependencies = GetFacadeDependencies(databaseFacade);
        var concurrencyDetector = facadeDependencies.CoreOptions.AreThreadSafetyChecksEnabled
            ? facadeDependencies.ConcurrencyDetector
            : null;

        concurrencyDetector?.EnterCriticalSection();

        try
        {
            var rawSqlCommand = facadeDependencies.RawSqlCommandBuilder.Build(sql, parameters ?? []);

            using var reader = await rawSqlCommand.RelationalCommand.ExecuteReaderAsync(
                new RelationalCommandParameterObject(
                    facadeDependencies.RelationalConnection,
                    rawSqlCommand.ParameterValues,
                    null,
                    ((IDatabaseFacadeDependenciesAccessor)databaseFacade).Context,
                    facadeDependencies.CommandLogger, CommandSource.ExecuteSqlRaw),
                cancellationToken).ConfigureAwait(false);

            return reader.DbDataReader.Cast<IDataRecord>().Select(resultSelector).ToList();
        }
        finally
        {
            concurrencyDetector?.ExitCriticalSection();
        }
    }

    public static async Task<List<T>> ExecuteSqlQueryAsync<T>(
        this DatabaseFacade databaseFacade,
        FormattableString sql,
        Func<IDataRecord, T> resultSelector,
        CancellationToken cancellationToken = default)
        => await databaseFacade.ExecuteSqlQueryRawAsync(sql.Format, sql.GetArguments()!, resultSelector, cancellationToken).ConfigureAwait(false);

    private static IRelationalDatabaseFacadeDependencies GetFacadeDependencies(DatabaseFacade databaseFacade)
    {
        var dependencies = ((IDatabaseFacadeDependenciesAccessor)databaseFacade).Dependencies;

        return dependencies is IRelationalDatabaseFacadeDependencies relationalDependencies
            ? relationalDependencies
            : throw new InvalidOperationException(RelationalStrings.RelationalNotInUse);
    }
}
