using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LessonService.Application.Features.LessonContexts.CreateBulkLessonContext;

public class CreateBulkLessonContextCommandHandler 
    : ICommandHandler<CreateBulkLessonContextCommand, CreateBulkLessonContextResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBulkLessonContextCommandHandler> _logger;

    public CreateBulkLessonContextCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateBulkLessonContextCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<CreateBulkLessonContextResponse>> Handle(
        CreateBulkLessonContextCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate session exists
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(request.SessionId);
            if (session == null)
            {
                return ApiResponse<CreateBulkLessonContextResponse>.FailureResponse(
                    "Session not found",
                    404
                );
            }

            // Sort by position to ensure correct order
            var sortedContexts = request.LessonContexts.OrderBy(x => x.Position).ToList();

            var createdItems = new List<LessonContextCreatedDto>();
            var lessonContextEntities = new List<LessonContext>();
            
            // Dictionary to track parent by level: key = level, value = last created item at that level
            var levelParentMap = new Dictionary<int, Guid>();

            foreach (var item in sortedContexts)
            {
                Guid? parentId = null;

                // Determine parent based on level
                if (item.Level > 0)
                {
                    // Find parent at level - 1
                    var parentLevel = item.Level - 1;
                    if (levelParentMap.ContainsKey(parentLevel))
                    {
                        parentId = levelParentMap[parentLevel];
                        _logger.LogInformation(
                            "🔗 Found parent for '{Title}' (Level {Level}): Parent ID = {ParentId} (from Level {ParentLevel})",
                            item.LessonTitle, item.Level, parentId, parentLevel
                        );
                    }
                    else
                    {
                        _logger.LogWarning(
                            "⚠️  No parent found at level {ParentLevel} for item '{Title}' at position {Position}. Creating as root level.",
                            parentLevel, item.LessonTitle, item.Position
                        );
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "🌳 Creating root item '{Title}' (Level 0) - No parent",
                        item.LessonTitle
                    );
                }

                var lessonContext = new LessonContext
                {
                    Id = Guid.NewGuid(),
                    SessionId = request.SessionId,
                    ParentLessonId = parentId,
                    LessonTitle = item.LessonTitle,
                    LessonContent = item.LessonContent,
                    Position = item.Position,
                    Level = item.Level,
                    CreatedAt = DateTime.UtcNow
                };

                lessonContextEntities.Add(lessonContext);

                // Update level map with this item as potential parent
                levelParentMap[item.Level] = lessonContext.Id;
                
                _logger.LogInformation(
                    "💾 Registered '{Title}' (ID: {Id}) as potential parent for Level {NextLevel}",
                    item.LessonTitle, lessonContext.Id, item.Level + 1
                );

                createdItems.Add(new LessonContextCreatedDto
                {
                    Id = lessonContext.Id,
                    ParentLessonId = parentId,
                    LessonTitle = item.LessonTitle,
                    Position = item.Position,
                    Level = item.Level
                });

                _logger.LogInformation(
                    "✅ LessonContext Created: '{Title}' | Level: {Level} | Position: {Position} | Parent: {ParentId}",
                    item.LessonTitle, item.Level, item.Position, parentId?.ToString() ?? "NULL (Root)"
                );
            }

            // Bulk insert all lesson contexts
            foreach (var entity in lessonContextEntities)
            {
                await _unitOfWork.LessonContextRepository.AddAsync(entity);
            }

            // Create outbox message for publishing to RabbitMQ
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "LessonContextBulkCreated",
                Exchange = "lesson-context-events",
                RoutingKey = "lessoncontext.created",
                Payload = JsonSerializer.Serialize(new
                {
                    SessionId = request.SessionId,
                    LessonContexts = lessonContextEntities.Select(entity => new
                    {
                        Id = entity.Id,
                        SessionId = entity.SessionId,
                        ParentLessonId = entity.ParentLessonId,  // ← GÁN TỪ ENTITY ĐÃ CÓ PARENT!
                        LessonTitle = entity.LessonTitle,
                        LessonContent = entity.LessonContent,
                        Position = entity.Position,
                        Level = entity.Level,
                        CreatedAt = entity.CreatedAt
                    }).ToList(),
                    TotalCount = lessonContextEntities.Count,
                    EventType = "LessonContextBulkCreated",
                    Timestamp = DateTime.UtcNow
                }),
                CreatedAt = DateTime.UtcNow,
                IsProcessed = false,
                RetryCount = 0
            };

            await _unitOfWork.OutboxRepository.AddAsync(outboxMessage);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "✅ Successfully created {Count} lesson contexts for session {SessionId}",
                createdItems.Count, request.SessionId
            );

            var response = new CreateBulkLessonContextResponse
            {
                TotalCreated = createdItems.Count,
                CreatedItems = createdItems
            };

            return ApiResponse<CreateBulkLessonContextResponse>.SuccessResponse(
                response,
                "Lesson contexts created successfully"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating bulk lesson contexts");
            return ApiResponse<CreateBulkLessonContextResponse>.FailureResponse(
                "An error occurred while creating lesson contexts",
                500
            );
        }
    }
}

