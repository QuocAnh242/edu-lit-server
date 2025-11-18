namespace AssessmentService.Application.DTOs
{
    // Match Question Service ApiResponse format
    public class QuestionServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int QuestionType { get; set; }
        public string? Metadata { get; set; }
        public string? Tags { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public Guid QuestionBankId { get; set; }
        public Guid AuthorId { get; set; }
    }

    public class QuestionOptionDto
    {
        public Guid QuestionOptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIdx { get; set; }
        public Guid QuestionId { get; set; }
    }
}

