using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddBankCountryIdToCustomerBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankCountryId",
                table: "TBL_CUSTOMER_BANK",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_BANK_BankCountryId",
                table: "TBL_CUSTOMER_BANK",
                column: "BankCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_CUSTOMER_BANK_TBL_COUNTRY_BankCountryId",
                table: "TBL_CUSTOMER_BANK",
                column: "BankCountryId",
                principalTable: "TBL_COUNTRY",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_CUSTOMER_BANK_TBL_COUNTRY_BankCountryId",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropIndex(
                name: "IX_TBL_CUSTOMER_BANK_BankCountryId",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "BankCountryId",
                table: "TBL_CUSTOMER_BANK");
        }
    }
}
