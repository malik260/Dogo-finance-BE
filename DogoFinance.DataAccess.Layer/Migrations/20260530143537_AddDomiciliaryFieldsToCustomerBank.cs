using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddDomiciliaryFieldsToCustomerBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryAccountName",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryAccountNumber",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryAddress",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondentBank",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValueSql: "('NGN')");

            migrationBuilder.AddColumn<string>(
                name: "FfcDetails",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SortCode",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "TBL_CUSTOMER_BANK",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficiaryAccountName",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAccountNumber",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAddress",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "CorrespondentBank",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "FfcDetails",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "SortCode",
                table: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "TBL_CUSTOMER_BANK");
        }
    }
}
