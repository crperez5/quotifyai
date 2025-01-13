using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace MinimalApi.Conversations;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
                {
                    entity.ToContainer(configuration["CosmosDbTableName"])
                        .HasPartitionKey(c => c.UserId)
                        .HasKey(c => c.Id);
                        
                    entity.Navigation(c => c.Messages)
                        .HasField("_messages")
                        .UsePropertyAccessMode(PropertyAccessMode.Field);

                    entity.Property(c => c.UserId)
                        .ToJsonProperty("userId")
                        .IsRequired();

                    entity.Property(c => c.Id)
                        .ToJsonProperty("id")
                        .IsRequired()
                        .HasValueGenerator<GuidValueGenerator>();

                    entity.Property(c => c.StartedAt)
                        .ToJsonProperty("startedAt")
                        .IsRequired();

                    entity.Property(c => c.LastMessageAt)
                        .ToJsonProperty("lastMessageAt")
                        .IsRequired();

                    entity.OwnsMany(c => c.Messages, message =>
                    {
                        message.ToJsonProperty("messages");
                        message.UsePropertyAccessMode(PropertyAccessMode.Field);

                        message.Property(m => m.Id)
                            .ToJsonProperty("id")
                            .IsRequired();

                        message.Property(m => m.Content)
                            .ToJsonProperty("content")
                            .IsRequired();

                        message.Property(m => m.Role)
                            .ToJsonProperty("role")
                            .IsRequired();

                        message.Property(m => m.Timestamp)
                            .ToJsonProperty("timestamp")
                            .IsRequired();
                    });
                });
    }
}