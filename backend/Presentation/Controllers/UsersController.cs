using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }
    [HttpGet("{id}/book-borrowing-requests")]
    [Authorize]
    public async Task<IActionResult> GetAllBookBorrowingRequestByUser(Guid id, [FromQuery] string? status, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
    {
        var userBookBorrowing = await _userService.GetAllBookBorrowingRequestByUser(id, status, pageIndex, pageSize);

        return Ok(userBookBorrowing);
    }
}
