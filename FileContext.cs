using CharacterGrade.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterGrade
{
    public class FileContext : DbContext
    {
        public DbSet<FileMetadata> Files { get; set; }
        public DbSet<FolderMetadata> Folders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=FilesMetadata.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileMetadata>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Guid).HasDefaultValueSql("newid()");
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Path).IsRequired();
                entity.Property(e => e.Metadata);

                entity.HasOne(e => e.FolderMetadata)
                   .WithMany()
                   .HasForeignKey(e => e.FolderMetadataId)
                   .IsRequired();
            });

            modelBuilder.Entity<FolderMetadata>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Guid).HasDefaultValueSql("newid()");
                entity.Property(e => e.Path).IsRequired();
                entity.Property(e => e.Metadata);
            });
        }
    }
}
