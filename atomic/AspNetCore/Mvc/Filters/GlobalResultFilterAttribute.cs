using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Filters;

public sealed class GlobalResultFilterAttribute(IOptionsSnapshot<GlobalObjectResult> options) : ResultFilterAttribute
{
    private readonly GlobalObjectResult _globalObjectResult = options.Get("Global_Object");

    public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.ActionDescriptor.EndpointMetadata.Any(metadata => metadata is NonGlobalResultFilterAttribute))
        {
            return base.OnResultExecutionAsync(context, next);
        }

        switch (context.Result)
        {
            case OkObjectResult okObjectResult:
                _globalObjectResult.Value = okObjectResult.Value;
                context.Result = new OkObjectResult(_globalObjectResult);
                break;
            case ObjectResult objectResult when null == objectResult.StatusCode:
                _globalObjectResult.Value = objectResult.Value;
                context.Result = new OkObjectResult(_globalObjectResult);
                break;
        }

        return base.OnResultExecutionAsync(context, next);
    }
}