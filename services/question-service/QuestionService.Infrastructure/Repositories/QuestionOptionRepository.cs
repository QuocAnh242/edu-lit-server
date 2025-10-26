using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Infrastructure.Data;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionOptionRepository : IQuestionOptionRepository
    {
        private readonly QuestionDbContext _context;

        public QuestionOptionRepository(QuestionDbContext context)
        {
            _context = context;
        }

        public async Task<QuestionOption?> GetByIdAsync(Guid questionOptionId)
        {
            return await _context.QuestionOptions
                .Include(qo => qo.Question)
                .FirstOrDefaultAsync(qo => qo.QuestionOptionId == questionOptionId);
        }

        public async Task<IEnumerable<QuestionOption>> GetAllAsync()
        {
            return await _context.QuestionOptions
                .Include(qo => qo.Question)
                .OrderBy(qo => qo.OrderIdx)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(Guid questionId)
        {
            return await _context.QuestionOptions
                .Include(qo => qo.Question)
                .Where(qo => qo.QuestionId == questionId)
                .OrderBy(qo => qo.OrderIdx)
                .ToListAsync();
        }

        public async Task<QuestionOption> CreateAsync(QuestionOption questionOption)
        {
            _context.QuestionOptions.Add(questionOption);
            await _context.SaveChangesAsync();
            return questionOption;
        }

        public async Task<QuestionOption> UpdateAsync(QuestionOption questionOption)
        {
            _context.QuestionOptions.Update(questionOption);
            await _context.SaveChangesAsync();
            return questionOption;
        }

        public async Task<bool> DeleteAsync(Guid questionOptionId)
        {
            var questionOption = await _context.QuestionOptions.FindAsync(questionOptionId);
            if (questionOption == null) return false;

            _context.QuestionOptions.Remove(questionOption);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByQuestionIdAsync(Guid questionId)
        {
            var questionOptions = await _context.QuestionOptions
                .Where(qo => qo.QuestionId == questionId)
                .ToListAsync();

            if (!questionOptions.Any()) return false;

            _context.QuestionOptions.RemoveRange(questionOptions);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid questionOptionId)
        {
            return await _context.QuestionOptions.AnyAsync(qo => qo.QuestionOptionId == questionOptionId);
        }
    }
}

