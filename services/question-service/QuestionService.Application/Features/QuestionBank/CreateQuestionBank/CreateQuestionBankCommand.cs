using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionBank.CreateQuestionBank
{
    public class CreateQuestionBankCommand : ICommand<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Subject { get; set; }
        public Guid OwnerId { get; set; }
    }
}



