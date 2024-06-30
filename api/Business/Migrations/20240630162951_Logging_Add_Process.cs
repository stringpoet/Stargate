using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class Logging_Add_Process : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Process",
                table: "SuccessLog",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Process",
                table: "ExceptionLog",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "CareerStartDate",
                value: new DateTime(2024, 6, 30, 12, 29, 51, 235, DateTimeKind.Local).AddTicks(3155));

            migrationBuilder.UpdateData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1,
                column: "DutyStartDate",
                value: new DateTime(2024, 6, 30, 12, 29, 51, 235, DateTimeKind.Local).AddTicks(3210));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Process",
                table: "SuccessLog");

            migrationBuilder.DropColumn(
                name: "Process",
                table: "ExceptionLog");

            migrationBuilder.UpdateData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "CareerStartDate",
                value: new DateTime(2024, 6, 30, 12, 23, 0, 794, DateTimeKind.Local).AddTicks(3586));

            migrationBuilder.UpdateData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1,
                column: "DutyStartDate",
                value: new DateTime(2024, 6, 30, 12, 23, 0, 794, DateTimeKind.Local).AddTicks(3636));
        }
    }
}
