namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal;

public class EntityFrameworkCoreSingletonOptions : IEntityFrameworkCoreSingletonOptions
{
    public bool EnableForeignKeyIndex { get; set; }

    public bool EnableForeignKeyConstraint { get; set; }

    public IEnumerable<string> XmlCommentPath { get; set; } = [];

    public SoftDeleteOptions SoftDeleteOptions { get; set; } = new SoftDeleteOptions();

    public void Initialize(IDbContextOptions options)
    {
        var metioCoreOptionsExtension = options.FindExtension<EntityFrameworkCoreDbContextOptionsExtension>();

        if (null != metioCoreOptionsExtension)
        {
            XmlCommentPath = metioCoreOptionsExtension.XmlCommentPath;
            SoftDeleteOptions = metioCoreOptionsExtension.SoftDeleteOptions;
            EnableForeignKeyIndex = metioCoreOptionsExtension.EnableForeignKeyIndex;
        }
    }

    public void Validate(IDbContextOptions options)
    {
    }
}
