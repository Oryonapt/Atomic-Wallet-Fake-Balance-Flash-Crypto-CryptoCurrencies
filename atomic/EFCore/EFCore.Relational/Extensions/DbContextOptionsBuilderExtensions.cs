using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UsePeachol(
        this DbContextOptionsBuilder optionsBuilder,
        Action<EntityFrameworkCoreDbContextOptionsBuilder>? dbContextOptionsAction = null)
    {
        var extension = GetOrCreateExtension(optionsBuilder);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        dbContextOptionsAction?.Invoke(new EntityFrameworkCoreDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    private static EntityFrameworkCoreDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<EntityFrameworkCoreDbContextOptionsExtension>()
            ?? new EntityFrameworkCoreDbContextOptionsExtension();
}