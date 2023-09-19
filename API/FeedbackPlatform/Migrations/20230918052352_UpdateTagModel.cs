using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackPlatform.Migrations
{
    public partial class UpdateTagModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("37cf0bb6-5716-4253-bf35-3e57fb932aca"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("86651582-0c5f-4f1f-bf5f-c3c08866f2f6"));

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Tags",
                newName: "Value");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("4b36cfdd-073a-42fc-a8a1-4aeeffe7b0ee"), "822aebd8-d688-4451-8211-dac7553b75d9", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("a6cf5194-7e3f-49b2-8558-33fd01e59a5e"), "98c11efe-f2c1-4f10-a081-5cdc3fd9f5b6", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4b36cfdd-073a-42fc-a8a1-4aeeffe7b0ee"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a6cf5194-7e3f-49b2-8558-33fd01e59a5e"));

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Tags",
                newName: "Text");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("37cf0bb6-5716-4253-bf35-3e57fb932aca"), "0fff5f4e-d47b-4307-bd00-708d1ef00f71", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("86651582-0c5f-4f1f-bf5f-c3c08866f2f6"), "fef3e41c-e217-4892-b076-aed08b871a21", "User", "USER" });
        }
    }
}
