using DAL.Entity;

namespace BLL.DTOs.CategoryDTO;

public class CategoryRequestDTO
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}
