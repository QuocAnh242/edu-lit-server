namespace LessonServiceQuery.Infrastructure.Configuration;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; set; } = "rabbitmq";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    // Exchange and Queue settings
    public string LessonExchange { get; set; } = "lesson-events";
    public string LessonCreatedQueue { get; set; } = "lesson-created-query";
    public string LessonUpdatedQueue { get; set; } = "lesson-updated-query";
    public string LessonDeletedQueue { get; set; } = "lesson-deleted-query";

    public string LessonContextExchange { get; set; } = "lesson-context-events";
    public string LessonContextCreatedQueue { get; set; } = "lesson-context-created-query";
    public string LessonContextUpdatedQueue { get; set; } = "lesson-context-updated-query";
    public string LessonContextDeletedQueue { get; set; } = "lesson-context-deleted-query";

    public string ActivityExchange { get; set; } = "activity-events";
    public string ActivityCreatedQueue { get; set; } = "activity-created-query";
    public string ActivityUpdatedQueue { get; set; } = "activity-updated-query";
    public string ActivityDeletedQueue { get; set; } = "activity-deleted-query";

    public string SyllabusExchange { get; set; } = "syllabus-events";
    public string SyllabusCreatedQueue { get; set; } = "syllabus-created-query";
    public string SyllabusUpdatedQueue { get; set; } = "syllabus-updated-query";
    public string SyllabusDeletedQueue { get; set; } = "syllabus-deleted-query";

    public string CourseraExchange { get; set; } = "coursera-events";
    public string CourseraCreatedQueue { get; set; } = "coursera-created-query";
    public string CourseraUpdatedQueue { get; set; } = "coursera-updated-query";
    public string CourseraDeletedQueue { get; set; } = "coursera-deleted-query";

    public string? SessionExchange { get; set; } = "session-events";
    public string? SessionCreatedQueue { get; set; } = "session-created-query";
    public string? SessionUpdatedQueue { get; set; } = "session-updated-query";
    public string? SessionDeletedQueue { get; set; } = "session-deleted-query";
}
