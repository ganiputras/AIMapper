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