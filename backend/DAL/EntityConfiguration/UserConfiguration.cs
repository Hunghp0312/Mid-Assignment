using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(user => user.PasswordHash)
            .IsRequired();
        builder.Property(user => user.PasswordSalt)
            .IsRequired();
        builder.Property(user => user.Role)
            .IsRequired();
    }
}
