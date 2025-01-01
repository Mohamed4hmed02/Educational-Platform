using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Application.Abstractions.CourseInterfaces
{
    public interface ICourseCommandServices : IEditable<CommandCourseModel>, IDeleteable<Course>, IDisposable
    {
        ValueTask SetTopicIntroAsync(object courseId, object topicId);
        ValueTask SetImageAsync(object courseId, IFormFile image);
        ValueTask SetVisibiltyAsync(object courseId, bool visibilty);
    }
}
