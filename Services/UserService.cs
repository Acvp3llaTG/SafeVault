using Microsoft.EntityFrameworkCore;
using SafeVault.Models;
using System.Threading.Tasks;

public class UserService
{
    private readonly SafeVaultDbContext _db;

    public UserService(SafeVaultDbContext db) => _db = db;

    // Secure query (EF Core parameterization)
    public async Task<User> GetUserAsync(string username, string password)
    {
        return await _db.Users
            .Where(u => u.Username == username && u.Password == password)
            .FirstOrDefaultAsync();
    }
}