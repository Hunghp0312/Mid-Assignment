using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EntityConfiguration;

public class BookBorrowingRequestConfiguration : IEntityTypeConfiguration<BookBorrowingRequest>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequest> builder)
    {
        builder.HasKey(request => request.Id);

        builder.Property(request => request.RequestorId)
            .IsRequired();

        builder.Property(request => request.RequestDate)
            .IsRequired();

        builder.Property(request => request.Status)
            .IsRequired();
        builder.Property(request => request.ApproverId)
            .IsRequired(false);
        // Config Relationships
        builder.HasOne(request => request.Requestor)
            .WithMany(user => user.RequestorBookBorrowingRequests)
            .HasForeignKey(request => request.RequestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(request => request.Approver)
            .WithMany(user => user.ApproverBookBorrowingRequests)
            .HasForeignKey(request => request.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}