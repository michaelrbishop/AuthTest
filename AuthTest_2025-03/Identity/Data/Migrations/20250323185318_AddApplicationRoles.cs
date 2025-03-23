using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthTest_2025_03.Identity.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), "None", "NONE", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "SiteAdmin", "SITEADMIN", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "UserAdmin", "USERADMIN", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "User", "USER", Guid.NewGuid().ToString() },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
