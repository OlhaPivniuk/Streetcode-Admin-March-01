using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetAll;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class GetAllTeamHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public GetAllTeamHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task GetAllTeam_ShouldReturnOk_WhenTeamExists()
        {
            // Arrange
            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllTeam_ShouldReturnCollectionOfCorrectCount_WhenTeamExists()
        {
            // Arrange
            var mockTeam = GetTeamList();
            var expectedCount = mockTeam.Count;

            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllTeamQuery(), CancellationToken.None);
            var actualCount = result.Value.Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllTeam_MapperShouldMapOnlyOnce_WhenTeamExists()
        {
            // Arrange
            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            await handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

            // Assert
            _mockMapper.Verify(
                mapper =>
                mapper.Map<IEnumerable<TeamMemberDto>>(It.IsAny<IEnumerable<TeamMember>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllTeam_ShouldReturnCollectionOfTeamMemberDto_WhenTeamExists()
        {
            // Arrange
            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

            // Assert
            Assert.IsType<List<TeamMemberDto>>(result.Value);
        }

        [Fact]
        public async Task GetAllTeam_ShouldReturnFail_WhenTeamIsNull()
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetAllTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task GetAllTeam_ShouldLogCorrectErrorMessage_WhenTeamIsNull()
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetAllTeamHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            var expectedError = string.Format(ErrorMessages.EntitiesNotFound, nameof(TeamMember));

            // Act
            var result = await handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(expectedError, result.Errors[0].Message);
        }

        private static List<TeamMember> GetTeamList()
        {
            return new List<TeamMember>
            {
                new TeamMember
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Description = "Software Engineer",
                    IsMain = true,
                    ImageId = 1,
                    TeamMemberLinks = new List<TeamMemberLink>(),
                    Positions = new List<Positions>()
                },
                new TeamMember
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Description = "UI/UX Designer",
                    IsMain = false,
                    ImageId = 2,
                    TeamMemberLinks = new List<TeamMemberLink>(),
                    Positions = new List<Positions>()
                }
            };
        }

        private static List<TeamMemberDto> GetTeamDtoList()
        {
            return new List<TeamMemberDto>
            {
                new TeamMemberDto
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Description = "Software Engineer",
                    IsMain = true,
                    ImageId = 1,
                    TeamMemberLinks = new List<TeamMemberLinkDto>(),
                    Positions = new List<PositionDto>()
                },
                new TeamMemberDto
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Description = "UI/UX Designer",
                    IsMain = false,
                    ImageId = 2,
                    TeamMemberLinks = new List<TeamMemberLinkDto>(),
                    Positions = new List<PositionDto>()
                }
            };
        }

        private void MockMapperSetup()
        {
            _mockMapper.Setup(x => x
                .Map<IEnumerable<TeamMemberDto>>(It.IsAny<IEnumerable<TeamMember>>()))
                .Returns(GetTeamDtoList());
        }

        private void MockRepositorySetupReturnsData()
        {
            _mockRepositoryWrapper.Setup(x => x.TeamRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>,
            IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(GetTeamList());
        }

        private void MockRepositorySetupReturnsNull()
        {
            _mockRepositoryWrapper.Setup(x => x.TeamRepository
           .GetAllAsync(
               It.IsAny<Expression<Func<TeamMember, bool>>>(),
               It.IsAny<Func<IQueryable<TeamMember>,
           IIncludableQueryable<TeamMember, object>>>()))
           .ReturnsAsync((IEnumerable<TeamMember>?)null);
        }
    }
}
