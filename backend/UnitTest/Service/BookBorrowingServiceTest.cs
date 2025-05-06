using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.CustomException;
using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.Services.Implementations;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace UnitTest.Service
{
    [TestFixture]
    public class BookBorrowingRequestServiceTest
    {
        private Mock<IBookBorrowingRequestRepository> _mockBookBorrowingRequestRepository;
        private Mock<IBookBorrowingRequestDetailRepository> _mockBookBorrowingRequestDetailRepository;
        private Mock<IBookRepository> _mockBookRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private BookBorrowingRequestService _bookBorrowingRequestService;
        private List<Book> _mockBooks;
        private List<User> _mockUsers;
        private List<BookBorrowingRequest> _mockRequests;

        [SetUp]
        public void SetUp()
        {
            _mockBookBorrowingRequestRepository = new Mock<IBookBorrowingRequestRepository>();
            _mockBookBorrowingRequestDetailRepository = new Mock<IBookBorrowingRequestDetailRepository>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _mockBooks = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Book 1", Available = 5, Author = "Author1", Description = "Description" },
                new Book { Id = Guid.NewGuid(), Title = "Book 2", Available = 0, Author = "Author1", Description = "Description" },
                new Book { Id = Guid.NewGuid(), Title = "Book 3", Available = 3, Author = "Author1", Description = "Description" }
            };

            _mockUsers = new List<User>
            {
                new User { Id = Guid.NewGuid(), Username = "User1", PasswordHash="b9nX8oPNh4n1VLUcMFuBXaBBfok737Q1RkbphVrQu6g=",PasswordSalt = "C+At2NY4wxdnriGqqg24iQ==", FirstName = "John", LastName= "Doe" },
                new User { Id = Guid.NewGuid(), Username = "Approver1", PasswordHash="OCqOmH72mdBYjZf0amTxF3OWtbesV9Lv4Y1Typ4519w=", PasswordSalt = "7gKSkgQhiq+RSmknIghQ4g==", FirstName = "John", LastName= "Doe" }
            };

            _mockRequests = new List<BookBorrowingRequest>
            {
                new BookBorrowingRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorId = _mockUsers[0].Id,
                    Status = BookBorrowingRequestStatus.Waiting,
                    BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>
                    {
                        new BookBorrowingRequestDetail { BookId = _mockBooks[0].Id, Book = _mockBooks[0] }
                    }
                }
            };

            _mockBookBorrowingRequestRepository.Setup(repo => repo.GetUserTotalRequestInMonth(It.IsAny<Guid>()))
                .ReturnsAsync(1);

            _mockBookBorrowingRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockRequests.FirstOrDefault(r => r.Id == id));

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockBooks.FirstOrDefault(b => b.Id == id));

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockUsers.FirstOrDefault(u => u.Id == id));

            _bookBorrowingRequestService = new BookBorrowingRequestService(
                _mockBookBorrowingRequestRepository.Object,
                _mockBookBorrowingRequestDetailRepository.Object,
                _mockBookRepository.Object,
                _mockUserRepository.Object
            );
        }

        [Test]
        public async Task AddBookBorrowingRequest_ValidRequest_AddsRequest()
        {
            // Arrange
            var requestorId = _mockUsers[0].Id;
            var bookIds = new List<Guid> { _mockBooks[0].Id, _mockBooks[2].Id };
            var requestDto = new BookBorrowingRequestRequestDTO { BookIds = bookIds };

            // Act
            await _bookBorrowingRequestService.AddBookBorrowingRequest(requestDto, requestorId);

            // Assert
            _mockBookBorrowingRequestRepository.Verify(repo => repo.AddAsync(It.IsAny<BookBorrowingRequest>()), Times.Once);
            _mockBookBorrowingRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Exactly(bookIds.Count));
        }

        [Test]
        public void AddBookBorrowingRequest_EmptyBookIds_ThrowsBadRequestException()
        {
            // Arrange
            var requestorId = _mockUsers[0].Id;
            var requestDto = new BookBorrowingRequestRequestDTO { BookIds = new List<Guid>() };

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(() =>
                _bookBorrowingRequestService.AddBookBorrowingRequest(requestDto, requestorId));
            Assert.That(ex.Message, Is.EqualTo("BookIds cannot be empty"));
        }

        [Test]
        public void AddBookBorrowingRequest_TooManyBooks_ThrowsBadRequestException()
        {
            // Arrange
            var requestorId = _mockUsers[0].Id;
            var bookIds = Enumerable.Range(1, 6).Select(_ => Guid.NewGuid()).ToList();
            var requestDto = new BookBorrowingRequestRequestDTO { BookIds = bookIds };

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(() =>
                _bookBorrowingRequestService.AddBookBorrowingRequest(requestDto, requestorId));
            Assert.That(ex.Message, Is.EqualTo("You can borrow a maximum of 5 books at a time"));
        }

        [Test]
        public async Task ApproveBookBorrowingRequest_ValidRequest_ApprovesRequest()
        {
            // Arrange
            var approverId = _mockUsers[1].Id;
            var requestId = _mockRequests[0].Id;

            // Act
            await _bookBorrowingRequestService.ApproveBookBorrowingRequest(approverId, requestId);

            // Assert
            _mockBookBorrowingRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            Assert.That(_mockRequests[0].Status, Is.EqualTo(BookBorrowingRequestStatus.Approved));
            Assert.That(_mockRequests[0].ApproverId, Is.EqualTo(approverId));
        }

        [Test]
        public void ApproveBookBorrowingRequest_RequestNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var approverId = _mockUsers[1].Id;
            var requestId = Guid.NewGuid(); // Non-existent request ID

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(() =>
                _bookBorrowingRequestService.ApproveBookBorrowingRequest(approverId, requestId));
            Assert.That(ex.Message, Is.EqualTo($"Book borrowing request with id {requestId} not found"));
        }

        [Test]
        public async Task RejectBookBorrowingRequest_ValidRequest_RejectsRequest()
        {
            // Arrange
            var approverId = _mockUsers[1].Id;
            var requestId = _mockRequests[0].Id;

            // Act
            await _bookBorrowingRequestService.RejectBookBorrowingRequest(approverId, requestId);

            // Assert
            _mockBookBorrowingRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Once);
            Assert.That(_mockRequests[0].Status, Is.EqualTo(BookBorrowingRequestStatus.Rejected));
            Assert.That(_mockRequests[0].ApproverId, Is.EqualTo(approverId));
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllRequests()
        {
            // Arrange
            _mockBookBorrowingRequestRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(_mockRequests);

            // Act
            var result = await _bookBorrowingRequestService.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(_mockRequests.Count));
        }

        [Test]
        public async Task GetAllWithPaginationAsync_ReturnsPaginatedRequests()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 2;
            _mockBookBorrowingRequestRepository.Setup(repo => repo.GetAllWithPaginationAsync(null, pageIndex, pageSize))
                .ReturnsAsync((_mockRequests, _mockRequests.Count));

            // Act
            var result = await _bookBorrowingRequestService.GetAllWithPaginationAsync(null, pageIndex, pageSize);

            // Assert
            Assert.That(result.Items.Count(), Is.EqualTo(_mockRequests.Count));
            Assert.That(result.TotalCount, Is.EqualTo(_mockRequests.Count));
        }
    }
}