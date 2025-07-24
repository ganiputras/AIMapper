using AIMapper;
using AIMapper.Profiles;
using AIMapper.SampleApp.Models;

var mapper = new Mapper();
mapper.ApplyProfilesFromCurrentAssembly();

var entity = new UserEntity
{
    Id = 1,
    Name = "Gani",
    HomeAddress = new Address { Street = "Jl. Mawar", City = "Bandung" },
    BillingAddress = new Address { Street = "Jl. Melati", City = "Jakarta" }
};

var dto = mapper.Map<UserEntity, UserDto>(entity);
Console.WriteLine($">>> DTO: Id={dto.Id}, Name={dto.Name}, City={dto.City}"); // City harus "Jakarta"