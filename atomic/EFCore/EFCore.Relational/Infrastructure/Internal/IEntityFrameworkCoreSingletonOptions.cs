namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal;

public interface IEntityFrameworkCoreSingletonOptions : ISingletonOptions
{
    public bool EnableForeignKeyIndex { get; set; }

    public bool EnableForeignKeyConstraint { get; set; }

    public IEnumerable<string> XmlCommentPath { get; set; }

    public SoftDeleteOptions SoftDeleteOptions { get; set; }
}
