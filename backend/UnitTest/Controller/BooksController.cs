using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTOs.BookDTO;
using BLL.DTOs.GenericDTO;
using BLL.Services.Interfaces;
using Common.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTest.Controller
{
    [TestFixture]
    public class BooksControllerTest
    {
        private Mock<IBookService> _mockBookService;
        private BooksController _booksController;
        private List<BookResponseDTO> _mockBooks;
        private PaginatedList<BookResponseDTO> _mockPaginatedBooks;
        private BookCreateDTO _mockBookCreateDTO;
        private BookResponseDTO _mockCreatedBook;
        private BookUpdateDTO _mockBookUpdateDTO;
        private BookResponseDTO _mockUpdatedBook;

        [SetUp]
        public void SetUp()
        {
            _mockBookService = new Mock<IBookService>();
            _booksController = new BooksController(_mockBookService.Object);

            // Mock data for books
            var categoryId = Guid.NewGuid();
            _mockBooks = new List<BookResponseDTO>
            {
                new BookResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 1",
                    Author = "Author 1",
                    Description = "Description 1",
                    Quantity = 10,
                    Available = 5,
                    ISBN = "1234567890",
                    PublishedDate = DateTime.UtcNow.AddYears(-1),
                    CategoryId = categoryId,
                    CategoryName = "Category 1"
                },
                new BookResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 2",
                    Author = "Author 2",
                    Description = "Description 2",
                    Quantity = 8,
                    Available = 3,
                    ISBN = "0987654321",
                    PublishedDate = DateTime.UtcNow.AddYears(-2),
                    CategoryId = categoryId,
                    CategoryName = "Category 1"
                }
            };

            // Mock data for paginated books
            _mockPaginatedBooks = new PaginatedList<BookResponseDTO>(
                _mockBooks,
                _mockBooks.Count,
                pageIndex: 1,
                pageSize: 10
            );

            // Mock data for book creation
            _mockBookCreateDTO = new BookCreateDTO
            {
                Title = "New Book",
                Author = "New Author",
                Description = "New Description",
                Quantity = 5,
                ISBN = "1122334455",
                PublishedDate = DateTime.UtcNow,
                CategoryId = categoryId
            };

            _mockCreatedBook = new BookResponseDTO
            {
                Id = Guid.NewGuid(),
                Title = _mockBookCreateDTO.Title,
                Author = _mockBookCreateDTO.Author,
                Description = _mockBookCreateDTO.Description,
                Quantity = _mockBookCreateDTO.Quantity,
                Available = _mockBookCreateDTO.Quantity,
                ISBN = _mockBookCreateDTO.ISBN,
                PublishedDate = _mockBookCreateDTO.PublishedDate,
                CategoryId = _mockBookCreateDTO.CategoryId,
                CategoryName = "Category 1"
            };

            // Mock data for book update
            _mockBookUpdateDTO = new BookUpdateDTO
            {
                Title = "Updated Book",
                Author = "Updated Author",
                Description = "Updated Description",
                Quantity = 7,
                ISBN = "5544332211",
                PublishedDate = DateTime.UtcNow.AddYears(-1),
                CategoryId = categoryId
            };

            _mockUpdatedBook = new BookResponseDTO
            {
                Id = _mockBooks[0].Id,
                Title = _mockBookUpdateDTO.Title,
                Author = _mockBookUpdateDTO.Author,
                Description = _mockBookUpdateDTO.Description,
                Quantity = _mockBookUpdateDTO.Quantity,
                Available = _mockBookUpdateDTO.Quantity,
                ISBN = _mockBookUpdateDTO.ISBN,
                PublishedDate = _mockBookUpdateDTO.PublishedDate,
                CategoryId = _mockBookUpdateDTO.CategoryId,
                CategoryName = "Category 1"
            };

            // Mock service methods
            _mockBookService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(_mockBooks);

            _mockBookService.Setup(service => service.GetAllWithPaginationAndFilter(It.IsAny<BookFilterParams>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_mockPaginatedBooks);

            _mockBookService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockBooks.Find(b => b.Id == id));

            _mockBookService.Setup(service => service.AddAsync(It.IsAny<BookCreateDTO>()))
                .ReturnsAsync(_mockCreatedBook);

            _mockBookService.Setup(service => service.Update(It.IsAny<Guid>(), It.IsAny<BookUpdateDTO>()))
                .ReturnsAsync(_mockUpdatedBook);

            _mockBookService.Setup(service => service.Remove(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task GetAllBooks_WithPagination_ReturnsPaginatedList()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;
            var filterParams = new BookFilterParams { Query = "test" };

            // Act
            var result = await _booksController.GetAllBooks(pageIndex, pageSize, filterParams) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockPaginatedBooks));
            _mockBookService.Verify(service => service.GetAllWithPaginationAndFilter(filterParams, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllBooks_WithoutPagination_ReturnsAllBooks()
        {
            // Act
            var result = await _booksController.GetAllBooks(null, null, null) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockBooks));
            _mockBookService.Verify(service => service.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetBookById_ValidId_ReturnsBook()
        {
            // Arrange
            var bookId = _mockBooks[0].Id;

            // Act
            var result = await _booksController.GetBookById(bookId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockBooks[0]));
            _mockBookService.Verify(service => service.GetByIdAsync(bookId), Times.Once);
        }

        [Test]
        public async Task AddBook_ValidRequest_ReturnsCreatedBook()
        {
            // Act
            var result = await _booksController.AddBook(_mockBookCreateDTO) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.Value, Is.EqualTo(_mockCreatedBook));
            Assert.That(result.ActionName, Is.EqualTo(nameof(_booksController.GetBookById)));
            _mockBookService.Verify(service => service.AddAsync(_mockBookCreateDTO), Times.Once);
        }

        [Test]
        public async Task UpdateBook_ValidRequest_ReturnsUpdatedBook()
        {
            // Arrange
            var bookId = _mockBooks[0].Id;

            // Act
            var result = await _booksController.UpdateBook(bookId, _mockBookUpdateDTO) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockUpdatedBook));
            _mockBookService.Verify(service => service.Update(bookId, _mockBookUpdateDTO), Times.Once);
        }

        [Test]
        public async Task DeleteBook_ValidId_ReturnsNoContent()
        {
            // Arrange
            var bookId = _mockBooks[0].Id;

            // Act
            var result = await _booksController.DeleteBook(bookId) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
            _mockBookService.Verify(service => service.Remove(bookId), Times.Once);
        }
    }
}