using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddCorporateSignatories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_CORPORATE_SIGNATORY",
                columns: table => new
                {
                    SignatoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OtherNames = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    ResidentialAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BusinessEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Bvn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SigningClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdentityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPep = table.Column<bool>(type: "bit", nullable: false),
                    PassportPhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SignatureCardUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IdentityDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CORPORATE_SIGNATORY", x => x.SignatoryId);
                    table.ForeignKey(
                        name: "FK_CORPORATE_SIGNATORY_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CORPORATE_SIGNATORY_CustomerId",
                table: "TBL_CORPORATE_SIGNATORY",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_CORPORATE_SIGNATORY");
        }
    }
}
