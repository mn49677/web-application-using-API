using Microsoft.EntityFrameworkCore.Migrations;

namespace VRPWebApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "VehicleUnloadTime",
                table: "VrpOrToolsRequestLogs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VehicleUnloadTime",
                table: "VrpOrToolsRequestLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
