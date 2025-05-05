namespace DAL.Entity;

public class Book
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
    public Category? Category { get; set; }
    public List<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; } = [];
    
    public List<BookRating> Ratings { get; set; } = [];
    public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Rating) : 0;
}
