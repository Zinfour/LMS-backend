using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class CompletedActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityApplicationUser",
                columns: table => new
                {
                    CompletedActivitiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedUsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityApplicationUser", x => new { x.CompletedActivitiesId, x.CompletedUsersId });
                    table.ForeignKey(
                        name: "FK_ActivityApplicationUser_Activity_CompletedActivitiesId",
                        column: x => x.CompletedActivitiesId,
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityApplicationUser_AspNetUsers_CompletedUsersId",
                        column: x => x.CompletedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityApplicationUser_CompletedUsersId",
                table: "ActivityApplicationUser",
                column: "CompletedUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityApplicationUser");
        }
    }
}
