using CarRentalProject.Data;
using CarRentalProject.Data.Abstract;
using CarRentalProject.Data.Repositories;
using CarRentalProject.Services;
using CarRentalProject.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()  // Allow all origins
               .AllowAnyMethod()  // Allow all methods
               .AllowAnyHeader(); // Allow all headers
    });
});
builder.Services.AddControllers();
builder.Services.AddSingleton<DbHelper>();

builder.Services.AddScoped<CarRentalProject.Data.Abstract.IUserService, UserRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<CarRentalProject.Services.IUserService, UserManager>();
builder.Services.AddScoped<ICarService, CarManager>();
builder.Services.AddScoped<IReservationService, ReservationManager>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>(); 
}
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
