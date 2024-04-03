using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.GetById;

public class GetFactByIdHandler : IRequestHandler<GetFactByIdQuery, Result<FactDto>>
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILoggerService _logger;

    public GetFactByIdHandler(IMapper mapper, IHttpContextAccessor httpContext, ILoggerService logger)
    {
        _mapper = mapper;
        _httpContext = httpContext;
        _logger = logger;
    }

    public async Task<Result<FactDto>> Handle(GetFactByIdQuery request, CancellationToken cancellationToken)
    {
        if (_httpContext.HttpContext!.Items[GeneralConstants.ENTITY] is FactEntity fact)
        {
            return Result.Ok(_mapper.Map<FactDto>(fact));
        }
        else
        {
            string errorMsg = string.Format(
                            ErrorMessages.IncorrectEntity,
                            typeof(FactEntity).Name);
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }
    }
}