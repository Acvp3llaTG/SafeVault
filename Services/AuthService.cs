using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;
using BCrypt.Net;

namespace SafeVault.Services
{
    public class AuthService
    {
        private readonly SafeVaultDbContext _db;
        private readonly AuthenticationStateProvider _authProvider;

        public AuthService(SafeVaultDbContext db, AuthenticationStateProvider authProvider)
        {
            _db = db;
            _authProvider = authProvider;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;
            return null;
        }

        public async Task<bool> Register(string username, string password, string role)
        {
            if (await _db.Users.AnyAsync(u => u.Username == username))
                return false;
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            _db.Users.Add(new User { Username = username, PasswordHash = hash, Role = role });
            await _db.SaveChangesAsync();
            return true;
        }
    }
}