using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Domain.Interfaces;

namespace UberSystem.Service
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DriverService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Driver>> GetAllDrivers()
        {
            var driverRepository = _unitOfWork.Repository<Driver>();
            return await driverRepository.GetAllAsync();
        }

        public async Task<Driver?> GetDriverByUserId(long userId)
        {
            var driverRepository = _unitOfWork.Repository<Driver>();
            return await driverRepository.GetAsync(d => d.UserId == userId);
        }

        public async Task Add(Driver driver)
        {
            try
            {
                var driverRepository = _unitOfWork.Repository<Driver>();
                if (driver != null)
                {
                    await _unitOfWork.BeginTransaction();

                    // Ensure the driver ID is unique and positive
                    if (driver.Id <= 0)
                    {
                        driver.Id = GenerateNewDriverId();
                    }

                    await driverRepository.InsertAsync(driver);
                    await _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

       

        public async Task Delete(long id)
        {
            try
            {
                var driverRepository = _unitOfWork.Repository<Driver>();
                var driver = await driverRepository.FindAsync(id);
                if (driver == null)
                {
                    throw new Exception("Driver not found.");
                }

                await _unitOfWork.BeginTransaction();
                await driverRepository.DeleteAsync(driver);
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        private long GenerateNewDriverId()
        {
            // Logic to generate a new positive long Id
            return Math.Abs(DateTime.UtcNow.Ticks); // Example, use your own logic
        }
    }
}
