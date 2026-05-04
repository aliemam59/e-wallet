using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using test_api_jwt.Data;
using test_api_jwt.DTOs.Auth;
using test_api_jwt.Models;
using test_api_jwt.Security;

namespace test_api_jwt.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(AppDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Username == dto.Username);

            if (userExists)
                throw new Exception("User already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Role = "User",
                
                Wallet = new Wallet { Balance = 0 }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully and wallet created";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }
    }
}