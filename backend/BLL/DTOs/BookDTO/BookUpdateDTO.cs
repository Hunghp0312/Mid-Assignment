using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.BookDTO;

public class BookUpdateDTO
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Author is required")]
    [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters")]
    public required string Author { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
    public int Quantity { get; set; }

    [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "ISBN must be in valid format")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be 10 or 13 characters")]
    public string ISBN { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime PublishedDate { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }
}
