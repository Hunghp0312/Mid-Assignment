using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.CategoryDTO;

public class CategoryRequestDTO
{
    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(50, ErrorMessage = "Category Name cannot exceed 50 characters")]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Category Description is required")]
    [StringLength(1000, ErrorMessage = "Category Description cannot exceed 1000 characters")]
    public required string Description { get; set; }
}
