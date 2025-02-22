using CarRentalProject.Data.Abstract;
using CarRentalProject.Models;

namespace CarRentalProject.Services.Concrete
{
    public class UserManager : IUserService
    {
        private readonly Data.Abstract.IUserService _userRepository;

        public UserManager(Data.Abstract.IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return user;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("Username cannot be empty");

            var existingUsers = await _userRepository.GetAllAsync();
            if (existingUsers.Any(u => u.Username.ToLower() == user.Username.ToLower()))
                throw new InvalidOperationException("Username is already taken");

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {user.Id} not found");

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            await _userRepository.DeleteAsync(id);
        }
    }
}
