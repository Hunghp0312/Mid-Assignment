namespace BLL.DTOs.BookDTO;

public class BookUpdateDTO
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Description { get; set; }
    public int Quantity { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public Guid CategoryId { get; set; }
}
