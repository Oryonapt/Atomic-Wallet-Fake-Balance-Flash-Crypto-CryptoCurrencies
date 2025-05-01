using System.Globalization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal;

public class EntityFrameworkCoreDbContextOptionsExtension : IDbContextOptionsExtension
{
    private bool _enableForeignKey;
    private bool _enableForeignKeyIndex;
    private SoftDeleteOptions _softDeleteOptions;
    private DbContextOptionsExtensionInfo? _info;
    private IEnumerable<string> _xPathDocumentPath;

    public EntityFrameworkCoreDbContextOptionsExtension()
    {
        _xPathDocumentPath = [];
        _enableForeignKeyIndex = false;
        _softDeleteOptions = new SoftDeleteOptions();
    }

    protected EntityFrameworkCoreDbContextOptionsExtension(EntityFrameworkCoreDbContextOptionsExtension copyFrom)
    {
        _enableForeignKey = copyFrom._enableForeignKey;
        _softDeleteOptions = copyFrom._softDeleteOptions;
        _xPathDocumentPath = copyFrom._xPathDocumentPath;
        _enableForeignKeyIndex = copyFrom._enableForeignKeyIndex;
    }

    public DbContextOptionsExtensionInfo Info
        => _info ??= new ExtensionInfo(this);

    protected virtual EntityFrameworkCoreDbContextOptionsExtension Clone()
        => new(this);

    public virtual EntityFrameworkCoreDbContextOptionsExtension WithForeignKey(bool enable = false)
    {
        var clone = Clone();
        clone._enableForeignKey = enable;
        return clone;
    }

    public virtual EntityFrameworkCoreDbContextOptionsExtension WithForeignKeyIndex(bool enable = false)
    {
        var clone = Clone();
        clone._enableForeignKeyIndex = enable;
        return clone;
    }

    public virtual EntityFrameworkCoreDbContextOptionsExtension WithSoftDelete(bool enable, string name, string? comment)
    {
        var clone = Clone();
        clone._softDeleteOptions = new SoftDeleteOptions(name, comment) { Enabled = true };
        return clone;
    }

    public virtual EntityFrameworkCoreDbContextOptionsExtension WithXmlCommentPath(IEnumerable<string> filePath)
    {
        var clone = Clone();
        clone._xPathDocumentPath = [.. clone._xPathDocumentPath, .. filePath];
        return clone;
    }

    public virtual bool EnableForeignKey
        => _enableForeignKey;

    public virtual bool EnableForeignKeyIndex
        => _enableForeignKeyIndex;

    public virtual SoftDeleteOptions SoftDeleteOptions
        => _softDeleteOptions;

    public virtual IEnumerable<string> XmlCommentPath
        => _xPathDocumentPath;

    public virtual void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkCoreServices();

        var serviceDescriptor1 = services.FirstOrDefault(f => f.ServiceType == typeof(IQueryTranslationPreprocessorFactory));
        if (serviceDescriptor1 is not null && serviceDescriptor1.ImplementationType is not null)
        {
            services.Add(new ServiceDescriptor(serviceDescriptor1.ImplementationType, serviceDescriptor1.ImplementationType, serviceDescriptor1.Lifetime));
            services.Replace(new ServiceDescriptor(serviceDescriptor1.ServiceType, typeof(QueryTranslationPreprocessorFactory<>).MakeGenericType(serviceDescriptor1.ImplementationType), serviceDescriptor1.Lifetime));
        }

        if (!EnableForeignKey)
        {
            var serviceDescriptor2 = services.FirstOrDefault(f => f.ServiceType == typeof(IMigrationsSqlGenerator));
            if (serviceDescriptor2 is not null && serviceDescriptor2.ImplementationType is not null)
            {
                services.Add(new ServiceDescriptor(serviceDescriptor2.ImplementationType, serviceDescriptor2.ImplementationType, serviceDescriptor2.Lifetime));
                services.Replace(new ServiceDescriptor(serviceDescriptor2.ServiceType, typeof(MigrationsSqlGenerator<>).MakeGenericType(serviceDescriptor2.ImplementationType), serviceDescriptor2.Lifetime));
            }
        }
    }

    public virtual void Validate(IDbContextOptions options)
    {
    }

    protected sealed class ExtensionInfo(EntityFrameworkCoreDbContextOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
    {
        private int? _serviceProviderHash;

        private new EntityFrameworkCoreDbContextOptionsExtension Extension
            => (EntityFrameworkCoreDbContextOptionsExtension)base.Extension;

        public override bool IsDatabaseProvider
            => false;

        public override string LogFragment => "Atomicsoft.EntityFrameworkCore";

        public override int GetServiceProviderHashCode()
        {
            if (_serviceProviderHash == null)
            {
                var hashCode = new HashCode();
                hashCode.Add(Extension._softDeleteOptions);
                hashCode.Add(Extension._xPathDocumentPath);
                hashCode.Add(Extension._enableForeignKeyIndex);
                _serviceProviderHash = hashCode.ToHashCode();
            }
            return _serviceProviderHash.Value;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            if (Extension._softDeleteOptions.Enabled)
            {
                debugInfo[$"MetioCore:{nameof(Extension.WithSoftDelete)}"] =
                    Extension._softDeleteOptions.GetHashCode().ToString(CultureInfo.InvariantCulture);
                debugInfo[$"MetioCore:{nameof(Extension.WithForeignKeyIndex)}"] =
                    Extension._enableForeignKeyIndex.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo otherInfo
                && Extension._xPathDocumentPath == otherInfo.Extension.XmlCommentPath
                && Extension._enableForeignKeyIndex == otherInfo.Extension._enableForeignKeyIndex
                && Extension._softDeleteOptions.Name == otherInfo.Extension._softDeleteOptions.Name
                && Extension._softDeleteOptions.Comment == otherInfo.Extension._softDeleteOptions.Comment
                && Extension._softDeleteOptions.Enabled == otherInfo.Extension._softDeleteOptions.Enabled;
    }
}
