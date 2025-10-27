using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.Question.GetQuestionsByAuthorId
{
    public class GetQuestionsByAuthorIdQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public Guid AuthorId { get; }

        public GetQuestionsByAuthorIdQuery(Guid authorId)
        {
            AuthorId = authorId;
        }
    }
}

