using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Contracts;
using Streetcode.BLL.Constants;
using Streetcode.DAL.Repositories.Interfaces.Base;
using AutoMapper;

namespace Streetcode.BLL.ActionFilters;

public class AsyncValidateEntityExistsFilter<T> : IAsyncActionFilter
    where T : class, IEntity
{
    private readonly IEntityRepositoryBase<T> _repositoryBase;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    public AsyncValidateEntityExistsFilter(IEntityRepositoryBase<T> repositoryBase, ILoggerService logger, IMapper mapper)
    {
        _repositoryBase = repositoryBase;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        const int INEXISTENTID = -1;
        int id = INEXISTENTID;

        if (context.ActionArguments.ContainsKey(GeneralConstants.ID))
        {
            if (context.ActionArguments[GeneralConstants.ID] is int i)
            {
                id = i;
            }
        }
        else
        {
            var args = context.ActionArguments.FirstOrDefault().Value;
            try
            {
                T iEntity = _mapper.Map<T>(args);
                if (iEntity is IEntity e)
                {
                    id = e.Id;
                }
            }
            catch (AutoMapperMappingException exception)
            {
                // this is an expected mapping exception, just continue working
                _logger.LogError(context, string.Format("{0} is an expected mapping exception, just continue working", exception.GetType().Name));
            }
        }

        if (id != INEXISTENTID)
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
                context.HttpContext.Items.Add(GeneralConstants.ENTITY, entity);
            }
        }

        var result = await next();
    }

    private static string GetErrorMessage(string requestType) => requestType switch
    {
        "GET" => ErrorMessages.EntityByIdNotFound,
        "PUT" => ErrorMessages.UpdateFailed,
        "DELETE" => ErrorMessages.DeleteFailed,
        _ => ErrorMessages.NotAppropriateRequestType
    };
}
