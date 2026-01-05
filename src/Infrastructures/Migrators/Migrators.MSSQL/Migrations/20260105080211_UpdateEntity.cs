using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                schema: "Chat",
                table: "ConversationReadStates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationReadStates_ConversationId_UserId_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                columns: new[] { "ConversationId", "UserId", "LastReadMessageId" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [LastReadMessageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationReadStates_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                column: "LastReadMessageId",
                unique: true,
                filter: "[LastReadMessageId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationReadStates_ConversationId_UserId_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.DropIndex(
                name: "IX_ConversationReadStates_LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                schema: "Chat",
                table: "ConversationReadStates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "LastReadMessageId",
                schema: "Chat",
                table: "ConversationReadStates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

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
                name: "FK_Conversations_ConversationReadStates_ConversationReadStateId",
                schema: "Chat",
                table: "Conversations",
                column: "ConversationReadStateId",
                principalSchema: "Chat",
                principalTable: "ConversationReadStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
