
namespace Microsoft.EntityFrameworkCore.Migrations;

public class MigrationsSqlGenerator<TMigrationsSqlGenerator>(
    MigrationsSqlGeneratorDependencies dependencies, TMigrationsSqlGenerator migrationsSqlGenerator)
    : MigrationsSqlGenerator(dependencies), IMigrationsSqlGenerator where TMigrationsSqlGenerator : IMigrationsSqlGenerator
{
    public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel? model = null, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
    {
        foreach (var item in operations.OfType<CreateTableOperation>())
        {
            item.ForeignKeys.Clear();
        }
        return migrationsSqlGenerator.Generate(operations, model, options);
    }
}
