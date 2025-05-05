using BLL.DTOs.BookDTO;
using DAL.Entity;

namespace BLL.DTOs.BookBorrowingRequestDTO;

public class BookBorrowingRequestResponseDTO
{
    public Guid Id { get; set; }
    public Guid RequestorId { get; set; }
    public string RequestorName { get; set; } = string.Empty;
    public string RequestorEmail { get; set; } = string.Empty;
    public string RequestorPhone { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public BookBorrowingRequestStatus Status { get; set; } // 0 - Waiting, 1 - Approved, 2 - Rejected
    public string ApproverName { get; set; } = string.Empty;
    public List<BookResponseDTO> BookDetails { get; set; } = [];

}
