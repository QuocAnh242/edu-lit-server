using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Domain.Enums;
using QuestionService.Infrastructure.Data;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuestionDbContext _context;

        public QuestionRepository(QuestionDbContext context)
        {
            _context = context;
        }

        public async Task<Question?> GetByIdAsync(Guid questionId)
        {
            return await _context.Questions
                .Include(q => q.QuestionBank)
                .Include(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.QuestionBank)
                .Include(q => q.QuestionOptions)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByQuestionBankIdAsync(Guid questionBankId)
        {
            return await _context.Questions
                .Include(q => q.QuestionBank)
                .Include(q => q.QuestionOptions)
                .Where(q => q.QuestionBankId == questionBankId)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByAuthorIdAsync(Guid authorId)
        {
            return await _context.Questions
                .Include(q => q.QuestionBank)
                .Include(q => q.QuestionOptions)
                .Where(q => q.AuthorId == authorId)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByQuestionTypeAsync(QuestionType questionType)
        {
            return await _context.Questions
                .Include(q => q.QuestionBank)
                .Include(q => q.QuestionOptions)
                .Where(q => q.QuestionType == questionType)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetPublishedQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.QuestionBank)
                .Include(q => q.QuestionOptions)
                .Where(q => q.IsPublished)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateAsync(Question question)
        {
            question.UpdatedAt = DateTime.UtcNow;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteAsync(Guid questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null) return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid questionId)
        {
            return await _context.Questions.AnyAsync(q => q.QuestionId == questionId);
        }
    }
}
