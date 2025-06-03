using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttenanceSystemApp.Migrations
{
    /// <inheritdoc />
    public partial class upravaDepartment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Departments");
        }
    }
}
