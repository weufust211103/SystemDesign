using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Enums;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
namespace UberSystem.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var userRepository = _unitOfWork.Repository<User>();
            return await userRepository.GetAllAsync();
        }
        // Update user
        public async Task Update(User user)
        {
            try
            {
                var userRepository = _unitOfWork.Repository<User>();
                if (user != null)
                {
                    await _unitOfWork.BeginTransaction();
                    await userRepository.UpdateAsync(user);
                    await _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        // Delete user by Id
        public async Task Delete(int id)
        {
            try
            {
                var userRepository = _unitOfWork.Repository<User>();
                var customerRepository = _unitOfWork.Repository<Customer>();
                var driverRepository = _unitOfWork.Repository<Driver>();

                // Find the user
                var user = await userRepository.FindAsync(id);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                // Check if user is a customer and delete related records
                if (user.Role == (int)UserRole.CUSTOMER)
                {
                    var customer = await customerRepository.GetAsync(c => c.UserId == user.Id);
                    if (customer != null)
                    {
                        await customerRepository.DeleteAsync(customer);
                    }
                }
                // Check if user is a driver and delete related records
                else if (user.Role == (int)UserRole.DRIVER)
                {
                    var driver = await driverRepository.GetAsync(d => d.UserId == user.Id);
                    if (driver != null)
                    {
                        await driverRepository.DeleteAsync(driver);
                    }
                }

                // Finally, delete the user
                await userRepository.DeleteAsync(user);
            }
            catch (Exception)
            {
                throw; // Rethrow the exception to be handled in the controller
            }
        }
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Get user by Id
        public async Task<User?> GetById(long id)
        {
            var userRepository = _unitOfWork.Repository<User>();
            return await userRepository.FindAsync(id);
        }

        // Get all drivers
        public async Task<IEnumerable<User>> GetDrivers()
        {
            var userRepository = _unitOfWork.Repository<User>();
            var users = await userRepository.GetAllAsync();
            return users.Where(u => u.Role == (int)UserRole.DRIVER);
        }



        public async Task Add(User user)
        {
            try
            {
                var userRepository = _unitOfWork.Repository<User>();
                var customerRepository = _unitOfWork.Repository<Customer>();
                var driverRepository = _unitOfWork.Repository<Driver>();
                if (user is not null)
                {
                    await _unitOfWork.BeginTransaction();
                    // check duplicate user
                    var existedUser = await userRepository.GetAsync(u => u.Id == user.Id || u.Email == user.Email);
                    if (existedUser is not null) throw new Exception("User already exists.");

                    await userRepository.InsertAsync(user);

                    // add customer or driver into tables
                    if (user.Role == (int)UserRole.CUSTOMER)
                    {
                        var customer = _mapper.Map<Customer>(user);
                        if (customer.Id <= 0)
                        {
                            customer.Id = GenerateNewCustomerId(); // Ensure this generates a positive unique Id
                        }

                        await customerRepository.InsertAsync(customer);
                    }
                    else if (user.Role == (int)UserRole.DRIVER)
                    {
                        var driver = _mapper.Map<Driver>(user);

                        if (driver.Id <= 0)
                        {
                            driver.Id = GenerateNewDriverId(); // Ensure this generates a positive unique Id
                        }
                        await driverRepository.InsertAsync(driver);
                    }
                    await _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        private long GenerateNewCustomerId()
        {
            // Logic to generate a new positive long Id
            return Math.Abs(DateTime.UtcNow.Ticks); // Example, use your own logic
        }

        private long GenerateNewDriverId()
        {
            // Logic to generate a new positive long Id
            return Math.Abs(DateTime.UtcNow.Ticks); // Example, use your own logic
        }
        public Task CheckPasswordAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindByEmail(string email)
        {
            return await _unitOfWork.Repository<User>().FindAsync(email);
        }

        public async Task<IEnumerable<User>> GetCustomers()
        {
            var userRepository = _unitOfWork.Repository<User>();
            var users = await userRepository.GetAllAsync();

            var customers = users.Where(u => u.Role == (int)UserRole.CUSTOMER);
            return customers;
        }

        public async Task<bool> Login(User user)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                var UserRepos = _unitOfWork.Repository<User>();
                var objUser = await UserRepos.FindAsync(user.Email);
                if (objUser == null)
                    return false;
                if (objUser.Password != user.Password)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> Login(string email, string password)
        {
            var userRepository = _unitOfWork.Repository<User>();
            var users = await userRepository.GetAllAsync();

            var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
            return user;
        }

        public async Task<User?> GetByVerificationToken(string token)
        {
            var userRepository = _unitOfWork.Repository<User>();
            return await userRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        }

        
    }
}

