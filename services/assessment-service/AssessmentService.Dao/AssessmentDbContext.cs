using AssessmentService.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AssessmentService.Dao;

public partial class AssessmentDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public AssessmentDbContext()
    {
    }

    public AssessmentDbContext(DbContextOptions<AssessmentDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<AssessmentAnswer> AssessmentAnswers { get; set; }

    public virtual DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }

    public virtual DbSet<AssignmentAttempt> AssignmentAttempts { get; set; }

    public virtual DbSet<GradingFeedback> GradingFeedbacks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = _configuration ?? new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Set it in appsettings.json or environment variables.");
            }

            // Use Pomelo MySQL provider; AutoDetect will infer server version from the connection string
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
        //=> optionsBuilder.UseMySql("server=localhost;port=3306;database=edulit_assessment_db;user=root;password=rootpass", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId).HasName("PRIMARY");

            entity.ToTable("assessments");

            entity.Property(e => e.AssessmentId)
                .HasColumnType("int(11)")
                .HasColumnName("assessment_id");
            entity.Property(e => e.CourseId)
                .HasMaxLength(255)
                .HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId)
                .HasMaxLength(255)
                .HasColumnName("creator_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationMinutes)
                .HasColumnType("int(11)")
                .HasColumnName("duration_minutes");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TotalQuestions)
                .HasColumnType("int(11)")
                .HasColumnName("total_questions");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<AssessmentAnswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PRIMARY");

            entity.ToTable("assessment_answer");

            entity.HasIndex(e => e.AssessmentQuestionId, "assessment_question_id");

            entity.HasIndex(e => e.AttemptsId, "attempts_id");

            entity.Property(e => e.AnswerId)
                .HasColumnType("int(11)")
                .HasColumnName("answer_id");
            entity.Property(e => e.AssessmentQuestionId)
                .HasColumnType("int(11)")
                .HasColumnName("assessment_question_id");
            entity.Property(e => e.AttemptsId)
                .HasColumnType("int(11)")
                .HasColumnName("attempts_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.SelectedAnswer)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("A, B, C, D mà student chọn")
                .HasColumnName("selected_answer");

            entity.HasOne(d => d.AssessmentQuestion).WithMany(p => p.AssessmentAnswers)
                .HasForeignKey(d => d.AssessmentQuestionId)
                .HasConstraintName("assessment_answer_ibfk_1");

            entity.HasOne(d => d.Attempts).WithMany(p => p.AssessmentAnswers)
                .HasForeignKey(d => d.AttemptsId)
                .HasConstraintName("assessment_answer_ibfk_2");
        });

        modelBuilder.Entity<AssessmentQuestion>(entity =>
        {
            entity.HasKey(e => e.AssessmentQuestionId).HasName("PRIMARY");

            entity.ToTable("assessment_question");

            entity.HasIndex(e => e.AssessmentId, "assessment_id");

            entity.Property(e => e.AssessmentQuestionId)
                .HasColumnType("int(11)")
                .HasColumnName("assessment_question_id");
            entity.Property(e => e.AssessmentId)
                .HasColumnType("int(11)")
                .HasColumnName("assessment_id");
            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("A, B, C, D")
                .HasColumnName("correct_answer");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.OrderNum)
                .HasColumnType("int(11)")
                .HasColumnName("order_num");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(255)
                .HasColumnName("question_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Assessment).WithMany(p => p.AssessmentQuestions)
                .HasForeignKey(d => d.AssessmentId)
                .HasConstraintName("assessment_question_ibfk_1");
        });

        modelBuilder.Entity<AssignmentAttempt>(entity =>
        {
            entity.HasKey(e => e.AttemptsId).HasName("PRIMARY");

            entity.ToTable("assignment_attempts");

            entity.HasIndex(e => e.AssessmentId, "assessment_id");

            entity.Property(e => e.AttemptsId)
                .HasColumnType("int(11)")
                .HasColumnName("attempts_id");
            entity.Property(e => e.AssessmentId)
                .HasColumnType("int(11)")
                .HasColumnName("assessment_id");
            entity.Property(e => e.AttemptNumber)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("attempt_number");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("completed_at");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("started_at");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Assessment).WithMany(p => p.AssignmentAttempts)
                .HasForeignKey(d => d.AssessmentId)
                .HasConstraintName("assignment_attempts_ibfk_1");
        });

        modelBuilder.Entity<GradingFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PRIMARY");

            entity.ToTable("grading_feedback");

            entity.HasIndex(e => e.AttemptsId, "attempts_id").IsUnique();

            entity.Property(e => e.FeedbackId)
                .HasColumnType("int(11)")
                .HasColumnName("feedback_id");
            entity.Property(e => e.AttemptsId)
                .HasColumnType("int(11)")
                .HasColumnName("attempts_id");
            entity.Property(e => e.CorrectCount)
                .HasComment("Số câu trả lời đúng")
                .HasColumnType("int(11)")
                .HasColumnName("correct_count");
            entity.Property(e => e.CorrectPercentage)
                .HasPrecision(5, 2)
                .HasComment("Phần trăm câu đúng (%)")
                .HasColumnName("correct_percentage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.TotalScore)
                .HasPrecision(5, 2)
                .HasComment("Tổng điểm trên thang 10")
                .HasColumnName("total_score");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.WrongCount)
                .HasComment("Số câu trả lời sai")
                .HasColumnType("int(11)")
                .HasColumnName("wrong_count");
            entity.Property(e => e.WrongPercentage)
                .HasPrecision(5, 2)
                .HasComment("Phần trăm câu sai (%)")
                .HasColumnName("wrong_percentage");

            entity.HasOne(d => d.Attempts).WithOne(p => p.GradingFeedback)
                .HasForeignKey<GradingFeedback>(d => d.AttemptsId)
                .HasConstraintName("grading_feedback_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
