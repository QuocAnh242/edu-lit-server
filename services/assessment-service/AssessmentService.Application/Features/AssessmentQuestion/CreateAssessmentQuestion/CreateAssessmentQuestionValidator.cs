using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion
{
    public class CreateAssessmentQuestionValidator : AbstractValidator<CreateAssessmentQuestionCommand>
    {
        public CreateAssessmentQuestionValidator()
        {
            RuleFor(x => x.AssessmentId)
                .GreaterThan(0)
                .WithMessage("AssessmentId phải lớn hơn 0");

            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .WithMessage("QuestionId không được để trống")
                .MaximumLength(255)
                .WithMessage("QuestionId không được vượt quá 255 ký tự");

            RuleFor(x => x.OrderNum)
                .GreaterThanOrEqualTo(0)
                .WithMessage("OrderNum phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty()
                .WithMessage("CorrectAnswer không được để trống")
                .Must(BeValidAnswer)
                .WithMessage("CorrectAnswer chỉ được là A, B, C hoặc D")
                .Length(1)
                .WithMessage("CorrectAnswer chỉ được là 1 ký tự");
        }

        private bool BeValidAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return false;

            var upperAnswer = answer.ToUpper();
            return upperAnswer == "A" || upperAnswer == "B" ||
                   upperAnswer == "C" || upperAnswer == "D";
        }
    }
}
