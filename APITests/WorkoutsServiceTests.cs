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
        public async Task GetAllWorkouts_WhenUserTriesToAccessAnotherUsersData_ThrowsForbiddenException()
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
        #region AddWorkout

        [Fact]
        public async Task AddWorkout_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            WorkoutAddRequest request = null;

            Func<Task> action = async () =>
            {
                await _workoutsService.AddWorkout(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddWorkout_WhenUserIsNotAuthorized_ThrowUnAuthorizedAccessException()
        {
            _currentUserServiceMock.Setup(u => u.UserId).Returns((int?) null);

            WorkoutAddRequest addRequest = new WorkoutAddRequest()
            {
                Title = "Test",
                Notes = "Notes"
            };

            Func<Task> action = async () =>
            {
                await _workoutsService.AddWorkout(addRequest);
            };

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task AddWorkout_WhenValidWorkout_Success()
        {
            _currentUserServiceMock.Setup(u => u.UserId).Returns(5);

            WorkoutAddRequest addRequest = new WorkoutAddRequest()
            {
                Title = "Test",
                Notes = "Notes"
            };

            await _workoutsService.AddWorkout(addRequest);

            _workoutsRepoMock.Verify(repo => repo.AddWorkout(It.Is<Workout>(w =>
            w.Title == "Test" &&
            w.Notes == "Notes" &&
            w.UserId == 5)), Times.Once);
            _workoutsRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        #endregion
        #region DeleteWorkout
        [Fact]
        public async Task DeleteWorkout_WhenWorkoutNotFound_ThrowsNotFoundException()
        {
            int workoutId = 1;
            _workoutsRepoMock.Setup(repo => repo.GetWorkoutById(It.IsAny<int>())).ReturnsAsync((Workout)null);

            Func<Task> action = async () =>
            {
                await _workoutsService.DeleteWorkout(workoutId);
            };

            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteWorkout_WhenUserDeletingOthersWorkouts_ThrowsForbiddenException()
        {
            int workoutId = 1;
            _currentUserServiceMock.Setup(x => x.UserId).Returns(14);

            _workoutsRepoMock.Setup(repo => repo.GetWorkoutById(It.IsAny<int>())).ReturnsAsync(new Workout()
            {
                Id = workoutId,
                UserId = 10,
                Title = "Full Body",
                Notes = "Next Time better",
                Duration = new TimeSpan(1, 20, 10),
            });

            Func<Task> action = async () =>
            {
                await _workoutsService.DeleteWorkout(workoutId);
            };

            await action.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task DeleteWorkout_WhenRepositoryFails_ReturnsFalse()
        {
            int workoutId = 1;
            _currentUserServiceMock.Setup(x => x.UserId).Returns(14);

            _workoutsRepoMock.Setup(repo => repo.GetWorkoutById(workoutId))
                .ReturnsAsync(new Workout { Id = workoutId, UserId = 14 });

            _workoutsRepoMock.Setup(repo => repo.DeleteWorkout(It.IsAny<Workout>()))
                .ReturnsAsync(false);

            bool result = await _workoutsService.DeleteWorkout(workoutId);

            result.Should().BeFalse();
            _workoutsRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }


        [Fact]
        public async Task DeleteWorkout_WhenProperDetails_Success()
        {
            int workoutId = 1;
            _currentUserServiceMock.Setup(x => x.UserId).Returns(14);
            _workoutsRepoMock.Setup(repo => repo.GetWorkoutById(It.IsAny<int>())).ReturnsAsync(new Workout()
            {
                Id = workoutId,
                UserId = 14,
                Title = "Full Body",
                Notes = "Next Time better",
                Duration = new TimeSpan(1,20,10),
            });

            _workoutsRepoMock.Setup(repo => repo.DeleteWorkout(It.IsAny<Workout>())).ReturnsAsync(true);

            bool isDeleted = await _workoutsService.DeleteWorkout(workoutId);
            
            isDeleted.Should().BeTrue();
            _workoutsRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        #endregion
    }
}
