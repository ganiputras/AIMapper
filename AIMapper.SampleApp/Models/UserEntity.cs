namespace AIMapper.SampleApp.Models;

/// <summary>
///     Representasi entitas pengguna dengan alamat rumah dan alamat penagihan.
/// </summary>
public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Address HomeAddress { get; set; } = new();
    public Address BillingAddress { get; set; } = new();
}