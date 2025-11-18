using System.Threading;
using System.Threading.Tasks;

namespace LessonService.Infrastructure.Messaging
{
    public interface IMessageBusPublisher
    {
        Task PublishAsync(string type, string payload, CancellationToken ct = default);
    }
}