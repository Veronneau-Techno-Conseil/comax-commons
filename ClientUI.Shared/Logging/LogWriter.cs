using System.Threading;
using System.Threading.Tasks;
using ClientUI.Shared.Models;

namespace ClientUI.Shared.Logging;

public class LogWriter
{
    private readonly LogQueue _queue;

    public LogWriter(LogQueue queue)
    {
        _queue = queue;
    }

    public ValueTask Write(LogMessage message, CancellationToken token = default) =>
        _queue.WriteLog(message, token);
}
