namespace DAL.Entity;
public enum UserRole
{
    SuperUser = 1,
    User = 0
}
public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User; // 0 - User, 1 - SuperUser
    public List<BookBorrowingRequest> RequestorBookBorrowingRequests { get; set; } = [];
    public List<BookBorrowingRequest> ApproverBookBorrowingRequests { get; set; } = [];

    public List<BookRating> Ratings { get; set; } = [];
}
