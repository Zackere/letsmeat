﻿// <auto-generated />
using System;
using LetsMeatAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LetsMeatAPI.Migrations
{
    [DbContext(typeof(LMDbContext))]
    [Migration("20210106220557_Reviews")]
    partial class Reviews
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("CustomLocationEvent", b =>
                {
                    b.Property<Guid>("CandidateCustomLocationsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EventsWithMeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CandidateCustomLocationsId", "EventsWithMeId");

                    b.HasIndex("EventsWithMeId");

                    b.ToTable("CustomLocationEvent");
                });

            modelBuilder.Entity("EventGoogleMapsLocation", b =>
                {
                    b.Property<string>("CandidateGoogleMapsLocationsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("EventsWithMeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CandidateGoogleMapsLocationsId", "EventsWithMeId");

                    b.HasIndex("EventsWithMeId");

                    b.ToTable("EventGoogleMapsLocation");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<Guid>("GroupsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UsersId")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("GroupsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.CustomLocation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("CreatedForId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedForId");

                    b.ToTable("CustomLocations");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.CustomLocationReview", b =>
                {
                    b.Property<Guid>("CustomLocationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("AmountOfFood")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Taste")
                        .HasColumnType("int");

                    b.Property<int>("WaitingTime")
                        .HasColumnType("int");

                    b.HasKey("CustomLocationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("CustomLocationReviews");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Debt", b =>
                {
                    b.Property<string>("FromId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ToId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.HasKey("FromId", "ToId", "GroupId");

                    b.HasIndex("GroupId");

                    b.HasIndex("ToId");

                    b.ToTable("Debts");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.DebtFromImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Satisfied")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.ToTable("DebtsFromImages");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.DebtHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid?>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("HistoryEntryCreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ImageDebtId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PendingDebtId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("ToId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DebtHistory");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CandidateTimes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Result")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("GroupId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.GoogleMapsLocation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BusinessStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FormattedAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PriceLevel")
                        .HasColumnType("int");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("UserRatingsTotal")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Vicinity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("GoogleMapsLocations");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.GoogleMapsLocationReview", b =>
                {
                    b.Property<string>("GoogleMapsLocationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("AmountOfFood")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Taste")
                        .HasColumnType("int");

                    b.Property<int>("WaitingTime")
                        .HasColumnType("int");

                    b.HasKey("GoogleMapsLocationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GoogleMapsLocationReviews");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UploadTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UploadedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UploadedById");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Invitation", b =>
                {
                    b.Property<string>("ToId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("Sent")
                        .HasColumnType("datetime2");

                    b.HasKey("ToId", "GroupId");

                    b.HasIndex("FromId");

                    b.HasIndex("GroupId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.PendingDebt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid?>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("ToId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("FromId");

                    b.HasIndex("GroupId");

                    b.HasIndex("ImageId");

                    b.HasIndex("ToId");

                    b.ToTable("PendingDebts");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.PendingDebtFromImageBound", b =>
                {
                    b.Property<Guid>("DebtFromImageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PendingDebtId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DebtFromImageId", "PendingDebtId");

                    b.HasIndex("DebtFromImageId")
                        .IsUnique();

                    b.HasIndex("PendingDebtId")
                        .IsUnique();

                    b.ToTable("PendingDebtFromImageBounds");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("AmountOfFoodPref")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PricePref")
                        .HasColumnType("int");

                    b.Property<int>("TastePref")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("WaitingTimePref")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Vote", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Order")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EventId", "UserId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("CustomLocationEvent", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.CustomLocation", null)
                        .WithMany()
                        .HasForeignKey("CandidateCustomLocationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Event", null)
                        .WithMany()
                        .HasForeignKey("EventsWithMeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EventGoogleMapsLocation", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.GoogleMapsLocation", null)
                        .WithMany()
                        .HasForeignKey("CandidateGoogleMapsLocationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Event", null)
                        .WithMany()
                        .HasForeignKey("EventsWithMeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LetsMeatAPI.Models.CustomLocation", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Group", "CreatedFor")
                        .WithMany("CustomLocations")
                        .HasForeignKey("CreatedForId")
                        .IsRequired();

                    b.Navigation("CreatedFor");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.CustomLocationReview", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.CustomLocation", "CustomLocation")
                        .WithMany("Reviews")
                        .HasForeignKey("CustomLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.User", "User")
                        .WithMany("CustomLocationReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CustomLocation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Debt", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.User", "From")
                        .WithMany("DebtsForOthers")
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Group", "Group")
                        .WithMany("Debts")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.User", "To")
                        .WithMany("DebtsForMe")
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("From");

                    b.Navigation("Group");

                    b.Navigation("To");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.DebtFromImage", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Image", "Image")
                        .WithMany("DebtsFromImage")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Event", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.User", "Creator")
                        .WithMany("CreatedEvents")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Group", "Group")
                        .WithMany("Events")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.GoogleMapsLocationReview", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.GoogleMapsLocation", "GoogleMapsLocation")
                        .WithMany("Reviews")
                        .HasForeignKey("GoogleMapsLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.User", "User")
                        .WithMany("GoogleMapsLocationReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GoogleMapsLocation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Group", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.User", "Owner")
                        .WithMany("OwnedGroups")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Image", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Event", "Event")
                        .WithMany("Images")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LetsMeatAPI.Models.Group", "Group")
                        .WithMany("Images")
                        .HasForeignKey("GroupId")
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.User", "UploadedBy")
                        .WithMany("UploadedImages")
                        .HasForeignKey("UploadedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Group");

                    b.Navigation("UploadedBy");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Invitation", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.User", "From")
                        .WithMany()
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.User", "To")
                        .WithMany("Invitations")
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("From");

                    b.Navigation("Group");

                    b.Navigation("To");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.PendingDebt", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Event", "Event")
                        .WithMany("PendingDebts")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LetsMeatAPI.Models.User", "From")
                        .WithMany("PendingDebtsForOthers")
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Group", "Group")
                        .WithMany("PendingDebts")
                        .HasForeignKey("GroupId")
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Image", "Image")
                        .WithMany("PendingDebtsWithMe")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("LetsMeatAPI.Models.User", "To")
                        .WithMany("PendingDebtsForMe")
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("From");

                    b.Navigation("Group");

                    b.Navigation("Image");

                    b.Navigation("To");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.PendingDebtFromImageBound", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.DebtFromImage", "DebtFromImage")
                        .WithOne("Bound")
                        .HasForeignKey("LetsMeatAPI.Models.PendingDebtFromImageBound", "DebtFromImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.PendingDebt", "PendingDebt")
                        .WithOne("Bound")
                        .HasForeignKey("LetsMeatAPI.Models.PendingDebtFromImageBound", "PendingDebtId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DebtFromImage");

                    b.Navigation("PendingDebt");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Vote", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Event", null)
                        .WithMany("Votes")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LetsMeatAPI.Models.CustomLocation", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.DebtFromImage", b =>
                {
                    b.Navigation("Bound");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Event", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("PendingDebts");

                    b.Navigation("Votes");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.GoogleMapsLocation", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Group", b =>
                {
                    b.Navigation("CustomLocations");

                    b.Navigation("Debts");

                    b.Navigation("Events");

                    b.Navigation("Images");

                    b.Navigation("PendingDebts");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Image", b =>
                {
                    b.Navigation("DebtsFromImage");

                    b.Navigation("PendingDebtsWithMe");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.PendingDebt", b =>
                {
                    b.Navigation("Bound");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.User", b =>
                {
                    b.Navigation("CreatedEvents");

                    b.Navigation("CustomLocationReviews");

                    b.Navigation("DebtsForMe");

                    b.Navigation("DebtsForOthers");

                    b.Navigation("GoogleMapsLocationReviews");

                    b.Navigation("Invitations");

                    b.Navigation("OwnedGroups");

                    b.Navigation("PendingDebtsForMe");

                    b.Navigation("PendingDebtsForOthers");

                    b.Navigation("UploadedImages");
                });
#pragma warning restore 612, 618
        }
    }
}
