using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeXiecheng.API.Migrations
{
    /// <inheritdoc />
    public partial class ShoppingCartMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "7131d408-91d0-4a31-a0a3-999d5f3a1671");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b00b1da-b1ee-4099-b609-98a5f7cbaa92", "AQAAAAEAACcQAAAAEB1zMmeMnZmGuGSBVRBfMSPipRMbVc6oLKMPn3V940a/e+vXBvDN1fjAM9mXCNwcuA==", "0ba89bef-1af0-4af0-a68c-36bc60d42083" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "ee69019c-3008-4286-812f-d760ab6e8e84");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b7b6e568-e0c7-4011-a8be-1aa54346baaf", "AQAAAAEAACcQAAAAEG0jGSie2gGBZUbTszSrBhZV8nG2Qpzp0O0MlD9IqUw9exGPrdJasc1PsGq54kjQ/A==", "ecd953a6-1ba9-4a0b-adb2-9681bd001076" });
        }
    }
}
