namespace Streetcode.BLL.DTO.Media.Video
{
    public sealed record CreateVideoRequestDto(string? Title, string? Description, string Url, int StreetcodeId);
}
