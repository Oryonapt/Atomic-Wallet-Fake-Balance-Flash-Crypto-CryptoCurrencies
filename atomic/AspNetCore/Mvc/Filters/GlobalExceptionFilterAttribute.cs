using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Filters;

public sealed class GlobalExceptionFilterAttribute(
    IWebHostEnvironment webHostEnvironment,
    IOptionsSnapshot<ExceptionResult> options,
    ILogger<GlobalExceptionFilterAttribute> logger) : ExceptionFilterAttribute
{
    private readonly ILogger<GlobalExceptionFilterAttribute> _logger = logger;

    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private readonly ExceptionResult _bizExceptionResult = options.Get("Biz_Exception");

    private readonly ExceptionResult _globalExceptionResult = options.Get("Global_Exception");

    public override Task OnExceptionAsync(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case BizException bizException:
                _logger.LogError(bizException, "Title:BizException HResult:{HResult}", bizException.HResult);

                context.ExceptionHandled = true;
                _bizExceptionResult.Message = bizException.Message;
                context.Result = new ObjectResult(_bizExceptionResult);
                break;
            case Exception handledException:
                _logger.LogError(handledException, "Title:Exception HResult:{HResult}", handledException.HResult);

                context.ExceptionHandled = true;
                _globalExceptionResult.Message = _webHostEnvironment.IsDevelopment()
                    ? handledException.ToString() : "Internal server error, Please contact the administrator";
                context.Result = new ObjectResult(_globalExceptionResult);
                break;
        }

        return base.OnExceptionAsync(context);
    }
}