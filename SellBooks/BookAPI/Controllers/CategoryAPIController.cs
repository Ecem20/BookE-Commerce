using AutoMapper;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;


        public CategoryController(ApplicationDbContext db, IMapper mapper)
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
                IEnumerable<Categories> objList = _db.categories.ToList();

                int totalItems = objList.Count();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (pageSize > 0)
                {
                    objList = objList.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                }
                _response.Result = _mapper.Map<IEnumerable<CategoriesDto>>(objList);
                _response.TotalPages = totalPages;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("getcategories")]
        public ResponseDto GetCategories()
        {
            try
            {
                IEnumerable<Categories> objList = _db.categories.ToList();
                _response.Result = _mapper.Map<IEnumerable<CategoriesDto>>(objList);
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
                Categories obj = _db.categories.First(u => u.CategoryId == id);
                _response.Result = _mapper.Map<CategoriesDto>(obj);
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
        public ResponseDto Post([FromBody] CategoriesDto categoriesDto)
        {
            try
            {
                Categories obj = _mapper.Map<Categories>(categoriesDto);

                _db.categories.Add(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CategoriesDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("Update")]
        public ResponseDto Put([FromBody] CategoriesDto categoriesDto)
        {
            try
            {
                Categories obj = _mapper.Map<Categories>(categoriesDto);

                _db.categories.Update(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CategoriesDto>(obj);
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
                Categories obj = _db.categories.First(u => u.CategoryId == id);
                _db.categories.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

    }
}
