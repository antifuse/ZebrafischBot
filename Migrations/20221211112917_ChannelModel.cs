using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZebrafischBot.Migrations
{
    /// <inheritdoc />
    public partial class ChannelModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reactions",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RouxlsRole = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.AddColumn<string>(
                name: "Reactions",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
