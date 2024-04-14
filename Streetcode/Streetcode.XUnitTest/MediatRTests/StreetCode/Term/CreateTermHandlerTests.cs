﻿using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Term;

public class CreateTermHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;
    private const int MINLENGTH = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateTermHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateTermRequest();
        SetupMock(request, SUCCESSFULSAVE, isUniqueTermTitle: true);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InvalidCreateTermCommand_WithNotUniqueTermTitle_ShouldReturnSingleError()
    {
        // Arrange
        var request = GetValidCreateTermRequest();
        SetupMock(request, SUCCESSFULSAVE, isUniqueTermTitle: false);
        string expectedErrorMsg = string.Format(
            ErrorMessages.PropertyMustBeUnique,
            nameof(request.Title),
            request.Title,
            typeof(TermEntity).Name);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMsg);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateTermRequest();
        var expectedType = typeof(Result<CreateTermResponseDto>);
        SetupMock(request, SUCCESSFULSAVE, isUniqueTermTitle: true);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateTermRequest();
        SetupMock(request, FAILEDSAVE, isUniqueTermTitle: true);

        var handler = CreateHandler();
        var command = new CreateTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateTermRequest();
        SetupMock(request, SUCCESSFULSAVE, isUniqueTermTitle: true);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private CreateTermHandler CreateHandler()
    {
        return new CreateTermHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateTermRequestDto request, int saveChangesAsyncResult, bool isUniqueTermTitle)
    {
        var validTerm = new TermEntity
        {
            Id = 1,
            Title = new string('a', MINLENGTH),
            Description = new string('a', MINLENGTH)
        };
        TermEntity? term = !isUniqueTermTitle ? validTerm : null;

        var dtoResponseTerm = new CreateTermResponseDto(
                Id: 1,
                Title: new string('a', MINLENGTH),
                Description: new string('a', MINLENGTH));

        _mockRepositoryWrapper
            .Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<TermEntity>(),
                AnyEntityInclude<TermEntity>()))
            .ReturnsAsync(term);

        _mockRepositoryWrapper.Setup(repo => repo.TermRepository.Create(
            It.IsAny<TermEntity>())).Returns(validTerm);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<TermEntity>(It.IsAny<CreateTermRequestDto>())).Returns(validTerm);
        _mockMapper
            .Setup(m => m.Map<CreateTermResponseDto>(It.IsAny<TermEntity>())).Returns(dtoResponseTerm);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreateTermRequestDto GetValidCreateTermRequest()
    {
        return new(
            Title: new string('a', MINLENGTH),
            Description: new string('a', MINLENGTH));
    }
}