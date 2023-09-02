using Microsoft.AspNetCore.Identity;

namespace ProductCrud.Models
{
    public class ApplicationUser:IdentityUser
    {
        // keep track of TenantId in IdentityUser Table to retreive data in O(1)
        public int TenantId { get; set; }
    }
}
