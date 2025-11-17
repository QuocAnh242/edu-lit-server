using MongoDB.Bson.Serialization.Attributes;

namespace LessonServiceQuery.Domain.Entities;

public class Course
{
    [BsonElement("course_id")]
    public Guid CourseId { get; set; }
    
    [BsonElement("syllabus_id")]
    public Guid SyllabusId { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("order_index")]
    public int OrderIndex { get; set; }
    
    [BsonElement("duration_weeks")]
    public int DurationWeeks { get; set; }
    
    [BsonElement("objectives")]
    public List<string> Objectives { get; set; } = new();
    
    [BsonElement("sessions")]
    public List<Session> Sessions { get; set; } = new();
    
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

