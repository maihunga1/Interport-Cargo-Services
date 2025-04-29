// Services/PasswordService.cs
using Microsoft.AspNetCore.Identity;
using Model;

namespace Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(string password)
        {
            // Using a dummy user since we only need the hasher
            var user = new User
            {
                UserName = string.Empty,
                FirstName = string.Empty,
                FamilyName = string.Empty,
                Email = string.Empty,
                PhoneNumber = string.Empty,
                CompanyName = string.Empty,
                Address = string.Empty,
                Password = string.Empty
            };
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var user = new User
            {
                UserName = string.Empty,
                FirstName = string.Empty,
                FamilyName = string.Empty,
                Email = string.Empty,
                PhoneNumber = string.Empty,
                CompanyName = string.Empty,
                Address = string.Empty,
                Password = hashedPassword
            };
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}