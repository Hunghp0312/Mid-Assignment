using BLL.CustomException;
using BLL.Services.Implementations;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Moq;

namespace UnitTest.Service
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IBookBorrowingRequestRepository> _mockBookBorrowingRequestRepository;
        private UserService _userService;
        private User _testUser;
        private List<BookBorrowingRequest> _testBookBorrowingRequests;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockBookBorrowingRequestRepository = new Mock<IBookBorrowingRequestRepository>();

            // Create test user
            _testUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St",
                Role = UserRole.User,
                PasswordHash = "hashedpassword",
                PasswordSalt = "salt"
            };

            // Create test book borrowing requests
            var bookId1 = Guid.NewGuid();
            var bookId2 = Guid.NewGuid();

            _testBookBorrowingRequests = new List<BookBorrowingRequest>
            {
                new BookBorrowingRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorId = _testUser.Id,
                    Requestor = _testUser,
                    RequestDate = DateTime.UtcNow.AddDays(-5),
                    Status = BookBorrowingRequestStatus.Approved,
                    ApproverId = Guid.NewGuid(),
                    Approver = new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Admin",
                        LastName = "User",
                        PasswordHash = "hashedpassword",
                        PasswordSalt = "salt",
                        Username = "adminuser",
                    },
                    BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>
                    {
                        new BookBorrowingRequestDetail
                        {
                            BookId = bookId1,
                            Book = new Book
                            {
                                Id = bookId1,
                                Title = "Test Book 1",
                                Author = "Test Author 1",
                                Description = "Test Description 1",
                                CategoryId = Guid.NewGuid()
                            }
                        }
                    }
                },
                new BookBorrowingRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorId = _testUser.Id,
                    Requestor = _testUser,
                    RequestDate = DateTime.UtcNow.AddDays(-2),
                    Status = BookBorrowingRequestStatus.Waiting,
                    BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>
                    {
                        new BookBorrowingRequestDetail
                        {
                            BookId = bookId2,
                            Book = new Book
                            {
                                Id = bookId2,
                                Title = "Test Book 2",
                                Author = "Test Author 2",
                                Description = "Test Description 2",
                                CategoryId = Guid.NewGuid()
                            }
                        }
                    }
                }
            };

            // Setup repository methods
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(_testUser.Id))
                .ReturnsAsync(_testUser);

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id != _testUser.Id)))
                .ReturnsAsync((User)null);

            _mockBookBorrowingRequestRepository.Setup(repo =>
                    repo.GetAllBookBorrowingRequestByUser(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()))
                .ReturnsAsync((_testBookBorrowingRequests, _testBookBorrowingRequests.Count));

            // Initialize service
            _userService = new UserService(_mockUserRepository.Object, _mockBookBorrowingRequestRepository.Object);
        }

        [Test]
        public async Task GetUserByIdAsync_ValidId_ReturnsUserDTO()
        {
            // Act
            var result = await _userService.GetUserByIdAsync(_testUser.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(_testUser.Id));
                Assert.That(result.Username, Is.EqualTo(_testUser.Username));
                Assert.That(result.FirstName, Is.EqualTo(_testUser.FirstName));
                Assert.That(result.LastName, Is.EqualTo(_testUser.LastName));
            });

            // Verify repository call
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(_testUser.Id), Times.Once);
        }

        [Test]
        public void GetUserByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(
                async () => await _userService.GetUserByIdAsync(invalidId));
            Assert.That(ex.Message, Is.EqualTo($"User with id {invalidId} not found"));

            // Verify repository call
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(invalidId), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_ValidId_ReturnsPaginatedList()
        {
            // Arrange
            var userId = _testUser.Id;
            var pageIndex = 1;
            var pageSize = 10;
            string status = null; // All statuses

            // Act
            var result = await _userService.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Items, Is.Not.Null);
                Assert.That(result.TotalCount, Is.EqualTo(_testBookBorrowingRequests.Count));
                Assert.That(result.PageIndex, Is.EqualTo(pageIndex));
                Assert.That(result.PageSize, Is.EqualTo(pageSize));
            });

            // Verify repository call
            _mockBookBorrowingRequestRepository.Verify(repo =>
                repo.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_WithStatusFilter_ReturnsPaginatedList()
        {
            // Arrange
            var userId = _testUser.Id;
            var pageIndex = 1;
            var pageSize = 10;
            var status = "Waiting";

            // Act
            var result = await _userService.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);

            // Verify repository call
            _mockBookBorrowingRequestRepository.Verify(repo =>
                repo.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_ValidIdButNoRequests_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var userId = _testUser.Id;
            var pageIndex = 1;
            var pageSize = 10;
            var status = "Rejected"; // No rejected requests in our test data

            _mockBookBorrowingRequestRepository.Setup(repo =>
                    repo.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize))
                .ReturnsAsync((new List<BookBorrowingRequest>(), 0));

            // Act
            var result = await _userService.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Items, Is.Not.Null);
                Assert.That(result.TotalCount, Is.EqualTo(0));
                Assert.That(result.PageIndex, Is.EqualTo(pageIndex));
                Assert.That(result.PageSize, Is.EqualTo(pageSize));
            });

            // Verify repository call
            _mockBookBorrowingRequestRepository.Verify(repo =>
                repo.GetAllBookBorrowingRequestByUser(userId, status, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestByUser_NonExistentUser_StillCallsRepository()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;
            string status = null;

            // Act
            var result = await _userService.GetAllBookBorrowingRequestByUser(nonExistentUserId, status, pageIndex, pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);

            // Verify repository call - the service doesn't verify user existence for this method
            _mockBookBorrowingRequestRepository.Verify(repo =>
                repo.GetAllBookBorrowingRequestByUser(nonExistentUserId, status, pageIndex, pageSize), Times.Once);
        }
    }
}