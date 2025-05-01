using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ValidationMetadataLocalizationProvider : IValidationMetadataProvider
{
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        var requiredAttribute = context.ValidationMetadata.ValidatorMetadata.OfType<RequiredAttribute>().FirstOrDefault();
        if (requiredAttribute != null && string.IsNullOrWhiteSpace(requiredAttribute.ErrorMessage))
        {
        }
    }
}