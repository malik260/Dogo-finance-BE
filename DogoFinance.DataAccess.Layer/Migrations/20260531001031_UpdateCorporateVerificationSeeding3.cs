using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class UpdateCorporateVerificationSeeding3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsSystemVerified", "SystemRule" },
                values: new object[] { true, "CheckSignatoryPhotos" });

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsSystemVerified", "SystemRule" },
                values: new object[] { true, "CheckDirectorsAdded" });

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsSystemVerified", "SystemRule" },
                values: new object[] { true, "CheckSignatoryDirectorsId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsSystemVerified", "SystemRule" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsSystemVerified", "SystemRule" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsSystemVerified", "SystemRule" },
                values: new object[] { false, null });
        }
    }
}
