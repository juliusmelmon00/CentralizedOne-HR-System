using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // Open to anyone logged in
        [HttpGet("general")]
        [Authorize]
        public IActionResult GeneralAccess()
        {
            return Ok("✅ You are logged in!");
        }

        // Only Employees
        [HttpGet("employee")]
        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeOnly()
        {
            return Ok("👤 Employee access confirmed!");
        }

        // Only HR/Admin
        [HttpGet("hradmin")]
        [Authorize(Roles = "HR/Admin")]
        public IActionResult HRAdminOnly()
        {
            return Ok("👩‍💼 HR/Admin access confirmed!");
        }

        // Only SuperAdmin
        [HttpGet("superadmin")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SuperAdminOnly()
        {
            return Ok("👑 SuperAdmin access confirmed!");
        }
    }
}