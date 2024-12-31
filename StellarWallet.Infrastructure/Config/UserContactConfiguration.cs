using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure.Config;

public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
{
    public void Configure(EntityTypeBuilder<UserContact> builder)
    {
        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.Alias)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(uc => uc.PublicKey)
            .IsRequired()
            .HasMaxLength(56);
        builder.Property(uc => uc.UserId)
            .IsRequired();

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserContacts)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
