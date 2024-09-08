using AutoMapper;
using Globe.Shared.Entities;
using Globe.Shared.Models.Privileges;

namespace Globe.Account.Service.Models.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<ScreensModel, ScreenEntity>().ReverseMap();
            CreateMap<UserReadModel, UserEntity>().ReverseMap();

            CreateMap<UserEntity, UserReadPrivilegesModel>().ReverseMap();

            CreateMap<ApplicationEntity, ApplicationModel>().ReverseMap();

        }
    }
}
