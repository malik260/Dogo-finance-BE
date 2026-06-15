using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddSourceOfFundsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_SOURCE_OF_FUND",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SOURCE_OF_FUND", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TBL_SOURCE_OF_FUND",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Investment" },
                    { 2, true, "Business Revenue" },
                    { 3, true, "Personal Savings" },
                    { 4, true, "Loan" },
                    { 5, true, "Gift/Inheritance" },
                    { 6, true, "Others" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_SOURCE_OF_FUND");
        }
    }
}
