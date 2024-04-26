namespace Streetcode.BLL.DTO.Media.Video
{
    public sealed record UpdateVideoRequestDto(int Id, string? Title, string? Description, string Url);
}
