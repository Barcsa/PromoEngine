using Microsoft.EntityFrameworkCore;
using PromoEngine.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PromoEngine.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<WinningTimestamp> WinningTimestamps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PromoCode>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.PromoCode)
                .WithMany()
                .HasForeignKey(s => s.PromoCodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WinningTimestamp>()
                .HasOne(w => w.WinnerSubmission)
                .WithMany()
                .HasForeignKey(w => w.WinnerSubmissionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
