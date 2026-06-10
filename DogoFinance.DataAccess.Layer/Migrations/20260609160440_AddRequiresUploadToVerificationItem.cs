using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddRequiresUploadToVerificationItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresUpload",
                table: "TBL_VERIFICATION_ITEM",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 2,
                column: "RequiresUpload",
                value: true);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 3,
                column: "RequiresUpload",
                value: true);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 4,
                column: "RequiresUpload",
                value: true);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 5,
                column: "RequiresUpload",
                value: true);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 7,
                column: "RequiresUpload",
                value: true);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 8,
                column: "RequiresUpload",
                value: true);

            migrationBuilder.UpdateData(
                table: "TBL_VERIFICATION_ITEM",
                keyColumn: "Id",
                keyValue: 9,
                column: "RequiresUpload",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresUpload",
                table: "TBL_VERIFICATION_ITEM");
        }
    }
}
