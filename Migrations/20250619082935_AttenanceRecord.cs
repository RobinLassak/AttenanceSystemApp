using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttenanceSystemApp.Migrations
{
    /// <inheritdoc />
    public partial class AttenanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttenanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttenanceIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    AttenanceOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    DoctorIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    DoctorOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    SmokeIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    SmokeOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsVacation = table.Column<bool>(type: "bit", nullable: false),
                    IsSickLeave = table.Column<bool>(type: "bit", nullable: false),
                    WorkedHours = table.Column<TimeSpan>(type: "time", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttenanceRecords_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttenanceRecords_EmployeeId",
                table: "AttenanceRecords",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttenanceRecords");
        }
    }
}
