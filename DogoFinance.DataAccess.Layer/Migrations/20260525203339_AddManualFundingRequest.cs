using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddManualFundingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "TBL_COMPANY_PROFILE");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "TBL_COMPANY_PROFILE",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_MANUAL_FUNDING_REQUEST",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "('Pending')"),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceiptPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByAdminId = table.Column<long>(type: "bigint", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_MANUAL_FUNDING_REQUEST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_MANUAL_FUNDING_REQUEST_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_MANUAL_FUNDING_REQUEST_TBL_USER_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_MANUAL_FUNDING_REQUEST_CustomerId",
                table: "TBL_MANUAL_FUNDING_REQUEST",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_MANUAL_FUNDING_REQUEST_ReviewedByAdminId",
                table: "TBL_MANUAL_FUNDING_REQUEST",
                column: "ReviewedByAdminId");
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
