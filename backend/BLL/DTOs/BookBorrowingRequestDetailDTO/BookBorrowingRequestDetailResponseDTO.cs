using DAL.Entity;

namespace BLL.DTOs.BookBorrowingRequestDetailDTO;

public class BookBorrowingRequestDetailResponseDTO
{
    public Guid BookBorrowingRequestId { get; set; }
    public BookBorrowingRequest BookBorrowingRequest { get; set; }
    public Guid BookId { get; set; }
    public Book Book { get; set; }
}
