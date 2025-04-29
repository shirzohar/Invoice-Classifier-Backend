using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusuMatchProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProtectedController : ControllerBase
    {
        // This endpoint is protected by [Authorize]
        // Only users with a valid JWT token can access it
        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            // Returns a success response only if the user is authenticated
            return Ok(" את רואה את זה רק כי את מחוברת עם טוקן!");
        }
    }
}
