using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeCoordinateEntity = Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinates.Types;

public class CreateStreetcodeCoordinateHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateStreetcodeCoordinateHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateStreetcodeCoordinateRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeId()
    {
        // Arrange
        var request = new CreateStreetcodeCoordinateRequestDto(
            StreetcodeId: int.MaxValue,
            Longtitude: 1,
            Latitude: 1);

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStreetcodeCoordinateRequest();
        var expectedType = typeof(Result<CreateStreetcodeCoordinateResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateStreetcodeCoordinateRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreateStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStreetcodeCoordinateRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStreetcodeCoordinateCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private CreateStreetcodeCoordinateHandler CreateHandler()
    {
        return new CreateStreetcodeCoordinateHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateStreetcodeCoordinateRequestDto request, int saveChangesAsyncResult)
    {
        var streetcode = request.StreetcodeId switch
        {
            1 => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        var streetcodeCoordinate = new StreetcodeCoordinateEntity { Id = 1, StreetcodeId = 1, Longtitude = 1, Latitude = 1 };

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeContent>(),
                AnyEntityInclude<StreetcodeContent>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeCoordinateRepository.Create(
            It.IsAny<StreetcodeCoordinateEntity>())).Returns(streetcodeCoordinate);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<StreetcodeCoordinateEntity>(It.IsAny<CreateStreetcodeCoordinateRequestDto>())).Returns(streetcodeCoordinate);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreateStreetcodeCoordinateRequestDto GetValidCreateStreetcodeCoordinateRequest()
    {
        return new(
            StreetcodeId: 1,
            Longtitude: 1,
            Latitude: 1);
    }
}