﻿using AutoMapper;
using OrdersAPI.Models;
using OrdersAPI.Models.Dto;


namespace ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<OrderHeaderDto,CartHeaderDto>().ForMember(dest=>dest.CartTotal,u=>u.MapFrom(src=>src.OrderTotal)).ReverseMap();
                //config.CreateMap<CartDetailsDto, OrderDetailsDto>().ForMember(dest => dest.BookName, u => u.MapFrom(src => src.Book.Title))
                //.ForMember(dest => dest.Price, u => u.MapFrom(src => src.Book.Price));

                config.CreateMap<OrderHeadDto, CartHeadDto>().ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();
                config.CreateMap<CartDetailDto, OrderDetailDto>().ForMember(dest => dest.BookName, u => u.MapFrom(src => src.Product.ProductTitle))
                .ForMember(dest => dest.ProductPrice, u => u.MapFrom(src => src.Product.ProductPrice));

                //config.CreateMap<OrderDetailsDto, CartDetailsDto>();
                //config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                //config.CreateMap<OrderDetailsDto,OrderDetails>().ReverseMap();


                config.CreateMap<OrderDetailDto, CartDetailDto>();
                config.CreateMap<OrderHead, OrderHeadDto>().ReverseMap();
                config.CreateMap<OrderDetailDto,OrderDetail>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
