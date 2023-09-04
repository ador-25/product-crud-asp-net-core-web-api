using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductCrud.Models;
using ProductCrud.Repositories;
using ProductCrud.ViewModels;

namespace ProductCrud.Controllers
{
    [Route("api/services/app/[controller]Sync")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // IGenericRepository to interact with database
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductController(IGenericRepository<Product> productRepository, UserManager<ApplicationUser> userManager,
            IGenericRepository<Tenant> tenantRepository
            )
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _tenantRepository = tenantRepository;

        }



        // get all products for a tenant 

        [HttpGet("GetAllProduct")]
        [Authorize(Roles =UserRoles.Tenant)]
        public async Task<IActionResult> GetAllProducts()
        {
            int headerId = GetTenantIdFromHeader(HttpContext);
            if (headerId == -1)
                return Ok(new Response{ Status= "Error", Message="Header Not Found" });
            var products = await _productRepository.GetAllByConditionAsync(u => u.TenantId == headerId);
            return Ok(products);
        }


        // get single product 
        [HttpGet("Get-Product")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<IActionResult> GetProduct([FromQuery] int productId)
        {
            int headerId = GetTenantIdFromHeader(HttpContext);
            if (headerId == -1)
                return Ok(new Response { Status = "Error", Message = "Header Not Found" });

            var product = await _productRepository.GetByIdIntAsync(productId);

            return Ok(product is not null ?
                product : 
                new Response() { Status = "Error", Message = "Product Not found" });

        }



        // create a product
        [HttpPost("CreateOrEdit")]
        [Authorize(Roles =UserRoles.Tenant)]
        public async Task<IActionResult> AddorEditProduct([FromBody] ProductViewModel product)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // verify if header id is correct 
            int headerId = GetTenantIdFromHeader(HttpContext);


            // verify if product belongs to header
            if (headerId == -1 || !VerifyTenantId(user, headerId))
                return Ok(new Response() { Status = "Error", Message = "TenantId Not Correct or Not found in header" });

            bool edit = false;

            // product already exists 
            Product old= new();
            if(product.Id is not null)
            {
                var exists = await _productRepository.GetByIdIntAsync((int)product.Id);
                
                if(exists is not null)
                {
                    // then check if the product belongs to tenant
                    if (exists.TenantId== headerId)
                    {
                        // edit 
                        edit = true;
                        old = exists;
                    }
                }
            }
            Product newProduct = product.GetProduct();
            if (edit)  // edit existing
            {
                old.Name =  product.Name;
                old.Description = product.Description;
                old.IsAvailable = product.IsAvailable;
                _productRepository.Update(old);

            }
            else // create new
            {
                Tenant tenant = await _tenantRepository.GetByIdIntAsync(user.TenantId);
                newProduct.TenantId = user.TenantId;
                newProduct.Tenant = tenant;
                _productRepository.Add(newProduct);
            }
            bool success = await _productRepository.SaveChangesAsync();
            if (success)
            {
                return Ok(new { Status = edit ? "Edited" : "Created", Product = newProduct });
            }
            else
            {
                return Ok(new Response() { Status = "Error", Message = "Could not add or edit" });
            }
        }


        [HttpDelete("Delete")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<IActionResult> Delete([FromQuery] int productId)
        {
            int headerId = GetTenantIdFromHeader(HttpContext);
            if (headerId == -1)
                return Ok(new Response { Status = "Error", Message = "Header Not Found" });

            var product = await _productRepository.GetByIdIntAsync(productId);
            if (product is null)
                return Ok( new Response() { Status = "Error", Message = "Product Not found" });

            _productRepository.Delete(product);
            bool del = await _productRepository.SaveChangesAsync() ;
            return Ok(new { Status= "Deleted"});
        }

        private int GetTenantIdFromHeader(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("Abp.TenantId", out var tenantIdValues))
            {
                string tenantId = tenantIdValues.FirstOrDefault();
                if (int.TryParse(tenantId, out int result))
                {
                    return result;
                }
            }

            return -1; // Return -1 if header not found or invalid
        }

        // verify if current user and tenant is same
        // return true if yes
        // return false otherwise
        private bool VerifyTenantId(ApplicationUser user,int id)
        {
            return user.TenantId == id;
        }

    }
}
