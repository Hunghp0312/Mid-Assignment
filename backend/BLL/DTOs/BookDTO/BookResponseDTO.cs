namespace BLL.DTOs.BookDTO;

public class BookResponseDTO
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Description { get; set; }
    public int Quantity { get; set; }
    public int Available { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
