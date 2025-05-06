using BLL.CustomException;
using BLL.DTOs.CategoryDTO;
using BLL.Services.Implementations;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Moq;

namespace UnitTest.Service
{
    [TestFixture]
    public class CategoryServiceTest
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private CategoryService _categoryService;
        private List<Category> _categoryData;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object);

            // Mock category data
            _categoryData = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Fiction", Description = "Fiction books" },
                new Category { Id = Guid.NewGuid(), Name = "Non-Fiction", Description = "Non-fiction books" }
            };
        }

        [Test]
        public async Task AddAsync_ShouldAddCategory_WhenValidRequest()
        {
            // Arrange
            var categoryRequest = new CategoryRequestDTO
            {
                Name = "Science",
                Description = "Science books"
            };
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryRequest.Name,
                Description = categoryRequest.Description
            };

            _categoryRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Category>())).ReturnsAsync(category);

            // Act
            var result = await _categoryService.AddAsync(categoryRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(categoryRequest.Name));
            Assert.That(result.Description, Is.EqualTo(categoryRequest.Description));
            _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            // Arrange
            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_categoryData);

            // Act
            var result = await _categoryService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(_categoryData.Count));
            _categoryRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllWithPagingAsync_ShouldReturnPaginatedCategories()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;
            _categoryRepositoryMock.Setup(repo => repo.GetCountAsync()).ReturnsAsync(_categoryData.Count);
            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_categoryData);

            // Act
            var result = await _categoryService.GetAllWithPagingAsync(pageIndex, pageSize);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count(), Is.EqualTo(_categoryData.Count));
            Assert.That(result.TotalCount, Is.EqualTo(_categoryData.Count));
            Assert.That(result.PageIndex, Is.EqualTo(pageIndex));
            Assert.That(result.PageSize, Is.EqualTo(pageSize));
            _categoryRepositoryMock.Verify(repo => repo.GetCountAsync(), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = _categoryData[0].Id;
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(_categoryData[0]);

            // Act
            var result = await _categoryService.GetByIdAsync(categoryId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(categoryId));
            Assert.That(result.Name, Is.EqualTo(_categoryData[0].Name));
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId), Times.Once);
        }

        [Test]
        public void GetByIdAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _categoryService.GetByIdAsync(categoryId));
            Assert.That(ex.Message, Does.Contain($"Category with ID {categoryId} not found."));
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = _categoryData[0].Id;
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(_categoryData[0]);

            // Act
            await _categoryService.RemoveAsync(categoryId);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.Remove(_categoryData[0]), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void RemoveAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _categoryService.RemoveAsync(categoryId));
            Assert.That(ex.Message, Does.Contain($"Category with ID {categoryId} not found."));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = _categoryData[0].Id;
            var categoryRequest = new CategoryRequestDTO
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };
            var category = _categoryData[0];
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _categoryService.UpdateAsync(categoryId, categoryRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(categoryRequest.Name));
            Assert.That(result.Description, Is.EqualTo(categoryRequest.Description));
            _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryRequest = new CategoryRequestDTO
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _categoryService.UpdateAsync(categoryId, categoryRequest));
            Assert.That(ex.Message, Does.Contain($"Category with ID {categoryId} not found."));
        }
    }
}