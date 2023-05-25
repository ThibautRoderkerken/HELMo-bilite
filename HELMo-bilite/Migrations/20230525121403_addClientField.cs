using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HELMo_bilite.Migrations
{
    /// <inheritdoc />
    public partial class addClientField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientUserId",
                table: "Deliveries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ClientUserId",
                table: "Deliveries",
                column: "ClientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Clients_ClientUserId",
                table: "Deliveries",
                column: "ClientUserId",
                principalTable: "Clients",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Clients_ClientUserId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_ClientUserId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "ClientUserId",
                table: "Deliveries");
        }
    }
}
