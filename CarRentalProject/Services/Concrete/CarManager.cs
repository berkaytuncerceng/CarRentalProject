using CarRentalProject.Data.Abstract;
using CarRentalProject.Models;

namespace CarRentalProject.Services.Concrete
{
    public class CarManager : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IReservationRepository _reservationRepository;

        public CarManager(ICarRepository carRepository, IReservationRepository reservationRepository)
        {
            _carRepository = carRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _carRepository.GetAllAsync();
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                throw new KeyNotFoundException($"Car with ID {id} not found");
            return car;
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("Start date must be before end date");

            var allCars = await _carRepository.GetAllAsync();
            var allReservations = await _reservationRepository.GetAllAsync();

            var availableCars = allCars.Where(car =>
                !allReservations.Any(r =>
                    r.CarId == car.Id &&
                    ((startDate >= r.StartDate && startDate < r.EndDate) ||
                     (endDate > r.StartDate && endDate <= r.EndDate) ||
                     (startDate <= r.StartDate && endDate >= r.EndDate))));

            if (availableCars == null)
            {
                throw new Exception("There are no available cars now");
            }
            return availableCars;
        }

        public async Task<Car> CreateCarAsync(Car car)
        {
            if (string.IsNullOrEmpty(car.Brand) || string.IsNullOrEmpty(car.Model))
                throw new ArgumentException("Car brand and model are required");

            if (car.PricePerDay <= 0)
                throw new ArgumentException("Price per day must be greater than zero");

            await _carRepository.AddAsync(car);
            return car;
        }

        public async Task UpdateCarAsync(Car car)
        {
            var existingCar = await _carRepository.GetByIdAsync(car.Id);
            if (existingCar == null)
                throw new KeyNotFoundException($"Car with ID {car.Id} not found");

            await _carRepository.UpdateAsync(car);
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                throw new KeyNotFoundException($"Car with ID {id} not found");

            var reservations = await _reservationRepository.GetAllAsync();
            if (reservations.Any(r => r.CarId == id && r.EndDate > DateTime.Now))
                throw new InvalidOperationException("Cannot delete a car with active reservations");

            await _carRepository.DeleteAsync(id);
        }
    }
}
