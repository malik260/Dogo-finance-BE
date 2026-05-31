using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddManualFundingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Table and columns already exist in DB.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_MANUAL_FUNDING_REQUEST");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "TBL_COMPANY_PROFILE");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "TBL_COMPANY_PROFILE",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
