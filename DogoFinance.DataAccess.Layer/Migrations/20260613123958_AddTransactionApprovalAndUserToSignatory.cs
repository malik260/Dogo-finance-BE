using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddTransactionApprovalAndUserToSignatory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "TBL_CORPORATE_SIGNATORY",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "TBL_CORPORATE_SIGNATORY",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_TRANSACTION_APPROVAL",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<long>(type: "bigint", nullable: false),
                    ApproverUserId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_TRANSACTION_APPROVAL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_TRANSACTION_APPROVAL_TBL_TRANSACTION_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "TBL_TRANSACTION",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_TRANSACTION_APPROVAL_TBL_USER_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CORPORATE_SIGNATORY_UserId",
                table: "TBL_CORPORATE_SIGNATORY",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TRANSACTION_APPROVAL_ApproverUserId",
                table: "TBL_TRANSACTION_APPROVAL",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TRANSACTION_APPROVAL_TransactionId",
                table: "TBL_TRANSACTION_APPROVAL",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_CORPORATE_SIGNATORY_TBL_USER_UserId",
                table: "TBL_CORPORATE_SIGNATORY",
                column: "UserId",
                principalTable: "TBL_USER",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_CORPORATE_SIGNATORY_TBL_USER_UserId",
                table: "TBL_CORPORATE_SIGNATORY");

            migrationBuilder.DropTable(
                name: "TBL_TRANSACTION_APPROVAL");

            migrationBuilder.DropIndex(
                name: "IX_TBL_CORPORATE_SIGNATORY_UserId",
                table: "TBL_CORPORATE_SIGNATORY");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TBL_CORPORATE_SIGNATORY");

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "TBL_CORPORATE_SIGNATORY",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
