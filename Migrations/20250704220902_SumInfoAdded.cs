using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HexStats.Migrations
{
    /// <inheritdoc />
    public partial class SumInfoAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SummonerIconId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummonerLevel",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummonerIconId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SummonerLevel",
                table: "Users");
        }
    }
}
