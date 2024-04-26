namespace Streetcode.BLL.Dto.Streetcode.TextContent.Fact
{
    public sealed record CreateFactDto(string Title, string FactContent, int ImageId, int StreetcodeId);
}
