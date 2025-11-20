using LessonService.Infrastructure.Persistance.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LessonService.Api.HostedServices;

public sealed class DatabaseMigratorHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigratorHostedService> _logger;

    public DatabaseMigratorHostedService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigratorHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delaySeconds = 5;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LessonDbContext>();

                _logger.LogInformation("Applying LessonService migrations...");
                await context.Database.MigrateAsync(stoppingToken);
                _logger.LogInformation("LessonService migrations completed.");

                await EnsureCoreTablesAsync(context, stoppingToken);

                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Database migration failed. Retrying in {Delay}s...", delaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
                delaySeconds = Math.Min(delaySeconds * 2, 60);
            }
        }
    }

    private async Task EnsureCoreTablesAsync(LessonDbContext context, CancellationToken cancellationToken)
    {
        const string ensureTablesSql = @"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'syllabus'
    ) THEN
        CREATE TABLE public.syllabus (
            id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
            title character varying(255) NOT NULL,
            academic_year character varying(10) NOT NULL,
            semester character varying(30) NOT NULL,
            owner_id character varying(36) NOT NULL,
            description text NULL,
            is_active boolean NOT NULL DEFAULT true,
            created_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE UNIQUE INDEX unique_program_per_semester ON public.syllabus (academic_year, semester);
        CREATE INDEX idx_syllabus_owner_id ON public.syllabus (owner_id);
        CREATE INDEX idx_syllabus_is_active ON public.syllabus (is_active);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'courses'
    ) THEN
        CREATE TABLE public.courses (
            id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
            title character varying(255) NOT NULL,
            course_code character varying(20) NOT NULL,
            description text NULL,
            syllabus_id uuid NOT NULL,
            created_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            CONSTRAINT fk_course_syllabus FOREIGN KEY (syllabus_id) REFERENCES public.syllabus (id) ON DELETE CASCADE
        );
        CREATE INDEX idx_courses_syllabus_id ON public.courses (syllabus_id);
        CREATE UNIQUE INDEX unique_course_in_syllabus ON public.courses (syllabus_id, course_code);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'sessions'
    ) THEN
        CREATE TABLE public.sessions (
            id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
            course_id uuid NOT NULL,
            title character varying(255) NOT NULL,
            description text NULL,
            duration_minutes integer NULL,
            position integer NOT NULL DEFAULT 0,
            created_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            CONSTRAINT fk_sessions_course FOREIGN KEY (course_id) REFERENCES public.courses (id) ON DELETE CASCADE
        );
        CREATE INDEX idx_sessions_course_id ON public.sessions (course_id);
        CREATE INDEX idx_sessions_position ON public.sessions (course_id, position);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'lesson_context'
    ) THEN
        CREATE TABLE public.lesson_context (
            id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
            lesson_title character varying(255) NOT NULL,
            lesson_content text NULL,
            session_id uuid NOT NULL,
            parent_lesson_id uuid NULL,
            level integer NOT NULL DEFAULT 0,
            position integer NOT NULL DEFAULT 0,
            created_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            CONSTRAINT fk_lesson_session FOREIGN KEY (session_id) REFERENCES public.sessions (id) ON DELETE CASCADE,
            CONSTRAINT fk_lesson_parent FOREIGN KEY (parent_lesson_id) REFERENCES public.lesson_context (id) ON DELETE CASCADE
        );
        CREATE INDEX idx_lesson_context_session_id ON public.lesson_context (session_id);
        CREATE INDEX idx_lesson_context_parent_id ON public.lesson_context (parent_lesson_id);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'activity'
    ) THEN
        CREATE TABLE public.activity (
            id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
            title character varying(255) NOT NULL,
            description text NULL,
            content text NULL,
            session_id uuid NOT NULL,
            activity_type character varying(50) NOT NULL,
            points integer NOT NULL DEFAULT 0,
            is_required boolean NOT NULL DEFAULT false,
            position integer NOT NULL DEFAULT 0,
            created_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            CONSTRAINT fk_activity_session FOREIGN KEY (session_id) REFERENCES public.sessions (id) ON DELETE CASCADE
        );
        CREATE INDEX idx_activity_session_id ON public.activity (session_id);
        CREATE INDEX idx_activity_type ON public.activity (activity_type);
    END IF;
END $$;";

        try
        {
            _logger.LogInformation("Ensuring core lesson tables exist...");
            await context.Database.ExecuteSqlRawAsync(ensureTablesSql, cancellationToken);
            _logger.LogInformation("Core lesson tables verified.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure core lesson tables.");
            throw;
        }
    }
}

