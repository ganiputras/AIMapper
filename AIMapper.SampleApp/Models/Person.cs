using AIMapper.Attributes;

namespace AIMapper.SampleApp.Models;

[Map(typeof(PersonDto))]
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class PersonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}