using DataService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DataService.Infrastructure
{
    /// <summary>
    /// Represents the Entity Framework database context for the application.
    /// Provides access to entity sets and configures model relationships.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> representing the collection of <see cref="DataEntity"/> records in the database.
        /// </summary>
        public DbSet<DataEntity> DataEntities { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Configures the entity model and relationships using the <see cref="ModelBuilder"/>.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DataEntity>(entity =>
            {
                // Configure the Value property to be required and have a maximum length of 100 characters.
                entity.Property(e => e.Value).IsRequired().HasMaxLength(100);

                // Configure the CreatedAt property to be required.
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
