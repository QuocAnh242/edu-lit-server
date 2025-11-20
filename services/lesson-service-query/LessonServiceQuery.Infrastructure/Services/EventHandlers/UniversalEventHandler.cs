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
    
    private static string GetStringOrDefault(JsonElement element, string propertyName, string fallbackPropertyName, string defaultValue = "")
    {
        // Try primary property name first
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind != JsonValueKind.Null)
        {
            return prop.GetString() ?? defaultValue;
        }
        
        // Try fallback property name
        if (element.TryGetProperty(fallbackPropertyName, out var fallbackProp) && 
            fallbackProp.ValueKind != JsonValueKind.Null)
        {
            return fallbackProp.GetString() ?? defaultValue;
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
                CourseId = courseId,
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                Position = GetIntOrDefault(data, "Position", GetIntOrDefault(data, "OrderIndex", 0)),
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Session: ID={Id}, Title={Title}, CourseId={CourseId}", 
                session.SessionId, session.Title, courseId);
            
            _logger.LogInformation("💾 Saving Session to MongoDB as independent collection...");
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
                CourseId = courseId,
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                Position = GetIntOrDefault(data, "Position", GetIntOrDefault(data, "OrderIndex", 0)),
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
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
            var sessionId = GetGuidOrDefault(data, "SessionId");
            if (sessionId == Guid.Empty)
            {
                _logger.LogError("❌ SessionId is missing or invalid in Lesson event");
                throw new InvalidOperationException("SessionId is required for creating a Lesson");
            }
            
            // Check if this is a batch LessonContext creation event (from Command service)
            if (data.TryGetProperty("LessonContexts", out var lessonContextsElement) && 
                lessonContextsElement.ValueKind == JsonValueKind.Array)
            {
                _logger.LogInformation("📦 Detected batch LessonContext creation for SessionId: {SessionId}", sessionId);
                await HandleBatchLessonContextCreation(sessionId, lessonContextsElement);
                return;
            }
            
            // Standard Lesson creation
            var lesson = new Lesson
            {
                LessonId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "LessonId"),
                SessionId = sessionId,
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                Position = GetIntOrDefault(data, "Position", GetIntOrDefault(data, "OrderIndex", 0)),
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow,
                LessonContexts = new List<LessonContext>(),
                Activities = new List<Activity>()
            };
            
            _logger.LogInformation("✓ Deserialized Lesson: ID={Id}, Title={Title}, SessionId={SessionId}", 
                lesson.LessonId, lesson.Title, sessionId);
            
            _logger.LogInformation("💾 Saving Lesson to MongoDB with embedded LessonContexts...");
            await _lessonDao.CreateAsync(lesson);
            
            _logger.LogInformation("✅ Successfully created Lesson in database with ID: {Id}", lesson.LessonId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or save Lesson data");
            throw;
        }
    }
    
    private async Task HandleBatchLessonContextCreation(Guid sessionId, JsonElement lessonContextsArray)
    {
        _logger.LogInformation("🔹 Processing batch LessonContext creation...");
        
        var contextsToCreate = new List<LessonContext>();
        
        foreach (var contextElement in lessonContextsArray.EnumerateArray())
        {
            var context = new LessonContext
            {
                LessonContextId = GetGuidOrDefault(contextElement, "Id"),
                Title = GetStringOrDefault(contextElement, "LessonTitle", GetStringOrDefault(contextElement, "Title")),
                Content = GetStringOrDefault(contextElement, "LessonContent", GetStringOrDefault(contextElement, "Content")),
                Position = GetIntOrDefault(contextElement, "Position"),
                Level = GetIntOrDefault(contextElement, "Level"),
                ParentId = GetGuidOrDefault(contextElement, "ParentLessonId") != Guid.Empty 
                    ? GetGuidOrDefault(contextElement, "ParentLessonId") 
                    : GetGuidOrDefault(contextElement, "ParentId") != Guid.Empty 
                        ? GetGuidOrDefault(contextElement, "ParentId") 
                        : null,
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(contextElement, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            contextsToCreate.Add(context);
            _logger.LogInformation("✓ Parsed LessonContext: ID={Id}, Title={Title}, Level={Level}, ParentId={ParentId}", 
                context.LessonContextId, context.Title, context.Level, context.ParentId);
        }
        
        _logger.LogInformation("📊 Total LessonContexts to create: {Count}", contextsToCreate.Count);
        
        // Find or create Lesson for this SessionId
        var existingLessons = await _lessonDao.GetBySessionIdAsync(sessionId);
        Lesson targetLesson;
        
        if (existingLessons == null || !existingLessons.Any())
        {
            // Create a new Lesson for this Session
            _logger.LogInformation("📝 Creating new Lesson for SessionId: {SessionId}", sessionId);
            targetLesson = new Lesson
            {
                LessonId = Guid.NewGuid(),
                SessionId = sessionId,
                Title = $"Lesson for Session {sessionId}",
                Description = "Auto-generated from LessonContext batch creation",
                Position = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _lessonDao.CreateAsync(targetLesson);
            _logger.LogInformation("✅ Created new Lesson with ID: {LessonId}", targetLesson.LessonId);
        }
        else
        {
            targetLesson = existingLessons.First();
            _logger.LogInformation("📝 Using existing Lesson ID: {LessonId}", targetLesson.LessonId);
        }
        
        // Save all contexts to separate collection
        foreach (var context in contextsToCreate)
        {
            await _lessonContextDao.CreateAsync(targetLesson.LessonId, context);
        }
        
        _logger.LogInformation("✅ Added {Count} LessonContexts to separate collection for Lesson ID: {LessonId}", 
            contextsToCreate.Count, targetLesson.LessonId);
    }

    private async Task HandleLessonUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Lesson data for update...");
        
        try
        {
            var sessionId = GetGuidOrDefault(data, "SessionId");
            if (sessionId == Guid.Empty)
            {
                _logger.LogError("❌ SessionId is missing or invalid in Lesson update event");
                throw new InvalidOperationException("SessionId is required for updating a Lesson");
            }
            
            var lesson = new Lesson
            {
                LessonId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "LessonId"),
                SessionId = sessionId,
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                DurationMinutes = GetIntOrDefault(data, "DurationMinutes", 0),
                Position = GetIntOrDefault(data, "Position", GetIntOrDefault(data, "OrderIndex", 0)),
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow,
                LessonContexts = new List<LessonContext>(),
                Activities = new List<Activity>()
            };
            
            _logger.LogInformation("✓ Deserialized Lesson: ID={Id}, Title={Title}, SessionId={SessionId}", 
                lesson.LessonId, lesson.Title, sessionId);
            
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
        
        try
        {
            // Check if this is a batch LessonContext creation event (LessonContextBulkCreated)
            if (data.TryGetProperty("LessonContexts", out var lessonContextsElement) && 
                lessonContextsElement.ValueKind == JsonValueKind.Array)
            {
                var sessionId = GetGuidOrDefault(data, "SessionId");
                if (sessionId == Guid.Empty)
                {
                    _logger.LogError("❌ SessionId is missing in batch LessonContext event");
                    throw new InvalidOperationException("SessionId is required for batch LessonContext creation");
                }
                
                _logger.LogInformation("📦 Detected batch LessonContext creation (LessonContextBulkCreated) for SessionId: {SessionId}", sessionId);
                await HandleBatchLessonContextCreation(sessionId, lessonContextsElement);
                return;
            }
            
            // Standard single LessonContext creation
            var sessionId2 = GetGuidOrDefault(data, "SessionId");
            if (sessionId2 == Guid.Empty)
            {
                _logger.LogError("❌ SessionId is missing in single LessonContext event");
                throw new InvalidOperationException("SessionId is required for LessonContext creation");
            }
            
            var lessonContext = new LessonContext
            {
                LessonContextId = GetGuidOrDefault(data, "Id"),
                Title = GetStringOrDefault(data, "LessonTitle", GetStringOrDefault(data, "Title")),
                Content = GetStringOrDefault(data, "LessonContent", GetStringOrDefault(data, "Content")),
                Position = GetIntOrDefault(data, "Position"),
                Level = GetIntOrDefault(data, "Level"),
                ParentId = GetGuidOrDefault(data, "ParentLessonId") != Guid.Empty 
                    ? GetGuidOrDefault(data, "ParentLessonId") 
                    : null,
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Parsed LessonContext: ID={Id}, Title={Title}, Level={Level}, ParentId={ParentId}", 
                lessonContext.LessonContextId, lessonContext.Title, lessonContext.Level, lessonContext.ParentId);
            
            // Find or create Lesson for this SessionId
            var existingLessons = await _lessonDao.GetBySessionIdAsync(sessionId2);
            Lesson targetLesson;
            
            if (existingLessons == null || !existingLessons.Any())
            {
                // Create a new Lesson for this Session
                _logger.LogInformation("📝 Creating new Lesson for SessionId: {SessionId}", sessionId2);
                targetLesson = new Lesson
                {
                    LessonId = Guid.NewGuid(),
                    SessionId = sessionId2,
                    Title = $"Lesson for Session {sessionId2}",
                    Description = "Auto-generated from LessonContext creation",
                    Position = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _lessonDao.CreateAsync(targetLesson);
                _logger.LogInformation("✅ Created new Lesson with ID: {LessonId}", targetLesson.LessonId);
            }
            else
            {
                targetLesson = existingLessons.First();
                _logger.LogInformation("📝 Using existing Lesson ID: {LessonId}", targetLesson.LessonId);
            }
            
            // Save context to separate collection
            await _lessonContextDao.CreateAsync(targetLesson.LessonId, lessonContext);
            _logger.LogInformation("✅ Successfully created LessonContext with ID: {Id} for Lesson: {LessonId}", 
                lessonContext.LessonContextId, targetLesson.LessonId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to process LessonContext created event");
            throw;
        }
    }

    private async Task HandleLessonContextUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing LessonContext data for update...");
        
        try
        {
            // Check if this is a batch update (LessonContextBulkUpdated)
            if (data.TryGetProperty("LessonContexts", out var lessonContextsElement) && 
                lessonContextsElement.ValueKind == JsonValueKind.Array)
            {
                _logger.LogInformation("📦 Detected batch LessonContext update (LessonContextBulkUpdated)");
                await HandleBatchLessonContextUpdate(lessonContextsElement);
                return;
            }
            
            // Single LessonContext update
            var sessionId = GetGuidOrDefault(data, "SessionId");
            if (sessionId == Guid.Empty)
            {
                _logger.LogError("❌ SessionId is missing in single LessonContext update event");
                throw new InvalidOperationException("SessionId is required for LessonContext update");
            }
            
            var lessonContextId = GetGuidOrDefault(data, "Id");
            
            // Deactivate existing version
            _logger.LogInformation("🔄 Deactivating existing version of LessonContext ID: {Id}", lessonContextId);
            await _lessonContextDao.DeactivateAllByIdAsync(lessonContextId);
            
            // Create new version
            var lessonContext = new LessonContext
            {
                LessonContextId = lessonContextId,
                Title = GetStringOrDefault(data, "LessonTitle", GetStringOrDefault(data, "Title")),
                Content = GetStringOrDefault(data, "LessonContent", GetStringOrDefault(data, "Content")),
                Position = GetIntOrDefault(data, "Position"),
                Level = GetIntOrDefault(data, "Level"),
                ParentId = GetGuidOrDefault(data, "ParentLessonId") != Guid.Empty 
                    ? GetGuidOrDefault(data, "ParentLessonId") 
                    : null,
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            // Find Lesson for this SessionId
            var existingLessons = await _lessonDao.GetBySessionIdAsync(sessionId);
            if (existingLessons != null && existingLessons.Any())
            {
                var targetLesson = existingLessons.First();
                await _lessonContextDao.CreateAsync(targetLesson.LessonId, lessonContext);
                
                _logger.LogInformation("✅ Successfully updated LessonContext with ID: {Id} (created new version)", 
                    lessonContext.LessonContextId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot find Lesson for SessionId: {SessionId}", sessionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to process LessonContext update event");
            throw;
        }
    }
    
    private async Task HandleBatchLessonContextUpdate(JsonElement lessonContextsArray)
    {
        _logger.LogInformation("🔹 Processing batch LessonContext update...");
        
        var contextsToUpdate = new List<LessonContext>();
        Guid? sessionId = null;
        
        foreach (var contextElement in lessonContextsArray.EnumerateArray())
        {
            var currentSessionId = GetGuidOrDefault(contextElement, "SessionId");
            if (sessionId == null)
            {
                sessionId = currentSessionId;
            }
            
            var lessonContextId = GetGuidOrDefault(contextElement, "Id");
            
            // Deactivate existing version
            _logger.LogInformation("🔄 Deactivating existing version of LessonContext ID: {Id}", lessonContextId);
            await _lessonContextDao.DeactivateAllByIdAsync(lessonContextId);
            
            var context = new LessonContext
            {
                LessonContextId = lessonContextId,
                Title = GetStringOrDefault(contextElement, "LessonTitle", GetStringOrDefault(contextElement, "Title")),
                Content = GetStringOrDefault(contextElement, "LessonContent", GetStringOrDefault(contextElement, "Content")),
                Position = GetIntOrDefault(contextElement, "Position"),
                Level = GetIntOrDefault(contextElement, "Level"),
                ParentId = GetGuidOrDefault(contextElement, "ParentLessonId") != Guid.Empty 
                    ? GetGuidOrDefault(contextElement, "ParentLessonId") 
                    : null,
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(contextElement, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            contextsToUpdate.Add(context);
            _logger.LogInformation("✓ Prepared LessonContext for update: ID={Id}, Title={Title}, Level={Level}", 
                context.LessonContextId, context.Title, context.Level);
        }
        
        if (sessionId == null || sessionId == Guid.Empty)
        {
            _logger.LogError("❌ No valid SessionId found in batch update");
            return;
        }
        
        // Find Lesson for this SessionId
        var existingLessons = await _lessonDao.GetBySessionIdAsync(sessionId.Value);
        if (existingLessons != null && existingLessons.Any())
        {
            var targetLesson = existingLessons.First();
            
            // Save all updated contexts
            foreach (var context in contextsToUpdate)
            {
                await _lessonContextDao.CreateAsync(targetLesson.LessonId, context);
            }
            
            _logger.LogInformation("✅ Successfully updated {Count} LessonContexts for Lesson ID: {LessonId}", 
                contextsToUpdate.Count, targetLesson.LessonId);
        }
        else
        {
            _logger.LogWarning("⚠️ Cannot find Lesson for SessionId: {SessionId}", sessionId.Value);
        }
    }

    // Activity handlers
    private async Task HandleActivityCreated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Activity data...");
        
        try
        {
            var sessionId = GetGuidOrDefault(data, "SessionId");
            if (sessionId == Guid.Empty)
            {
                _logger.LogError("❌ SessionId is missing or invalid in Activity event");
                throw new InvalidOperationException("SessionId is required for creating an Activity");
            }
            
            var activity = new Activity
            {
                ActivityId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "ActivityId"),
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                ActivityType = GetStringOrDefault(data, "ActivityType"),
                Instructions = GetStringOrDefault(data, "Content", GetStringOrDefault(data, "Instructions")),
                EstimatedTimeMinutes = GetIntOrDefault(data, "EstimatedTimeMinutes", 0),
                Position = GetIntOrDefault(data, "Position", 0),
                IsActive = GetBoolOrDefault(data, "IsActive", true),
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Activity: ID={Id}, Title={Title}, SessionId={SessionId}", 
                activity.ActivityId, activity.Title, sessionId);
            
            // Find Lesson by SessionId
            _logger.LogInformation("🔍 Finding Lesson with SessionId: {SessionId}", sessionId);
            var lessons = await _lessonDao.GetBySessionIdAsync(sessionId);
            var lesson = lessons.FirstOrDefault();
            
            if (lesson != null)
            {
                _logger.LogInformation("💾 Saving Activity to separate collection for Lesson ID: {LessonId}", lesson.LessonId);
                
                // Save to separate Activity collection
                await _activityDao.CreateAsync(lesson.LessonId, activity);
                
                _logger.LogInformation("✅ Successfully created Activity {ActivityId} in separate collection for Lesson {LessonId} (Session {SessionId})", 
                    activity.ActivityId, lesson.LessonId, sessionId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot create Activity: Lesson with SessionId {SessionId} not found in MongoDB", sessionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or save Activity data");
            throw;
        }
    }

    private async Task HandleActivityUpdated(JsonElement data)
    {
        _logger.LogInformation("🔹 Deserializing Activity data for update...");
        
        try
        {
            var sessionId = GetGuidOrDefault(data, "SessionId");
            if (sessionId == Guid.Empty)
            {
                _logger.LogError("❌ SessionId is missing or invalid in Activity update event");
                throw new InvalidOperationException("SessionId is required for updating an Activity");
            }
            
            var activityId = GetGuidOrDefault(data, "Id") != Guid.Empty ? GetGuidOrDefault(data, "Id") : GetGuidOrDefault(data, "ActivityId");
            
            var activity = new Activity
            {
                ActivityId = activityId,
                Title = GetStringOrDefault(data, "Title"),
                Description = GetStringOrDefault(data, "Description"),
                ActivityType = GetStringOrDefault(data, "ActivityType"),
                Instructions = GetStringOrDefault(data, "Content", GetStringOrDefault(data, "Instructions")),
                EstimatedTimeMinutes = GetIntOrDefault(data, "EstimatedTimeMinutes", 0),
                Position = GetIntOrDefault(data, "Position", 0),
                IsActive = true,
                CreatedAt = GetDateTimeOrDefault(data, "CreatedAt"),
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("✓ Deserialized Activity: ID={Id}, Title={Title}, SessionId={SessionId}", 
                activity.ActivityId, activity.Title, sessionId);
            
            // Find Lesson by SessionId
            _logger.LogInformation("🔍 Finding Lesson with SessionId: {SessionId}", sessionId);
            var lessons = await _lessonDao.GetBySessionIdAsync(sessionId);
            var lesson = lessons.FirstOrDefault();
            
            if (lesson != null)
            {
                // Set LessonId for activity
                activity.LessonId = lesson.LessonId;
                
                // Deactivate all existing versions in separate collection
                _logger.LogInformation("🔄 Deactivating all existing versions of Activity ID: {Id}", activityId);
                await _activityDao.DeactivateAllByIdAsync(activityId);
                
                // Create new version in separate collection
                _logger.LogInformation("💾 Creating new version of Activity in separate collection...");
                await _activityDao.CreateAsync(lesson.LessonId, activity);
                
                _logger.LogInformation("✅ Successfully updated Activity {ActivityId} for Lesson {LessonId} (created new version in separate collection)", 
                    activity.ActivityId, lesson.LessonId);
            }
            else
            {
                _logger.LogWarning("⚠️ Cannot update Activity: Lesson with SessionId {SessionId} not found in MongoDB", sessionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to deserialize or update Activity data");
            throw;
        }
    }
}

