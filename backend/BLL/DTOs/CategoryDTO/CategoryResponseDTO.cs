namespace BLL.DTOs.CategoryDTO;

public class CategoryResponseDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

}
