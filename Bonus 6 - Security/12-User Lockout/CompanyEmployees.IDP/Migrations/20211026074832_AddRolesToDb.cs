using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyEmployees.IDP.Migrations;

public partial class AddRolesToDb : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.InsertData(
			table: "AspNetRoles",
			columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
			values: new object[] { "6d506b42-9fa0-4ef7-a92a-0b5b0a123665", "9c9e9506-5104-477c-ae4e-647a3eab6979", "Visitor", "VISITOR" });

		migrationBuilder.InsertData(
			table: "AspNetRoles",
			columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
			values: new object[] { "c3a0cb55-ddaf-4f2f-8419-f3f937698aa1", "087cde98-aab2-4976-9781-aeabcd3bcfa4", "Administrator", "ADMINISTRATOR" });
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DeleteData(
			table: "AspNetRoles",
			keyColumn: "Id",
			keyValue: "6d506b42-9fa0-4ef7-a92a-0b5b0a123665");

		migrationBuilder.DeleteData(
			table: "AspNetRoles",
			keyColumn: "Id",
			keyValue: "c3a0cb55-ddaf-4f2f-8419-f3f937698aa1");
	}
}
