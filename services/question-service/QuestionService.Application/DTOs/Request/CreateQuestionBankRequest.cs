using System.ComponentModel.DataAnnotations;

namespace QuestionService.Application.DTOs.Request
{
    public class CreateQuestionBankRequest
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Subject { get; set; }
    }
}




