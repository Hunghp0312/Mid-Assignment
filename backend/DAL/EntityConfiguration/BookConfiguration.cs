using System.Reflection.Emit;
using System.Reflection.Metadata;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfiguration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(book => book.Author).IsRequired().HasMaxLength(100);
        builder.Property(book => book.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(book => book.Quantity).IsRequired();
        builder.Property(book => book.Available).IsRequired();
        builder.Property(book => book.ISBN)
            .HasColumnType("CHAR(13)").HasAnnotation("RegularExpression", "^(97(8|9))?\\d{9}(\\d|X)$");
        builder.Ignore(book => book.AverageRating);
        // Config Relationships
        builder.HasOne(book => book.Category).WithMany(category => category.Book).OnDelete(DeleteBehavior.Restrict);
        
    }
}
