using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniBet.UserService.Infrastructure.Data.Entities;

namespace MiniBet.UserService.Infrastructure.Data.EntityConfigurations;

public class OutboxMessageConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EventType).IsRequired();
        builder.Property(e => e.Payload).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}