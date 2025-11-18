using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

public class Session
{
    [BsonElement("session_id")]
    public Guid SessionId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("order_index")]
    public int OrderIndex { get; set; }
    
    [BsonElement("duration_minutes")]
    public int DurationMinutes { get; set; }
    
    [BsonElement("lesson_id")]
    public Guid? LessonId { get; set; }
    
    [BsonElement("objectives")]
    public List<string> Objectives { get; set; } = new();
    
    [BsonElement("materials")]
    public List<string> Materials { get; set; } = new();
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

