using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionOption.CreateQuestionOption
{
    public class CreateQuestionOptionCommand : ICommand<Guid>
    {
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIdx { get; set; }
        public Guid QuestionId { get; set; }
    }
}

