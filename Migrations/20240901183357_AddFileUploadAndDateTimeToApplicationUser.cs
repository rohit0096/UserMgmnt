using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMgmnt.Migrations
{
    public partial class AddFileUploadAndDateTimeToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUploadPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedDateTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUploadPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UploadedDateTime",
                table: "AspNetUsers");
        }
    }
}
