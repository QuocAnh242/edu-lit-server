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
        WHERE table_schema = 'public' AND table_name = 'syllabus'
    ) THEN
        DROP TABLE public.syllabus;
    END IF;
END $$;
");
        }
    }
}
