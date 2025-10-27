using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Entities
{
    public partial class Question
    {
        public Guid QuestionId { get; set; }

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public QuestionType QuestionType { get; set; }

        public string? Metadata { get; set; }

        public string? Tags { get; set; }

        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsPublished { get; set; }

        public Guid QuestionBankId { get; set; }

        public Guid AuthorId { get; set; }

        public virtual QuestionBank QuestionBank { get; set; } = null!;

        public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
    }
}
