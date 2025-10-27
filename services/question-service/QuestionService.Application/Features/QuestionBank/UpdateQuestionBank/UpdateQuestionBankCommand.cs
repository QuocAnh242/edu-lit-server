using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionBank.UpdateQuestionBank
{
    public class UpdateQuestionBankCommand : ICommand<Guid>
    {
        public Guid QuestionBankId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Subject { get; set; }
        public Guid OwnerId { get; set; }
    }
}

