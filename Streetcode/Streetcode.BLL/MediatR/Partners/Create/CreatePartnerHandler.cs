using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Partners.Create;

public class CreatePartnerHandler : IRequestHandler<CreatePartnerCommand, Result<CreatePartnerResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreatePartnerHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreatePartnerResponseDto>> Handle(CreatePartnerCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsLogoUniqueAsync(request.LogoId))
        {
            return LogoIsNotUniqueError(request);
        }

        using var transaction = _repositoryWrapper.BeginTransaction();

        var partnerToCreate = _mapper.Map<Partner>(request);
        var logoIdValidationResult = await ValidateLogoIdAsync(request.LogoId);
        if (logoIdValidationResult.IsFailed)
        {
            return logoIdValidationResult;
        }

        partnerToCreate.Streetcodes.Clear();
        partnerToCreate = _repositoryWrapper.PartnersRepository.Create(partnerToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!resultIsSuccess)
        {
            return FailedToCreatePartnerError(request);
        }

        var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllAsync(s => request.Streetcodes.Contains(s.Id));
        if (streetcodes is not null)
        {
            partnerToCreate.Streetcodes.AddRange(streetcodes);
        }

        resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!resultIsSuccess)
        {
            return FailedToCreatePartnerError(request);
        }

        transaction.Complete();

        return Result.Ok(_mapper.Map<CreatePartnerResponseDto>(partnerToCreate));
    }

    private async Task<bool> IsLogoUniqueAsync(int logoId)
    {
        var partner = await _repositoryWrapper.PartnersRepository
            .GetFirstOrDefaultAsync(sr => sr.LogoId == logoId);

        return partner is null;
    }

    private Result<CreatePartnerResponseDto> LogoIsNotUniqueError(CreatePartnerRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.PotencialPrimaryKeyIsNotUnique,
            "Logo",
            request.LogoId);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<Result> ValidateLogoIdAsync(int logoId)
    {
        string resultFailMessage = "Invalid image file. Please upload an gif, jpeg or png file.";

        var existingLogo = await _repositoryWrapper.ImageRepository.GetSingleOrDefaultAsync(a => a.Id == logoId);
        if (existingLogo is not null
            &&
            (existingLogo.MimeType!.Equals("image/gif")
            || existingLogo.MimeType.Equals("image/jpeg")
            || existingLogo.MimeType.Equals("image/png")))
        {
            return Result.Ok();
        }

        return Result.Fail(resultFailMessage);
    }

    private Result<CreatePartnerResponseDto> FailedToCreatePartnerError(CreatePartnerRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(Partner).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}