namespace AIMapper.SampleApp.Models;

/// <summary>
///     Data transfer object (DTO) untuk pengguna, digunakan untuk view atau API response.
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!; // Diambil dari BillingAddress.City (override flattening)
}