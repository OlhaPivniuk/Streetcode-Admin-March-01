﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.Interfaces.Logging;
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
        var newPartner = _mapper.Map<Partner>(command.Request);
        try
        {
            newPartner.Streetcodes.Clear();
            newPartner = _repositoryWrapper.PartnersRepository.Create(newPartner);
            await _repositoryWrapper.SaveChangesAsync();
            var streetcodeIds = newPartner.Streetcodes.Select(s => s.Id).ToList();
            newPartner.Streetcodes.AddRange(await _repositoryWrapper
                .StreetcodeRepository
                .GetAllAsync(s => streetcodeIds.Contains(s.Id)));

            await _repositoryWrapper.SaveChangesAsync();
            return Result.Ok(_mapper.Map<CreatePartnerResponseDto>(newPartner));
        }
        catch (Exception ex)
        {
            _logger.LogError(command, ex.Message);
            return Result.Fail(ex.Message);
        }
    }
}