using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

public class DbTests
{
    public DbTests()
    {
    }

    public class TestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    // User-Entität
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class UserService
    {
        private readonly TestDbContext _context;
        public UserService(TestDbContext context)
        {
            _context = context;
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public User GetUser(int id)
        {
            return _context.Users.SingleOrDefault(u => u.Id == id);
        }
    }

    [Fact]
    public async void Db_Connection_ExpectedResult()
    {
        // Arrange: In-Memory-Datenbank einrichten
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var userService = new UserService(new TestDbContext(options));

        // Act
        userService.AddUser(new User { Id = 1, Name = "John Doe" });

        // Assert
        var user = userService.GetUser(1);
        Assert.NotNull(user);
        Assert.Equal("John Doe", user.Name);
    }

    [Fact]
    public void AddUser_ShouldStoreUserInDatabaseUsingDI_ExpectedData()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<TestDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"))
            .AddSingleton<UserService>()
            .BuildServiceProvider();


        var userService = serviceProvider.GetRequiredService<UserService>();

        // Act
        userService.AddUser(new User { Id = 2, Name = "John Doe" });


        // Assert
        var user = userService.GetUser(1);
        Assert.NotNull(user);
        Assert.Equal("John Doe", user.Name);
    }


}
