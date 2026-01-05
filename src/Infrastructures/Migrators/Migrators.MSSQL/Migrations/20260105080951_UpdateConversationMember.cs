using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationMembers_ConversationRoleId",
                schema: "Chat",
                table: "ConversationMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_ConversationRoleId",
                schema: "Chat",
                table: "ConversationMembers",
                column: "ConversationRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationMembers_ConversationRoleId",
                schema: "Chat",
                table: "ConversationMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_ConversationRoleId",
                schema: "Chat",
                table: "ConversationMembers",
                column: "ConversationRoleId",
                unique: true);
        }
    }
}
