using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VRPWebApp.Migrations
{
    public partial class Inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VrpOrToolsRequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Locations = table.Column<string>(nullable: true),
                    DistanceMatrix = table.Column<string>(nullable: true),
                    Vehicles = table.Column<string>(nullable: true),
                    PickupsDeliveries = table.Column<string>(nullable: true),
                    TimeWindows = table.Column<string>(nullable: true),
                    Depots = table.Column<string>(nullable: true),
                    Demands = table.Column<string>(nullable: true),
                    VehicleLoadTime = table.Column<int>(nullable: false),
                    VehicleUnloadTime = table.Column<int>(nullable: false),
                    VehicleLocationUnloadTime = table.Column<string>(nullable: true),
                    DemandsByType = table.Column<string>(nullable: true),
                    VehicleCapacitiesByType = table.Column<string>(nullable: true),
                    Starts = table.Column<string>(nullable: true),
                    Ends = table.Column<string>(nullable: true),
                    Configuration = table.Column<string>(nullable: true),
                    DateTimeOfRequest = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VrpOrToolsRequestLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VrpOrToolsResponseLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeOfResponse = table.Column<DateTime>(nullable: false),
                    Response = table.Column<string>(nullable: true),
                    requestLogId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VrpOrToolsResponseLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VrpOrToolsResponseLogs_VrpOrToolsRequestLogs_requestLogId",
                        column: x => x.requestLogId,
                        principalTable: "VrpOrToolsRequestLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VrpOrToolsResponseLogs_requestLogId",
                table: "VrpOrToolsResponseLogs",
                column: "requestLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VrpOrToolsResponseLogs");

            migrationBuilder.DropTable(
                name: "VrpOrToolsRequestLogs");
        }
    }
}
