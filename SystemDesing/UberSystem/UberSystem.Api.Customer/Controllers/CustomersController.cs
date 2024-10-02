using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Infrastructure;
using UberSytem.Dto.DTOs;

namespace UberSystem.Api.Customer.Controllers
{
    public class CustomersController : BaseApiController
    {
        private readonly UberSystemDbContext _context;
        private readonly IUserService _userService;

        public CustomersController(UberSystemDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        
        /// <summary>
        /// Retrieve customers in system
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpGet("customers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Customer>>> GetCustomers()
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            return await _context.Customers.ToListAsync();
        }

        /// <summary>
        /// Retrieve customers in system
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>

        [HttpGet("customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Domain.Entities.Customer>> GetCustomerByToken()
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }

            // Extract the userId from the token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            // Find the customer by userId
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }


        [HttpPut("update-customer")]
        [Authorize(Roles = "0")] // Ensure only authorized users with role 0 can update
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customerDto)
        {
            // Extract the userId from the token claims
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
            if (!string.IsNullOrWhiteSpace(customerDto.UserName))
            {
                user.UserName = customerDto.UserName;
            }
            if (!string.IsNullOrWhiteSpace(customerDto.Password))
            {
                user.Password = customerDto.Password;
            }
            if (!string.IsNullOrWhiteSpace(customerDto.Email))
            {
                user.Email = customerDto.Email;
            }

            // Update user record in the database
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("An error occurred while updating the user.");
            }

            return Ok("User updated successfully.");
        }

        private bool UserExists(long userId)
        {
            return _context.Users.Any(e => e.Id == userId); 
        }

        [HttpDelete("customer")]
        [Authorize(Roles = "0")] 
        public async Task<IActionResult> DeleteCustomerByToken()
        {
            if (_context.Customers == null || _context.Users == null)
            {
                return NotFound();
            }

            // Extract the userId from the token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            // Find customer by userId
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Find user by userId
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove customer and user
            _context.Customers.Remove(customer);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("Customer and associated user deleted successfully.");
        }


    }
}
