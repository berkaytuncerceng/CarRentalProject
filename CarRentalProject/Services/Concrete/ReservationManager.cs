using CarRentalProject.Data.Abstract;
using CarRentalProject.Models;

namespace CarRentalProject.Services.Concrete
{
    public class ReservationManager : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICarRepository _carRepository;
        private readonly Data.Abstract.IUserService _userRepository;

        public ReservationManager(IReservationRepository reservationRepository,
            ICarRepository carRepository,
            Data.Abstract.IUserService userRepository)
        {
            _reservationRepository = reservationRepository;
            _carRepository = carRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _reservationRepository.GetAllAsync();
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with ID {id} not found");
            return reservation;
        }

        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var allReservations = await _reservationRepository.GetAllAsync();
            return allReservations.Where(r => r.UserId == userId);
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            // Validate dates
            if (reservation.StartDate >= reservation.EndDate)
                throw new ArgumentException("Start date must be before end date");

            if (reservation.StartDate < DateTime.Now)
                throw new ArgumentException("Start date cannot be in the past");

            // Validate car exists
            var car = await _carRepository.GetByIdAsync(reservation.CarId);
            if (car == null)
                throw new KeyNotFoundException($"Car with ID {reservation.CarId} not found");

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(reservation.UserId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {reservation.UserId} not found");

            // Check if car is available for the requested period
            var isAvailable = await IsCarAvailableAsync(reservation.CarId, reservation.StartDate, reservation.EndDate);
            if (!isAvailable)
                throw new InvalidOperationException("Car is not available for the requested period");

            // Create reservation
            await _reservationRepository.AddAsync(reservation);

            // Update car status
            car.IsRented = true;
            await _carRepository.UpdateAsync(car);

            return reservation;
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            var existingReservation = await _reservationRepository.GetByIdAsync(reservation.Id);
            if (existingReservation == null)
                throw new KeyNotFoundException($"Reservation with ID {reservation.Id} not found");

            // If dates are being modified, check availability
            if (reservation.StartDate != existingReservation.StartDate ||
                reservation.EndDate != existingReservation.EndDate)
            {
                // Validate new dates
                if (reservation.StartDate >= reservation.EndDate)
                    throw new ArgumentException("Start date must be before end date");

                if (reservation.StartDate < DateTime.Now)
                    throw new ArgumentException("Start date cannot be in the past");

                // Check car availability for new dates (excluding this reservation)
                var allReservations = await _reservationRepository.GetAllAsync();
                var carReservations = allReservations
                    .Where(r => r.CarId == reservation.CarId && r.Id != reservation.Id)
                    .ToList();

                var isTimeSlotAvailable = !carReservations.Any(r =>
                    (reservation.StartDate >= r.StartDate && reservation.StartDate < r.EndDate) ||
                    (reservation.EndDate > r.StartDate && reservation.EndDate <= r.EndDate) ||
                    (reservation.StartDate <= r.StartDate && reservation.EndDate >= r.EndDate));

                if (!isTimeSlotAvailable)
                    throw new InvalidOperationException("Car is not available for the updated period");
            }

            await _reservationRepository.UpdateAsync(reservation);
        }

        public async Task CancelReservationAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with ID {id} not found");

            // Can only cancel future reservations
            if (reservation.StartDate <= DateTime.Now)
                throw new InvalidOperationException("Cannot cancel a reservation that has already started");

            await _reservationRepository.DeleteAsync(id);

            // Check if car has other active reservations
            var allReservations = await _reservationRepository.GetAllAsync();
            var hasActiveReservations = allReservations
                .Any(r => r.CarId == reservation.CarId && r.Id != id && r.EndDate > DateTime.Now);

            if (!hasActiveReservations)
            {
                // Update car status if no other active reservations
                var car = await _carRepository.GetByIdAsync(reservation.CarId);
                if (car != null)
                {
                    car.IsRented = false;
                    await _carRepository.UpdateAsync(car);
                }
            }
        }

        public async Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("Start date must be before end date");

            var car = await _carRepository.GetByIdAsync(carId);
            if (car == null)
                throw new KeyNotFoundException($"Car with ID {carId} not found");

            var allReservations = await _reservationRepository.GetAllAsync();
            var carReservations = allReservations.Where(r => r.CarId == carId);

            return !carReservations.Any(r =>
                (startDate >= r.StartDate && startDate < r.EndDate) ||
                (endDate > r.StartDate && endDate <= r.EndDate) ||
                (startDate <= r.StartDate && endDate >= r.EndDate));
        }
    }
}
