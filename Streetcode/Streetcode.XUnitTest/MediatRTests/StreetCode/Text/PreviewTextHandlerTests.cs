using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Preview;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Text;

public class PreviewTextHandlerTests
{
    private const string PREFILLEDTEXT = "Текст підготовлений спільно з ";

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task Handle_ShouldReturnStringWithPREFILLEDTEXT_IfAdditionalTextIsNotNullOrEmpty()
    {
        // Arrange
        var request = new PreviewTextRequestDto(
            Title: "title",
            TextContent: "text content",
            AdditionalText: "additional text");

        var handler = CreateHandler();
        var query = new PreviewTextQuery(request);
        string expextedAdditionalText = PREFILLEDTEXT + request.AdditionalText;

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(expextedAdditionalText, result.Value.AdditionalText);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_IfAdditionalTextIsNull()
    {
        // Arrange
        var request = new PreviewTextRequestDto(
            Title: "title",
            TextContent: "text content",
            AdditionalText: null);

        var handler = CreateHandler();
        var query = new PreviewTextQuery(request);
        string? expextedAdditionalText = null;

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(expextedAdditionalText, result.Value.AdditionalText);
    }

    [Fact]
    public async Task Handle_ShouldNull_IfAdditionalTextIsEmpty()
    {
        // Arrange
        var request = new PreviewTextRequestDto(
            Title: "title",
            TextContent: "text content",
            AdditionalText: string.Empty);

        var handler = CreateHandler();
        var query = new PreviewTextQuery(request);
        string? expextedAdditionalText = null;

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(expextedAdditionalText, result.Value.AdditionalText);
    }

    private static PreviewTextHandler CreateHandler()
    {
        return new PreviewTextHandler();
    }
}