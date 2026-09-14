using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AddTmdbMovieData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "WatchlistItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "PosterPath",
                table: "WatchlistItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TmdbId",
                table: "WatchlistItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistItems_UserId_TmdbId",
                table: "WatchlistItems",
                columns: new[] { "UserId", "TmdbId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WatchlistItems_UserId_TmdbId",
                table: "WatchlistItems");

            migrationBuilder.DropColumn(
                name: "PosterPath",
                table: "WatchlistItems");

            migrationBuilder.DropColumn(
                name: "TmdbId",
                table: "WatchlistItems");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "WatchlistItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
