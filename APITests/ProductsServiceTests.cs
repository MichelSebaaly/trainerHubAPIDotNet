using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Data.Entities;

namespace APITests
{
    public class ProductsServiceTests
    {
        private readonly IProgramsRepository _programsRepository;
        private readonly IProgramsService _programsService;

        private readonly Mock<IProgramsRepository> _programsRepositoryMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IFormFile> _formFileMock;
        private readonly IFixture _fixture;

        public ProductsServiceTests()
        {
            _fixture = new Fixture();
            _programsRepositoryMock = new Mock<IProgramsRepository>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _formFileMock = new Mock<IFormFile>();

            _programsRepository = _programsRepositoryMock.Object;

            _webHostEnvironmentMock.Setup(env => env.WebRootPath).Returns("C:\\fakepath\\wwwroot");

            SetupFakeIFormFile();

            _programsService = new ProgramsService(_programsRepository, _webHostEnvironmentMock.Object, _httpContextAccessorMock.Object);
        }

        private void SetupFakeIFormFile()
        {
            string content = "Hello World from a fake file";
            string fileName = "testfile.txt";
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            _formFileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            _formFileMock.Setup(f => f.FileName).Returns(fileName);
            _formFileMock.Setup(f => f.Length).Returns(ms.Length);
        }

        #region AddProgram
        [Fact]
        public async Task AddProgram_WhenProgramIsNull_ThrowsArgumentNullException()
        {
            ProgramAddRequest request = null;

            Func<Task> action = async () =>
            {
                await _programsService.AddProgram(request, "trainer");
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddProgram_WhenForbidenUserIsAdding_ThrowsUnAuthorized()
        {
            ProgramAddRequest request = _fixture.Build<ProgramAddRequest>()
                .With(temp => temp.File_URL, _formFileMock.Object)
                .With(temp => temp.Cover_Photo, _formFileMock.Object)
                .Create();
            string role = "other";

            Func<Task> action = async () =>
            {
                await _programsService.AddProgram(request, role); 
            };

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task AddProgram_WhenProperDetailsAreAdded_Success()
        {
            ProgramAddRequest request = _fixture.Build<ProgramAddRequest>()
                .With(temp => temp.File_URL, _formFileMock.Object)
                .With(temp => temp.Cover_Photo, _formFileMock.Object)
                .Create();
            string role = "trainer";

            await _programsService.AddProgram(request,role);

            _programsRepositoryMock.Verify(repo => repo.AddProgram(It.IsAny<Program>()), Times.Once());
        }
        #endregion
    }
}
