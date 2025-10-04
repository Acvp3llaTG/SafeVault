using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;
using Xunit;
using System.Threading.Tasks;

public class SecurityTests
{
    private SafeVaultDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SafeVaultDbContext>()
            .UseInMemoryDatabase("SecureTestDb")
            .Options;
        return new SafeVaultDbContext(options);
    }

    [Fact]
    public async Task SQLInjection_ShouldNotSucceed()
    {
        var db = CreateDbContext();
        db.Users.Add(new User { Username = "test", Password = "safe" });
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var injected = "' OR 1=1 --";
        var user = await service.GetUserAsync(injected, injected);

        Assert.Null(user); // Injection should fail
    }

    [Fact]
    public void XSS_ShouldBeSanitized()
    {
        var input = "<script>alert('xss')</script>";
        var sanitized = System.Net.WebUtility.HtmlEncode(input);

        Assert.DoesNotContain("<script>", sanitized);
        Assert.Contains("&lt;script&gt;", sanitized);
    }
}