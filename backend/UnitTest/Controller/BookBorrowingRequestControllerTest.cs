using System.Security.Claims;
using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.DTOs.GenericDTO;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace UnitTest.Controller
{
    [TestFixture]
    public class BookBorrowingRequestControllerTest
    {
        private Mock<IBookBorrowingRequestService> _mockService;
        private BookBorrowingRequestController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IBookBorrowingRequestService>();
            _controller = new BookBorrowingRequestController(_mockService.Object);

            // Mock user claims for authorization
            var claims = new List<Claim>
            {
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Test]
        public async Task AddBookBorrowingRequest_ValidRequest_ReturnsOk()
        {
            // Arrange
            var requestDto = new BookBorrowingRequestRequestDTO
            {
                BookIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockService.Setup(service => service.AddBookBorrowingRequest(requestDto, It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddBookBorrowingRequest(requestDto) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            _mockService.Verify(service => service.AddBookBorrowingRequest(requestDto, It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task ApproveBookBorrowingRequest_ValidRequest_ReturnsOk()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _mockService.Setup(service => service.ApproveBookBorrowingRequest(It.IsAny<Guid>(), requestId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ApproveBookBorrowingRequest(requestId) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            _mockService.Verify(service => service.ApproveBookBorrowingRequest(It.IsAny<Guid>(), requestId), Times.Once);
        }

        [Test]
        public async Task RejectBookBorrowingRequest_ValidRequest_ReturnsOk()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _mockService.Setup(service => service.RejectBookBorrowingRequest(It.IsAny<Guid>(), requestId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RejectBookBorrowingRequest(requestId) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            _mockService.Verify(service => service.RejectBookBorrowingRequest(It.IsAny<Guid>(), requestId), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequests_WithPagination_ReturnsPaginatedList()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;
            var status = "Waiting";

            var paginatedList = new PaginatedList<BookBorrowingRequestResponseDTO>(
                new List<BookBorrowingRequestResponseDTO>
                {
                    new BookBorrowingRequestResponseDTO { Id = Guid.NewGuid(), RequestorName = "User 1" },
                    new BookBorrowingRequestResponseDTO { Id = Guid.NewGuid(), RequestorName = "User 2" }
                },
                2,
                pageIndex,
                pageSize
            );

            _mockService.Setup(service => service.GetAllWithPaginationAsync(status, pageIndex, pageSize))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetAllBookBorrowingRequests(pageIndex, pageSize, status) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(paginatedList));
            _mockService.Verify(service => service.GetAllWithPaginationAsync(status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequests_WithoutPagination_ReturnsAllRequests()
        {
            // Arrange
            var requests = new List<BookBorrowingRequestResponseDTO>
            {
                new BookBorrowingRequestResponseDTO { Id = Guid.NewGuid(), RequestorName = "User 1" },
                new BookBorrowingRequestResponseDTO { Id = Guid.NewGuid(), RequestorName = "User 2" }
            };

            _mockService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(requests);

            // Act
            var result = await _controller.GetAllBookBorrowingRequests(null, null, null) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(requests));
            _mockService.Verify(service => service.GetAllAsync(), Times.Once);
        }
    }
}