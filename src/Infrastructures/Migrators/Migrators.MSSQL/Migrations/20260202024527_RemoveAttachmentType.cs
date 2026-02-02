using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAttachmentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentType",
                schema: "Chat",
                table: "MessageAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttachmentType",
                schema: "Chat",
                table: "MessageAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
