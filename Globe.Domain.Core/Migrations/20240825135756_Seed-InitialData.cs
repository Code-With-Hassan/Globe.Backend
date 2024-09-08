using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Globe.Domain.Core.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        private const string PROJECT_NAME = "Globe.Domain.Core";
        private const string MIGRATION_SQL_SCRIPT_FILE_NAME = @"SqlScripts\20240825135756_SeedInitialData.sql";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, PROJECT_NAME, MIGRATION_SQL_SCRIPT_FILE_NAME);
            migrationBuilder.Sql(File.ReadAllText(sql));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
