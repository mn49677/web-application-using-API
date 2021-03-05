using Microsoft.EntityFrameworkCore.Migrations;

namespace VRPWebApp.Migrations
{
    public partial class Update01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vehicles",
                table: "VrpOrToolsRequestLogs");

            migrationBuilder.AlterColumn<long>(
                name: "VehicleUnloadTime",
                table: "VrpOrToolsRequestLogs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VehicleNumber",
                table: "VrpOrToolsRequestLogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "VrpOrToolsRequestLogs");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleUnloadTime",
                table: "VrpOrToolsRequestLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "Vehicles",
                table: "VrpOrToolsRequestLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
