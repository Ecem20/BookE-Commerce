using AutoMapper;
using EmailAPI.Models;
using EmailAPI.Models.Dto;


namespace EmailAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Email, EmailDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
