using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApíRecognizer.Models
{
    public partial class AzureRecognizerContext : DbContext
    {
        public AzureRecognizerContext()
        {
        }

        public AzureRecognizerContext(DbContextOptions<AzureRecognizerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LogAzure> LogAzures { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Persist Security Info= True; User ID=sa; Pwd =  ; Server = ; Database= AzureRecognizer");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogAzure>(entity =>
            {
                entity.ToTable("LogAzure");

                entity.Property(e => e.Base64).IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentJson).IsUnicode(false);

                entity.Property(e => e.FechaHora).HasColumnType("datetime");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
