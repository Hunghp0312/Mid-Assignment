﻿// <auto-generated />
using System;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAL.Entity.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Available")
                        .HasColumnType("int");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("CHAR(13)")
                        .HasAnnotation("RegularExpression", "^(97(8|9))?\\d{9}(\\d|X)$");

                    b.Property<DateTime>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("DAL.Entity.BookBorrowingRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ApproverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RequestorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApproverId");

                    b.HasIndex("RequestorId");

                    b.ToTable("BookBorrowingRequests");
                });

            modelBuilder.Entity("DAL.Entity.BookBorrowingRequestDetail", b =>
                {
                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookBorrowingRequestId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("BookId", "BookBorrowingRequestId");

                    b.HasIndex("BookBorrowingRequestId");

                    b.ToTable("BookBorrowingRequestDetails");
                });

            modelBuilder.Entity("DAL.Entity.BookRating", b =>
                {
                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rating")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("BookId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("BookRating");
                });

            modelBuilder.Entity("DAL.Entity.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("DAL.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("41d78630-fb80-4154-8142-9bc52e134da5"),
                            Address = "123 Admin St",
                            Email = "admin@example.com",
                            FirstName = "Hung",
                            LastName = "Hoang",
                            PasswordHash = "bCxEK/uLUZ7Z8+BYfdU/YdhbrVuhRvzYwReovdU2+04=",
                            PasswordSalt = "jQW5ssWVeAE85CJ9pzgeDQ==",
                            PhoneNumber = "1234567890",
                            RefreshTokenExpiryTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Role = 1,
                            Username = "admin"
                        },
                        new
                        {
                            Id = new Guid("08ff8293-ab17-44b1-a08d-ec387d621d73"),
                            Address = "456 User Ave",
                            Email = "john@example.com",
                            FirstName = "John",
                            LastName = "Doe",
                            PasswordHash = "OCqOmH72mdBYjZf0amTxF3OWtbesV9Lv4Y1Typ4519w=",
                            PasswordSalt = "7gKSkgQhiq+RSmknIghQ4g==",
                            PhoneNumber = "0987654321",
                            RefreshTokenExpiryTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Role = 0,
                            Username = "user1"
                        },
                        new
                        {
                            Id = new Guid("6d0b19db-1f85-451d-b934-7ede5e0a3ef7"),
                            Address = "789 Member Rd",
                            Email = "jane@example.com",
                            FirstName = "Jane",
                            LastName = "Smith",
                            PasswordHash = "b9nX8oPNh4n1VLUcMFuBXaBBfok737Q1RkbphVrQu6g=",
                            PasswordSalt = "C+At2NY4wxdnriGqqg24iQ==",
                            PhoneNumber = "0123456789",
                            RefreshTokenExpiryTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Role = 0,
                            Username = "user2"
                        });
                });

            modelBuilder.Entity("DAL.Entity.Book", b =>
                {
                    b.HasOne("DAL.Entity.Category", "Category")
                        .WithMany("Book")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("DAL.Entity.BookBorrowingRequest", b =>
                {
                    b.HasOne("DAL.Entity.User", "Approver")
                        .WithMany("ApproverBookBorrowingRequests")
                        .HasForeignKey("ApproverId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DAL.Entity.User", "Requestor")
                        .WithMany("RequestorBookBorrowingRequests")
                        .HasForeignKey("RequestorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Approver");

                    b.Navigation("Requestor");
                });

            modelBuilder.Entity("DAL.Entity.BookBorrowingRequestDetail", b =>
                {
                    b.HasOne("DAL.Entity.BookBorrowingRequest", "BookBorrowingRequest")
                        .WithMany("BookBorrowingRequestDetails")
                        .HasForeignKey("BookBorrowingRequestId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DAL.Entity.Book", "Book")
                        .WithMany("BookBorrowingRequestDetails")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("BookBorrowingRequest");
                });

            modelBuilder.Entity("DAL.Entity.BookRating", b =>
                {
                    b.HasOne("DAL.Entity.Book", "Book")
                        .WithMany("Ratings")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entity.User", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entity.Book", b =>
                {
                    b.Navigation("BookBorrowingRequestDetails");

                    b.Navigation("Ratings");
                });

            modelBuilder.Entity("DAL.Entity.BookBorrowingRequest", b =>
                {
                    b.Navigation("BookBorrowingRequestDetails");
                });

            modelBuilder.Entity("DAL.Entity.Category", b =>
                {
                    b.Navigation("Book");
                });

            modelBuilder.Entity("DAL.Entity.User", b =>
                {
                    b.Navigation("ApproverBookBorrowingRequests");

                    b.Navigation("Ratings");

                    b.Navigation("RequestorBookBorrowingRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
