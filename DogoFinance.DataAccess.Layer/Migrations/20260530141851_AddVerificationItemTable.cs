using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddVerificationItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_VERIFICATION_ITEM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSystemVerified = table.Column<bool>(type: "bit", nullable: false),
                    SystemRule = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetEntityTypes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_VERIFICATION_ITEM", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TBL_VERIFICATION_ITEM",
                columns: new[] { "Id", "DisplayOrder", "Icon", "IsActive", "IsSystemVerified", "Name", "SystemRule", "TargetEntityTypes", "Type" },
                values: new object[,]
                {
                    { 1, 1, "ri-file-list-3-line", true, true, "1. Completed Application Form", "CheckAppForm", "Corporate", "appForm" },
                    { 2, 2, "ri-verified-badge-line", true, false, "2. Certificate of Incorporation", null, "Corporate", "incorporation" },
                    { 3, 3, "ri-user-line", true, false, "3. Passport Photography of each Authorized Signatory", null, "Corporate", "passport" },
                    { 4, 4, "ri-book-read-line", true, false, "4. Memorandum & Articles of Association", null, "Corporate", "memart" },
                    { 5, 5, "ri-pie-chart-line", true, false, "5. Form CAC 2 (Return of Allotment of Shares)", null, "Corporate", "cac2" },
                    { 6, 6, "ri-folder-user-line", true, false, "6. Form CAC 7 (Particulars of Directors)", null, "Corporate", "cac7" },
                    { 7, 7, "ri-map-pin-user-line", true, false, "7. Form CAC 3 (Notice of Situation/Change of Registered Address)", null, "Corporate", "cac3" },
                    { 8, 8, "ri-shield-user-line", true, false, "8. Copy of Identification of Authorized Signatories and Directors", null, "Corporate", "signatoryId" },
                    { 9, 9, "ri-team-line", true, false, "9. Board Resolution/minutes of meeting confirming Authorized Signatories", null, "Corporate", "boardResolution" },
                    { 10, 10, "ri-bank-line", true, true, "10. Link Settlement Bank Account", "CheckBankLinked", "Corporate", "settlementLink" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_VERIFICATION_ITEM");
        }
    }
}
