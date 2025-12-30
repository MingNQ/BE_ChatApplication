using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationReadStates_Conversations_ConversationId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_LastMessageAt",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ConversationReadStates_ConversationId_UserId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.DropColumn(
                name: "LastMessageAt",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.AddColumn<long>(
                name: "ConversationReadStateId",
                schema: "Chat",
                table: "Conversations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ConversationReadStateId",
                schema: "Chat",
                table: "Conversations",
                column: "ConversationReadStateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationReadStates_ConversationId_UserId_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                columns: new[] { "ConversationId", "UserId", "LastReadMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationReadStates_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                column: "LastReadMessageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationReadStates_Conversations_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                column: "LastReadMessageId",
                principalSchema: "Chat",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ConversationReadStates_ConversationReadStateId",
                schema: "Chat",
                table: "Conversations",
                column: "ConversationReadStateId",
                principalSchema: "Chat",
                principalTable: "ConversationReadStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationReadStates_Conversations_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConversationReadStates_ConversationReadStateId",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ConversationReadStateId",
                schema: "Chat",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ConversationReadStates_ConversationId_UserId_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.DropIndex(
                name: "IX_ConversationReadStates_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.DropColumn(
                name: "ConversationReadStateId",
                schema: "Chat",
                table: "Conversations");

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
                name: "IX_Conversations_LastMessageAt",
                schema: "Chat",
                table: "Conversations",
                column: "LastMessageAt");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationReadStates_ConversationId_UserId",
                schema: "Chat",
                table: "ConversationReadStates",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationReadStates_Conversations_ConversationId",
                schema: "Chat",
                table: "ConversationReadStates",
                column: "ConversationId",
                principalSchema: "Chat",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
