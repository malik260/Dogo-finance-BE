using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddCorporateDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_CORPORATE_DOCUMENT",
                columns: table => new
                {
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByAdminId = table.Column<long>(type: "bigint", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CORPORATE_DOCUMENT", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_TBL_CORPORATE_DOCUMENT_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_CORPORATE_DOCUMENT_TBL_USER_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CORPORATE_DOCUMENT_CustomerId",
                table: "TBL_CORPORATE_DOCUMENT",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CORPORATE_DOCUMENT_ReviewedByAdminId",
                table: "TBL_CORPORATE_DOCUMENT",
                column: "ReviewedByAdminId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_CORPORATE_DOCUMENT");
        }
    }
}
