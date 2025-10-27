using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.Features.Question.UpdateQuestion
{
    public class UpdateQuestionCommand : ICommand<Guid>
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string? Metadata { get; set; }
        public string? Tags { get; set; }
        public int Version { get; set; }
        public bool IsPublished { get; set; }
        public Guid QuestionBankId { get; set; }
        public Guid AuthorId { get; set; }
    }
}

