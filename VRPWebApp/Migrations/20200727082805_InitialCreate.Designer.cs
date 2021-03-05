﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models;

namespace VRPWebApp.Migrations
{
    [DbContext(typeof(VrpContext))]
    [Migration("20200727082805_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VRPWebApp.Models.VrpOrToolsRequestLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Configuration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTimeOfRequest")
                        .HasColumnType("datetime2");

                    b.Property<string>("Demands")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DemandsByType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Depots")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DistanceMatrix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ends")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Locations")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PickupsDeliveries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Starts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimeWindows")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleCapacitiesByType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VehicleLoadTime")
                        .HasColumnType("int");

                    b.Property<string>("VehicleLocationUnloadTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("VehicleUnloadTime")
                        .HasColumnType("bigint");

                    b.Property<string>("Vehicles")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VrpOrToolsRequestLogs");
                });

            modelBuilder.Entity("VRPWebApp.Models.VrpOrToolsResponseLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTimeOfResponse")
                        .HasColumnType("datetime2");

                    b.Property<string>("Response")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("requestLogId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("requestLogId");

                    b.ToTable("VrpOrToolsResponseLogs");
                });

            modelBuilder.Entity("VRPWebApp.Models.VrpOrToolsResponseLog", b =>
                {
                    b.HasOne("VRPWebApp.Models.VrpOrToolsRequestLog", "requestLog")
                        .WithMany()
                        .HasForeignKey("requestLogId");
                });
#pragma warning restore 612, 618
        }
    }
}