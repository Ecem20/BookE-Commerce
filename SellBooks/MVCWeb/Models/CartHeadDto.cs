namespace MVCWeb.Models
{
    public class CartHeadDto
    {
        public int CartHeadId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public double CartTotal { get; set; }
        public string? Address { get; set; }

        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? PhoneNumber { get; set; }


    }
}
