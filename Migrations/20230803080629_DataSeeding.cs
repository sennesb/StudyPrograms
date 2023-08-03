using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeXiecheng.API.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdatedTime" },
                values: new object[] { new Guid("ab49fad1-5348-414f-b3cb-9bf52e04efe1"), new DateTime(2023, 8, 3, 8, 6, 29, 644, DateTimeKind.Utc).AddTicks(9608), null, "shuoming", null, "<p>吃住行游购娱</p>", "<p>交通费用自理</p>", "<p>小心危险</p>", 0m, "ceshititle", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("ab49fad1-5348-414f-b3cb-9bf52e04efe1"));
        }
    }
}
