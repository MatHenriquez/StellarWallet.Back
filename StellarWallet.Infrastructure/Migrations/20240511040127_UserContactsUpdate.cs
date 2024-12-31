using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StellarWallet.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UserContactsUpdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_UserContacts_BlockchainAccounts_BlockchainAccountId",
            table: "UserContacts");

        migrationBuilder.DropIndex(
            name: "IX_UserContacts_BlockchainAccountId",
            table: "UserContacts");

        migrationBuilder.DropIndex(
            name: "IX_UserContacts_UserId_BlockchainAccountId",
            table: "UserContacts");

        migrationBuilder.DropColumn(
            name: "BlockchainAccountId",
            table: "UserContacts");

        migrationBuilder.AddColumn<int>(
            name: "UserContactId",
            table: "BlockchainAccounts",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_UserId",
            table: "UserContacts",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BlockchainAccounts_UserContactId",
            table: "BlockchainAccounts",
            column: "UserContactId");

        migrationBuilder.AddForeignKey(
            name: "FK_BlockchainAccounts_UserContacts_UserContactId",
            table: "BlockchainAccounts",
            column: "UserContactId",
            principalTable: "UserContacts",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_BlockchainAccounts_UserContacts_UserContactId",
            table: "BlockchainAccounts");

        migrationBuilder.DropIndex(
            name: "IX_UserContacts_UserId",
            table: "UserContacts");

        migrationBuilder.DropIndex(
            name: "IX_BlockchainAccounts_UserContactId",
            table: "BlockchainAccounts");

        migrationBuilder.DropColumn(
            name: "UserContactId",
            table: "BlockchainAccounts");

        migrationBuilder.AddColumn<int>(
            name: "BlockchainAccountId",
            table: "UserContacts",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_BlockchainAccountId",
            table: "UserContacts",
            column: "BlockchainAccountId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserContacts_UserId_BlockchainAccountId",
            table: "UserContacts",
            columns: new[] { "UserId", "BlockchainAccountId" },
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_UserContacts_BlockchainAccounts_BlockchainAccountId",
            table: "UserContacts",
            column: "BlockchainAccountId",
            principalTable: "BlockchainAccounts",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
