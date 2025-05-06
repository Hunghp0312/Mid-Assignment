using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.Extensions;
using DAL.Entity;
using BLL.CustomException;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using BLL.DTOs.GenericDTO;
namespace BLL.Services.Implementations;

public class BookBorrowingRequestService : IBookBorrowingRequestService
{
    private readonly IBookBorrowingRequestRepository _bookBorrowingRequestRepository;
    private readonly IBookBorrowingRequestDetailRepository _bookBorrowingRequestDetailRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    public BookBorrowingRequestService(IBookBorrowingRequestRepository bookBorrowingRequestRepository,
        IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository,
        IBookRepository bookRepository,
        IUserRepository userRepository)
    {
        _bookBorrowingRequestRepository = bookBorrowingRequestRepository;
        _bookBorrowingRequestDetailRepository = bookBorrowingRequestDetailRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
    }

    public async Task AddBookBorrowingRequest(BookBorrowingRequestRequestDTO bookBorrowingRequest, Guid requestorId)
    {
        // Check the length of the BookIds list
        if (!bookBorrowingRequest.BookIds.Any())
        {
            throw new BadRequestException("BookIds cannot be empty");
        }
        if(bookBorrowingRequest.BookIds.Count() > 5)
        {
            throw new BadRequestException("You can borrow a maximum of 5 books at a time");
        }
        // Check the total request in month
        var totalRequestInMonth = await _bookBorrowingRequestRepository.GetUserTotalRequestInMonth(requestorId);
        if(totalRequestInMonth >= 3)
        {
            throw new BadRequestException("You can only borrow 3 books in a month");
        }
        
        var bookBorrowing = bookBorrowingRequest.ToBookBorrowingRequest(requestorId);

        foreach (var bookId in bookBorrowingRequest.BookIds)
        {
            // checking book availability
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                throw new NotFoundException($"Book with id {bookId} not found");
            }
            if (book.Available == 0)
            {
                throw new BadRequestException("Book is not available");
            }
            // Update the book's available quantity
            book.Available -= 1;
            _bookRepository.Update(book);
            // Add book borrowing request detail
            var bookBorrowingRequestDetail = new BookBorrowingRequestDetail
            {
                BookId = bookId,
            };
            bookBorrowing.BookBorrowingRequestDetails.Add(bookBorrowingRequestDetail);
        }
        // Add the book borrowing request to the repository
        await _bookBorrowingRequestRepository.AddAsync(bookBorrowing);
        await _bookBorrowingRequestRepository.SaveChangesAsync();
    }

    public async Task ApproveBookBorrowingRequest(Guid approverId, Guid bookBorrowingRequestId)
    {
        var bookBorrowingRequest = await _bookBorrowingRequestRepository.GetByIdAsync(bookBorrowingRequestId) ?? throw new NotFoundException($"Book borrowing request with id {bookBorrowingRequestId} not found");
        var approver = await _userRepository.GetByIdAsync(approverId);
        if (bookBorrowingRequest == null)
        {
            throw new NotFoundException($"Book borrowing request with id {bookBorrowingRequestId} not found");
        }
        if (bookBorrowingRequest.Status != BookBorrowingRequestStatus.Waiting)
        {
            throw new BadRequestException("Book borrowing request is not waiting");
        }
        bookBorrowingRequest.Status = BookBorrowingRequestStatus.Approved;
        bookBorrowingRequest.ApproverId = approverId;
        await _bookBorrowingRequestRepository.SaveChangesAsync();
    }

    public async Task RejectBookBorrowingRequest(Guid approverId, Guid bookBorrowingRequestId)
    {
        var bookBorrowingRequest = await _bookBorrowingRequestRepository.GetByIdAsync(bookBorrowingRequestId) ?? throw new NotFoundException($"Book borrowing request with id {bookBorrowingRequestId} not found");
        var approver = await _userRepository.GetByIdAsync(approverId);
        if (bookBorrowingRequest == null)
        {
            throw new NotFoundException($"Book borrowing request with id {bookBorrowingRequestId} not found");
        }
        if (bookBorrowingRequest.Status != BookBorrowingRequestStatus.Waiting)
        {
            throw new BadRequestException("Book borrowing request is not waiting");
        }
        foreach (var bookBorrowingRequestDetail in bookBorrowingRequest.BookBorrowingRequestDetails)
        {
            var book = bookBorrowingRequestDetail.Book;
            if (book == null)
            {
                throw new NotFoundException($"Book with id {bookBorrowingRequestDetail.BookId} not found");
            }
            // Update the book's available quantity
            book.Available += 1;
            _bookRepository.Update(book);
        }
        bookBorrowingRequest.Status = BookBorrowingRequestStatus.Rejected;
        bookBorrowingRequest.ApproverId = approverId;

        await _bookBorrowingRequestRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<BookBorrowingRequestResponseDTO>> GetAllAsync()
    {
        var bookBorrowingRequests = await _bookBorrowingRequestRepository.GetAllAsync();
        var bookBorrowingRequestResponseDTOs = bookBorrowingRequests.Select(x => x.ToBookBorrowingRequestResponseDTO()).ToList();

        return bookBorrowingRequestResponseDTOs;
    }
    public async Task<PaginatedList<BookBorrowingRequestResponseDTO>> GetAllWithPaginationAsync(string? status, int pageIndex, int pageSize)
    {
        var (bookBorrowingRequests, count) = await _bookBorrowingRequestRepository.GetAllWithPaginationAsync(status, pageIndex, pageSize);
        var bookBorrowingRequestResponseDTOs = bookBorrowingRequests.Select(x => x.ToBookBorrowingRequestResponseDTO()).ToList();
        var paginatedBookBorrowingRequests = new PaginatedList<BookBorrowingRequestResponseDTO>(bookBorrowingRequestResponseDTOs, count, pageIndex, pageSize);

        return paginatedBookBorrowingRequests;
    }
}
