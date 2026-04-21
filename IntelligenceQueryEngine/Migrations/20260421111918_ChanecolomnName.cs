using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelligenceQueryEngine.Migrations
{
    /// <inheritdoc />
    public partial class ChanecolomnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Profiles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Profiles",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Profiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "gender_probability",
                table: "Profiles",
                newName: "GenderProbability");

            migrationBuilder.RenameColumn(
                name: "country_probability",
                table: "Profiles",
                newName: "CountryProbability");

            migrationBuilder.RenameColumn(
                name: "country_name",
                table: "Profiles",
                newName: "CountryName");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "Profiles",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_name",
                table: "Profiles",
                newName: "IX_Profiles_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Profiles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Profiles",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Profiles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "GenderProbability",
                table: "Profiles",
                newName: "gender_probability");

            migrationBuilder.RenameColumn(
                name: "CountryProbability",
                table: "Profiles",
                newName: "country_probability");

            migrationBuilder.RenameColumn(
                name: "CountryName",
                table: "Profiles",
                newName: "country_name");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Profiles",
                newName: "country_id");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_Name",
                table: "Profiles",
                newName: "IX_Profiles_name");
        }
    }
}
