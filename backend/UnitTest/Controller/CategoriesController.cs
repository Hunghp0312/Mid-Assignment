using BLL.DTOs.CategoryDTO;
using BLL.DTOs.GenericDTO;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace UnitTest.Controller
{
    [TestFixture]
    public class CategoriesControllerTest
    {
        private Mock<ICategoryService> _mockCategoryService;
        private CategoriesController _categoriesController;
        private List<CategoryResponseDTO> _mockCategories;
        private PaginatedList<CategoryResponseDTO> _mockPaginatedCategories;
        private CategoryRequestDTO _mockCategoryRequestDTO;
        private CategoryResponseDTO _mockCreatedCategory;
        private CategoryResponseDTO _mockUpdatedCategory;

        [SetUp]
        public void SetUp()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _categoriesController = new CategoriesController(_mockCategoryService.Object);

            // Mock data for categories
            _mockCategories = new List<CategoryResponseDTO>
            {
                new CategoryResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Category 1",
                    Description = "Description 1"
                },
                new CategoryResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Category 2",
                    Description = "Description 2"
                }
            };

            // Mock data for paginated categories
            _mockPaginatedCategories = new PaginatedList<CategoryResponseDTO>(
                _mockCategories,
                _mockCategories.Count,
                pageIndex: 1,
                pageSize: 10
            );

            // Mock data for category creation
            _mockCategoryRequestDTO = new CategoryRequestDTO
            {
                Name = "New Category",
                Description = "New Description"
            };

            _mockCreatedCategory = new CategoryResponseDTO
            {
                Id = Guid.NewGuid(),
                Name = _mockCategoryRequestDTO.Name,
                Description = _mockCategoryRequestDTO.Description
            };

            // Mock data for category update
            _mockUpdatedCategory = new CategoryResponseDTO
            {
                Id = _mockCategories[0].Id,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            // Mock service methods
            _mockCategoryService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(_mockCategories);

            _mockCategoryService.Setup(service => service.GetAllWithPagingAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_mockPaginatedCategories);

            _mockCategoryService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockCategories.Find(c => c.Id == id));

            _mockCategoryService.Setup(service => service.AddAsync(It.IsAny<CategoryRequestDTO>()))
                .ReturnsAsync(_mockCreatedCategory);

            _mockCategoryService.Setup(service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryRequestDTO>()))
                .ReturnsAsync(_mockUpdatedCategory);

            _mockCategoryService.Setup(service => service.RemoveAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task GetAllCategories_WithPagination_ReturnsPaginatedList()
        {
            // Arrange
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var result = await _categoriesController.GetAllCategories(pageIndex, pageSize) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockPaginatedCategories));
            _mockCategoryService.Verify(service => service.GetAllWithPagingAsync(pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetAllCategories_WithoutPagination_ReturnsAllCategories()
        {
            // Act
            var result = await _categoriesController.GetAllCategories(null, null) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockCategories));
            _mockCategoryService.Verify(service => service.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetCategoryById_ValidId_ReturnsCategory()
        {
            // Arrange
            var categoryId = _mockCategories[0].Id;

            // Act
            var result = await _categoriesController.GetCategoryById(categoryId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockCategories[0]));
            _mockCategoryService.Verify(service => service.GetByIdAsync(categoryId), Times.Once);
        }

        [Test]
        public async Task AddCategory_ValidRequest_ReturnsCreatedCategory()
        {
            // Act
            var result = await _categoriesController.AddCategory(_mockCategoryRequestDTO) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.Value, Is.EqualTo(_mockCreatedCategory));
            Assert.That(result.ActionName, Is.EqualTo(nameof(_categoriesController.GetCategoryById)));
            _mockCategoryService.Verify(service => service.AddAsync(_mockCategoryRequestDTO), Times.Once);
        }

        [Test]
        public async Task UpdateCategory_ValidRequest_ReturnsUpdatedCategory()
        {
            // Arrange
            var categoryId = _mockCategories[0].Id;

            // Act
            var result = await _categoriesController.UpdateCategory(categoryId, _mockCategoryRequestDTO) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Value, Is.EqualTo(_mockUpdatedCategory));
            _mockCategoryService.Verify(service => service.UpdateAsync(categoryId, _mockCategoryRequestDTO), Times.Once);
        }

        [Test]
        public async Task DeleteCategory_ValidId_ReturnsNoContent()
        {
            // Arrange
            var categoryId = _mockCategories[0].Id;

            // Act
            var result = await _categoriesController.DeleteCategory(categoryId) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
            _mockCategoryService.Verify(service => service.RemoveAsync(categoryId), Times.Once);
        }
    }
}