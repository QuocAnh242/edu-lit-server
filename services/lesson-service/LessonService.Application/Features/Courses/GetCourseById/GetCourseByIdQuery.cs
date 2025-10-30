using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Courses.GetCourseById;

public class GetCourseByIdQuery : IQuery<GetCourseByIdResponse>
{
    public Guid Id { get; }

    public GetCourseByIdQuery(Guid id)
    {
        Id = id;
    }
}