using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddCorporateProfileAdditionalFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Columns already exist in DB via manual script.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualTurnover",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "ClientSegmentation",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "NatureOfBusiness",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "OtherEntityType",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "SourceOfFunds",
                table: "TBL_CUSTOMER");
        }
    }
}
