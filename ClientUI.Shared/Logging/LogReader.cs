using System.Collections.Generic;
using System.Threading;
using ClientUI.Shared.Models;

namespace ClientUI.Shared.Logging;

public class LogReader
{
    private readonly LogQueue _queue;

    public LogReader(LogQueue queue)
    {
        _queue = queue;
    }

    public IAsyncEnumerable<LogMessage> Read(CancellationToken token = default) =>
        _queue.ReadLogs(token);
}
