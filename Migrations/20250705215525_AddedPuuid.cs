using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HexStats.Migrations
{
    /// <inheritdoc />
    public partial class AddedPuuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Puuid",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Puuid",
                table: "Users");
        }
    }
}
