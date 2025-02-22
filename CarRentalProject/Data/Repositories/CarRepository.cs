using CarRentalProject.Data.Abstract;
using CarRentalProject.Models;

namespace CarRentalProject.Data.Repositories
{

    public class CarRepository : DapperRepositoryBase<Car>, ICarRepository
    {
        public CarRepository(DbHelper dbHelper) : base(dbHelper)
        {
        }
    }
}
