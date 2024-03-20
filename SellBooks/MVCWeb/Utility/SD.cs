namespace MVCWeb.Utility
{
    public class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static string AuthAPIBase { get; set; }
        public static string BookAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }
        public static string EmailAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";
        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_Cancelled = "Canceled";
    }
}
