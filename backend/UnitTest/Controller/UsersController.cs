using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.CustomException;
using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.DTOs.BookDTO;
using BLL.DTOs.GenericDTO;
using BLL.DTOs.UserDTO;
using BLL.Services.Interfaces;
using DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTest.Controller
{
    [TestFixture]
    public class UsersControllerTest
    {
        private Mock<IUserService> _mockUserService;
        private UsersController _usersController;
        private UserResponseDTO _mockUser;
        private PaginatedList<BookBorrowingRequestResponseDTO> _mockBorrowingRequests;

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _usersController = new UsersController(_mockUserService.Object);

            // Mock data for a user
            _mockUser = new UserResponseDTO
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Test Street",
                Role = "User"
            };

            // Mock data for book borrowing requests
            var borrowingRequests = new List<BookBorrowingRequestResponseDTO>
            {
                new BookBorrowingRequestResponseDTO
                {
                    Id = Guid.NewGuid(),
                    RequestorId = _mockUser.Id,
                    RequestorName = $"{_mockUser.FirstName} {_mockUser.LastName}",
                    RequestorEmail = _mockUser.Email,
                    RequestorPhone = _mockUser.PhoneNumber,
                    RequestDate = DateTime.UtcNow.AddDays(-5),
                    Status = BookBorrowingRequestStatus.Waiting,
                    BookDetails = new List<BookResponseDTO>
                    {
                        new BookResponseDTO { Id = Guid.NewGuid(), Title = "Book 1", Author = "Author 1", Description = "Description" }
                    }
                },
                new BookBorrowingRequestResponseDTO
                {
                    Id = Guid.NewGuid(),
                    RequestorId = _mockUser.Id,
                    RequestorName = $"{_mockUser.FirstName} {_mockUser.LastName}",
                    RequestorEmail = _mockUser.Email,
                    RequestorPhone = _mockUser.PhoneNumber,
                    RequestDate = DateTime.UtcNow.AddDays(-10),
                    Status = BookBorrowingRequestStatus.Approved,
                    ApproverName = "Admin User",
                    BookDetails = new List<BookResponseDTO>
                    {
                        new BookResponseDTO { Id = Guid.NewGuid(), Title = "Book 2", Author = "Author 2", Description = "Description" }
                    }
                }
            };

            _mockBorrowingRequests = new PaginatedList<BookBorrowingRequestResponseDTO>(
                borrowingRequests,
                borrowingRequests.Count,
                1,
                5
            );

            // Mock service methods
            _mockUserService.Setup(service => service.GetUserByIdAsync(_mockUser.Id))
                .ReturnsAsync(_mockUser);

            _mockUserService.Setup(service => service.GetUserByIdAsync(It.Is<Guid>(id => id != _mockUser.Id)))
                .ThrowsAsync(new NotFoundException($"User with id {Guid.NewGuid()} not found"));

            _mockUserService.Setup(service => service.GetAllBookBorrowingRequestByUser(
                _mockUser.Id, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_mockBorrowingRequests);
        }

        [Test]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            // Act
            var result = await _usersController.GetUserById(_mockUser.Id) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

            var returnedUser = result.Value as UserResponseDTO;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedUser.Id, Is.EqualTo(_mockUser.Id));
                Assert.That(returnedUser.Username, Is.EqualTo(_mockUser.Username));
                Assert.That(returnedUser.FirstName, Is.EqualTo(_mockUser.FirstName));
                Assert.That(returnedUser.LastName, Is.EqualTo(_mockUser.LastName));
            });

            _mockUserService.Verify(service => service.GetUserByIdAsync(_mockUser.Id), Times.Once);
        }

        [Test]
        public void GetUserById_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Make sure this ID is different from _mockUser.Id
            while (invalidId == _mockUser.Id)
            {
                invalidId = Guid.NewGuid();
            }

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _usersController.GetUserById(invalidId));

            _mockUserService.Verify(service => service.GetUserByIdAsync(invalidId), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_ValidId_ReturnsPaginatedRequests()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 5;
            string status = null;

            // Act
            var result = await _usersController.GetAllBookBorrowingRequestByUser(
                _mockUser.Id, status, pageIndex, pageSize) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

            var returnedRequests = result.Value as PaginatedList<BookBorrowingRequestResponseDTO>;
            Assert.That(returnedRequests, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedRequests.Items.Count(), Is.EqualTo(_mockBorrowingRequests.Items.Count()));
                Assert.That(returnedRequests.PageIndex, Is.EqualTo(pageIndex));
                Assert.That(returnedRequests.PageSize, Is.EqualTo(pageSize));
            });

            _mockUserService.Verify(service =>
                service.GetAllBookBorrowingRequestByUser(_mockUser.Id, status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_WithStatusFilter_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 5;
            string status = "Approved";

            // Act
            await _usersController.GetAllBookBorrowingRequestByUser(
                _mockUser.Id, status, pageIndex, pageSize);

            // Assert
            _mockUserService.Verify(service =>
                service.GetAllBookBorrowingRequestByUser(_mockUser.Id, status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_WithCustomPagination_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var pageIndex = 2;
            var pageSize = 10;
            string status = null;

            // Act
            await _usersController.GetAllBookBorrowingRequestByUser(
                _mockUser.Id, status, pageIndex, pageSize);

            // Assert
            _mockUserService.Verify(service =>
                service.GetAllBookBorrowingRequestByUser(_mockUser.Id, status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public void GetAllBookBorrowingRequestByUser_InvalidUserId_PropagatesException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            while (invalidId == _mockUser.Id)
            {
                invalidId = Guid.NewGuid();
            }

            _mockUserService.Setup(service => service.GetAllBookBorrowingRequestByUser(
                invalidId, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException($"User with id {invalidId} not found"));

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _usersController.GetAllBookBorrowingRequestByUser(invalidId, null, 1, 5));

            _mockUserService.Verify(service =>
                service.GetAllBookBorrowingRequestByUser(invalidId, null, 1, 5), Times.Once);
        }
    }
}