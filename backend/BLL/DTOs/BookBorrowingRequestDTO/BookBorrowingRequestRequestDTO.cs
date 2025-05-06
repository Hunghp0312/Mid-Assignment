using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.BookBorrowingRequestDTO;

public class BookBorrowingRequestRequestDTO
{
    public IEnumerable<Guid> BookIds { get; set; } = [];
}
