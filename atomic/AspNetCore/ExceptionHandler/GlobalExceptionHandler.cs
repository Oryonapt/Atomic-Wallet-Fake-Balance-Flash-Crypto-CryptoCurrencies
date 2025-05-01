using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Diagnostics;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IOptionsSnapshot<ExceptionResult> options,
    IWebHostEnvironment webHostEnvironment) : IExceptionHandler
{
    private readonly ExceptionResult _bizResult = options.Get("Biz_Exception");

    private readonly ExceptionResult _globalResult = options.Get("Global_Exception");

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case BizException bizException:
                logger.LogError(bizException, "Title:业务异常 HResult:{HResult}", bizException.HResult);

                _bizResult.Message = bizException.Message;
                await context.Response.WriteAsJsonAsync(_bizResult, cancellationToken);
                return await ValueTask.FromResult(true);
            case Exception handledException:
                logger.LogError(handledException, "Title:系统异常 HResult:{HResult}", handledException.HResult);

                _globalResult.Message = webHostEnvironment.IsDevelopment()
                    ? handledException.ToString() : "服务器发生错误,请联系管理员";
                await context.Response.WriteAsJsonAsync(_globalResult, cancellationToken);
                return await ValueTask.FromResult(true);
        }
        return await ValueTask.FromResult(false);
    }
}