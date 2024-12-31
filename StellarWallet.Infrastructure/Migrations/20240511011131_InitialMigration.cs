using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StellarWallet.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PublicKey = table.Column<string>(type: "nvarchar(56)", maxLength: 56, nullable: false),
                SecretKey = table.Column<string>(type: "nvarchar(56)", maxLength: 56, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BlockchainAccounts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PublicKey = table.Column<string>(type: "nvarchar(56)", maxLength: 56, nullable: false),
                SecretKey = table.Column<string>(type: "nvarchar(56)", maxLength: 56, nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlockchainAccounts", x => x.Id);
                table.ForeignKey(
                    name: "FK_BlockchainAccounts_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserContacts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                BlockchainAccountId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserContacts", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserContacts_BlockchainAccounts_BlockchainAccountId",
                    column: x => x.BlockchainAccountId,
                    principalTable: "BlockchainAccounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserContacts_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_BlockchainAccounts_PublicKey",
            table: "BlockchainAccounts",
            column: "PublicKey",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BlockchainAccounts_UserId",
            table: "BlockchainAccounts",
            column: "UserId");

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

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserContacts");

        migrationBuilder.DropTable(
            name: "BlockchainAccounts");

        migrationBuilder.DropTable(
            name: "Users");
    }
}
