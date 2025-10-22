namespace QuestionService.Domain.Entities
{
    public partial class QuestionBank
    {
        public Guid QuestionBanksId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Subject { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OwnerId { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
