using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Contracts;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.ActionFilters;

public class AsyncValidateEntityExistsFilter<T> : IAsyncActionFilter
    where T : class, IEntity
{
    private readonly IEntityRepositoryBase<T> _repositoryBase;
    private readonly ILoggerService _logger;
    public AsyncValidateEntityExistsFilter(IEntityRepositoryBase<T> repositoryBase, ILoggerService logger)
    {
        _repositoryBase = repositoryBase;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        const int INEXISTEDID = -1;
        int id = INEXISTEDID;

        if (context.ActionArguments.ContainsKey("id"))
        {
            id = (int)context.ActionArguments["id"] !;
        }
        else
        {
            var args = context.ActionArguments.FirstOrDefault().Value;
            if (args is IEntity e)
            {
                id = e.Id;
            }
        }

        if (id != INEXISTEDID)
        {
            var entity = await _repositoryBase.GetFirstOrDefaultAsync(e => e.Id.Equals(id));

            if (entity == null)
            {
                string errorMessageType = GetErrorMessage(context.HttpContext.Request.Method);
                var errorMsg = string.Format(errorMessageType, typeof(T).Name, id);
                _logger.LogError(context, errorMsg);
                context.Result = new NotFoundObjectResult(errorMsg);
                return;
            }
            else
            {
                context.HttpContext.Items.Add("entity", entity);
            }
        }

        var result = await next();
    }

    private static string GetErrorMessage(string requestType) => requestType switch
    {
        "GET" => ErrorMessages.EntityByIdNotFound,
        "PUT" => ErrorMessages.UpdateFailed,
        "DELETE" => ErrorMessages.DeleteFailed,
        _ => "Not appropriate Request type for {0} with id={1}"
    };
}
