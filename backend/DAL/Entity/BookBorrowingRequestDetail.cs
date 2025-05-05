namespace DAL.Entity;

public class BookBorrowingRequestDetail
{
    public Guid BookBorrowingRequestId { get; set; }
    public BookBorrowingRequest BookBorrowingRequest { get; set; }
    public Guid BookId { get; set; }
    public Book Book { get; set; }
}
