using BLL.DTOs.CategoryDTO;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] int? pageIndex, [FromQuery] int? pageSize)
    {
        if(pageSize.HasValue && pageIndex.HasValue)
        {
            var categoryPaging = await _categoryService.GetAllWithPagingAsync(pageIndex.Value, pageSize.Value);

            return Ok(categoryPaging);
        }
        var categories = await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryRequestDTO category)
    {
        var newCategory = await _categoryService.AddAsync(category);

        return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.Id }, newCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequestDTO category)
    {
        var updatedCategory = await _categoryService.UpdateAsync(id, category);

        return Ok(updatedCategory);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _categoryService.RemoveAsync(id);

        return NoContent();
    }
}
