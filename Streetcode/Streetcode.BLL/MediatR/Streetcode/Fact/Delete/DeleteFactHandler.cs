using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

public class DeleteFactHandler : IRequestHandler<DeleteFactCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    public DeleteFactHandler(IRepositoryWrapper repositoryWrapper, IHttpContextAccessor httpContext, ILoggerService logger, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _httpContext = httpContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<Unit>> Handle(DeleteFactCommand request, CancellationToken cancellationToken)
    {
        if (_httpContext.HttpContext!.Items[GeneralConstants.ENTITY] is not FactEntity fact)
        {
            string errorMsg = string.Format(
                            ErrorMessages.IncorrectEntity,
                            typeof(FactEntity).Name);
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        _repositoryWrapper.FactRepository.Delete(fact);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            string errorMsg = string.Format(
                ErrorMessages.DeleteFailed,
                typeof(FactEntity).Name,
                request.Id);

            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }
    }
}
