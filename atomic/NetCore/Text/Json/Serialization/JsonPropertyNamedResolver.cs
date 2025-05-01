using System.Collections.Concurrent;
using System.Text.Json.Serialization.Metadata;
using System.Xml.XPath;

namespace System.Text.Json.Serialization;

internal static class JsonPropertyResolver
{
    private static readonly ConcurrentDictionary<FieldInfo, string> _mapping = new();

    public static void AddEnumModifier(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (var item in jsonTypeInfo.Type.GetProperties().Where(w => w.PropertyType.IsEnum))
        {
            var jsonPropertyResolverAttribute = item.GetCustomAttribute<JsonPropertyResolverAttribute>();
            if (jsonPropertyResolverAttribute is null)
            {
                continue;
            }
            var jsonPropertyName = jsonPropertyResolverAttribute.Name.IsNullOrWhiteSpace($"{item.Name}Name")!;
            var jsonNamingPolicy = jsonTypeInfo.Options.PropertyNamingPolicy;
            if (jsonNamingPolicy is not null)
            {
                jsonPropertyName = jsonNamingPolicy.ConvertName(jsonPropertyName);
            }
            var jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(typeof(string), jsonPropertyName);
            jsonPropertyInfo.Get = (obj) =>
            {
                var value = item.GetValue(obj);
                if (value is null)
                {
                    return jsonPropertyResolverAttribute.Default ?? string.Empty;
                }

                var fieldInfo = item.PropertyType.GetField(value.ToString()!);
                return fieldInfo is null
                    ? (jsonPropertyResolverAttribute.Default ?? string.Empty)
                    : _mapping.GetOrAdd(fieldInfo, GetDescriptionOrComment(fieldInfo));
            };
            jsonTypeInfo.Properties.Add(jsonPropertyInfo);
        }
    }

    private static string GetDescriptionOrComment(FieldInfo fieldInfo)
    {
        var description = fieldInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
        if (!string.IsNullOrWhiteSpace(description))
        {
            return description;
        }

        foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
        {
            using var stream = new FileStream(xmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var xmlDocumentationComments = new XmlDocumentationComments(new XPathDocument(stream));
            return xmlDocumentationComments.GetMemberNameForFieldOrProperty(fieldInfo) ?? string.Empty;
        }

        return string.Empty;
    }

    public static void AddIntegerModifier(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (var item in jsonTypeInfo.Type.GetProperties().Where(w => w.PropertyType.IsInteger()))
        {
            var jsonPropertyResolverAttribute = item.GetCustomAttribute<JsonPropertyResolverAttribute>();
            if (jsonPropertyResolverAttribute is null || 0 == jsonPropertyResolverAttribute.Values.Length)
            {
                continue;
            }
            var jsonPropertyName = jsonPropertyResolverAttribute.Name.IsNullOrWhiteSpace($"{item.Name}Name")!;
            var jsonNamingPolicy = jsonTypeInfo.Options.PropertyNamingPolicy;
            if (jsonNamingPolicy is not null)
            {
                jsonPropertyName = jsonNamingPolicy.ConvertName(jsonPropertyName);
            }
            var jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(typeof(string), jsonPropertyName);
            jsonPropertyInfo.Get = (obj) =>
            {
                return int.TryParse(item.GetValue(obj)?.ToString(), out var value) && value >= 0 && value <= jsonPropertyResolverAttribute.Values.Length
                    ? jsonPropertyResolverAttribute.Values[value] : (jsonPropertyResolverAttribute.Default ?? string.Empty);
            };
            jsonTypeInfo.Properties.Add(jsonPropertyInfo);
        }
    }
}
