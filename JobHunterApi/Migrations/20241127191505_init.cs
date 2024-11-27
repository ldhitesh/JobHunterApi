using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunterApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_References",
                table: "References");

            migrationBuilder.RenameTable(
                name: "References",
                newName: "CompanyReferences");

            migrationBuilder.RenameColumn(
                name: "Oraganization",
                table: "CompanyReferences",
                newName: "Organization");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Companies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "CompanyReferences",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "CompanyReferences",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyReferences",
                table: "CompanyReferences",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PendingRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRegistrations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingRegistrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyReferences",
                table: "CompanyReferences");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "CompanyReferences");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "CompanyReferences");

            migrationBuilder.RenameTable(
                name: "CompanyReferences",
                newName: "References");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "References",
                newName: "Oraganization");

            migrationBuilder.AddPrimaryKey(
                name: "PK_References",
                table: "References",
                column: "Id");
        }
    }
}
