using AdvancedEFCoreAPI.Data;
using AdvancedEFCoreAPI.DTOs;
using AdvancedEFCoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedEFCoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
private readonly ApplicationDbContext _context;


public CategoriesController(ApplicationDbContext context)
{
    _context = context;
}

[HttpGet]
public async Task<ActionResult<List<Category>>> GetCategories()
{
    var categories = await _context.Categories
        .AsNoTracking()
        .OrderBy(c => c.Id)
        .ToListAsync();

    return Ok(categories);
}

[HttpPost]
public async Task<ActionResult<Category>> CreateCategory(
    CreateCategoryDto dto)
{
    var category = new Category
    {
        Name = dto.Name
    };

    _context.Categories.Add(category);

    await _context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetCategories),
        new { id = category.Id },
        category);
}


}
