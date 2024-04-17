﻿using System.Text.RegularExpressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;
using StreetcodeEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public class CreateStreetcodeHandler
    : IRequestHandler<CreateStreetcodeCommand, Result<CreateStreetcodeResponseDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateStreetcodeHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CreateStreetcodeResponseDto>> Handle(CreateStreetcodeCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            using var transaction = _repositoryWrapper.BeginTransaction();

            var streetcode = _mapper.Map<CreateStreetcodeRequestDto, StreetcodeEntity>(request);

            streetcode.Index = await GenerateIndexAsync();

            var lifeperiod = (streetcode.EventStartOrPersonBirthDate, streetcode.EventEndOrPersonDeathDate);

            streetcode.DateString = lifeperiod.CreateDateString();

            _repositoryWrapper.StreetcodeRepository.Create(streetcode);

            var audioValidationResult = await ValidateAudioFileAsync(request.AudioId);
            if (audioValidationResult.IsFailed)
            {
                return audioValidationResult;
            }

            var urlValidationResult = await ValidateUniqueUrlAsync(request.TransliterationUrl);
            if (urlValidationResult.IsFailed)
            {
                return urlValidationResult;
            }

            var imageValidationResult = await ValidateImageFilesAsync(request.ImageIds);
            if (imageValidationResult.IsFailed)
            {
                return imageValidationResult;
            }

            await _repositoryWrapper.SaveChangesAsync();

            CreateStreetcodeTagIndices(request, streetcode);
            CreateStreetcodeImages(request, streetcode);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return FailedToCreateStreetCodeError(request);
            }

            transaction.Complete();
            return Result.Ok(new CreateStreetcodeResponseDto(streetcode.Id));
        }

        private async Task<int> GenerateIndexAsync()
        {
            var maxIndex = await _repositoryWrapper.StreetcodeRepository
                .FindAll()
                .MaxAsync(s => s.Index);

            return maxIndex + 1;
        }

        private async Task<Result> ValidateAudioFileAsync(int? audioId)
        {
            if (!audioId.HasValue)
            {
                return Result.Ok();
            }

            var existingAudioFile = await _repositoryWrapper.AudioRepository.GetSingleOrDefaultAsync(a => a.Id == audioId.Value);
            if (existingAudioFile == null || !existingAudioFile.MimeType.Equals("audio/mpeg"))
            {
                return Result.Fail("Invalid audio file. Please upload an MP3 file.");
            }

            return Result.Ok();
        }

        private async Task<Result> ValidateUniqueUrlAsync(string transliterationUrl)
        {
            var existingStreetcode = await _repositoryWrapper.StreetcodeRepository.GetSingleOrDefaultAsync(s => s.TransliterationUrl == transliterationUrl);
            if (existingStreetcode != null)
            {
                return Result.Fail("URL already exists. Please use a different URL.");
            }

            return Result.Ok();
        }

        private async Task<Result> ValidateImageFilesAsync(IEnumerable<int> imageIds)
        {
            var imageExtensions = new Dictionary<string, string>
            {
                { "image/gif", ".gif" },
                { "image/jpeg", ".jpeg" }
            };

            foreach (var imageId in imageIds)
            {
                var image = await _repositoryWrapper.ImageRepository.GetSingleOrDefaultAsync(i => i.Id == imageId);
                if (image == null)
                {
                    string errorMsg = string.Format(ErrorMessages.EntityByIdNotFound, nameof(Image), imageId);
                    _logger.LogError(imageId, errorMsg);
                    return Result.Fail(errorMsg);
                }

                if (!imageExtensions.ContainsValue(image.MimeType))
                {
                    return Result.Fail($"Invalid image file format for image with ID {imageId}. Please upload a .gif or .jpeg file.");
                }
            }

            return Result.Ok();
        }

        private void CreateStreetcodeTagIndices(CreateStreetcodeRequestDto request, StreetcodeEntity streetcode)
        {
            var streetcodeTags = request.TagIds.Select((tagId, index) => new StreetcodeTagIndex
            {
                StreetcodeId = streetcode.Id,
                TagId = tagId,
                Index = index,
                IsVisible = true,
            });

            _repositoryWrapper.StreetcodeTagIndexRepository.CreateRange(streetcodeTags);
        }

        private async Task<string> GenerateIndexAsync(int streetcodeId)
        {
            var indexString = streetcodeId.ToString().PadLeft(4, '0');
            return indexString;
        }

        private void CreateStreetcodeImages(CreateStreetcodeRequestDto request, StreetcodeEntity streetcode)
        {
            var streetcodeImages = request.ImageIds.Select(imageId => new StreetcodeImage
            {
                StreetcodeId = streetcode.Id,
                ImageId = imageId,
            });

            _repositoryWrapper.StreetcodeImageRepository.CreateRange(streetcodeImages);
        }

        private Result<CreateStreetcodeResponseDto> FailedToCreateStreetCodeError(CreateStreetcodeRequestDto request)
        {
            var errorMsg = string.Format(
                ErrorMessages.CreateFailed,
                typeof(StreetcodeEntity).Name);
            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }
    }
}
