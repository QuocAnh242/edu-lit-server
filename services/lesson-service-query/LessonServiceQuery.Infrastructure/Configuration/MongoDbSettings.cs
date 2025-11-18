namespace LessonServiceQuery.Infrastructure.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    
    // Collection names
    public string LessonsCollectionName { get; set; } = "lessons";
    public string LessonContextsCollectionName { get; set; } = "lesson_contexts";
    public string ActivitiesCollectionName { get; set; } = "activities";
    public string SyllabusesCollectionName { get; set; } = "syllabuses";
    public string CoursesCollectionName { get; set; } = "courses";
    public string SessionsCollectionName { get; set; } = "sessions";
}
