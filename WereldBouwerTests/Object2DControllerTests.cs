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
    public class Object2DControllerTests
    {
        private readonly Mock<IObject2DRepository> _mockRepo;
        private readonly Mock<ILogger<Object2DController>> _mockLogger;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Object2DController _controller;

        public Object2DControllerTests()
        {
            _mockRepo = new Mock<IObject2DRepository>();
            _mockLogger = new Mock<ILogger<Object2DController>>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new Object2DController(_mockRepo.Object, _mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetByEnvironmentId_ReturnsAllObject2Ds()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            var objects = new List<Object2D>
            {
                new Object2D { id = Guid.NewGuid(), environmentId = environmentId, prefabId = "Prefab1" },
                new Object2D { id = Guid.NewGuid(), environmentId = environmentId, prefabId = "Prefab2" }
            };
            _mockRepo.Setup(repo => repo.GetByEnvironmentIdAsync(environmentId)).ReturnsAsync(objects);

            // Act
            var result = await _controller.GetByEnvironmentId(environmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Object2D>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task AddObject2D_CreatesNewObject2D()
        {
            // Arrange
            var object2D = new Object2D { id = Guid.NewGuid(), environmentId = Guid.NewGuid(), prefabId = "Prefab1" };
            _mockRepo.Setup(repo => repo.AddObject2DAsync(It.IsAny<Object2D>())).ReturnsAsync(object2D);

            // Act
            var result = await _controller.AddObject2D(object2D);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Object2D>(okResult.Value);
            Assert.Equal(object2D.prefabId, returnValue.prefabId);
        }

        [Fact]
        public async Task AddObject2D_MissingPrefabId_ReturnsBadRequest()
        {
            // Arrange
            var object2D = new Object2D { id = Guid.NewGuid(), environmentId = Guid.NewGuid(), prefabId = null };

            // Act
            var result = await _controller.AddObject2D(object2D);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("prefabId"));
        }

        [Fact]
        public async Task UpdateObject2D_UpdatesExistingObject2D()
        {
            // Arrange
            var object2D = new Object2D { id = Guid.NewGuid(), environmentId = Guid.NewGuid(), prefabId = "Prefab1" };
            _mockRepo.Setup(repo => repo.UpdateObject2DAsync(It.IsAny<Object2D>())).ReturnsAsync(object2D);

            // Act
            var result = await _controller.UpdateObject2DAsync(object2D);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Object2D>(okResult.Value);
            Assert.Equal(object2D.prefabId, returnValue.prefabId);
        }

        [Fact]
        public async Task DeleteObject2D_ExistingObject2D_ReturnsNoContent()
        {
            // Arrange
            var object2DId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.DeleteObjectAsync(object2DId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteObject2D(object2DId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAllByEnvironmentId_DeletesAllObject2Ds()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.DeleteAllByEnvironmentIdAsync(environmentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAllByEnvironmentId(environmentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
