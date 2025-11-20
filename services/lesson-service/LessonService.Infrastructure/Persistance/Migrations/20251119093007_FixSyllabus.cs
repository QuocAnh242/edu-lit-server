using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonService.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class FixSyllabus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure syllabus table exists for environments where the baseline SQL scripts were not executed
            migrationBuilder.Sql(@"
DO $$
BEGIN
    -- SYLLABUS TABLE --------------------------------------------------------
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
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'unique_program_per_semester'
    ) THEN
        CREATE UNIQUE INDEX unique_program_per_semester ON public.syllabus (academic_year, semester);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_syllabus_owner_id'
    ) THEN
        CREATE INDEX idx_syllabus_owner_id ON public.syllabus (owner_id);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_syllabus_is_active'
    ) THEN
        CREATE INDEX idx_syllabus_is_active ON public.syllabus (is_active);
    END IF;

    -- COURSES TABLE ---------------------------------------------------------
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
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_courses_syllabus_id'
    ) THEN
        CREATE INDEX idx_courses_syllabus_id ON public.courses (syllabus_id);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'unique_course_in_syllabus'
    ) THEN
        CREATE UNIQUE INDEX unique_course_in_syllabus ON public.courses (syllabus_id, course_code);
    END IF;

    -- SESSIONS TABLE --------------------------------------------------------
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
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_sessions_course_id'
    ) THEN
        CREATE INDEX idx_sessions_course_id ON public.sessions (course_id);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_sessions_position'
    ) THEN
        CREATE INDEX idx_sessions_position ON public.sessions (course_id, position);
    END IF;

    -- LESSON_CONTEXT TABLE --------------------------------------------------
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
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_lesson_context_session_id'
    ) THEN
        CREATE INDEX idx_lesson_context_session_id ON public.lesson_context (session_id);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_lesson_context_parent_id'
    ) THEN
        CREATE INDEX idx_lesson_context_parent_id ON public.lesson_context (parent_lesson_id);
    END IF;

    -- ACTIVITY TABLE --------------------------------------------------------
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
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_activity_session_id'
    ) THEN
        CREATE INDEX idx_activity_session_id ON public.activity (session_id);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'public' AND indexname = 'idx_activity_type'
    ) THEN
        CREATE INDEX idx_activity_type ON public.activity (activity_type);
    END IF;
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'activity'
    ) THEN
        DROP TABLE public.activity;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'lesson_context'
    ) THEN
        DROP TABLE public.lesson_context;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'sessions'
    ) THEN
        DROP TABLE public.sessions;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'courses'
    ) THEN
        DROP TABLE public.courses;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_name = 'syllabus'
    ) THEN
        DROP TABLE public.syllabus;
    END IF;
END $$;
");
        }
    }
}
