using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFileStorageDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentType",
                schema: "Common",
                table: "FileStorages");

            migrationBuilder.DropColumn(
                name: "Module",
                schema: "Common",
                table: "FileStorages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                schema: "Common",
                table: "FileStorages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Module",
                schema: "Common",
                table: "FileStorages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
