using Xunit;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Services;
using System.Threading.Tasks;

namespace SafeVault.Tests
{
    public class AuthTests
    {
        private SafeVaultDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SafeVaultDbContext>()
                .UseInMemoryDatabase("TestAuthDb")
                .Options;
            return new SafeVaultDbContext(options);
        }

        [Fact]
        public async Task InvalidLogin_ShouldFail()
        {
            var db = GetDbContext();
            var service = new AuthService(db, null);
            await service.Register("alice", "Password123!", "user");
            var result = await service.Authenticate("alice", "wrongpassword");
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidLogin_ShouldPass()
        {
            var db = GetDbContext();
            var service = new AuthService(db, null);
            await service.Register("bob", "Secure!Pass1", "admin");
            var result = await service.Authenticate("bob", "Secure!Pass1");
            Assert.NotNull(result);
            Assert.Equal("admin", result.Role);
        }

        [Fact]
        public async Task RoleAssignment_And_Access()
        {
            var db = GetDbContext();
            var service = new AuthService(db, null);
            await service.Register("charlie", "UserPass1!", "user");
            await service.Register("dana", "AdminPass1!", "admin");

            var user = await service.Authenticate("charlie", "UserPass1!");
            var admin = await service.Authenticate("dana", "AdminPass1!");

            Assert.NotNull(user);
            Assert.Equal("user", user.Role);

            Assert.NotNull(admin);
            Assert.Equal("admin", admin.Role);
        }
    }
}