using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using WereldbouwerAPI;
using WereldbouwerAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace WereldBouwerTests
{
    public class WereldBouwerControllerTests
    {
        private readonly Mock<IWereldBouwerRepository> _mockRepo;
        private readonly Mock<ILogger<WereldBouwerController>> _mockLogger;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly WereldBouwerController _controller;

        public WereldBouwerControllerTests()
        {
            _mockRepo = new Mock<IWereldBouwerRepository>();
            _mockLogger = new Mock<ILogger<WereldBouwerController>>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new WereldBouwerController(_mockRepo.Object, _mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Get_ReturnsAllWereldBouwers()
        {
            // Arrange
            var wereldBouwers = new List<WereldBouwer>
            {
                new WereldBouwer { id = Guid.NewGuid(), name = "Test1" },
                new WereldBouwer { id = Guid.NewGuid(), name = "Test2" }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(wereldBouwers);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<WereldBouwer>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task Get_ById_ReturnsWereldBouwer()
        {
            // Arrange
            var wereldBouwerId = Guid.NewGuid();
            var wereldBouwer = new WereldBouwer { id = wereldBouwerId, name = "Test" };
            _mockRepo.Setup(repo => repo.GetByWereldbouwerIdAsync(wereldBouwerId)).ReturnsAsync(wereldBouwer);

            // Act
            var result = await _controller.Get(wereldBouwerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<WereldBouwer>(okResult.Value);
            Assert.Equal(wereldBouwerId, returnValue.id);
        }

        [Fact]
        public async Task Get_ById_ReturnsNotFound()
        {
            // Arrange
            var wereldBouwerId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetByWereldbouwerIdAsync(wereldBouwerId)).ReturnsAsync((WereldBouwer)null);

            // Act
            var result = await _controller.Get(wereldBouwerId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Get_ByUserId_ReturnsWereldBouwer()
        {
            // Arrange
            var userId = "test-user-id";
            var wereldBouwer = new WereldBouwer { id = Guid.NewGuid(), name = "Test", ownerUserId = userId };
            _mockRepo.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(new List<WereldBouwer> { wereldBouwer });

            // Act
            var result = await _controller.GetWereld(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<WereldBouwer>>(okResult.Value);
            Assert.Single(returnValue); // Controleer of de lijst slechts één item bevat
            Assert.Equal(userId, returnValue[0].ownerUserId); // Controleer de eigenschap van het eerste item
        }

        [Fact]
        public async Task Get_ByUserId_ReturnsNotFound()
        {
            // Arrange
            var userId = "test-user-id";
            _mockRepo.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync((IEnumerable<WereldBouwer>)null);

            // Act
            var result = await _controller.GetWereld(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Post_CreatesNewWereldBouwer()
        {
            // Arrange
            var wereldBouwer = new WereldBouwer { name = "Test" };
            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("test-user-id");
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<WereldBouwer>())).ReturnsAsync(wereldBouwer);

            // Act
            var result = await _controller.Post(wereldBouwer);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnValue = Assert.IsType<WereldBouwer>(createdAtRouteResult.Value);
            Assert.Equal(wereldBouwer.name, returnValue.name);
        }

        [Fact]
        public async Task Put_UpdatesExistingWereldBouwer()
        {
            // Arrange
            var wereldBouwerId = Guid.NewGuid();
            var existingWereldBouwer = new WereldBouwer { id = wereldBouwerId, name = "Existing" };
            var updatedWereldBouwer = new WereldBouwer { id = wereldBouwerId, name = "Updated" };
            _mockRepo.Setup(repo => repo.GetByWereldbouwerIdAsync(wereldBouwerId)).ReturnsAsync(existingWereldBouwer);
            _mockRepo.Setup(repo => repo.UpdateAsync(updatedWereldBouwer)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Put(wereldBouwerId, updatedWereldBouwer);

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnValue = Assert.IsType<WereldBouwer>(okResult.Value);
            Assert.Equal(updatedWereldBouwer.name, returnValue.name);
        }

        [Fact]
        public async Task Put_NonExistingWereldBouwer_ReturnsNotFound()
        {
            // Arrange
            var wereldBouwerId = Guid.NewGuid();
            var updatedWereldBouwer = new WereldBouwer { id = wereldBouwerId, name = "Updated" };
            _mockRepo.Setup(repo => repo.GetByWereldbouwerIdAsync(wereldBouwerId)).ReturnsAsync((WereldBouwer)null);

            // Act
            var result = await _controller.Put(wereldBouwerId, updatedWereldBouwer);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingWereldBouwer_ReturnsOkObjectResult()
        {
            // Arrange
            var wereldBouwerId = Guid.NewGuid();
            var wereldBouwer = new WereldBouwer { id = wereldBouwerId, name = "Test" };
            _mockRepo.Setup(repo => repo.GetByWereldbouwerIdAsync(wereldBouwerId)).ReturnsAsync(wereldBouwer);
            _mockRepo.Setup(repo => repo.DeleteAsync(wereldBouwerId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(wereldBouwerId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        // Additional helper methods for generating test data
        private List<WereldBouwer> GenerateRandomWereldBouwers(int numberOfWereldBouwers)
        {
            var list = new List<WereldBouwer>();

            for (int i = 0; i < numberOfWereldBouwers; i++)
            {
                list.Add(GenerateRandomWereldBouwer($"Random WereldBouwer {i}"));
            }

            return list;
        }

        private WereldBouwer GenerateRandomWereldBouwer(string name)
        {
            var random = new Random();
            return new WereldBouwer
            {
                id = Guid.NewGuid(),
                name = name ?? "Random WereldBouwer",
                maxHeight = random.Next(1, 100),
                maxLength = random.Next(1, 100)
            };
        }
    }
}
