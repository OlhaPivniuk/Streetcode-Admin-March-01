using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners.Delete;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Partners.Delete;

public class DeletePartnerHandler :
    IRequestHandler<DeletePartnerCommand, Result<DeletePartnerResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeletePartnerHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeletePartnerResponseDto>> Handle(DeletePartnerCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var partner = await _repositoryWrapper.PartnersRepository
           .GetFirstOrDefaultAsync(sr => sr.Id == request.Id);

        if (partner is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(Partner).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.PartnersRepository.Delete(partner);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeletePartnerError(request);
        }

        var responseDto = new DeletePartnerResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeletePartnerResponseDto> FailedToDeletePartnerError(DeletePartnerRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(Partner).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
