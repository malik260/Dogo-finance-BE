using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddStatusToCorporateDirSig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "TBL_CORPORATE_SIGNATORY",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TBL_CORPORATE_SIGNATORY",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "TBL_CORPORATE_DIRECTOR",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TBL_CORPORATE_DIRECTOR",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "TBL_CORPORATE_SIGNATORY");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TBL_CORPORATE_SIGNATORY");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "TBL_CORPORATE_DIRECTOR");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TBL_CORPORATE_DIRECTOR");
        }
    }
}
