using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto,bool withBearer=true);
    }
}
