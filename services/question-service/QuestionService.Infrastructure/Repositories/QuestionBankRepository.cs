using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Infrastructure.Data;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionBankRepository : IQuestionBankRepository
    {
        private readonly QuestionDbContext _context;

        public QuestionBankRepository(QuestionDbContext context)
        {
            _context = context;
        }

        public async Task<QuestionBank?> GetByIdAsync(Guid questionBanksId)
        {
            return await _context.QuestionBanks
                .Include(qb => qb.Questions)
                .FirstOrDefaultAsync(qb => qb.QuestionBanksId == questionBanksId);
        }

        public async Task<IEnumerable<QuestionBank>> GetAllAsync()
        {
            return await _context.QuestionBanks
                .Include(qb => qb.Questions)
                .OrderBy(qb => qb.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuestionBank>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.QuestionBanks
                .Include(qb => qb.Questions)
                .Where(qb => qb.OwnerId == ownerId)
                .OrderBy(qb => qb.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuestionBank>> GetBySubjectAsync(string subject)
        {
            return await _context.QuestionBanks
                .Include(qb => qb.Questions)
                .Where(qb => qb.Subject == subject)
                .OrderBy(qb => qb.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuestionBank> CreateAsync(QuestionBank questionBank)
        {
            _context.QuestionBanks.Add(questionBank);
            await _context.SaveChangesAsync();
            return questionBank;
        }

        public async Task<QuestionBank> UpdateAsync(QuestionBank questionBank)
        {
            _context.QuestionBanks.Update(questionBank);
            await _context.SaveChangesAsync();
            return questionBank;
        }

        public async Task<bool> DeleteAsync(Guid questionBanksId)
        {
            var questionBank = await _context.QuestionBanks.FindAsync(questionBanksId);
            if (questionBank == null) return false;

            _context.QuestionBanks.Remove(questionBank);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid questionBanksId)
        {
            return await _context.QuestionBanks.AnyAsync(qb => qb.QuestionBanksId == questionBanksId);
        }
    }
}

