using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelligenceQueryEngine.Migrations
{
    /// <inheritdoc />
    public partial class @int : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "profiles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "profiles",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profiles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CountryName",
                table: "profiles",
                newName: "country_name");

            migrationBuilder.RenameIndex(
                name: "IX_profiles_Name",
                table: "profiles",
                newName: "IX_profiles_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "profiles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "profiles",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "profiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "country_name",
                table: "profiles",
                newName: "CountryName");

            migrationBuilder.RenameIndex(
                name: "IX_profiles_name",
                table: "profiles",
                newName: "IX_profiles_Name");
        }
    }
}
