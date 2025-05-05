using BLL.DTOs.CategoryDTO;
using BLL.DTOs.GenericDTO;

namespace BLL.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponseDTO> GetByIdAsync(Guid id);
    Task<IEnumerable<CategoryResponseDTO>> GetAllAsync();
    Task<CategoryResponseDTO> AddAsync(CategoryRequestDTO category);
    Task<CategoryResponseDTO> UpdateAsync(Guid id, CategoryRequestDTO category);
    Task RemoveAsync(Guid id);
    Task<PaginatedList<CategoryResponseDTO>> GetAllWithPagingAsync(int pageIndex, int pageSize);
}
