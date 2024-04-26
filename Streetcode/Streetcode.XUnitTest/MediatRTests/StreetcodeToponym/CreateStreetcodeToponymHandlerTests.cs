using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.StreetcodeToponym;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.StreetcodeToponym.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeToponymEntity = Streetcode.DAL.Entities.Toponyms.StreetcodeToponym;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeToponym;

public class CreateStreetcodeToponymHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateStreetcodeToponymHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentToponymId()
    {
        // Arrange
        var request = new CreateStreetcodeToponymRequestDto(
            StreetcodeId: 1,
            ToponymId: int.MaxValue);

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(Toponym).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeId()
    {
        // Arrange
        var request = new CreateStreetcodeToponymRequestDto(
            StreetcodeId: int.MaxValue,
            ToponymId: 1);

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        var expectedType = typeof(Result<CreateStreetcodeToponymResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreateStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncTwice_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStreetcodeToponymCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private CreateStreetcodeToponymHandler CreateHandler()
    {
        return new CreateStreetcodeToponymHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateStreetcodeToponymRequestDto request, int saveChangesAsyncResult)
    {
        var streetcode = request.StreetcodeId switch
        {
            1 => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        var toponym = request.ToponymId switch
        {
            1 => new Toponym { Id = request.ToponymId },
            _ => null,
        };

        var streetcodeToponym = new StreetcodeToponymEntity { StreetcodeId = 1, ToponymId = 1 };

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeContent>(),
                AnyEntityInclude<StreetcodeContent>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper
                .Setup(repo => repo.ToponymRepository.GetFirstOrDefaultAsync(
                    AnyEntityPredicate<Toponym>(),
                    AnyEntityInclude<Toponym>()))
                .ReturnsAsync(toponym);

        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeToponymRepository.Create(
            It.IsAny<StreetcodeToponymEntity>())).Returns(streetcodeToponym);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<StreetcodeToponymEntity>(It.IsAny<CreateStreetcodeToponymRequestDto>())).Returns(streetcodeToponym);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreateStreetcodeToponymRequestDto GetValidCreateStreetcodeToponymRequest()
    {
        return new(
            StreetcodeId: 1,
            ToponymId: 1);
    }
}
