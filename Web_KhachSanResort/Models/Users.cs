using Microsoft.AspNetCore.Identity;

namespace Web_KhachSanResort.Models
{
    public class Users: IdentityUser
    {
        public string FullName { get; set; }
    }
}
