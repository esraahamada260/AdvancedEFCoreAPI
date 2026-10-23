using AdvancedEFCoreAPI.Data;
using AdvancedEFCoreAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvancedEFCoreAPI.Models;

namespace AdvancedEFCoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
private readonly ApplicationDbContext _context;


public ProductsController(ApplicationDbContext context)
{
    _context = context;
}

[HttpPost]
public async Task<ActionResult<ProductResponseDto>> CreateProduct(
CreateProductDto dto)
{
var categoryExists = await _context.Categories
.AnyAsync(c => c.Id == dto.CategoryId);


if (!categoryExists)
{
    return BadRequest("The specified category does not exist.");
}

var product = new Product
{
    Name = dto.Name,
    Price = dto.Price,
    CategoryId = dto.CategoryId
};

_context.Products.Add(product);

await _context.SaveChangesAsync();

var response = await _context.Products
    .AsNoTracking()
    .Where(p => p.Id == product.Id)
    .Select(p => new ProductResponseDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        CategoryName = p.Category.Name,
        RowVersion = p.RowVersion
    })
    .SingleAsync();

return Created($"/api/products/{product.Id}", response);


}


[HttpGet]
public async Task<ActionResult<PaginatedResponseDto<ProductResponseDto>>> GetProducts(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
    {
        return BadRequest("PageNumber must be at least 1, and PageSize must be between 1 and 100.");
    }

    var query = _context.Products
        .AsNoTracking()
        .Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category.Name,
            RowVersion = p.RowVersion
        });

    var totalCount = await query.CountAsync();

    var products = await query
        .OrderBy(p => p.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var response = new PaginatedResponseDto<ProductResponseDto>
    {
        Items = products,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };

    return Ok(response);
}

[HttpPut("{id:int}")]
public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
{
var product = await _context.Products
.FirstOrDefaultAsync(p => p.Id == id);


if (product == null)
    return NotFound("Product not found.");

var categoryExists = await _context.Categories
    .AnyAsync(c => c.Id == dto.CategoryId);

if (!categoryExists)
    return BadRequest("The specified category does not exist.");

_context.Entry(product)
    .Property(p => p.RowVersion)
    .OriginalValue = dto.RowVersion;

product.Name = dto.Name;
product.Price = dto.Price;
product.CategoryId = dto.CategoryId;

try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException)
{
    var productExists = await _context.Products
        .AnyAsync(p => p.Id == id);

    if (!productExists)
        return NotFound("Product no longer exists.");

    return Conflict(new
    {
        message = "The product was modified by another user. Please reload the product and try again."
    });
}

return NoContent();


}



[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteProduct(int id)
{
var product = await _context.Products
.FirstOrDefaultAsync(p => p.Id == id);


if (product == null)
    return NotFound("Product not found.");

product.IsDeleted = true;

await _context.SaveChangesAsync();

return NoContent();


}



}
