namespace QuestionService.Application.DTOs
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public string? Tags { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public Guid QuestionBankId { get; set; }
        public Guid AuthorId { get; set; }
        public List<QuestionOptionDto> QuestionOptions { get; set; } = new List<QuestionOptionDto>();
    }
}
