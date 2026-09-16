using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalonServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalonServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SalonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Duration = table.Column<long>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonServices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalonServices_SalonId_IsActive",
                table: "SalonServices",
                columns: new[] { "SalonId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_SalonServices_SalonId_Name",
                table: "SalonServices",
                columns: new[] { "SalonId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalonServices");
        }
    }
}
