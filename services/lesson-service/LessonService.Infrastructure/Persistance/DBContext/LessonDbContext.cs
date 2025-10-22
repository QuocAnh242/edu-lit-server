using System;
using Microsoft.EntityFrameworkCore;
using LessonService.Domain.Entities;
using LessonService.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LessonService.Infrastructure.Persistance.DBContext;

public partial class LessonDbContext : DbContext
{
    public LessonDbContext()
    {
    }

    public LessonDbContext(DbContextOptions<LessonDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<LessonContext> LessonContexts { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Syllabus> Syllabi { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("activity_pkey");

            entity.ToTable("activity");

            entity.HasIndex(e => e.SessionId, "idx_activity_session_id");

            entity.HasIndex(e => e.ActivityType, "idx_activity_type");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .HasColumnName("activity_type");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsRequired)
                .HasDefaultValue(false)
                .HasColumnName("is_required");
            entity.Property(e => e.Points)
                .HasDefaultValue(0)
                .HasColumnName("points");
            entity.Property(e => e.Position)
                .HasDefaultValue(0)
                .HasColumnName("position");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Session).WithMany(p => p.Activities)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("fk_activity_session");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.HasIndex(e => e.SyllabusId, "idx_courses_syllabus_id");

            entity.HasIndex(e => new { e.SyllabusId, e.CourseCode }, "unique_course_in_syllabus").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(20)
                .HasColumnName("course_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.SyllabusId).HasColumnName("syllabus_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.Courses)
                .HasForeignKey(d => d.SyllabusId)
                .HasConstraintName("fk_course_syllabus");
        });

        modelBuilder.Entity<LessonContext>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_context_pkey");

            entity.ToTable("lesson_context");

            entity.HasIndex(e => e.ParentLessonId, "idx_lesson_context_parent_id");

            entity.HasIndex(e => e.SessionId, "idx_lesson_context_session_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.LessonContent).HasColumnName("lesson_content");
            entity.Property(e => e.LessonTitle)
                .HasMaxLength(255)
                .HasColumnName("lesson_title");
            entity.Property(e => e.Level)
                .HasDefaultValue(0)
                .HasColumnName("level");
            entity.Property(e => e.ParentLessonId).HasColumnName("parent_lesson_id");
            entity.Property(e => e.Position)
                .HasDefaultValue(0)
                .HasColumnName("position");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ParentLesson).WithMany(p => p.InverseParentLesson)
                .HasForeignKey(d => d.ParentLessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_lesson_parent");

            entity.HasOne(d => d.Session).WithMany(p => p.LessonContexts)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("fk_lesson_session");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessions_pkey");

            entity.ToTable("sessions");

            entity.HasIndex(e => e.CourseId, "idx_sessions_course_id");

            entity.HasIndex(e => new { e.CourseId, e.Position }, "idx_sessions_position");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.Position)
                .HasDefaultValue(0)
                .HasColumnName("position");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("fk_sessions_course");
        });

        modelBuilder.Entity<Syllabus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("syllabus_pkey");

            entity.ToTable("syllabus");

            entity.HasIndex(e => e.IsActive, "idx_syllabus_is_active");

            entity.HasIndex(e => e.OwnerId, "idx_syllabus_owner_id");

            entity.HasIndex(e => new { e.AcademicYear, e.Semester }, "unique_program_per_semester").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(10)
                .HasColumnName("academic_year");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OwnerId)
                .HasMaxLength(36)
                .HasColumnName("owner_id");

            // Map Semester enum <-> exact Vietnamese strings used in DB CHECK constraint
            var semesterToString = new Dictionary<Semester, string>
            {
                { Semester.HocKiI, "Học kì I" },
                { Semester.HocKiII, "Học kì II" },
                { Semester.GiuaHocKiI, "Giữa học kì I" },
                { Semester.GiuaHocKiII, "Giữa học kì II" },
                { Semester.CuoiHocKiI, "Cuối học kì I" },
                { Semester.CuoiHocKiII, "Cuối học kì II" }
            };
            var stringToSemester = semesterToString.ToDictionary(kv => kv.Value, kv => kv.Key);

            var semesterConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<Semester, string>(
                v => semesterToString[v],
                v => stringToSemester[v]
            );

            entity.Property(e => e.Semester)
                .HasConversion(semesterConverter)
                .HasMaxLength(30)
                .HasColumnName("semester");

            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
