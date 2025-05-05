using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfiguration;

public class BookBorrowingRequestDetailConfiguration : IEntityTypeConfiguration<BookBorrowingRequestDetail>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequestDetail> builder)
    {
        builder.HasKey(detail => new { detail.BookId, detail.BookBorrowingRequestId });
        builder.Property(detail => detail.BookId)
            .IsRequired();
        builder.Property(detail => detail.BookBorrowingRequestId)
            .IsRequired();
        // Config Relationships
        builder.HasOne(detail => detail.Book)
            .WithMany(book => book.BookBorrowingRequestDetails)
            .HasForeignKey(detail => detail.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(detail => detail.BookBorrowingRequest)
            .WithMany(request => request.BookBorrowingRequestDetails)
            .HasForeignKey(detail => detail.BookBorrowingRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
