using CarRentalProject.Data.Abstract;
using CarRentalProject.Models;
using Dapper;

namespace CarRentalProject.Data.Repositories
{
    public class ReservationRepository : DapperRepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(DbHelper dbHelper) : base(dbHelper)
        {

        }
    }
}
