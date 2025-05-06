using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial9.DTO;
using Tutorial9.Services;

namespace Tutorial9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        public IDbService _dbService { get; set; }

        public WarehouseController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> PostData(PostDTO post)
        {
            throw new NotImplementedException();
        }
    }
}
