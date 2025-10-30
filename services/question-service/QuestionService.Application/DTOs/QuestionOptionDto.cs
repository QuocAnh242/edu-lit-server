namespace QuestionService.Application.DTOs
{
    public class QuestionOptionDto
    {
        public Guid QuestionOptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIdx { get; set; }
        public Guid QuestionId { get; set; }
    }
}




