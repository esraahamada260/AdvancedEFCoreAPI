using System.ComponentModel.DataAnnotations;

namespace AdvancedEFCoreAPI.DTOs;

public class UpdateProductDto
{
[Required]
[StringLength(200, MinimumLength = 2)]
public string Name { get; set; } = string.Empty;


[Range(typeof(decimal), "0.01", "9999999999999999.99")]
public decimal Price { get; set; }

[Range(1, int.MaxValue)]
public int CategoryId { get; set; }

[Required]
public byte[] RowVersion { get; set; } = Array.Empty<byte>();


}
