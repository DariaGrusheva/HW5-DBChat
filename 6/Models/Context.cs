using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6.Models
{
    public class Context : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public Context() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine).UseLazyLoadingProxies().UseNpgsql("Host=localhost;Username=postgres;Password=example;Database=ChatDB");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity => {

                entity.HasKey(x => x.Id).HasName("message_pkey");
                entity.ToTable("Messages");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Text).HasColumnName("text");
                entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
                entity.Property(e => e.ToUserId).HasColumnName("to_user_id");

                entity.HasOne(d => d.FromUser).WithMany(p => p.FromMessages)
                .HasForeignKey(e => e.FromUserId).HasConstraintName("messages_from_user_id_fkey");
                entity.HasOne(d => d.ToUser).WithMany(p => p.ToMessages)
                .HasForeignKey(e => e.ToUserId).HasConstraintName("messages_to_user_id_fkey");
            });
            modelBuilder.Entity<User>(entity => {

                entity.HasKey(x => x.Id).HasName("user_pkey");
                entity.ToTable("Users");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Name).HasMaxLength(255).HasColumnName("name");



            });
            base.OnModelCreating(modelBuilder);
        }

    }
}

    //dotnet ef migrations add InitialCreate команда миграции
    //dotnet tool install --global dotnet-ef если не работает 
