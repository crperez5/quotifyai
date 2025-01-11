namespace MinimalApi.Conversations;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>()
            .ToContainer(configuration["CosmosDbTableName"])
            .HasPartitionKey(c => c.UserId)
            .HasKey(c => c.Id);

        modelBuilder.Entity<Conversation>()
            .OwnsMany(c => c.Messages);
    }
}