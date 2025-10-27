using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.Features.Question.CreateQuestion
{
    public class CreateQuestionCommand : ICommand<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string? Metadata { get; set; }
        public string? Tags { get; set; }
        public int Version { get; set; } = 1;
        public bool IsPublished { get; set; } = false;
        public Guid QuestionBankId { get; set; }
        public Guid AuthorId { get; set; }
    }
}

