using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StellarWallet.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRoleInUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Role",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Role",
            table: "Users");
    }
}
