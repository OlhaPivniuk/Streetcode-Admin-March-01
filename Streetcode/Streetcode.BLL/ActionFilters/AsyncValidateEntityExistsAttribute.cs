using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Contracts;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.ActionFilters;

public class AsyncValidateEntityExistsAttribute<T> : IAsyncActionFilter
    where T : class, IEntity
{
    private readonly IEntityRepositoryBase<T> _repositoryBase;
    private readonly ILoggerService _logger;
    public AsyncValidateEntityExistsAttribute(IEntityRepositoryBase<T> repositoryBase, ILoggerService logger)
    {
        _repositoryBase = repositoryBase;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        int id;
        if (context.ActionArguments.ContainsKey("Id"))
        {
            id = (int)context.ActionArguments["Id"] !;
        }
        else
        {
            var errorMsg = string.Format(ErrorMessages.RequestDoesNotContainIdParameter, typeof(T).Name);
            _logger.LogError(context, errorMsg);
            context.Result = new BadRequestObjectResult(errorMsg);
            return;
        }

        var entity = await _repositoryBase.GetSingleOrDefaultAsync(x => x.Id.Equals(id));

        if (entity == null)
        {
            var requestType = context.HttpContext.Request.Headers[":method"];
            string errorMessageType = GetErrorMessage(requestType);
            var errorMsg = string.Format(errorMessageType, typeof(T).Name, id);
            _logger.LogError(context, errorMsg);
            context.Result = new NotFoundObjectResult(errorMsg);
            return;
        }
        else
        {
            context.HttpContext.Items.Add("entity", entity);
        }

        var result = await next();
    }

    private string GetErrorMessage(string requestType) => requestType switch
    {
        "GET" => ErrorMessages.EntityByIdNotFound,
        "PUT" => ErrorMessages.UpdateFailed,
        "DELETE" => ErrorMessages.DeleteFailed,
        _ => "Not appropriate Request type",
    };
}
