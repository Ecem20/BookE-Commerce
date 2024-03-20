using BookAPI.Data;
using AutoMapper;
using BookAPI.Models;
using BookAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;
using System.Net;
using Azure;

namespace BookAPI.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public BookAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get(int pageSize = 4, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Product> objList = _db.products.ToList();

                int totalItems = objList.Count();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (pageSize > 0)
                {
                    objList = objList.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                }
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
                _response.TotalPages = totalPages;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("getbooks")]
        public ResponseDto GetBooks()
        {
            try
            {
                IEnumerable<Product> objList = _db.products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product obj = _db.products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);

                // Kategori nesnesini al
                var category = _db.categories.FirstOrDefault(c => c.CategoryId == productDto.CategoryId);

                if (category == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid category ID";
                    return _response;
                }

                // Kategoriyi kitaba ekle
                product.Categories = category;

                _db.products.Add(product);
                _db.SaveChanges();

                _response.IsSuccess = true;
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [HttpPost("Update")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);

                var category = _db.categories.FirstOrDefault(c => c.CategoryId == productDto.CategoryId);

                if (category == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid category ID";
                    return _response;
                }
                product.Categories = category;

                _db.products.Update(product);
                _db.SaveChanges();
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {

            try
            {
                Product obj = _db.products.First(u => u.ProductId == id);
                _db.products.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        //[HttpGet("GetBookByISBN/{isbn}/{bookId}")]
        //public ResponseDto GetBookByISBN(string isbn, int bookId)
        //{
        //    try
        //    {
        //        var existingBook = _db.books.FirstOrDefault(b => b.ISBNNo == isbn && b.BookId != bookId);

        //        if (existingBook != null)
        //        {
        //            _response.Result = existingBook;
        //        }
        //        else
        //        {
        //            _response.IsSuccess = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}

    }
}