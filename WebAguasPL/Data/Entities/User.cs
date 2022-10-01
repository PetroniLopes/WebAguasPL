using Microsoft.AspNetCore.Identity;

namespace WebAguasPL.Data.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
