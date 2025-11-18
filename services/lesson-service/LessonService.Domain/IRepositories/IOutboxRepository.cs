using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;

namespace LessonService.Domain.IRepositories;

public interface IOutboxRepository : IGenericRepository<OutboxMessage>
{
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 10);
    Task MarkAsProcessedAsync(Guid id);
    Task MarkAsFailedAsync(Guid id, string error);
}

