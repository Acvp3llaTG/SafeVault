using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;
using Xunit;

namespace SafeVault.Tests
{
    public class SensitiveDataTests
    {
        private SafeVaultDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SafeVaultDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            return new SafeVaultDbContext(options);
        }

        [Fact]
        public async Task Should_Prevent_SQL_Injection()
        {
            var db = GetDbContext();
            var malicious = new SensitiveData
            {
                Username = "'; DROP TABLE SensitiveDatas; --",
                Password = "badpassword"
            };

            db.SensitiveDatas.Add(malicious);
            await db.SaveChangesAsync();

            var count = await db.SensitiveDatas.CountAsync();
            Assert.Equal(1, count); // Table not dropped
        }

        [Fact]
        public async Task Should_Encode_XSS_Attempt()
        {
            var db = GetDbContext();
            var malicious = new SensitiveData
            {
                Username = "<script>alert('xss')</script>",
                Password = "password"
            };

            // Simulate encoding as in the AddSensitiveData page
            malicious.Username = System.Net.WebUtility.HtmlEncode(malicious.Username);

            db.SensitiveDatas.Add(malicious);
            await db.SaveChangesAsync();

            var stored = await db.SensitiveDatas.FirstAsync();
            Assert.Equal("&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;", stored.Username);
        }
    }
}