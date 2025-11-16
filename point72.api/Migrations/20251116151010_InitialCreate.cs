using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace point72.api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InversionRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Request = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Response = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestCount = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InversionRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InversionRecords_CreatedAt",
                table: "InversionRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InversionRecords_Request_Response",
                table: "InversionRecords",
                columns: new[] { "Request", "Response" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InversionRecords");
        }
    }
}
