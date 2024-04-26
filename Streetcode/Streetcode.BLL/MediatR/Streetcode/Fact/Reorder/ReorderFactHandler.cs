using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public sealed class ReorderFactHandler : IRequestHandler<ReorderFactCommand, Result<ReorderFactResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public ReorderFactHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<ReorderFactResponseDto>> Handle(
        ReorderFactCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var arr = request.ReorderedIdArr;

        if (request.ReorderedIdArr is null || arr.Length == 0)
        {
            return FactIdArrIsNullOrEmpty(request);
        }

        var factsWithStreetcodeId = await _repositoryWrapper.FactRepository.GetAllAsync(f => f.StreetcodeId == request.StreetcodeId);
        var factsCount = factsWithStreetcodeId is null ? 0 : factsWithStreetcodeId.Count();

        if (factsCount == 0)
        {
            return CannotFindFactsWithStreetcodeId(request);
        }

        if (request.ReorderedIdArr.Length != factsCount)
        {
            return IncorrectIdsNumberInArray(request, arr.Length, factsCount);
        }

        for (int i = 0; i < arr.Length; i++)
        {
            var tmpFact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(f => f.Id == arr[i] && f.StreetcodeId == request.StreetcodeId);
            if (tmpFact is null)
            {
                return IncorrectFactIdInArray(request, arr[i]);
            }
            else
            {
                tmpFact.Number = i + 1;
                _repositoryWrapper.FactRepository.Update(tmpFact);
            }
        }

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return CannotUpdateFactsAfterReordering(request);
        }

        return Result.Ok(new ReorderFactResponseDto(true));
    }

    private Result<ReorderFactResponseDto> FactIdArrIsNullOrEmpty(ReorderFactRequestDto request)
    {
        var errorMsg = string.Format(Resources.Errors.ValidationErrors.Fact.ReorderFactErrors.IncomingFactIdArrIsNullOrEmpty);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<ReorderFactResponseDto> CannotFindFactsWithStreetcodeId(ReorderFactRequestDto request)
    {
        var errorMsg = string.Format(
            Resources.Errors.ValidationErrors.Fact.ReorderFactErrors.ThereAreNoFactsWithCorrespondingStreetcodeId,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<ReorderFactResponseDto> IncorrectIdsNumberInArray(ReorderFactRequestDto request, int idArrLength, int factsCount)
    {
        var errorMsg = string.Format(
            Resources.Errors.ValidationErrors.Fact.ReorderFactErrors.IncorrectIdsNumberInArray,
            idArrLength,
            factsCount,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<ReorderFactResponseDto> IncorrectFactIdInArray(ReorderFactRequestDto request, int id)
    {
        var errorMsg = string.Format(
            Resources.Errors.ValidationErrors.Fact.ReorderFactErrors.IncorrectFactIdInArray,
            id,
            request.StreetcodeId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<ReorderFactResponseDto> CannotUpdateFactsAfterReordering(ReorderFactRequestDto request)
    {
        var errorMsg = string.Format(Resources.Errors.ValidationErrors.Fact.ReorderFactErrors.CannotUpdateNumberInFact);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}