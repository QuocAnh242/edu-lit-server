namespace QuestionService.Domain.Entities
{
    public partial class QuestionOption
    {
        public Guid QuestionOptionId { get; set; }

        public string OptionText { get; set; } = null!;

        public bool IsCorrect { get; set; }

        public int OrderIdx { get; set; }

        public Guid QuestionId { get; set; }

        public virtual Question Question { get; set; } = null!;
    }
}
