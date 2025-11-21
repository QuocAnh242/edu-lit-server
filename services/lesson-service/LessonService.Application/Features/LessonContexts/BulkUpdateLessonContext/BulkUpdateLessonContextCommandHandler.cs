using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LessonService.Application.Features.LessonContexts.BulkUpdateLessonContext;

public class BulkUpdateLessonContextCommandHandler 
    : ICommandHandler<BulkUpdateLessonContextCommand, BulkUpdateLessonContextResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkUpdateLessonContextCommandHandler> _logger;

    public BulkUpdateLessonContextCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<BulkUpdateLessonContextCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<BulkUpdateLessonContextResponse>> Handle(
        BulkUpdateLessonContextCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.Items == null || !request.Items.Any())
            {
                return ApiResponse<BulkUpdateLessonContextResponse>.FailureResponse(
                    "No items to update",
                    400
                );
            }

            var updatedIds = new List<Guid>();
            var updatedEntities = new List<LessonContext>();

            foreach (var item in request.Items)
            {
                // Get existing entity
                var entity = await _unitOfWork.LessonContextRepository.GetByIdAsync(item.Id);
                
                if (entity == null)
                {
                    _logger.LogWarning(
                        "⚠️  LessonContext {Id} not found, skipping",
                        item.Id
                    );
                    continue;
                }

                // Update only provided fields (partial update)
                if (item.LessonTitle != null)
                {
                    entity.LessonTitle = item.LessonTitle;
                }

                if (item.LessonContent != null)
                {
                    entity.LessonContent = item.LessonContent;
                }

                if (item.Position.HasValue)
                {
                    entity.Position = item.Position.Value;
                }

                if (item.Level.HasValue)
                {
                    entity.Level = item.Level.Value;
                }

                entity.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.LessonContextRepository.Update(entity);
                updatedIds.Add(entity.Id);
                updatedEntities.Add(entity);

                _logger.LogInformation(
                    "📝 Updated LessonContext {Id}: Title={Title}, Position={Position}, Level={Level}",
                    entity.Id, entity.LessonTitle, entity.Position, entity.Level
                );
            }

            // Create outbox message for publishing
            if (updatedEntities.Any())
            {
                var outboxMessage = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = "LessonContextBulkUpdated",
                    Exchange = "lesson-context-events",
                    RoutingKey = "lessoncontext.updated",
                    Payload = JsonSerializer.Serialize(new
                    {
                        LessonContexts = updatedEntities.Select(entity => new
                        {
                            entity.Id,
                            entity.SessionId,
                            entity.ParentLessonId,
                            entity.LessonTitle,
                            entity.LessonContent,
                            entity.Position,
                            entity.Level,
                            entity.UpdatedAt
                        }).ToList(),
                        TotalCount = updatedEntities.Count,
                        EventType = "LessonContextBulkUpdated",
                        Timestamp = DateTime.UtcNow
                    }),
                    CreatedAt = DateTime.UtcNow,
                    IsProcessed = false,
                    RetryCount = 0
                };

                await _unitOfWork.OutboxRepository.AddAsync(outboxMessage);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "✅ Successfully updated {Count} lesson contexts",
                updatedIds.Count
            );

            var response = new BulkUpdateLessonContextResponse
            {
                TotalUpdated = updatedIds.Count,
                UpdatedIds = updatedIds
            };

            return ApiResponse<BulkUpdateLessonContextResponse>.SuccessResponse(
                response,
                $"Successfully updated {updatedIds.Count} lesson contexts"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk updating lesson contexts");
            return ApiResponse<BulkUpdateLessonContextResponse>.FailureResponse(
                "An error occurred while updating lesson contexts",
                500
            );
        }
    }
}

