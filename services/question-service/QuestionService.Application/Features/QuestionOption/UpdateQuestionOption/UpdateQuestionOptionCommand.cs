using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionOption.UpdateQuestionOption
{
    public class UpdateQuestionOptionCommand : ICommand<Guid>
    {
        public Guid QuestionOptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIdx { get; set; }
    }
}




