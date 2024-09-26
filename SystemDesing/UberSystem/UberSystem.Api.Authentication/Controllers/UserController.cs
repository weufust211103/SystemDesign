using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;

namespace UberSystem.Api.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Apply to the entire controller
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users); // Return all users
        }

        // POST: api/User
        [HttpPost]
        [Authorize(Roles = "2")] // Only admin can add users
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            await _userService.Add(user);
            return Ok("User added successfully.");
        }

        // GET: api/User/email/{email}
        [HttpGet("email/{email}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.FindByEmail(email);
            if (user == null)
                return NotFound("User not found.");
            return Ok(user);
        }

        // PUT: api/User
        [HttpPut]
        [Authorize(Roles = "2")] // Only admin can update users
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            await _userService.Update(user);
            return Ok("User updated successfully.");
        }
        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "2")] // Only admin can delete users
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.Delete(id); // Make sure this method exists in your IUserService
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return NotFound($"User with ID {id} not found. Error: {ex.Message}");
            }
        }

    }
}
