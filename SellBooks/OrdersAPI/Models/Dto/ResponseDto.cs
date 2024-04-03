namespace OrdersAPI.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
        public int TotalCount {  get; set; }
        public int TotalPages { get; set; }

    }
}
