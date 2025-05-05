using BLL.DTOs.CategoryDTO;
using DAL.Entity;

namespace BLL.Extensions;

public static class CategoryMappingExtensions
{
    public static Category ToEntity(this CategoryRequestDTO category)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = category.Name,
            Description = category.Description
        };
    }
    public static CategoryResponseDTO ToResponseDTO(this Category category)
    {
        return new CategoryResponseDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
