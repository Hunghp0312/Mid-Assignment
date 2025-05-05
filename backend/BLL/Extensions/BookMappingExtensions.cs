using BLL.DTOs.BookDTO;
using DAL.Entity;

namespace BLL.Extensions;

public static class BookMappingExtensions
{
    public static BookResponseDTO ToResponseDTO(this Book book)
    {
        return new BookResponseDTO
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Quantity = book.Quantity,
            Available = book.Available,
            ISBN = book.ISBN,
            PublishedDate = book.PublishedDate,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? string.Empty
        };
    }
    public static Book ToEntity(this BookCreateDTO bookRequestDTO)
    {
        return new Book
        {
            Id = Guid.NewGuid(),
            Title = bookRequestDTO.Title,
            Author = bookRequestDTO.Author,
            Description = bookRequestDTO.Description,
            Quantity = bookRequestDTO.Quantity,
            Available = bookRequestDTO.Quantity,
            ISBN = bookRequestDTO.ISBN,
            PublishedDate = bookRequestDTO.PublishedDate,
            CategoryId = bookRequestDTO.CategoryId
        };
    }
}
