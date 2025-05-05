using BLL.DTOs.BookDTO;
using BLL.DTOs.GenericDTO;
using Common.Parameters;

namespace BLL.Services.Interfaces;

public interface IBookService
{
    Task<BookResponseDTO> GetByIdAsync(Guid id);
    Task<IEnumerable<BookResponseDTO>> GetAllAsync();
    Task<PaginatedList<BookResponseDTO>> GetAllWithPaginationAndFilter(BookFilterParams filterParams,int pageIndex, int pageSize);
    Task<BookResponseDTO> AddAsync(BookCreateDTO bookRequestDTO);
    Task<BookResponseDTO> Update(Guid id, BookUpdateDTO bookRequestDTO);
    Task Remove(Guid id);
}
