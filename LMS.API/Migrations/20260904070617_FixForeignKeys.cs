using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Course_CourseId",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Module_ModuleId",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityResource_Activity_ActivityId",
                table: "ActivityResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseResource_Course_CourseId",
                table: "CourseResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleResource_Module_ModuleId",
                table: "ModuleResource");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_AspNetUsers_ApplicationUserId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_ApplicationUserId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Activity_CourseId",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Activity");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Submission",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "ModuleResource",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "ModuleResource",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "ModuleResource",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Module",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "CourseResource",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "CourseResource",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "CourseResource",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "ActivityResource",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "ActivityResource",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "ActivityResource",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "Activity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_StudentId",
                table: "Submission",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleResource_CreatedByUserId",
                table: "ModuleResource",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleResource_UpdatedByUserId",
                table: "ModuleResource",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseId",
                table: "Module",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResource_CreatedByUserId",
                table: "CourseResource",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResource_UpdatedByUserId",
                table: "CourseResource",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityResource_CreatedByUserId",
                table: "ActivityResource",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityResource_UpdatedByUserId",
                table: "ActivityResource",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Module_ModuleId",
                table: "Activity",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityResource_Activity_ActivityId",
                table: "ActivityResource",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityResource_AspNetUsers_CreatedByUserId",
                table: "ActivityResource",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityResource_AspNetUsers_UpdatedByUserId",
                table: "ActivityResource",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseResource_AspNetUsers_CreatedByUserId",
                table: "CourseResource",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseResource_AspNetUsers_UpdatedByUserId",
                table: "CourseResource",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseResource_Course_CourseId",
                table: "CourseResource",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Course_CourseId",
                table: "Module",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleResource_AspNetUsers_CreatedByUserId",
                table: "ModuleResource",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleResource_AspNetUsers_UpdatedByUserId",
                table: "ModuleResource",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleResource_Module_ModuleId",
                table: "ModuleResource",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_AspNetUsers_StudentId",
                table: "Submission",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Module_ModuleId",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityResource_Activity_ActivityId",
                table: "ActivityResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityResource_AspNetUsers_CreatedByUserId",
                table: "ActivityResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityResource_AspNetUsers_UpdatedByUserId",
                table: "ActivityResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseResource_AspNetUsers_CreatedByUserId",
                table: "CourseResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseResource_AspNetUsers_UpdatedByUserId",
                table: "CourseResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseResource_Course_CourseId",
                table: "CourseResource");

            migrationBuilder.DropForeignKey(
                name: "FK_Module_Course_CourseId",
                table: "Module");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleResource_AspNetUsers_CreatedByUserId",
                table: "ModuleResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleResource_AspNetUsers_UpdatedByUserId",
                table: "ModuleResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleResource_Module_ModuleId",
                table: "ModuleResource");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_AspNetUsers_StudentId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_StudentId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_ModuleResource_CreatedByUserId",
                table: "ModuleResource");

            migrationBuilder.DropIndex(
                name: "IX_ModuleResource_UpdatedByUserId",
                table: "ModuleResource");

            migrationBuilder.DropIndex(
                name: "IX_Module_CourseId",
                table: "Module");

            migrationBuilder.DropIndex(
                name: "IX_CourseResource_CreatedByUserId",
                table: "CourseResource");

            migrationBuilder.DropIndex(
                name: "IX_CourseResource_UpdatedByUserId",
                table: "CourseResource");

            migrationBuilder.DropIndex(
                name: "IX_ActivityResource_CreatedByUserId",
                table: "ActivityResource");

            migrationBuilder.DropIndex(
                name: "IX_ActivityResource_UpdatedByUserId",
                table: "ActivityResource");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Module");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Submission",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Submission",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "ModuleResource",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "ModuleResource",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "ModuleResource",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "CourseResource",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "CourseResource",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "CourseResource",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "ActivityResource",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "ActivityResource",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "ActivityResource",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "Activity",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Activity",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ApplicationUserId",
                table: "Submission",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_CourseId",
                table: "Activity",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Course_CourseId",
                table: "Activity",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Module_ModuleId",
                table: "Activity",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityResource_Activity_ActivityId",
                table: "ActivityResource",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseResource_Course_CourseId",
                table: "CourseResource",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleResource_Module_ModuleId",
                table: "ModuleResource",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_AspNetUsers_ApplicationUserId",
                table: "Submission",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
