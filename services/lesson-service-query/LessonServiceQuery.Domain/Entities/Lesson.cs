using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

[BsonIgnoreExtraElements]
public class Lesson
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("lesson_id")]
    public Guid LessonId { get; set; }
    
    [BsonElement("session_id")]
    public Guid SessionId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("duration_minutes")]
    public int DurationMinutes { get; set; }
    
    [BsonElement("position")]
    public int Position { get; set; }
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // References to separate collections (not embedded)
    // These will be populated when querying through DAOs
    [BsonIgnore]
    public List<LessonContext> LessonContexts { get; set; } = new();
    
    [BsonIgnore]
    public List<Activity> Activities { get; set; } = new();
}
