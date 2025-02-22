namespace CarRentalProject.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsRented { get; set; }
    }
}
