using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelligenceQueryEngine.Migrations
{
    /// <inheritdoc />
    public partial class updateseedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenderProbability",
                table: "Profiles",
                newName: "gender_probability");

            migrationBuilder.RenameColumn(
                name: "CountryProbability",
                table: "Profiles",
                newName: "country_probability");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Profiles",
                newName: "country_id");

            migrationBuilder.RenameColumn(
                name: "AgeGroup",
                table: "Profiles",
                newName: "age_group");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "gender_probability",
                table: "Profiles",
                newName: "GenderProbability");

            migrationBuilder.RenameColumn(
                name: "country_probability",
                table: "Profiles",
                newName: "CountryProbability");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "Profiles",
                newName: "CountryId");

            migrationBuilder.RenameColumn(
                name: "age_group",
                table: "Profiles",
                newName: "AgeGroup");
        }
    }
}
