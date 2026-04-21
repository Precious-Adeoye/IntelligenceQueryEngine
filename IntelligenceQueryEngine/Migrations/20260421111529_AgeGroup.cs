using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelligenceQueryEngine.Migrations
{
    /// <inheritdoc />
    public partial class AgeGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "age_group",
                table: "Profiles",
                newName: "AgeGroup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AgeGroup",
                table: "Profiles",
                newName: "age_group");
        }
    }
}
