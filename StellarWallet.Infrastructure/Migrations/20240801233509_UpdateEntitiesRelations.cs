using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StellarWallet.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateEntitiesRelations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_UserContacts_Id",
            table: "UserContacts");

        migrationBuilder.AlterColumn<string>(
            name: "Role",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "guest",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "PublicKey",
            table: "UserContacts",
            type: "nvarchar(56)",
            maxLength: 56,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Role",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldDefaultValue: "guest");

        migrationBuilder.AlterColumn<string>(
            name: "PublicKey",
            table: "UserContacts",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(56)",
            oldMaxLength: 56);

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_Id",
            table: "UserContacts",
            column: "Id",
            unique: true);
    }
}
