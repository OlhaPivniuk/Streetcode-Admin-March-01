namespace Streetcode.BLL.Dto.Streetcode.TextContent.Fact
{
    public sealed record UpdateFactDto(int Id, string Title, string FactContent, int ImageId, int StreetcodeId);
}
