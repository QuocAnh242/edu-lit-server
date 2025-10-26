using AssessmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssessmentService.Core.Data
{
    public class AssessmentDbContext : DbContext
    {
        public AssessmentDbContext(DbContextOptions<AssessmentDbContext> options) : base(options) { }

        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }
        public DbSet<AssignmentAttempt> AssignmentAttempts { get; set; }
        public DbSet<AssessmentAnswer> AssessmentAnswers { get; set; }
        public DbSet<GradingFeedback> GradingFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assessment>().ToTable("assessments");
            modelBuilder.Entity<AssessmentQuestion>().ToTable("assessment_question");
            modelBuilder.Entity<AssignmentAttempt>().ToTable("assignment_attempts");
            modelBuilder.Entity<AssessmentAnswer>().ToTable("assessment_answer");
            modelBuilder.Entity<GradingFeedback>().ToTable("grading_feedback");

            modelBuilder.Entity<GradingFeedback>()
                .HasOne(g => g.AssignmentAttempt)
                .WithOne(a => a.Feedback)
                .HasForeignKey<GradingFeedback>(g => g.AttemptsId);
        }
    }
}
