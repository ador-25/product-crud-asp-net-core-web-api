using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductCrud.Models
{
    /*    
    assuming that one tenant can have multiple products under him,    
    we establish a one-to-many relation between tenant and product
    */
    public class Product
    {
        //define primary key
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; } 

        // foreign key of Tenant table 
        public int TenantId { get; set; }
        [JsonIgnore]
        public Tenant Tenant { get; set; }
    }
}
