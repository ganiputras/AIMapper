using AIMapper.Profiles;
using AIMapper.SampleApp.Models;

namespace AIMapper.SampleApp;

public class UserProfile : MapperProfile
{
    public override void Configure(IMapper mapper)
    {
        mapper.Configure<UserEntity, UserDto>(cfg =>
        {
            cfg.PropertyOptions["City"] = new MappingPropertyOptions
            {
                CustomPath = "BillingAddress.City"
            };
        });
    }
}