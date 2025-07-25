namespace AIMapper.SampleApp.Models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public UserProfile? Profile { get; set; }
    public StatusEnum Status { get; set; }
    public int? Point { get; set; }
}

public enum StatusEnum
{
    Inactive = 0,
    Active = 1
}

public class UserProfile
{
    public string? Address { get; set; }
    public string? City { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Registered { get; set; }
    public string? Address { get; set; }
    public int Status { get; set; }
    public int Point { get; set; }
    public bool IsActive { get; set; }
}