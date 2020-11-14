﻿// <auto-generated />
using System;
using LetsMeatAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LetsMeatAPI.Migrations
{
    [DbContext(typeof(LMDbContext))]
    partial class LMDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("EventLocation", b =>
                {
                    b.Property<string>("CandidateLocationsId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("EventsWithMeId")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("CandidateLocationsId", "EventsWithMeId");

                    b.HasIndex("EventsWithMeId");

                    b.ToTable("EventLocation");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<string>("GroupsId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("UsersId")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("GroupsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Debt", b =>
                {
                    b.Property<string>("FromId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ToId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.HasKey("FromId", "ToId", "GroupId");

                    b.HasIndex("GroupId");

                    b.HasIndex("ToId");

                    b.ToTable("Debts");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Event", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("CandidateTimes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Result")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Group", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Invitation", b =>
                {
                    b.Property<string>("FromId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ToId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("FromId", "ToId", "GroupId");

                    b.HasIndex("FromId")
                        .IsUnique();

                    b.HasIndex("GroupId");

                    b.HasIndex("ToId");

                    b.ToTable("Invitation");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Location", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Info")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prefs")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("{}");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Vote", b =>
                {
                    b.Property<string>("EventId")
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Order")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EventId", "UserId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("EventLocation", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Location", null)
                        .WithMany()
                        .HasForeignKey("CandidateLocationsId")
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

            modelBuilder.Entity("LetsMeatAPI.Models.Debt", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.User", "From")
                        .WithMany("DebtsForOthers")
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LetsMeatAPI.Models.Group", "Group")
                        .WithMany()
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

            modelBuilder.Entity("LetsMeatAPI.Models.Event", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Group", null)
                        .WithMany("Events")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Invitation", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.User", "From")
                        .WithOne()
                        .HasForeignKey("LetsMeatAPI.Models.Invitation", "FromId")
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

            modelBuilder.Entity("LetsMeatAPI.Models.Location", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Group", null)
                        .WithMany("Locations")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Vote", b =>
                {
                    b.HasOne("LetsMeatAPI.Models.Event", null)
                        .WithMany("Votes")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Event", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.Group", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Locations");
                });

            modelBuilder.Entity("LetsMeatAPI.Models.User", b =>
                {
                    b.Navigation("DebtsForMe");

                    b.Navigation("DebtsForOthers");

                    b.Navigation("Invitations");
                });
#pragma warning restore 612, 618
        }
    }
}
