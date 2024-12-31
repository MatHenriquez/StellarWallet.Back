using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StellarWallet.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPublicKeyToUserContact : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PublicKey",
            table: "UserContacts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PublicKey",
            table: "UserContacts");
    }
}
