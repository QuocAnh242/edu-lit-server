using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

public class Syllabus
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("syllabus_id")]
    public Guid SyllabusId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("grade_level")]
    public string GradeLevel { get; set; } = string.Empty;
    
    [BsonElement("subject")]
    public string Subject { get; set; } = string.Empty;
    
    [BsonElement("version")]
    public string Version { get; set; } = string.Empty;
    
    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
    
    [BsonElement("created_by")]
    public Guid CreatedBy { get; set; }
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // Not stored in MongoDB - populated by repository from Course collection
    [BsonIgnore]
    public List<Course> Courses { get; set; } = new();
}

