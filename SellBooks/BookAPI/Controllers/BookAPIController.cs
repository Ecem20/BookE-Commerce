using BookAPI.Data;
using AutoMapper;
using BookAPI.Models;
using BookAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BookAPI.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public BookAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get(int pageSize = 4, int pageNumber = 1)
        {
            IEnumerable<Product> objList = _db.products.OrderByDescending(u => u.ProductId).ToList();

            int totalItems = objList.Count();
            int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            if (pageSize > 0)
            {
                objList = objList.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (pageSize <= 0 || pageNumber <= 0 || pageSize == null || pageNumber == null)
            {
                throw new Exception("Page size or page number is less than or equal to zero");
            }

            var response = new ResponseDto
            {
                Result = _mapper.Map<IEnumerable<ProductDto>>(objList),
                TotalPages = totalPages
            };

            return response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            Product obj = _db.products.First(u => u.ProductId == id);

            var response = new ResponseDto
            {
                Result = _mapper.Map<ProductDto>(obj)
            };

            return response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            // Kategori nesnesini al
            var category = _db.categories.FirstOrDefault(c => c.CategoryId == productDto.CategoryId);

            if (category == null)
            {
                throw new Exception("Invalid category ID");
            }

            // Kategoriyi kitaba ekle
            product.Categories = category;

            _db.products.Add(product);
            _db.SaveChanges();

            return new ResponseDto
            {
                IsSuccess = true,
                Result = _mapper.Map<ProductDto>(product)
            };
        }

        [HttpPost("Update")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            var category = _db.categories.FirstOrDefault(c => c.CategoryId == productDto.CategoryId);

            if (category == null)
            {
                throw new Exception("Invalid category ID");
            }

            product.Categories = category;

            _db.products.Update(product);
            _db.SaveChanges();

            return new ResponseDto
            {
                IsSuccess = true,
                Result = _mapper.Map<ProductDto>(product)
            };
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            Product obj = _db.products.First(u => u.ProductId == id);
            _db.products.Remove(obj);
            _db.SaveChanges();

            return new ResponseDto();
        }


        [HttpGet("ISBN")]
        public ResponseDto GetBookByISBN(string isbn, int id)
        {
            ResponseDto response = new ResponseDto();

            var existingBook = _db.products.FirstOrDefault(b => b.ISBNNumber == isbn && b.ProductId != id);

            if (existingBook == null)
            {
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
            return response;
        }

    }
}