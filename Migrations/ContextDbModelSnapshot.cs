﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UpxApi.Data;

#nullable disable

namespace UpxApi.Migrations
{
    [DbContext(typeof(ContextDb))]
    partial class ContextDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("UpxApi.Models.Region", b =>
                {
                    b.Property<int>("RegionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RegionId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.HasKey("RegionId");

                    b.ToTable("Regions", (string)null);
                });

            modelBuilder.Entity("UpxApi.Models.Spot", b =>
                {
                    b.Property<int>("SpotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SpotId"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<double?>("Latitude")
                        .IsRequired()
                        .HasColumnType("float(53)");

                    b.Property<double?>("Longitude")
                        .IsRequired()
                        .HasColumnType("float(53)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<bool>("Occupied")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(50)")
                        .HasDefaultValue("car");

                    b.HasKey("SpotId");

                    b.ToTable("Spots", (string)null);
                });

            modelBuilder.Entity("UpxApi.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ConfirmPassword")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Ra")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<int?>("SpotId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Students", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}