using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Contracts;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.ActionFilters;

public class AsyncValidateEntityExistsAttribute<T> : IAsyncActionFilter
    where T : class, IEntity
{
    private readonly IEntityRepositoryBase<T> _repositoryBase;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;
    public AsyncValidateEntityExistsAttribute(IEntityRepositoryBase<T> repositoryBase, ILoggerService logger, IMapper mapper)
    {
        _repositoryBase = repositoryBase;
        _logger = logger;
        _mapper = mapper;
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
            var iEntity = _mapper.Map<T>(args);
            if (args is not null && iEntity is not null)
            {
                id = iEntity.Id;
            }
        }

        if (id == INEXISTEDID)
        {
            var errorMsg = string.Format(ErrorMessages.RequestDoesNotContainIdParameter, typeof(T).Name);
            _logger.LogError(context, errorMsg);
            context.Result = new BadRequestObjectResult(errorMsg);
            return;
        }

        var entity = await _repositoryBase.GetFirstOrDefaultAsync(x => x.Id.Equals(id));

        if (entity == null)
        {
            string errorMessageType = GetErrorMessage(context.HttpContext.Request.Method);
            var errorMsg = string.Format(errorMessageType, typeof(T).Name, id);
            var expectedMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(Fact),
            id);
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
