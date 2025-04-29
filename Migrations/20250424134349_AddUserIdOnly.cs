using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusuMatchProject.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Expenses",
                type: "TEXT", // אם את ב־SQLite
                nullable: true); // כדי שלא יקרוס בטעות אם יש שורות ישנות
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Expenses");
        }
    }
}
