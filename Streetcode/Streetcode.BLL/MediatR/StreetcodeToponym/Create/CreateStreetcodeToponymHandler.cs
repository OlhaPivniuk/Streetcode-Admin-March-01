using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.DTO.StreetcodeToponym;
using Streetcode.DAL.Entities.Toponyms;
using StreetcodeToponymEntity = Streetcode.DAL.Entities.Toponyms.StreetcodeToponym;

namespace Streetcode.BLL.MediatR.StreetcodeToponym.Create;

public class CreateStreetcodeToponymHandler :
    IRequestHandler<CreateStreetcodeToponymCommand, Result<CreateStreetcodeToponymResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateStreetcodeToponymHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateStreetcodeToponymResponseDto>> Handle(CreateStreetcodeToponymCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsStreetcodeExistAsync(request.StreetcodeId))
        {
            return StreetcodeNotFoundError(request);
        }

        if (!await IsToponymExistAsync(request.ToponymId))
        {
            return ToponymNotFoundError(request);
        }

        if (!await IsPrimarKeyUnique(request.StreetcodeId, request.ToponymId))
        {
            return PrimaryKeyIsNotUniqueError(request);
        }

        var streetcodeToponymToCreate = _mapper.Map<StreetcodeToponymEntity>(request);

        var streetcodeToponym = _repositoryWrapper.StreetcodeToponymRepository.Create(streetcodeToponymToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateStreetcodeToponymError(request);
        }

        var responseDto = new CreateStreetcodeToponymResponseDto(
            streetcodeToponym.StreetcodeId,
            streetcodeToponym.ToponymId,
            GetPhysicalStreetCode(streetcodeToponym.StreetcodeId, streetcodeToponym.ToponymId));

        return Result.Ok(responseDto);
    }

    private async Task<bool> IsPrimarKeyUnique(int streetcodeId, int toponymId)
    {
        var streetcodeToponym = await _repositoryWrapper.StreetcodeToponymRepository
            .GetFirstOrDefaultAsync(st => st.StreetcodeId == streetcodeId && st.ToponymId == toponymId);

        return streetcodeToponym is null;
    }

    private Result<CreateStreetcodeToponymResponseDto> PrimaryKeyIsNotUniqueError(CreateStreetcodeToponymRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.PrimaryKeyIsNotUnique,
            typeof(StreetcodeToponymEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsToponymExistAsync(int toponymId)
    {
        var toponym = await _repositoryWrapper.ToponymRepository
            .GetFirstOrDefaultAsync(i => i.Id == toponymId);

        return toponym is not null;
    }

    private Result<CreateStreetcodeToponymResponseDto> ToponymNotFoundError(CreateStreetcodeToponymRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(Toponym).Name,
            request.ToponymId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsStreetcodeExistAsync(int streetcodeId)
    {
        var streetcode = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

        return streetcode is not null;
    }

    private Result<CreateStreetcodeToponymResponseDto> StreetcodeNotFoundError(CreateStreetcodeToponymRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<CreateStreetcodeToponymResponseDto> FailedToCreateStreetcodeToponymError(CreateStreetcodeToponymRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(StreetcodeToponymEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private static string GetPhysicalStreetCode(int streetcodeId, int toponymId)
    {
        string template = "000000";
        string phisicalStreetcode = template.Remove(template.Length - streetcodeId.ToString().Length)
            + streetcodeId.ToString();
        phisicalStreetcode += template.Remove(template.Length - toponymId.ToString().Length)
            + toponymId.ToString();
        return phisicalStreetcode;
    }
}