using AutoMapper;
using BookAPI.Models;
using BookAPI.Models.Dto;

namespace BookAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<BookDto, Book>().ReverseMap();
                config.CreateMap<CategoriesDto, Categories>().ReverseMap();
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
