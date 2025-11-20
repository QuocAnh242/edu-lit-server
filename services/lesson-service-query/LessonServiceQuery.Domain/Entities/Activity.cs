using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

public class Activity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("activity_id")]
    public Guid ActivityId { get; set; }
    
    [BsonElement("lesson_id")]
    public Guid LessonId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("activity_type")]
    public string ActivityType { get; set; } = string.Empty;
    
    [BsonElement("instructions")]
    public string Instructions { get; set; } = string.Empty;
    
    [BsonElement("estimated_time_minutes")]
    public int EstimatedTimeMinutes { get; set; }
    
    [BsonElement("position")]
    public int Position { get; set; }
    
    [BsonElement("materials")]
    public List<string> Materials { get; set; } = new();
    
    [BsonElement("objectives")]
    public List<string> Objectives { get; set; } = new();
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
