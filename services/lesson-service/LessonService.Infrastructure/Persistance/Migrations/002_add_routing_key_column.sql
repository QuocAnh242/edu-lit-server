-- Add routing_key column to outbox_messages table
ALTER TABLE outbox_messages 
ADD COLUMN IF NOT EXISTS routing_key VARCHAR(255) NOT NULL DEFAULT '';

-- Update existing records with default routing keys based on event type
UPDATE outbox_messages 
SET routing_key = CASE
    -- Syllabus events
    WHEN type = 'SyllabusCreated' THEN 'syllabus.created'
    WHEN type = 'SyllabusUpdated' THEN 'syllabus.updated'
    WHEN type = 'SyllabusDeleted' THEN 'syllabus.deleted'
    
    -- Course events
    WHEN type = 'CourseCreated' THEN 'course.created'
    WHEN type = 'CourseUpdated' THEN 'course.updated'
    WHEN type = 'CourseDeleted' THEN 'course.deleted'
    
    -- Session events
    WHEN type = 'SessionCreated' THEN 'session.created'
    WHEN type = 'SessionUpdated' THEN 'session.updated'
    WHEN type = 'SessionDeleted' THEN 'session.deleted'
    
    -- LessonContext events
    WHEN type = 'LessonContextCreated' THEN 'lessoncontext.created'
    WHEN type = 'LessonContextBulkCreated' THEN 'lessoncontext.created'
    WHEN type = 'LessonContextUpdated' THEN 'lessoncontext.updated'
    WHEN type = 'LessonContextDeleted' THEN 'lessoncontext.deleted'
    
    -- Activity events
    WHEN type = 'ActivityCreated' THEN 'activity.created'
    WHEN type = 'ActivityUpdated' THEN 'activity.updated'
    WHEN type = 'ActivityDeleted' THEN 'activity.deleted'
    
    -- Default
    ELSE '#'
END
WHERE routing_key = '';

-- Create index on routing_key for better performance
CREATE INDEX IF NOT EXISTS idx_outbox_routing_key ON outbox_messages(routing_key);

-- Add comment
COMMENT ON COLUMN outbox_messages.routing_key IS 'Routing key for RabbitMQ Topic exchange (e.g., syllabus.created, course.updated)';

