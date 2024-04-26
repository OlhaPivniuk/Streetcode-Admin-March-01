using System.Text;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Preview;

public class PreviewTextHandler :
    IRequestHandler<PreviewTextQuery, Result<PreviewTextResponseDto>>
{
    private const string PREFILLEDTEXT = "Текст підготовлений спільно з ";

    public Task<Result<PreviewTextResponseDto>> Handle(PreviewTextQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;
        PreviewTextResponseDto responseDto;

        if (request.AdditionalText is not null && request.AdditionalText != string.Empty)
        {
            StringBuilder sb = new StringBuilder(PREFILLEDTEXT);
            responseDto = new PreviewTextResponseDto(
                Title: request.Title,
                TextContent: request.TextContent,
                AdditionalText: sb.Append(request.AdditionalText).ToString());
        }
        else
        {
            responseDto = new PreviewTextResponseDto(
                Title: request.Title,
                TextContent: request.TextContent,
                AdditionalText: null);
        }

        return Task.FromResult(Result.Ok(responseDto));
    }
}