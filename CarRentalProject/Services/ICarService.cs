using CarRentalProject.Models;

namespace CarRentalProject.Services
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate);
        Task<Car> CreateCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);
    }
}
