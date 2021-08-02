﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository;

namespace Repository.Migrations.MariaDbMigrations
{
    [DbContext(typeof(MariaDbFiveStarDbContext))]
    partial class MariaDbFiveStarDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("Core.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ReferenceNumber")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ReferenceNumber");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("Core.MonthlyExpenses", b =>
                {
                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.Property<decimal>("Apr")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Aug")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Dec")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Feb")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Jan")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Jul")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Jun")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Mar")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("May")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Nov")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Oct")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Sep")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Year");

                    b.ToTable("MonthlyExpenses");
                });

            modelBuilder.Entity("Core.MonthlySales", b =>
                {
                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.Property<decimal>("Apr")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Aug")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Dec")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Feb")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Jan")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Jul")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Jun")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Mar")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("May")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Nov")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Oct")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Sep")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Year");

                    b.ToTable("MonthlySales");
                });

            modelBuilder.Entity("Core.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Cost")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<string>("Upc")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("Upc");

                    b.HasIndex("Name", "Upc");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Core.Invoice", b =>
                {
                    b.OwnsMany("Core.InvoiceItem", "Items", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<int>("InvoiceId")
                                .HasColumnType("int");

                            b1.Property<int?>("ProductId")
                                .HasColumnType("int");

                            b1.Property<int>("Quantity")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("InvoiceId");

                            b1.HasIndex("ProductId");

                            b1.ToTable("InvoiceItems");

                            b1.WithOwner()
                                .HasForeignKey("InvoiceId");

                            b1.HasOne("Core.Product", "Product")
                                .WithMany()
                                .HasForeignKey("ProductId");

                            b1.Navigation("Product");
                        });

                    b.Navigation("Items");
                });

            modelBuilder.Entity("Core.Product", b =>
                {
                    b.OwnsOne("Core.AuditLog", "AuditLog", b1 =>
                        {
                            b1.Property<int>("ProductId")
                                .HasColumnType("int");

                            b1.Property<string>("CreatedBy")
                                .HasColumnType("longtext")
                                .HasColumnName("CreatedBy");

                            b1.Property<DateTime>("CreatedTime")
                                .HasColumnType("datetime(6)")
                                .HasColumnName("CreatedTime");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("longtext")
                                .HasColumnName("ModifiedBy");

                            b1.Property<DateTime>("ModifiedTime")
                                .HasColumnType("datetime(6)")
                                .HasColumnName("ModifiedTime");

                            b1.HasKey("ProductId");

                            b1.ToTable("Products");

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.Navigation("AuditLog");
                });
#pragma warning restore 612, 618
        }
    }
}
