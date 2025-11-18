using System.Text.Json;
using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using Microsoft.Extensions.Logging;

namespace LessonServiceQuery.Infrastructure.Services.EventHandlers;

public class UniversalEventHandler
{
    private readonly ISyllabusDao _syllabusDao;
    private readonly ICourseDao _courseDao;
    private readonly ISessionDao _sessionDao;
    private readonly ILessonDao _lessonDao;
    private readonly ILessonContextDao _lessonContextDao;
    private readonly IActivityDao _activityDao;
    private readonly ILogger<UniversalEventHandler> _logger;

    public UniversalEventHandler(
        ISyllabusDao syllabusDao,
        ICourseDao courseDao,
        ISessionDao sessionDao,
        ILessonDao lessonDao,
        ILessonContextDao lessonContextDao,
        IActivityDao activityDao,
        ILogger<UniversalEventHandler> logger)
    {
        _syllabusDao = syllabusDao;
        _courseDao = courseDao;
        _sessionDao = sessionDao;
        _lessonDao = lessonDao;
        _lessonContextDao = lessonContextDao;
        _activityDao = activityDao;
        _logger = logger;
    }

    // Helper methods to safely extract values from JsonElement
    private static string GetStringOrDefault(JsonElement element, string propertyName, string defaultValue = "")
    {
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind != JsonValueKind.Null)
        {
            return prop.GetString() ?? defaultValue;
        }
        return defaultValue;
    }

    private static Guid GetGuidOrDefault(JsonElement element, string propertyName, Guid defaultValue = default)
    {
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind != JsonValueKind.Null)
        {
            try
            {
                return prop.GetGuid();
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    private static int GetIntOrDefault(JsonElement element, string propertyName, int defaultValue = 0)
    {
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind != JsonValueKind.Null)
        {
            try
            {
                return prop.GetInt32();
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    private static bool GetBoolOrDefault(JsonElement element, string propertyName, bool defaultValue = true)
    {
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind != JsonValueKind.Null)
        {
            try
            {
                return prop.GetBoolean();
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    private static DateTime GetDateTimeOrDefault(JsonElement element, string propertyName, DateTime? defaultValue = null)
    {
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind != JsonValueKind.Null)
        {
            try
            {
                return prop.GetDateTime();
            }
            catch
            {
                return defaultValue ?? DateTime.UtcNow;
            }
        }
        return defaultValue ?? DateTime.UtcNow;
    }

    public async Task HandleCreatedEvent(string entityType, JsonElement data)
    {
        try
        {
            _logger.LogInformation("🔷 START: Handling Created event for {EntityType}", entityType);
            _logger.LogInformation("Data to process: {Data}", data.GetRawText());

            switch (entityType.ToLower())
            {
                case "syllabus":
                    await HandleSyllabusCreated(data);
                    break;
                case "course":
                    await HandleCourseCreated(data);
                    break;
                case "session":
                    await HandleSessionCreated(data);
                    break;
                case "lesson":
                    await HandleLessonCreated(data);
                    break;
                case "lessoncontext":
                    await HandleLessonContextCreated(data);
                    break;
                case "activity":
                    await HandleActivityCreated(data);
                    break;
                default:
                    _logger.LogWarning("⚠️ Unknown entity type: {EntityType}", entityType);
                    break;
            }
            
            _logger.LogInformation("🔷 COMPLETE: Created event handled successfully for {EntityType}", entityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERROR: Failed to handle Created event for {EntityType}", entityType);
            throw;
        }
    }

    public async Task HandleUpdatedEvent(string entityType, JsonElement data)
    {
        try
        {
            _logger.LogInformation("🔶 START: Handling Updated event for {EntityType}", entityType);
            _logger.LogInformation("Data to process: {Data}", data.GetRawText());

            switch (entityType.ToLower())
            {
                case "syllabus":
                    await HandleSyllabusUpdated(data);
                    break;
                case "course":
                    await HandleCourseUpdated(data);
                    break;
                case "session":
                    await HandleSessionUpdated(data);
                    break;
                case "lesson":
                    await HandleLessonUpdated(data);
                    break;
                case "lessoncontext":
                    await HandleLessonContextUpdated(data);
                    break;
                case "activity":
                    await HandleActivityUpdated(data);
                    break;
                default:
                    _logger.LogWarning("⚠️ Unknown entity type: {EntityType}", entityType);
                    break;
            }
            
            _logger.LogInformation("🔶 COMPLETE: Updated event handled successfully for {EntityType}", entityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERROR: Failed to handle Updated event for {EntityType}", entityType);
            throw;
        }
    }

    public async Task HandleDeletedEvent(string entityType, Guid id)
    {
        try
        {
            _logger.LogInformation("🔴 START: Handling Deleted event for {EntityType} with ID: {Id}", entityType, id);

            switch (entityType.ToLower())
            {
                case "syllabus":
                    _logger.LogInformation("Deleting Syllabus with ID: {Id}", id);
                    await _syllabusDao.DeleteAsync(id);
                    break;
                case "course":
                    _logger.LogInformation("Deleting Course with ID: {Id}", id);
                    await _courseDao.DeleteAsync(id);
                    break;
                case "session":
                    _logger.LogInformation("Deleting Session with ID: {Id}", id);
                    await _sessionDao.DeleteAsync(id);
                    break;
                case "lesson":
                    _logger.LogInformation("Deleting Lesson with ID: {Id}", id);
                    await _lessonDao.DeleteAsync(id);
                    break;
                case "lessoncontext":
                    _logger.LogInformation("Deleting LessonContext with ID: {Id}", id);
                    await _lessonContextDao.DeleteAsync(id);
                    break;
                case "activity":
                    _logger.LogInformation("Deleting Activity with ID: {Id}", id);
                    await _activityDao.DeleteAsync(id);
                    break;
                default:
                    _logger.LogWarning("⚠️ Unknown entity type: {EntityType}", entityType);
                    break;
            }

            _logger.LogInformation("🔴 COMPLETE: Successfully deleted {EntityType} with ID: {Id}", entityType, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERROR: Failed to handle Deleted event for {EntityType} with ID: {Id}", entityType, id);
            throw;
        }
    }

    // Syllabus handlers
    private async Task HandleSyllabusCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Syllabus data...");
        
        try
        {
            var syllabus = new Syllabus
            {
                SyllabusId = GetGuidOrDefault(data, "Id"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                GradeLevel = GetStringOrDefault(data, "AcademicYear"),
                Subject = GetStringOrDefault(data, "Semester"),
                CreatedBy = GetGuidOrDefault(data, "OwnerId"),
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow,
                Version = "1.0",
                Status = "Active"
            };
            
            _logger.LogInformation("✓ Deserialized Syllabus: ID={Id}, Title={Title}", 
                syllabus.SyllabusId, syllabus.Title);
            
            _logger.LogInformation("💾 Saving Syllabus to MongoDB...");
            await _syllabusDao.CreateAsync(syllabus);
            
            _logger.LogInformation("✅ Successfully created Syllabus in database with ID: {Id}", syllabus.SyllabusId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or save Syllabus data");
            throw;
        }
    }

    private async Task HandleSyllabusUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Syllabus data for update...");
        
        try
        {
            var syllabus = new Syllabus
            {
                SyllabusId = GetGuidOrDefault(data, "Id"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                GradeLevel = GetStringOrDefault(data, "AcademicYear"),
                Subject = GetStringOrDefault(data, "Semester"),
                CreatedBy = GetGuidOrDefault(data, "OwnerId"),
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow,
                Version = "1.0",
                Status = "Active"
            };
            
            _logger.LogInformation("✓ Deserialized Syllabus: ID={Id}, Title={Title}", 
                syllabus.SyllabusId, syllabus.Title);
            
            // Set all existing records to IsActive = false
            _logger.LogInformation("🔄 Deactivating all existing versions of Syllabus ID: {Id}", syllabus.SyllabusId);
            await _syllabusDao.DeactivateAllByIdAsync(syllabus.SyllabusId);
            
            // Create new record
            _logger.LogInformation("💾 Creating new version of Syllabus in MongoDB...");
            await _syllabusDao.CreateAsync(syllabus);
            
            _logger.LogInformation("✅ Successfully updated Syllabus with ID: {Id} (created new version)", syllabus.SyllabusId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or update Syllabus data");
            throw;
        }
    }

    // Course handlers
    private async Task HandleCourseCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Course data...");
        
        try
        {
            var syllabusId = GetGuidOrDefault(data, "SyllabusId");
            if (syllabusId == Guid.Empty)
            {
                _logger.LogError("❌ SyllabusId is missing or invalid in Course event");
                throw new InvalidOperationException("SyllabusId is required for creating a Course");
            }
            
            var course = new Course
            {
                CourseId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "CourseId"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                OrderIndex = GetIntOrDefault(data, "OrderIndex", 0),
                DurationWeeks = GetIntOrDefault(data, "DurationWeeks", 0),
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Course: ID={Id}, Title={Title}, SyllabusId={SyllabusId}", 
                course.CourseId, course.Title, syllabusId);
            
            _logger.LogInformation("💾 Saving Course to MongoDB (pushing to Syllabus.Courses)...");
            await _courseDao.CreateAsync(syllabusId, course);
            
            _logger.LogInformation("✅ Successfully created Course in database with ID: {Id}", course.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or save Course data");
            throw;
        }
    }

    private async Task HandleCourseUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Course data for update...");
        
        try
        {
            var syllabusId = GetGuidOrDefault(data, "SyllabusId");
            if (syllabusId == Guid.Empty)
            {
                _logger.LogError("❌ SyllabusId is missing or invalid in Course update event");
                throw new InvalidOperationException("SyllabusId is required for updating a Course");
            }
            
            var course = new Course
            {
                CourseId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "CourseId"),
                SyllabusId = syllabusId,
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                OrderIndex = GetIntOrDefault(data, "OrderIndex", 0),
                DurationWeeks = GetIntOrDefault(data, "DurationWeeks", 0),
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Course: ID={Id}, Title={Title}, SyllabusId={SyllabusId}", 
                course.CourseId, course.Title, syllabusId);
            
            // Set all existing records to IsActive = false
            _logger.LogInformation("🔄 Deactivating all existing versions of Course ID: {Id}", course.CourseId);
            await _courseDao.DeactivateAllByIdAsync(course.CourseId);
            
            // Create new record
            _logger.LogInformation("💾 Creating new version of Course in MongoDB...");
            await _courseDao.CreateAsync(syllabusId, course);
            
            _logger.LogInformation("✅ Successfully updated Course with ID: {Id} (created new version)", course.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or update Course data");
            throw;
        }
    }

    // Session handlers
    private async Task HandleSessionCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Session data...");
        
        try
        {
            var courseId = GetGuidOrDefault(data, "CourseId");
            if (courseId == Guid.Empty)
            {
                _logger.LogError("❌ CourseId is missing or invalid in Session event");
                throw new InvalidOperationException("CourseId is required for creating a Session");
            }
            
            var session = new Session
            {
                SessionId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "SessionId"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                OrderIndex = GetIntOrDefault(data, "OrderIndex", GetIntOrDefault(data, "Position", 0)),  // Support both OrderIndex and Position
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                LessonId = GetGuidOrDefault(data, "LessonId") != Guid.Empty ? GetGuidOrDefault(data, "LessonId") : null,
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Session: ID={Id}, Title={Title}, CourseId={CourseId}", 
                session.SessionId, session.Title, courseId);
            
            _logger.LogInformation("💾 Saving Session to MongoDB (pushing to Course.Sessions)...");
            await _sessionDao.CreateAsync(courseId, session);
            
            _logger.LogInformation("✅ Successfully created Session in database with ID: {Id}", session.SessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or save Session data");
            throw;
        }
    }

    private async Task HandleSessionUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Session data for update...");
        
        try
        {
            var courseId = GetGuidOrDefault(data, "CourseId");
            if (courseId == Guid.Empty)
            {
                _logger.LogError("❌ CourseId is missing or invalid in Session update event");
                throw new InvalidOperationException("CourseId is required for updating a Session");
            }
            
            var session = new Session
            {
                SessionId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "SessionId"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                OrderIndex = GetIntOrDefault(data, "OrderIndex", GetIntOrDefault(data, "Position", 0)),  // Support both OrderIndex and Position
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                LessonId = GetGuidOrDefault(data, "LessonId") != Guid.Empty ? GetGuidOrDefault(data, "LessonId") : null,
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Session: ID={Id}, Title={Title}, CourseId={CourseId}", 
                session.SessionId, session.Title, courseId);
            
            // Set all existing records to IsActive = false
            _logger.LogInformation("🔄 Deactivating all existing versions of Session ID: {Id}", session.SessionId);
            await _sessionDao.DeactivateAllByIdAsync(session.SessionId);
            
            // Create new record
            _logger.LogInformation("💾 Creating new version of Session in MongoDB...");
            await _sessionDao.CreateAsync(courseId, session);
            
            _logger.LogInformation("✅ Successfully updated Session with ID: {Id} (created new version)", session.SessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or update Session data");
            throw;
        }
    }

    // Lesson handlers
    private async Task HandleLessonCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Lesson data...");
        
        try
        {
            var lesson = new Lesson
            {
                LessonId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "LessonId"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                GradeLevel = GetStringOrDefault(data, "GradeLevel"),
                Subject = GetStringOrDefault(data, "Subject"),
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                TeacherId = GetGuidOrDefault(data, "TeacherId"),
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Lesson: ID={Id}, Title={Title}", 
                lesson.LessonId, lesson.Title);
            
            _logger.LogInformation("💾 Saving Lesson to MongoDB...");
            await _lessonDao.CreateAsync(lesson);
            
            _logger.LogInformation("✅ Successfully created Lesson in database with ID: {Id}", lesson.LessonId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or save Lesson data");
            throw;
        }
    }

    private async Task HandleLessonUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Lesson data for update...");
        
        try
        {
            var lesson = new Lesson
            {
                LessonId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "LessonId"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                GradeLevel = GetStringOrDefault(data, "GradeLevel"),
                Subject = GetStringOrDefault(data, "Subject"),
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                TeacherId = GetGuidOrDefault(data, "TeacherId"),
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Lesson: ID={Id}, Title={Title}", 
                lesson.LessonId, lesson.Title);
            
            // Set all existing records to IsActive = false
            _logger.LogInformation("🔄 Deactivating all existing versions of Lesson ID: {Id}", lesson.LessonId);
            await _lessonDao.DeactivateAllByIdAsync(lesson.LessonId);
            
            // Create new record
            _logger.LogInformation("💾 Creating new version of Lesson in MongoDB...");
            await _lessonDao.CreateAsync(lesson);
            
            _logger.LogInformation("✅ Successfully updated Lesson with ID: {Id} (created new version)", lesson.LessonId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or update Lesson data");
            throw;
        }
    }

    // LessonContext handlers
    private async Task HandleLessonContextCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing LessonContext data...");
        var lessonContext = JsonSerializer.Deserialize<LessonContext>(data.GetRawText());
        
        if (lessonContext != null)
        {
            _logger.LogInformation("✓ Deserialized LessonContext: ID={Id}", lessonContext.LessonContextId);
            
            // Extract lessonId from the event data
            Guid lessonId = Guid.Empty;
            if (data.TryGetProperty("LessonId", out var lessonIdProp) || 
                data.TryGetProperty("lessonId", out lessonIdProp) ||
                data.TryGetProperty("lesson_id", out lessonIdProp))
            {
                lessonId = lessonIdProp.GetGuid();
                _logger.LogInformation("✓ Found LessonId: {LessonId}", lessonId);
            }
            
            if (lessonId != Guid.Empty)
            {
                _logger.LogInformation("💾 Saving LessonContext to MongoDB with LessonId: {LessonId}...", lessonId);
                await _lessonContextDao.CreateAsync(lessonId, lessonContext);
                
                _logger.LogInformation("✅ Successfully created LessonContext with ID: {Id} for Lesson: {LessonId}", 
                    lessonContext.LessonContextId, lessonId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot create LessonContext: lessonId not found in event data");
            }
        }
        else
        {
            _logger.LogWarning("⚠️ Failed to deserialize LessonContext data");
        }
    }

    private async Task HandleLessonContextUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing LessonContext data for update...");
        var lessonContext = JsonSerializer.Deserialize<LessonContext>(data.GetRawText());
        
        if (lessonContext != null)
        {
            _logger.LogInformation("✓ Deserialized LessonContext: ID={Id}", lessonContext.LessonContextId);
            
            // Set all existing records to IsActive = false
            _logger.LogInformation("🔄 Deactivating all existing versions of LessonContext ID: {Id}", lessonContext.LessonContextId);
            await _lessonContextDao.DeactivateAllByIdAsync(lessonContext.LessonContextId);
            
            // Extract lessonId from the event data
            Guid lessonId = Guid.Empty;
            if (data.TryGetProperty("LessonId", out var lessonIdProp) || 
                data.TryGetProperty("lessonId", out lessonIdProp) ||
                data.TryGetProperty("lesson_id", out lessonIdProp))
            {
                lessonId = lessonIdProp.GetGuid();
                _logger.LogInformation("✓ Found LessonId: {LessonId}", lessonId);
            }
            
            if (lessonId != Guid.Empty)
            {
                // Create new record
                lessonContext.IsActive = true;
                _logger.LogInformation("💾 Creating new version of LessonContext in MongoDB...");
                await _lessonContextDao.CreateAsync(lessonId, lessonContext);
                
                _logger.LogInformation("✅ Successfully updated LessonContext with ID: {Id} (created new version)", 
                    lessonContext.LessonContextId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot update LessonContext: lessonId not found in event data");
            }
        }
        else
        {
            _logger.LogWarning("⚠️ Failed to deserialize LessonContext data for update");
        }
    }

    // Activity handlers
    private async Task HandleActivityCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Activity data...");
        var activity = JsonSerializer.Deserialize<Activity>(data.GetRawText());
        
        if (activity != null)
        {
            _logger.LogInformation("✓ Deserialized Activity: ID={Id}", activity.ActivityId);
            
            // Extract lessonContextId from the event data
            Guid lessonContextId = Guid.Empty;
            if (data.TryGetProperty("LessonContextId", out var lessonContextIdProp) || 
                data.TryGetProperty("lessonContextId", out lessonContextIdProp) ||
                data.TryGetProperty("lesson_context_id", out lessonContextIdProp))
            {
                lessonContextId = lessonContextIdProp.GetGuid();
                _logger.LogInformation("✓ Found LessonContextId: {LessonContextId}", lessonContextId);
            }
            
            if (lessonContextId != Guid.Empty)
            {
                _logger.LogInformation("💾 Saving Activity to MongoDB with LessonContextId: {LessonContextId}...", lessonContextId);
                await _activityDao.CreateAsync(lessonContextId, activity);
                
                _logger.LogInformation("✅ Successfully created Activity with ID: {Id} for LessonContext: {LessonContextId}", 
                    activity.ActivityId, lessonContextId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot create Activity: lessonContextId not found in event data");
            }
        }
        else
        {
            _logger.LogWarning("⚠️ Failed to deserialize Activity data");
        }
    }

    private async Task HandleActivityUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Activity data for update...");
        var activity = JsonSerializer.Deserialize<Activity>(data.GetRawText());
        
        if (activity != null)
        {
            _logger.LogInformation("✓ Deserialized Activity: ID={Id}", activity.ActivityId);
            
            // Set all existing records to IsActive = false
            _logger.LogInformation("🔄 Deactivating all existing versions of Activity ID: {Id}", activity.ActivityId);
            await _activityDao.DeactivateAllByIdAsync(activity.ActivityId);
            
            // Extract lessonContextId from the event data
            Guid lessonContextId = Guid.Empty;
            if (data.TryGetProperty("LessonContextId", out var lessonContextIdProp) || 
                data.TryGetProperty("lessonContextId", out lessonContextIdProp) ||
                data.TryGetProperty("lesson_context_id", out lessonContextIdProp))
            {
                lessonContextId = lessonContextIdProp.GetGuid();
                _logger.LogInformation("✓ Found LessonContextId: {LessonContextId}", lessonContextId);
            }
            
            if (lessonContextId != Guid.Empty)
            {
                // Create new record
                activity.IsActive = true;
                _logger.LogInformation("💾 Creating new version of Activity in MongoDB...");
                await _activityDao.CreateAsync(lessonContextId, activity);
                
                _logger.LogInformation("✅ Successfully updated Activity with ID: {Id} (created new version)", 
                    activity.ActivityId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot update Activity: lessonContextId not found in event data");
            }
        }
        else
        {
            _logger.LogWarning("⚠️ Failed to deserialize Activity data for update");
        }
    }
}

