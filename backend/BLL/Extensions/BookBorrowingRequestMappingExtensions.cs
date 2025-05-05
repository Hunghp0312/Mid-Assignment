using BLL.DTOs.BookBorrowingRequestDTO;
using DAL.Entity;

namespace BLL.Extensions;

public static class BookBorrowingRequestMappingExtensions
{
    public static BookBorrowingRequest ToBookBorrowingRequest(this BookBorrowingRequestRequestDTO bookBorrowingRequestDto, Guid requestorId)
    {
        return new BookBorrowingRequest
        {
            Id = Guid.NewGuid(),
            RequestorId = requestorId,
            RequestDate = DateTime.UtcNow,
            Status = BookBorrowingRequestStatus.Waiting
        };
    }
    public static BookBorrowingRequestResponseDTO ToBookBorrowingRequestResponseDTO(this BookBorrowingRequest bookBorrowingRequest)
    {
        return new BookBorrowingRequestResponseDTO
        {
            Id = bookBorrowingRequest.Id,
            RequestorId = bookBorrowingRequest.RequestorId,
            RequestorName = $"{bookBorrowingRequest.Requestor?.FirstName} {bookBorrowingRequest.Requestor?.LastName}" ?? " ",
            RequestorEmail = bookBorrowingRequest.Requestor?.Email ?? " ",
            RequestorPhone = bookBorrowingRequest.Requestor?.PhoneNumber ?? " ",
            RequestDate = bookBorrowingRequest.RequestDate,
            Status = bookBorrowingRequest.Status,
            ApproverName = $"{bookBorrowingRequest.Requestor?.FirstName} {bookBorrowingRequest.Requestor?.LastName}" ?? " ",
            BookDetails = [.. bookBorrowingRequest.BookBorrowingRequestDetails.Select(bookBorrowingRequest => bookBorrowingRequest.Book.ToResponseDTO())]
        };
    }
}
