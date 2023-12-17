using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "68dba18c-7566-41d0-978b-67ddf0f27ab7", null, "Administrator", "ADMINISTRATOR" },
                    { "b6f48aa9-b3c4-4af1-9a41-222ab0f226de", null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68dba18c-7566-41d0-978b-67ddf0f27ab7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b6f48aa9-b3c4-4af1-9a41-222ab0f226de");
        }
    }
}