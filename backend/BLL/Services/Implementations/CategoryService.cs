using BLL.CustomException;
using BLL.DTOs.CategoryDTO;
using BLL.DTOs.GenericDTO;
using BLL.Extensions;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BLL.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDTO> AddAsync(CategoryRequestDTO categoryDto)
    {
        var category = categoryDto.ToEntity();
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        var categoryResponse = category.ToResponseDTO();

        return categoryResponse;
    }

    public async Task<IEnumerable<CategoryResponseDTO>> GetAllAsync()
    {
        var category = await _categoryRepository.GetAllAsync();
        var categoryResponses = category.Select(x => x.ToResponseDTO()).ToList();

        return categoryResponses;
    }

    public async Task<PaginatedList<CategoryResponseDTO>> GetAllWithPagingAsync(int pageIndex, int pageSize)
    {
        var count = await _categoryRepository.GetCountAsync();
        var category = await _categoryRepository.GetAllAsync();
        var categoryResponses = category.Select(x => x.ToResponseDTO()).ToList();
        var paginatedCategories = new PaginatedList<CategoryResponseDTO>(categoryResponses, count, pageIndex, pageSize);
        return paginatedCategories;
    }

    public async Task<CategoryResponseDTO> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Category with ID {id} not found.");
        var categoryResponse = category.ToResponseDTO();

        return categoryResponse;
    }

    public async Task RemoveAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Category with ID {id} not found.");
        _categoryRepository.Remove(category);

        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<CategoryResponseDTO> UpdateAsync(Guid id, CategoryRequestDTO categoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Category with ID {id} not found.");
        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;
        await _categoryRepository.SaveChangesAsync();
        var categoryResponse = category.ToResponseDTO();

        return categoryResponse;
    }
}
