namespace QuestionService.Application.DTOs
{
    public class QuestionBankDto
    {
        public Guid QuestionBanksId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OwnerId { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
