using BLL.CustomException;
using BLL.DTOs.BookDTO;
using BLL.Services.Implementations;
using Common.Parameters;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Moq;

namespace UnitTest.Service
{
    [TestFixture]
    public class BookServiceTest
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private BookService _bookService;
        private List<Book> _bookData;
        private List<Category> _categoryData;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _bookService = new BookService(_bookRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Initialize mock data
            var categoryId1 = Guid.NewGuid();
            var categoryId2 = Guid.NewGuid();

            _categoryData = new List<Category>
            {
                new Category { Id = categoryId1, Name = "Fiction", Description = "Fiction books" },
                new Category { Id = categoryId2, Name = "Non-Fiction", Description = "Non-fiction books" }
            };

            _bookData = new List<Book>
            {
                new Book {
                    Id = Guid.NewGuid(),
                    Title = "Test Book 1",
                    Author = "Author 1",
                    Description = "Book description 1",
                    Quantity = 5,
                    Available = 5,
                    ISBN = "1234567890",
                    PublishedDate = new DateTime(2020, 1, 1),
                    CategoryId = categoryId1
                },
                new Book {
                    Id = Guid.NewGuid(),
                    Title = "Test Book 2",
                    Author = "Author 2",
                    Description = "Book description 2",
                    Quantity = 5,
                    Available = 3,
                    ISBN = "0987654321",
                    PublishedDate = new DateTime(2021, 1, 1),
                    CategoryId = categoryId2
                }
            };
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = _bookData[0].Id;
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(_bookData[0]);

            // Act
            var result = await _bookService.GetByIdAsync(bookId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(bookId));
            _bookRepositoryMock.Verify(repo => repo.GetByIdAsync(bookId), Times.Once);
        }

        [Test]
        public void GetByIdAsync_ShouldThrowNotFoundException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _bookService.GetByIdAsync(bookId));

            Assert.That(ex.Message, Does.Contain($"Book with Id {bookId} not found"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllBooks()
        {
            // Arrange
            _bookRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_bookData);

            // Act
            var result = await _bookService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            _bookRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllWithPaginationAndFilter_ShouldReturnPaginatedBooks()
        {
            // Arrange
            var filterParams = new BookFilterParams();
            var pageIndex = 1;
            var pageSize = 10;

            _bookRepositoryMock.Setup(repo => repo.GetAllWithPaging(filterParams, pageIndex, pageSize))
                .ReturnsAsync((_bookData, _bookData.Count));

            // Act
            var result = await _bookService.GetAllWithPaginationAndFilter(filterParams, pageIndex, pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count(), Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.PageIndex, Is.EqualTo(pageIndex));
            Assert.That(result.PageSize, Is.EqualTo(pageSize));
            _bookRepositoryMock.Verify(repo => repo.GetAllWithPaging(filterParams, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task AddAsync_ShouldAddBook_WhenCategoryExists()
        {
            // Arrange
            var categoryId = _categoryData[0].Id;
            var bookCreateDTO = new BookCreateDTO
            {
                Title = "New Book",
                Author = "New Author",
                Description = "New Description",
                CategoryId = categoryId,
                ISBN = "1122334455",
                PublishedDate = DateTime.Now,
                Quantity = 3
            };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(_categoryData[0]);

            _bookRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book b) => b);

            // Act
            var result = await _bookService.AddAsync(bookCreateDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(bookCreateDTO.Title));
            Assert.That(result.Author, Is.EqualTo(bookCreateDTO.Author));
            Assert.That(result.CategoryId, Is.EqualTo(categoryId));

            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void AddAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var bookCreateDTO = new BookCreateDTO
            {
                Title = "New Book",
                Author = "New Author",
                Description = "New Description",
                CategoryId = categoryId
            };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _bookService.AddAsync(bookCreateDTO));

            Assert.That(ex.Message, Does.Contain($"Category with Id {categoryId} not found"));
        }

        [Test]
        public async Task Update_ShouldUpdateBook_WhenBookAndCategoryExist()
        {
            // Arrange
            var book = _bookData[0];
            var categoryId = _categoryData[1].Id;

            var bookUpdateDTO = new BookUpdateDTO
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Description = "Updated Description",
                Quantity = book.Quantity + 2,
                ISBN = "9876543210",
                PublishedDate = DateTime.Now,
                CategoryId = categoryId
            };

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(book.Id))
                .ReturnsAsync(book);

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(_categoryData[1]);

            // Act
            var result = await _bookService.Update(book.Id, bookUpdateDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(bookUpdateDTO.Title));
            Assert.That(result.Author, Is.EqualTo(bookUpdateDTO.Author));
            Assert.That(result.Quantity, Is.EqualTo(bookUpdateDTO.Quantity));
            Assert.That(result.Available, Is.EqualTo(book.Available));
             // Available should increase by 2
            _bookRepositoryMock.Verify(repo => repo.Update(book), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void Update_ShouldThrowNotFoundException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookUpdateDTO = new BookUpdateDTO
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Description = "Updated Description",
                CategoryId = _categoryData[0].Id
            };

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _bookService.Update(bookId, bookUpdateDTO));

            Assert.That(ex.Message, Does.Contain($"Book with Id {bookId} not found"));
        }

        [Test]
        public void Update_ShouldThrowBadRequestException_WhenQuantityDecreaseBelowAvailable()
        {
            // Arrange
            var book = _bookData[1]; // Book with Available = 3, Quantity = 5

            var bookUpdateDTO = new BookUpdateDTO
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Quantity = 1, // Try to decrease quantity below available (3)
                ISBN = book.ISBN,
                PublishedDate = book.PublishedDate,
                CategoryId = book.CategoryId
            };

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(book.Id))
                .ReturnsAsync(book);

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(book.CategoryId))
                .ReturnsAsync(_categoryData[1]);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(async () =>
                await _bookService.Update(book.Id, bookUpdateDTO));

            Assert.That(ex.Message, Does.Contain("You cannot update the quantity to a value less than the available quantity"));
        }

        [Test]
        public void Remove_ShouldThrowBadRequestException_WhenBookIsBorrowed()
        {
            // Arrange
            var book = _bookData[1]; // Book with Available < Quantity
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(book.Id))
                .ReturnsAsync(book);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(async () =>
                await _bookService.Remove(book.Id));

            Assert.That(ex.Message, Does.Contain("You cannot delete a book that is currently borrowed"));
        }

        [Test]
        public async Task Remove_ShouldRemoveBook_WhenBookIsNotBorrowed()
        {
            // Arrange
            var book = _bookData[0]; // Book with Available == Quantity
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(book.Id))
                .ReturnsAsync(book);

            // Act
            await _bookService.Remove(book.Id);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.Remove(book), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}