using Microsoft.EntityFrameworkCore.Migrations;

namespace ClemBot.Api.Data.Migrations
{
    public partial class fixedremindmodelagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Messages_MessageId",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_MessageId",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Reminders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MessageId",
                table: "Reminders",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_MessageId",
                table: "Reminders",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Messages_MessageId",
                table: "Reminders",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
