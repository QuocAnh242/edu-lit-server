using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

public class LessonContext
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("lesson_context_id")]
    public Guid LessonContextId { get; set; }
    
    [BsonElement("lesson_id")]
    public Guid LessonId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;
    
    [BsonElement("position")]
    public int Position { get; set; }
    
    [BsonElement("level")]
    public int Level { get; set; }
    
    [BsonElement("parent_id")]
    public Guid? ParentId { get; set; }
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
