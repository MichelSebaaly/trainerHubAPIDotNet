using Microsoft.EntityFrameworkCore;
using Data.Entities;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Program> programs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users", "identity");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.Phone_number).HasColumnName("phone_number").IsRequired();
                entity.Property(e => e.Password).HasColumnName("password").IsRequired();
                entity.Property(e => e.Profile_pic).HasColumnName("profile_pic");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.createdAt).HasColumnName("createdAt");
                entity.Property(e => e.updatedAt).HasColumnName("updatedAt");
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.ToTable("programs", "content");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Trainer_Id).HasColumnName("trainer_id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").IsRequired();
                entity.Property(e => e.Price).HasColumnName("price").IsRequired();
                entity.Property(e => e.File_URL).HasColumnName("file_url").IsRequired();
                entity.Property(e => e.Cover_Photo).HasColumnName("cover_photo");
                entity.Property(e => e.Equipment).HasColumnName("equipment");
                entity.Property(e => e.Goal).HasColumnName("goal");
                entity.Property(e => e.createdAt).HasColumnName("createdAt");
                entity.Property(e => e.updatedAt).HasColumnName("updatedAt");

                entity.HasOne(p => p.Trainer)
                .WithMany()
                .HasForeignKey(p => p.Trainer_Id)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
