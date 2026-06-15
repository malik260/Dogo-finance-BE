using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class AddAddressAndNatureOfBusinessSplit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "TBL_CUSTOMER",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NatureOfBusinessId",
                table: "TBL_CUSTOMER",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "TBL_CUSTOMER",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_COUNTRY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_COUNTRY", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_NATURE_OF_BUSINESS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_NATURE_OF_BUSINESS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_STATE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_STATE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_STATE_TBL_COUNTRY_CountryId",
                        column: x => x.CountryId,
                        principalTable: "TBL_COUNTRY",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TBL_COUNTRY",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "NG", "Nigeria" },
                    { 2, "US", "United States" }
                });

            migrationBuilder.InsertData(
                table: "TBL_NATURE_OF_BUSINESS",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Real Estate" },
                    { 2, true, "Agriculture" },
                    { 3, true, "Technology" },
                    { 4, true, "Finance" },
                    { 5, true, "Manufacturing" }
                });

            migrationBuilder.InsertData(
                table: "TBL_STATE",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Lagos" },
                    { 2, 1, "Abuja" },
                    { 3, 1, "Rivers" },
                    { 4, 2, "New York" },
                    { 5, 2, "California" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_CountryId",
                table: "TBL_CUSTOMER",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_NatureOfBusinessId",
                table: "TBL_CUSTOMER",
                column: "NatureOfBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_StateId",
                table: "TBL_CUSTOMER",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_STATE_CountryId",
                table: "TBL_STATE",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_CUSTOMER_COUNTRY",
                table: "TBL_CUSTOMER",
                column: "CountryId",
                principalTable: "TBL_COUNTRY",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_CUSTOMER_NATUREOFBUSINESS",
                table: "TBL_CUSTOMER",
                column: "NatureOfBusinessId",
                principalTable: "TBL_NATURE_OF_BUSINESS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_CUSTOMER_STATE",
                table: "TBL_CUSTOMER",
                column: "StateId",
                principalTable: "TBL_STATE",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_CUSTOMER_COUNTRY",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_CUSTOMER_NATUREOFBUSINESS",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_CUSTOMER_STATE",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropTable(
                name: "TBL_NATURE_OF_BUSINESS");

            migrationBuilder.DropTable(
                name: "TBL_STATE");

            migrationBuilder.DropTable(
                name: "TBL_COUNTRY");

            migrationBuilder.DropIndex(
                name: "IX_TBL_CUSTOMER_CountryId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropIndex(
                name: "IX_TBL_CUSTOMER_NatureOfBusinessId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropIndex(
                name: "IX_TBL_CUSTOMER_StateId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "NatureOfBusinessId",
                table: "TBL_CUSTOMER");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "TBL_CUSTOMER");
        }
    }
}
