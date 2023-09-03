using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductCrud.Controllers
{
    [Route("api/services/app/[controller]")]
    [ApiController]
    public class ProductSyncController : ControllerBase
    {
        public ProductSyncController()
        {
            
        }
    }
}
