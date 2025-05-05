using System.Security.Claims;
using BLL.CustomException;
using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/book-borrowing-requests")]
[ApiController]
public class BookBorrowingRequestController : ControllerBase
{
    private readonly IBookBorrowingRequestService _bookBorrowingRequestService;
    public BookBorrowingRequestController(IBookBorrowingRequestService bookBorrowingRequestService)
    {
        _bookBorrowingRequestService = bookBorrowingRequestService;
    }
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> AddBookBorrowingRequest([FromBody] BookBorrowingRequestRequestDTO bookBorrowing)
    {
        var requestorId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new NotFoundException("User not found"));
        await _bookBorrowingRequestService.AddBookBorrowingRequest(bookBorrowing, requestorId);
        return Ok();
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "SuperUser")]
    public async Task<IActionResult> ApproveBookBorrowingRequest(Guid id)
    {
        var approverId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new NotFoundException("User not found"));
        await _bookBorrowingRequestService.ApproveBookBorrowingRequest(approverId, id);
        return Ok();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "SuperUser")]
    public async Task<IActionResult> RejectBookBorrowingRequest(Guid id)
    {
        var approverId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new NotFoundException("User not found"));
        await _bookBorrowingRequestService.RejectBookBorrowingRequest(approverId, id);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAllBookBorrowingRequests([FromQuery] int? pageIndex, [FromQuery] int? pageSize, [FromQuery]string? status)
    {
        if (pageSize.HasValue && pageIndex.HasValue)
        {
            var bookBorrowingRequestPaging = await _bookBorrowingRequestService.GetAllWithPaginationAsync(status, pageIndex.Value, pageSize.Value);
            return Ok(bookBorrowingRequestPaging);
        }
        var bookBorrowingRequests = await _bookBorrowingRequestService.GetAllAsync();
        return Ok(bookBorrowingRequests);
    }

}
