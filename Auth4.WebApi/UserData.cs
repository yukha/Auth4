namespace Auth4.WebApi
{
    public class UserData
    {
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }
}