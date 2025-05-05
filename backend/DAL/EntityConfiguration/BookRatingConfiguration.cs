using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfiguration;

public class BookRatingConfiguration : IEntityTypeConfiguration<BookRating>
{
    public void Configure(EntityTypeBuilder<BookRating> builder)
    {
        builder.HasKey(br => new { br.BookId, br.UserId });
        builder.Property(br => br.Rating)
            .IsRequired()
            .HasDefaultValue(0);
        builder.HasOne(br => br.Book)
            .WithMany(b => b.Ratings)
            .HasForeignKey(br => br.BookId);
        builder.HasOne(br => br.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(br => br.UserId);
    }
}

