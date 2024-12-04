using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunterApi.Migrations
{
    /// <inheritdoc />
    public partial class newserverinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "PendingRegistrations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "PendingRegistrations");
        }
    }
}
