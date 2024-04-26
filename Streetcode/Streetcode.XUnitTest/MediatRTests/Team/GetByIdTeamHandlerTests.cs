using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetById;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class GetByIdTeamHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public GetByIdTeamHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_ShouldReturnOk_IfIdExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsTeam(id);
            MockMapperSetup(id);

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_RepositoryShouldCallGetSingleOrDefaultAsyncOnlyOnce_IfTeamExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsTeam(id);
            MockMapperSetup(id);

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);

            // Assert
            _mockRepositoryWrapper.Verify(
                repo =>
                repo.TeamRepository.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<TeamMember, bool>>>(),
                    It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_MapperShouldCallMapOnlyOnce_IfTeamExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsTeam(id);
            MockMapperSetup(id);

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);

            // Assert
            _mockMapper.Verify(
                mapper => mapper.Map<TeamMemberDto>(It.IsAny<TeamMember>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_ShouldReturnTeamWithCorrectId_IfTeamExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsTeam(id);
            MockMapperSetup(id);

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);

            // Assert
            Assert.Equal(id, result.Value.Id);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_ShouldReturnTeamMemberDto_IfTeamExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsTeam(id);
            MockMapperSetup(id);

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);

            // Assert
            Assert.IsType<TeamMemberDto>(result.Value);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_ShouldReturnFail_WhenTeamIsNotFound(int id)
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdTeam_ShouldLogCorrectErrorMessage_WhenTeamIsNotFound(int id)
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            var expectedMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(TeamMember),
                id);

            // Act
            var result = await handler.Handle(new GetByIdTeamQuery(id), CancellationToken.None);
            var actualMessage = result.Errors[0].Message;

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        private void MockMapperSetup(int id)
        {
            _mockMapper.Setup(x => x
                .Map<TeamMemberDto>(It.IsAny<TeamMember>()))
                .Returns(new TeamMemberDto
                {
                    Id = id,
                    FirstName = "John",
                    LastName = "Doe",
                    Description = "Software Engineer",
                    IsMain = true,
                    ImageId = 1,
                    TeamMemberLinks = new List<TeamMemberLinkDto>(),
                    Positions = new List<PositionDto>()
                });
        }

        private void MockRepositorySetupReturnsTeam(int id)
        {
            _mockRepositoryWrapper.Setup(x => x.TeamRepository
                .GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<TeamMember, bool>>>(),
                    It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
                .ReturnsAsync(new TeamMember
                {
                    Id = id,
                    FirstName = "John",
                    LastName = "Doe",
                    Description = "Software Engineer",
                    IsMain = true,
                    ImageId = 1,
                    TeamMemberLinks = new List<TeamMemberLink>(),
                    Positions = new List<Positions>()
                });
        }

        private void MockRepositorySetupReturnsNull()
        {
            _mockRepositoryWrapper.Setup(x => x.TeamRepository
                .GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<TeamMember, bool>>>(),
                    It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
                .ReturnsAsync((TeamMember)null);
        }
    }
}
