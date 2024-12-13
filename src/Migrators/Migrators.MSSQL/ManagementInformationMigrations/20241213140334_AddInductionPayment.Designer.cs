﻿// <auto-generated />
using System;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.ManagementInformationMigrations
{
    [DbContext(typeof(ManagementInformationDbContext))]
    [Migration("20241213140334_AddInductionPayment")]
    partial class AddInductionPayment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cfo.Cats.Domain.Entities.ManagementInformation.EnrolmentPayment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Approved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ConsentAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ConsentSigned")
                        .HasColumnType("datetime2");

                    b.Property<string>("ContractId")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("EligibleForPayment")
                        .HasColumnType("bit");

                    b.Property<string>("IneligibilityReason")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("LocationType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)")
                        .HasDefaultValue("");

                    b.Property<string>("ParticipantId")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<string>("ReferralRoute")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("SubmissionToAuthority")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SubmissionToPqa")
                        .HasColumnType("datetime2");

                    b.Property<int>("SubmissionsToAuthority")
                        .HasColumnType("int");

                    b.Property<string>("SupportWorker")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("EnrolmentPayment", "Attachments");
                });

            modelBuilder.Entity("Cfo.Cats.Domain.Entities.ManagementInformation.InductionPayment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Approved")
                        .HasColumnType("datetime2");

                    b.Property<string>("ContractId")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("EligibleForPayment")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Induction")
                        .HasColumnType("datetime2");

                    b.Property<string>("IneligibilityReason")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("LocationType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)")
                        .HasDefaultValue("");

                    b.Property<string>("ParticipantId")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<string>("SupportWorker")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId", "ContractId")
                        .HasDatabaseName("ix_InductionPayment_ParticipantId");

                    b.ToTable("InductionPayment", "Attachments");
                });
#pragma warning restore 612, 618
        }
    }
}
