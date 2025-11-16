
using Data.Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using AutoFixture;

namespace APITests
{
    public class UsersServiceTests
    {
        private readonly IUserService _userService;
        private readonly IUsersRepository _usersRepository;
        private readonly ITokenService _tokenService;
        private readonly IFixture _fixture;

        private readonly Mock<IUsersRepository> _usersRepoMock;
        private readonly Mock<ITokenService> _tokensRepoMock;

        public UsersServiceTests()
        {
            _fixture = new Fixture();
            _usersRepoMock = new Mock<IUsersRepository>();
            _tokensRepoMock = new Mock<ITokenService>();

            _usersRepository = _usersRepoMock.Object;
            _tokenService = _tokensRepoMock.Object;

            _userService = new UsersService(_usersRepository, _tokenService);
        }
        #region register
        [Fact]
        public async Task Register_WhenUserAddRequestIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            UserAddRequest? request = null;

            //Act
            Func<Task> action = async () =>
            {
                await _userService.Register(request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task Register_WhenUserAddRequestIsCorrect_Success()
        {
            UserAddRequest request = _fixture.Build<UserAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Name, "Michel")
                .With(temp => temp.Phone_number, "+96176835385")
                .With(temp => temp.Role, "trainer")
                .Create();
            User user = request.ToUser();
            user.Id = 10;

            _usersRepoMock.Setup(repo => repo.Register(It.IsAny<User>()))
              .Returns<User>(u =>
              {
                  u.Id = 10;
                  return Task.CompletedTask;
              });

            UserResponse expected_user_response = user.ToUserResponse();

            UserResponse response_from_add = await _userService.Register(request);
            expected_user_response.Id = response_from_add.Id;

            response_from_add.Id.Should().Be(10);
            response_from_add.Should().BeEquivalentTo(expected_user_response);
        }
        #endregion
        #region login

        [Fact]
        public async Task Login_WhenCredentialsAreNotProvided_ThrowsArgumentNullException()
        {
            LoginRequest request = null;
            Func<Task> action = async () =>
            {
                await _userService.Login(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Login_WhenEmailNotValid_ThrowsArgumentException()
        {
            LoginRequest request = _fixture.Build<LoginRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            _usersRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User)null);

            Func<Task> action = async () =>
            {
                await _userService.Login(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Login_WhenPasswordIsInvalid_ThrowsArgumentException()
        {
            LoginRequest request = _fixture.Build<LoginRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Password, "Password123")
                .Create();

            _usersRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new User
            {
                Password = BCrypt.Net.BCrypt.HashPassword("password12")
            });

            Func<Task> action = async () =>
            {
                await _userService.Login(request);
            };

            await action.Should().ThrowAsync<ArgumentException>().WithMessage("Invalid Credentials");
        }

        [Fact]
        public async Task Login_WhenValidCredentials_ReturnsToken()
        {
            LoginRequest request = _fixture.Build<LoginRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Password, "Password123")
                .Create();

            _tokensRepoMock.Setup(repo => repo.GenerateJwtToken(It.IsAny<User>())).ReturnsAsync("Fake_JWT");

            _usersRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new User
            {
                Id = 1,
                Email = request.Email,
                Name = "Michel",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = "admin"
            });
            LoginResponse? token = await _userService.Login(request);

            token.Should().NotBeNull();
        }
        #endregion
        #region GetAllUsers

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            List<User> users = new List<User>()
          {
              _fixture.Build<User>()
              .With(temp => temp.Id, 1)
              .With(temp => temp.Name, "Michel")
              .With(temp => temp.Email, "someone@example.com")
              .With(temp => temp.Profile_pic, "Michel.jpg")
              .With(temp => temp.Password, BCrypt.Net.BCrypt.HashPassword("TestPassword"))
              .With(temp => temp.Phone_number, "+96176835385")
              .With(temp => temp.Role, "trainer")
              .With(temp => temp.createdAt, DateTime.UtcNow)
              .With(temp => temp.updatedAt, DateTime.UtcNow)
              .Create()
          };
            _usersRepoMock.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);

            List<UserResponse> users_from_response = await _userService.GetAllUsers();

            users_from_response.Should()
                .BeEquivalentTo(users, options => options.ExcludingMissingMembers());
        }

        #endregion
        #region UpdateUserInfo

        [Fact]
        public async Task UpdateUserInfo_WhenUserUpdateRequestIsNull_ThrowsArgumentNulLException()
        {
            UserUpdateRequest request = null;

            Func<Task> action = async () =>
            {
                await _userService.UpdateUserInfo(1, request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateUserInfo_WhenUserNotFound_ThrowsKeyNotFoundException()
        {
            UserUpdateRequest request = _fixture.Build<UserUpdateRequest>()
                .With(temp => temp.Name, "Michel")
                .With(temp => temp.Email, "someone@gmail.com")
                .With(temp => temp.Phone_number, "+96176835385")
                .Create();

            _usersRepoMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            Func<Task> action = async () =>
            {
                await _userService.UpdateUserInfo(1, request);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateUserInfo_WhenProvidingProperInfo_Success()
        {
            UserUpdateRequest request = _fixture.Build<UserUpdateRequest>()
                .With(temp => temp.Name, "Michel")
                .With(temp => temp.Email, "someone@gmail.com")
                .With(temp => temp.Phone_number, "+96176835385")
                .Create();

            User existingUser = _fixture.Build<User>()
              .With(temp => temp.Id, 1)
              .With(temp => temp.Name, "Miiichel")
              .With(temp => temp.Email, "michelsebaaly4@gmail.com")
              .With(temp => temp.Profile_pic, "Michel.jpg")
              .With(temp => temp.Password, BCrypt.Net.BCrypt.HashPassword("TestPassword"))
              .With(temp => temp.Phone_number, "+96176835345")
              .With(temp => temp.Role, "trainer")
              .With(temp => temp.createdAt, DateTime.UtcNow)
              .With(temp => temp.updatedAt, DateTime.UtcNow)
              .Create();

            _usersRepoMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(existingUser);

            await _userService.UpdateUserInfo(1, request);

            existingUser.Name.Should().Be("Michel");
            existingUser.Email.Should().Be("someone@gmail.com");
            existingUser.Phone_number.Should().Be("+96176835385");

            _usersRepoMock.Verify(repo => repo.SaveChanges(), Times.Once);
        }
        #endregion
        #region ChangePassword

        [Fact]
        public async Task ChangePassword_WhenUserNotFound_ThrowsArgumentException()
        {
            ChangePasswordRequest request = _fixture.Build<ChangePasswordRequest>().Create();

            _usersRepoMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            Func<Task> action = async () =>
            {
                await _userService.ChangePassword(1, request);
            };

            await action.Should().ThrowAsync<ArgumentException>().WithMessage("User not found");
        }

        [Fact]
        public async Task ChangePassword_WhenOldPasswordNotMatch_ThrowsArgumentException()
        {
            ChangePasswordRequest request = _fixture.Build<ChangePasswordRequest>()
                .With(temp => temp.oldPassword, "Pass123")
                .With(temp => temp.newPassword, "Michel123")
                .Create();

            User existingUser = _fixture.Build<User>()
                .With(temp => temp.Id, 1)
                .With(temp => temp.Name, "Miiichel")
                .With(temp => temp.Email, "michelsebaaly4@gmail.com")
                .With(temp => temp.Profile_pic, "Michel.jpg")
                .With(temp => temp.Password, BCrypt.Net.BCrypt.HashPassword("TestPassword"))
                .With(temp => temp.Phone_number, "+96176835345")
                .With(temp => temp.Role, "trainer")
                .With(temp => temp.createdAt, DateTime.UtcNow)
                .With(temp => temp.updatedAt, DateTime.UtcNow)
                .Create();

            _usersRepoMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(existingUser);

            Func<Task> action = async () =>
            {
                await _userService.ChangePassword(1, request);
            };

            await action.Should().ThrowAsync<ArgumentException>().WithMessage("Wrong Password");
        }

        [Fact]
        public async Task ChangePassword_Success()
        {
            ChangePasswordRequest request = _fixture.Build<ChangePasswordRequest>()
                .With(temp => temp.oldPassword, "TestPassword")
                .With(temp => temp.newPassword, "Michel123")
                .Create();

            User existingUser = _fixture.Build<User>()
                .With(temp => temp.Id, 1)
                .With(temp => temp.Name, "Miiichel")
                .With(temp => temp.Email, "michelsebaaly4@gmail.com")
                .With(temp => temp.Profile_pic, "Michel.jpg")
                .With(temp => temp.Password, BCrypt.Net.BCrypt.HashPassword("TestPassword"))
                .With(temp => temp.Phone_number, "+96176835345")
                .With(temp => temp.Role, "trainer")
                .With(temp => temp.createdAt, DateTime.UtcNow)
                .With(temp => temp.updatedAt, DateTime.UtcNow)
                .Create();

            _usersRepoMock.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(existingUser);

            await _userService.ChangePassword(1, request);

            BCrypt.Net.BCrypt.Verify(request.newPassword, existingUser.Password)
                              .Should().BeTrue();
            _usersRepoMock.Verify(repo => repo.SaveChanges(), Times.Once);
        }
        #endregion
    }
}