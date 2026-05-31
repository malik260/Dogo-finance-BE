using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddCorporateCustomerFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "TBL_CUSTOMER",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "TBL_CUSTOMER",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerTypeId",
                table: "TBL_CUSTOMER",
                type: "int",
                nullable: true,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfIncorporation",
                table: "TBL_CUSTOMER",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "TBL_CUSTOMER",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxIdentificationNumber",
                table: "TBL_CUSTOMER",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_CUSTOMER_TYPE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CUSTOMER_TYPE", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TBL_CUSTOMER_TYPE",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[] { 1, "Individual Customer Account", true, "Individual" });

            migrationBuilder.InsertData(
                table: "TBL_CUSTOMER_TYPE",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[] { 2, "Corporate/Business Account", true, "Corporate" });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_CustomerTypeId",
                table: "TBL_CUSTOMER",
                column: "CustomerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_CUSTOMER_CUSTOMER_TYPE",
                table: "TBL_CUSTOMER",
                column: "CustomerTypeId",
                principalTable: "TBL_CUSTOMER_TYPE",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_CUSTOMER_CUSTOMER_TYPE",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropTable(
                name: "TBL_CUSTOMER_TYPE");

            migrationBuilder.DropIndex(
                name: "IX_TBL_CUSTOMER_CustomerTypeId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "CustomerTypeId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "DateOfIncorporation",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationNumber",
                table: "TBL_CUSTOMER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "TBL_CUSTOMER",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }
    }
}
