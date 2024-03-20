namespace MVCWeb.Models
{
    public class ConnectedUsers
    {
        public static Dictionary<string, Information> Id { get; set; } = new Dictionary<string, Information>();
    }

    public class Information
    {
        public string Browser { get; set; }
        public string UserId { get; set; }
    }
}
