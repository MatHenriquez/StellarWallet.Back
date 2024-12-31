using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure.Config;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Password).IsRequired().IsUnicode();
        builder.Property(u => u.PublicKey).IsRequired().HasMaxLength(56);
        builder.Property(u => u.SecretKey).IsRequired().HasMaxLength(56);
        builder.Property(u => u.Role).HasDefaultValue("guest");

        builder.HasMany(u => u.BlockchainAccounts).WithOne(b => b.User).HasForeignKey(b => b.UserId);
        builder.HasMany(u => u.UserContacts).WithOne(uc => uc.User).HasForeignKey(uc => uc.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}
