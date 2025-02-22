using CarRentalProject.Models;

namespace CarRentalProject.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetUserReservationsAsync(int userId);
        Task<Reservation> CreateReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task CancelReservationAsync(int id);
        Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate);
    }
}
