namespace AdvancedEFCoreAPI.DTOs;

public class ProductResponseDto
{
public int Id { get; set; }


public string Name { get; set; } = string.Empty;

public decimal Price { get; set; }

public string CategoryName { get; set; } = string.Empty;

public byte[] RowVersion { get; set; } = Array.Empty<byte>();

}
