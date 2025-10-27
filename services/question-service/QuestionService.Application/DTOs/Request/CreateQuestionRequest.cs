using System.ComponentModel.DataAnnotations;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.DTOs.Request
{
    public class CreateQuestionRequest
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Body { get; set; } = string.Empty;

        [Required]
        public QuestionType QuestionType { get; set; }

        public string? Metadata { get; set; }

        [MaxLength(500)]
        public string? Tags { get; set; }

        public int Version { get; set; } = 1;

        public bool IsPublished { get; set; } = false;

        [Required]
        public Guid QuestionBankId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }
    }
}
