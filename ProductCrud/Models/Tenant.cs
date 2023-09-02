using System.ComponentModel.DataAnnotations;

namespace ProductCrud.Models
{

    /*    
    assuming that one tenant can have multiple products under him,    
    we establish a one-to-many relation between tenant and product
    */
    public class Tenant
    {
        // define primary key
        [Key]
        public int TenantId { get; set; }
        [Required]
        public string TenantName { get; set; }

        // for one to many relation
        public List<Tenant> Tenants { get; set; }


        // define other required fields here
        // ...
    }
}
