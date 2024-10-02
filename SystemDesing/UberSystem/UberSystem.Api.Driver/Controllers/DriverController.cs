using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Infrastructure;
using UberSytem.Dto.DTOs;

namespace UberSystem.Api.Driver.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly UberSystemDbContext _context;

        public DriverController(UberSystemDbContext context,IDriverService driverService)
        {
            _driverService = driverService;
            _context = context;
        }

        // GET: api/driver
        [HttpGet("uber-system")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UberSystem.Domain.Entities.Driver>>> GetAllDrivers()
        {
            var drivers = await _driverService.GetAllDrivers();
            return Ok(drivers);
        }

        // GET: api/driver/user/
        [HttpGet("uber-system/get-user-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UberSystem.Domain.Entities.Driver>> GetDriverByToken()
        {
            // Extract userId from token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var driver = await _driverService.GetDriverByUserId(userId);
            if (driver == null)
            {
                return NotFound("Driver not found.");
            }
            return Ok(driver);
        }



        [HttpPut("update-driver")]
        [Authorize(Roles = "1")] // Assuming role 0 is for admin or similar
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDriver([FromBody] DriverDto driverDto)
        {
            // Extract userId from token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            // Find the user based on the userId from the token
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user fields
            if (!string.IsNullOrWhiteSpace(driverDto.UserName))
            {
                user.UserName = driverDto.UserName;
            }
            if (!string.IsNullOrWhiteSpace(driverDto.Password))
            {
                user.Password = driverDto.Password; // Consider hashing the password before saving
            }

            // Update driver details
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null)
            {
                return NotFound("Driver not found.");
            }

            driver.LocationLatitude = driverDto.LocationLatitude;
            driver.LocationLongitude = driverDto.LocationLongitude;

            // Update user and driver records in the database
            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(driver).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userId))
                {
                    return NotFound("User not found.");
                }
                else
                {
                    throw; // Handle other potential exceptions
                }
            }

            return Ok("Driver and user updated successfully.");
        }

        // DELETE: api/driver/uber-system
        [HttpDelete("uber-system/self-delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDriverByToken()
        {
            // Extract userId from token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var existingDriver = await _driverService.GetDriverByUserId(userId);
            if (existingDriver == null)
            {
                return NotFound("Driver not found.");
            }

            await _driverService.Delete(userId);
            return NoContent();
        }
    }
}
