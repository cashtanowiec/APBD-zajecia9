using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial9.DTO;
using Tutorial9.Exceptions;
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
            try
            {
                var id = await _dbService.PostData(post);
                return Ok(id);
            }
            catch (NotFoundException exc)
            {
                return NotFound(exc.Message);
            }
            catch (OrderFulfilledException exc)
            {
                return BadRequest(exc.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occurred:");
                Console.WriteLine($"Message: {exc.Message}");
                Console.WriteLine($"Stack Trace: {exc.StackTrace}");

                if (exc.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Message: {exc.InnerException.Message}");
                    Console.WriteLine($"Stack Trace: {exc.InnerException.StackTrace}");
                }
                return Problem();
            }
        }
        
        [HttpPost("proc")]
        public async Task<IActionResult> PostDataWithProcedure(PostDTO post)
        {
            try
            {
                var id = await _dbService.PostDataWithProcedure(post);
                return Ok(id);
            }
            catch (SqlException exc)
            {
                if (exc.Message.StartsWith("Invalid parameter"))
                    return BadRequest(exc.Message);

                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occurred:");
                Console.WriteLine($"Message: {exc.Message}");
                Console.WriteLine($"Stack Trace: {exc.StackTrace}");

                if (exc.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Message: {exc.InnerException.Message}");
                    Console.WriteLine($"Stack Trace: {exc.InnerException.StackTrace}");
                }
                return Problem();
            }
        }
    }
}
