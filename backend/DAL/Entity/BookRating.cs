namespace DAL.Entity;

public class BookRating
{
    public Guid BookId { get; set; }
    public Book Book { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public int Rating { get; set; } // Example: 1 to 5
    public string? Comment { get; set; }
}
