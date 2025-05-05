using BLL.CustomException;
using BLL.DTOs.BookDTO;
using BLL.Extensions;
using BLL.DTOs.GenericDTO;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Common.Parameters;

namespace BLL.Services.Implementations;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;

    public BookService(IBookRepository bookRepository, ICategoryRepository categoryRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<BookResponseDTO> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Book with Id {id} not found");

        return book.ToResponseDTO();
    }

    public async Task<IEnumerable<BookResponseDTO>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        var bookResponseDTOs = books.Select(x => x.ToResponseDTO()).ToList();

        return bookResponseDTOs;
    }

    public async Task<PaginatedList<BookResponseDTO>> GetAllWithPaginationAndFilter(BookFilterParams filterParams, int pageIndex, int pageSize)
    {
        var (books, count) = await _bookRepository.GetAllWithPaging(filterParams, pageIndex, pageSize);
        var bookResponseDTOs = books.Select(x => x.ToResponseDTO()).ToList();
        var paginatedBooks = new PaginatedList<BookResponseDTO>(bookResponseDTOs, count, pageIndex, pageSize);

        return paginatedBooks;
    }

    public async Task<BookResponseDTO> AddAsync(BookCreateDTO bookRequestDTO)
    {
        var category = await _categoryRepository.GetByIdAsync(bookRequestDTO.CategoryId) ?? throw new NotFoundException($"Category with Id {bookRequestDTO.CategoryId} not found");
        var book = bookRequestDTO.ToEntity();
        book.Category = category;
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();

        return book.ToResponseDTO();
    }

    public async Task<BookResponseDTO> Update(Guid id, BookUpdateDTO bookRequestDTO)
    {
        var book = await _bookRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Book with Id {id} not found");
        var category = await _categoryRepository.GetByIdAsync(bookRequestDTO.CategoryId) ?? throw new NotFoundException($"Category with Id {bookRequestDTO.CategoryId} not found");
        var quantityChange = bookRequestDTO.Quantity - book.Quantity;
        if (book.Available + quantityChange < 0)
        {
            throw new BadRequestException("You cannot update the quantity to a value less than the available quantity");
        }
        book.Title = bookRequestDTO.Title;
        book.Author = bookRequestDTO.Author;
        book.Description = bookRequestDTO.Description;
        book.Quantity = bookRequestDTO.Quantity;
        book.Available = book.Available + quantityChange;
        book.ISBN = bookRequestDTO.ISBN;
        book.PublishedDate = bookRequestDTO.PublishedDate;
        book.Category = category;
        _bookRepository.Update(book);
        await _bookRepository.SaveChangesAsync();

        return book.ToResponseDTO();
    }

    public async Task Remove(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Book with Id {id} not found");
        if (book.Quantity != book.Available)
        {
            throw new BadRequestException("You cannot delete a book that is currently borrowed");
        }
        _bookRepository.Remove(book);

        await _bookRepository.SaveChangesAsync();
    }
}
