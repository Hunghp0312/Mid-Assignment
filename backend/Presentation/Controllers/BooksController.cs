using BLL.DTOs.BookDTO;
using BLL.Services.Interfaces;
using Common.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooks([FromQuery] int? pageIndex, [FromQuery] int? pageSize, [FromQuery] BookFilterParams filterParams)
    {
        if (pageSize.HasValue && pageIndex.HasValue)
        {
            var bookPaging = await _bookService.GetAllWithPaginationAndFilter(filterParams, pageIndex.Value, pageSize.Value);

            return Ok(bookPaging);
        }
        var books = await _bookService.GetAllAsync();

        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return Ok(book);
    }

    [HttpPost]
    [Authorize(Roles = "SuperUser")]
    public async Task<IActionResult> AddBook([FromForm] BookCreateDTO book)
    {
        var newBook = await _bookService.AddAsync(book);
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperUser")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromForm] BookUpdateDTO book)
    {
        var updatedBook = await _bookService.Update(id, book);
        return Ok(updatedBook);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperUser")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        await _bookService.Remove(id);
        return NoContent();
    }
}
