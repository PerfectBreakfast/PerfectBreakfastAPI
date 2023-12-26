﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PerfectBreakfast.Infrastructure;

#nullable disable

namespace PerfectBreakfast.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.DeliveryAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeliveryUnitId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ManagementUnitId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryUnitId");

                    b.HasIndex("ManagementUnitId");

                    b.ToTable("DeliveryAssignments");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.DeliveryUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("Id");

                    b.ToTable("DeliveryUnits");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Food", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Foods");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.ManagementUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("Id");

                    b.ToTable("ManagementUnits");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Menu", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.MenuFood", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("MenuId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.HasIndex("MenuId");

                    b.ToTable("MenuFoods");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeliveryUnitId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ManagementUnitId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("ShipperId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("SupplierId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid?>("WorkerId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryUnitId");

                    b.HasIndex("ManagementUnitId");

                    b.HasIndex("ShipperId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("WorkerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.OrderDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("FoodId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.OrderHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Action")
                        .HasColumnType("int");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("OrderHistories");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.PaymentMethod", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Supplier", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.SupplyAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ManagementUnitId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("SupplierId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ManagementUnitId");

                    b.HasIndex("SupplierId");

                    b.ToTable("SupplyAssignments");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("CompanyId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeleteBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DeletionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DeliveryUnitId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ManagementUnitId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ModificationBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModificationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("SupplierId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DeliveryUnitId");

                    b.HasIndex("ManagementUnitId");

                    b.HasIndex("RoleId");

                    b.HasIndex("SupplierId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.DeliveryAssignment", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.DeliveryUnit", "DeliveryUnit")
                        .WithMany("DeliveryAssignments")
                        .HasForeignKey("DeliveryUnitId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.ManagementUnit", "ManagementUnit")
                        .WithMany("DeliveryAssignments")
                        .HasForeignKey("ManagementUnitId");

                    b.Navigation("DeliveryUnit");

                    b.Navigation("ManagementUnit");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Food", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.Category", "Category")
                        .WithMany("Foods")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.MenuFood", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.Food", "Food")
                        .WithMany("MenuFoods")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerfectBreakfast.Domain.Entities.Menu", "Menu")
                        .WithMany("MenuFoods")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");

                    b.Navigation("Menu");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Order", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.DeliveryUnit", "DeliveryUnit")
                        .WithMany("Orders")
                        .HasForeignKey("DeliveryUnitId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.ManagementUnit", "ManagementUnit")
                        .WithMany("Orders")
                        .HasForeignKey("ManagementUnitId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.User", "Shipper")
                        .WithMany("OrdersShipper")
                        .HasForeignKey("ShipperId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.Supplier", "Supplier")
                        .WithMany("Orders")
                        .HasForeignKey("SupplierId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.User", "Worker")
                        .WithMany("OrdersWorker")
                        .HasForeignKey("WorkerId");

                    b.Navigation("DeliveryUnit");

                    b.Navigation("ManagementUnit");

                    b.Navigation("Shipper");

                    b.Navigation("Supplier");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.OrderDetail", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.Food", "Food")
                        .WithMany("OrderDetails")
                        .HasForeignKey("FoodId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId");

                    b.Navigation("Food");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.OrderHistory", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.Order", "Order")
                        .WithMany("OrderHistories")
                        .HasForeignKey("OrderId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.User", "User")
                        .WithMany("OrderHistories")
                        .HasForeignKey("UserId");

                    b.Navigation("Order");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.PaymentMethod", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.Order", "Order")
                        .WithOne("PaymentMethod")
                        .HasForeignKey("PerfectBreakfast.Domain.Entities.PaymentMethod", "OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.SupplyAssignment", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.ManagementUnit", "ManagementUnit")
                        .WithMany("SupplyAssignments")
                        .HasForeignKey("ManagementUnitId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.Supplier", "Supplier")
                        .WithMany("SupplyAssignments")
                        .HasForeignKey("SupplierId");

                    b.Navigation("ManagementUnit");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.User", b =>
                {
                    b.HasOne("PerfectBreakfast.Domain.Entities.Company", "Company")
                        .WithMany("Workers")
                        .HasForeignKey("CompanyId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.DeliveryUnit", "DeliveryUnit")
                        .WithMany("Users")
                        .HasForeignKey("DeliveryUnitId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.ManagementUnit", "ManagementUnit")
                        .WithMany("Users")
                        .HasForeignKey("ManagementUnitId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("PerfectBreakfast.Domain.Entities.Supplier", "Supplier")
                        .WithMany("Users")
                        .HasForeignKey("SupplierId");

                    b.Navigation("Company");

                    b.Navigation("DeliveryUnit");

                    b.Navigation("ManagementUnit");

                    b.Navigation("Role");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Category", b =>
                {
                    b.Navigation("Foods");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Company", b =>
                {
                    b.Navigation("Workers");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.DeliveryUnit", b =>
                {
                    b.Navigation("DeliveryAssignments");

                    b.Navigation("Orders");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Food", b =>
                {
                    b.Navigation("MenuFoods");

                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.ManagementUnit", b =>
                {
                    b.Navigation("DeliveryAssignments");

                    b.Navigation("Orders");

                    b.Navigation("SupplyAssignments");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Menu", b =>
                {
                    b.Navigation("MenuFoods");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("OrderHistories");

                    b.Navigation("PaymentMethod");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.Supplier", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("SupplyAssignments");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("PerfectBreakfast.Domain.Entities.User", b =>
                {
                    b.Navigation("OrderHistories");

                    b.Navigation("OrdersShipper");

                    b.Navigation("OrdersWorker");
                });
#pragma warning restore 612, 618
        }
    }
}
