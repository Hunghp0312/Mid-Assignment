using DAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
    public DbSet<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("41d78630-fb80-4154-8142-9bc52e134da5"),
                Username = "admin",
                PasswordHash = "bCxEK/uLUZ7Z8+BYfdU/YdhbrVuhRvzYwReovdU2+04=",
                PasswordSalt = "jQW5ssWVeAE85CJ9pzgeDQ==",
                FirstName = "Hung",
                LastName = "Hoang",
                Email = "admin@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Admin St",
                Role = UserRole.SuperUser
            },
            new User
            {
                Id = Guid.Parse("08ff8293-ab17-44b1-a08d-ec387d621d73"),
                Username = "user1",
                PasswordHash = "OCqOmH72mdBYjZf0amTxF3OWtbesV9Lv4Y1Typ4519w=",
                PasswordSalt = "7gKSkgQhiq+RSmknIghQ4g==",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "0987654321",
                Address = "456 User Ave",
                Role = UserRole.User
            },
            new User
            {
                Id = Guid.Parse("6d0b19db-1f85-451d-b934-7ede5e0a3ef7"),
                Username = "user2",
                PasswordHash = "b9nX8oPNh4n1VLUcMFuBXaBBfok737Q1RkbphVrQu6g=",
                PasswordSalt = "C+At2NY4wxdnriGqqg24iQ==",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "0123456789",
                Address = "789 Member Rd",
                Role = UserRole.User
            }
        );
    }
}
