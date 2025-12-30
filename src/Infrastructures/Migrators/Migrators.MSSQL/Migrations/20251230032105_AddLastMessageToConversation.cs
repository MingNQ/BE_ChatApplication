using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddLastMessageToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastMessageAt",
                schema: "Chat",
                table: "Conversations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastMessageId",
                schema: "Chat",
                table: "Conversations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentAt",
                schema: "Chat",
                table: "Messages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastMessageAt",
                schema: "Chat",
                table: "Conversations",
                column: "LastMessageAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SentAt",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_LastMessageAt",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessageAt",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                schema: "Chat",
                table: "Conversations");
        }
    }
}
