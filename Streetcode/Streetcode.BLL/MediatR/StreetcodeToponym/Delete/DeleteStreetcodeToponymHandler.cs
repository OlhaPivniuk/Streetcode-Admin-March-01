using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.DTO.StreetcodeToponym;
using StreetcodeToponymEntity = Streetcode.DAL.Entities.Toponyms.StreetcodeToponym;

namespace Streetcode.BLL.MediatR.StreetcodeToponym.Delete;

public class DeleteStreetcodeToponymHandler :
    IRequestHandler<DeleteStreetcodeToponymCommand, Result<DeleteStreetcodeToponymResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteStreetcodeToponymHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteStreetcodeToponymResponseDto>> Handle(DeleteStreetcodeToponymCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var streetcodeToponym = await _repositoryWrapper.StreetcodeToponymRepository
           .GetFirstOrDefaultAsync(st => st.StreetcodeId == request.StreetcodeId && st.ToponymId == request.ToponymId);

        if (streetcodeToponym is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByPrimaryKeyNotFound,
            typeof(StreetcodeToponymEntity).Name);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.StreetcodeToponymRepository.Delete(streetcodeToponym);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!resultIsSuccess)
        {
            return FailedToDeleteStreetcodeToponymError(request);
        }

        var responseDto = new DeleteStreetcodeToponymResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteStreetcodeToponymResponseDto> FailedToDeleteStreetcodeToponymError(DeleteStreetcodeToponymRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(StreetcodeToponymEntity).Name,
            GetPhysicalStreetCode(request.StreetcodeId, request.ToponymId));
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
