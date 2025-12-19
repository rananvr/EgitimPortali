using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgitimPortali.Migrations
{
    /// <inheritdoc />
    public partial class UserTablosuDuzeltme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_UserId1",
                table: "Enrollments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_UserId1",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Enrollments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Enrollments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_UserId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId1",
                table: "Enrollments",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_UserId1",
                table: "Enrollments",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
