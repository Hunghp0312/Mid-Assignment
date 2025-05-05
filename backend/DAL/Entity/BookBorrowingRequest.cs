namespace DAL.Entity;
public enum BookBorrowingRequestStatus
{
    Waiting = 0,
    Approved = 1,
    Rejected = 2
}

public class BookBorrowingRequest
{
    public Guid Id { get; set; }
    public Guid RequestorId { get; set; }
    public User? Requestor { get; set; }
    public DateTime RequestDate { get; set; }
    public BookBorrowingRequestStatus Status { get; set; } // 0 - Waiting, 1 - Approved, 2 - Rejected
    public Guid? ApproverId { get; set; }
    public User? Approver { get; set; }
    public List<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; } = [];
}
