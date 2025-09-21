using Linkfox.domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core Db Context. Maps Url and Click table
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<Url> Urls { get; set; } = null;
        public DbSet<Click> Clicks { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Url entity mapping
            modelBuilder.Entity<Url>(eb =>
            {
                eb.ToTable("Urls", "dbo");
                eb.HasKey(u => u.Id);

                // Match types and sizes in user DDL:
                eb.Property(u => u.ShortCode)
                  .IsRequired()
                  .HasColumnType("varchar(200)");

                eb.Property(u => u.LongUrl)
                  .IsRequired()
                  .HasColumnType("nvarchar(2048)");

                eb.Property(u => u.CreatedAt)
                  .HasColumnType("datetime2")
                  .HasDefaultValueSql("SYSUTCDATETIME()");

                eb.Property(u => u.ClickCount)
                  .HasDefaultValue(0);

                // Unique index on ShortCode
                eb.HasIndex(u => u.ShortCode)
                  .IsUnique()
                  .HasDatabaseName("IX_Urls_ShortCode");
            });

            // Click entity mapping
            modelBuilder.Entity<Click>(cb =>
            {
                cb.ToTable("Clicks", "dbo");
                cb.HasKey(c => c.Id);

                cb.Property(c => c.IpAddress)
                  .IsRequired()
                  .HasColumnType("varchar(45)");

                cb.Property(c => c.UserAgent)
                  .HasColumnType("nvarchar(1024)");

                cb.Property(c => c.Referrer)
                  .HasColumnType("nvarchar(1024)");

                cb.Property(c => c.AcceptLanguage)
                  .HasColumnType("nvarchar(200)");

                cb.Property(c => c.Country)
                  .HasColumnType("nvarchar(100)");

                cb.Property(c => c.City)
                  .HasColumnType("nvarchar(200)");

                cb.Property(c => c.DeviceCategory)
                  .HasColumnType("varchar(50)");

                cb.Property(c => c.ClickedAt)
                  .HasColumnType("datetime2")
                  .HasDefaultValueSql("SYSUTCDATETIME()");

                // FK relationship
                cb.HasOne(c => c.Url)
                  .WithMany(u => u.Clicks)
                  .HasForeignKey(c => c.UrlId)
                  .HasConstraintName("FK_Clicks_Urls")
                  .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
