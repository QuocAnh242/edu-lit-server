using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

public class Session
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("session_id")]
    public Guid SessionId { get; set; }
    
    [BsonElement("course_id")]
    public Guid CourseId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("position")]
    public int Position { get; set; }
    
    [BsonElement("duration_minutes")]
    public int DurationMinutes { get; set; }
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

