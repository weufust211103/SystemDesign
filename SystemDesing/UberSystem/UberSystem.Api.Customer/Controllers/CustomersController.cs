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

        [HttpGet("customer/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Domain.Entities.Customer>> GetCustomerByUserId(int userId)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                                         .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }


        [HttpPut("update-customer")]
        [Authorize(Roles = "0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customerDto)
        {
            // Find the user based on the provided UserId
            var user = await _context.Users.FindAsync(customerDto.UserId);
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
                if (!UserExists(customerDto.UserId))
                {
                    return NotFound("User not found.");
                }
                else
                {
                    throw; // Handle other potential exceptions
                }
            }

            return Ok("User updated successfully.");
        }

        private bool UserExists(long userId)
        {
            return _context.Users.Any(e => e.Id == userId); // Adjust as necessary to match your user identifier
        }

        [HttpDelete("customer/{userId}")]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> DeleteCustomerByUserId(long userId)
        {
            if (_context.Customers == null || _context.Users == null)
            {
                return NotFound();
            }

            // Find customer by UserId
            var customer = await _context.Customers
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Find user by UserId
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove customer and user
            _context.Customers.Remove(customer);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            // Return a success message
            return Ok("Customer and associated user deleted successfully.");
        }



    }
}
