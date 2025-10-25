using System.ComponentModel.DataAnnotations;

namespace QuestionService.Application.DTOs.Request
{
    public class CreateQuestionOptionRequest
    {
        [Required]
        [MaxLength(1000)]
        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;

        public int OrderIdx { get; set; } = 0;

        [Required]
        public Guid QuestionId { get; set; }
    }
}
