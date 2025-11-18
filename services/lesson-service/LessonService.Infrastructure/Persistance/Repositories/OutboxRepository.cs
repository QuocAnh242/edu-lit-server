using LessonService.Domain.Entities;
using LessonService.Domain.IDAOs;
using LessonService.Domain.IRepositories;
using LessonService.Infrastructure.Persistance.DAOs;
using LessonService.Infrastructure.Persistance.DBContext;
using Microsoft.EntityFrameworkCore;

namespace LessonService.Infrastructure.Persistance.Repositories;

public class OutboxRepository : GenericRepository<OutboxMessage>, IOutboxRepository
{
    private readonly LessonDbContext _context;

    public OutboxRepository(LessonDbContext context) : base(new GenericDAO<OutboxMessage>(context))
    {
        _context = context;
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 10)
    {
        return await _context.OutboxMessages
            .Where(m => !m.IsProcessed && m.RetryCount < 3)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task MarkAsProcessedAsync(Guid id)
    {
        var message = await _context.OutboxMessages.FindAsync(id);
        if (message != null)
        {
            message.IsProcessed = true;
            message.ProcessedAt = DateTime.UtcNow;
            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAsFailedAsync(Guid id, string error)
    {
        var message = await _context.OutboxMessages.FindAsync(id);
        if (message != null)
        {
            message.RetryCount++;
            message.Error = error;
            message.ProcessedAt = DateTime.UtcNow;
            
            // Mark as processed if retry limit reached
            if (message.RetryCount >= 3)
            {
                message.IsProcessed = true;
            }
            
            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}

