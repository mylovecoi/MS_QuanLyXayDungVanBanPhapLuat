using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Authorization;
public sealed class KhaoSatPermissionFilter(IQuanTriHeThongPermissionClient permissions) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var controller = descriptor?.ControllerName;
        if (string.IsNullOrWhiteSpace(controller) || !controller.StartsWith("KhaoSatThiHanhPhapLuat", StringComparison.Ordinal)) { await next(); return; }
        var permissionType = ResolvePermissionType(context, descriptor?.ActionName);
        if (!await permissions.HasPermissionAsync(controller, "Index", permissionType, context.HttpContext.RequestAborted)) { context.Result = new ForbidResult(); return; }
        await next();
    }

    private static string ResolvePermissionType(ActionExecutingContext context, string? actionName)
    {
        var method = context.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method)) return "Index";
        if (HttpMethods.IsDelete(method)) return "Delete";
        if (HttpMethods.IsPut(method) || HttpMethods.IsPatch(method)) return "Edit";

        return actionName switch
        {
            "Confirm" or "FinalizeReport" or "PublishTemplate" or "RequestSupplement" => "Approve",
            "ExportWord" => "Public",
            _ => "Create"
        };
    }
}
