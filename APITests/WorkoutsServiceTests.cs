using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Services.CustomExceptions;
using Data.Entities;
using System.Collections.Generic;

namespace APITests
{
    public class WorkoutsServiceTests
    {
        private readonly Mock<IWorkoutsRepository> _workoutsRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly IWorkoutsService _workoutsService;
        private readonly IWorkoutsRepository _workoutsRepository;
        private readonly IFixture _fixture;

        public WorkoutsServiceTests()
        {
            _fixture = new Fixture();
            _workoutsRepoMock = new Mock<IWorkoutsRepository>();
            _workoutsRepository = _workoutsRepoMock.Object;
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _workoutsService = new WorkoutsService(_workoutsRepository, _currentUserServiceMock.Object);
        }

        #region GetAllWorkouts
        [Fact]
        public async Task GetAllWorkouts_WhenUserTriesToAccessAnotherUsersData_ThrowsForbiddenException ()
        {
            _currentUserServiceMock.Setup(x => x.UserId).Returns(5);
            Func<Task> action = async () =>
            {
                await _workoutsService.GetAllWorkouts(10);
            };
            await action.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task GetAllWorkouts_WhenUserIsAuthorized_ReturnsWorkouts()
        {
            List<Workout> workouts = new List<Workout>()
            {
                _fixture.Build<Workout>().Create(),
                _fixture.Build<Workout>().Create()
            };
            _currentUserServiceMock.Setup(x => x.UserId).Returns(5);

            _workoutsRepoMock.Setup(repo => repo.GetAllWorkouts(5)).ReturnsAsync(workouts);

            List<WorkoutResponse> workouts_from_server = await _workoutsService.GetAllWorkouts(5);

            workouts_from_server.Count.Should().Be(2);
        }
        #endregion
    }
}
