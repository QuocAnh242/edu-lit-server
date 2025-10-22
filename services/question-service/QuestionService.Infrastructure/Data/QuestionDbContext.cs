using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;

namespace QuestionService.Infrastructure.Data
{
    public class QuestionDbContext : DbContext
    {
        public QuestionDbContext(DbContextOptions<QuestionDbContext> options) : base(options)
        {
        }

        public virtual DbSet<QuestionBank> QuestionBanks { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            // QuestionBank entity configuration
            modelBuilder.Entity<QuestionBank>(entity =>
            {
                entity.HasKey(e => e.QuestionBanksId).HasName("question_banks_pkey");
                entity.ToTable("question_banks");

                entity.Property(e => e.QuestionBanksId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .HasColumnName("question_banks_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.Subject)
                    .HasMaxLength(100)
                    .HasColumnName("subject");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'")
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("created_at");

                entity.Property(e => e.OwnerId)
                    .IsRequired()
                    .HasColumnName("owner_id");
            });

            // Question entity configuration
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId).HasName("questions_pkey");
                entity.ToTable("questions");

                entity.Property(e => e.QuestionId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .HasColumnName("question_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .HasColumnName("body");

                entity.Property(e => e.QuestionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("question_type");

                entity.Property(e => e.Metadata)
                    .HasColumnName("metadata");

                entity.Property(e => e.Tags)
                    .HasMaxLength(500)
                    .HasColumnName("tags");

                entity.Property(e => e.Version)
                    .HasColumnName("version");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'")
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("updated_at");

                entity.Property(e => e.IsPublished)
                    .HasColumnName("is_published");

                entity.Property(e => e.QuestionBankId)
                    .IsRequired()
                    .HasColumnName("question_bank_id");

                entity.Property(e => e.AuthorId)
                    .IsRequired()
                    .HasColumnName("author_id");

                // Foreign key relationships
                entity.HasOne(d => d.QuestionBank)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QuestionBankId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("questions_question_bank_id_fkey");
            });

            // QuestionOption entity configuration
            modelBuilder.Entity<QuestionOption>(entity =>
            {
                entity.HasKey(e => e.QuestionOptionId).HasName("question_options_pkey");
                entity.ToTable("question_options");

                entity.Property(e => e.QuestionOptionId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .HasColumnName("question_option_id");

                entity.Property(e => e.OptionText)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasColumnName("option_text");

                entity.Property(e => e.IsCorrect)
                    .HasColumnName("is_correct");

                entity.Property(e => e.OrderIdx)
                    .HasColumnName("order_idx");

                entity.Property(e => e.QuestionId)
                    .IsRequired()
                    .HasColumnName("question_id");

                // Foreign key relationships
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionOptions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("question_options_question_id_fkey");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
