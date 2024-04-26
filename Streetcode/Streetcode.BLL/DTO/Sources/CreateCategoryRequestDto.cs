using MediatR;

namespace Streetcode.BLL.Dto.Sources;

public record CreateCategoryRequestDto(string Title, int ImageId, int StreetcodeId, string? Text) :
    IRequest<SourceLinkCategoryDto>;
