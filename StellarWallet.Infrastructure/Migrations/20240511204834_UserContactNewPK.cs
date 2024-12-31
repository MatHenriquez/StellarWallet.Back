using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StellarWallet.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UserContactNewPK : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_UserContacts_UserId",
            table: "UserContacts");

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_Id",
            table: "UserContacts",
            column: "Id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_UserId",
            table: "UserContacts",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_UserContacts_Id",
            table: "UserContacts");

        migrationBuilder.DropIndex(
            name: "IX_UserContacts_UserId",
            table: "UserContacts");

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_UserId",
            table: "UserContacts",
            column: "UserId",
            unique: true);
    }
}
