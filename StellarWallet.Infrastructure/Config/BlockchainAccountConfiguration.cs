using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure.Config;

public class BlockchainAccountConfiguration : IEntityTypeConfiguration<BlockchainAccount>
{
    public void Configure(EntityTypeBuilder<BlockchainAccount> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.PublicKey).IsUnique();

        builder.Property(b => b.PublicKey).IsRequired().HasMaxLength(56);
        builder.Property(b => b.SecretKey).IsRequired().HasMaxLength(56);
        builder.Property(b => b.UserId).IsRequired();

        builder.HasOne(b => b.User)
            .WithMany(u => u.BlockchainAccounts)
            .HasForeignKey(b => b.UserId);
    }
}
