using AutoMapper;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;

namespace ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHead, CartHeadDto>().ReverseMap();
                config.CreateMap<CartDetail, CartDetailDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
