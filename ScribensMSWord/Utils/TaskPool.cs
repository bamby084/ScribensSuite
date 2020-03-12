using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using System.Diagnostics;

namespace ScribensMSWord.Utils
{
    public class TaskPool
    {
        private static ConcurrentDictionary<Guid, TaskInfo> _tasks = new ConcurrentDictionary<Guid, TaskInfo>();

        public static async Task StartNew(Action<CancellationToken, Guid> taskAction)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var taskId = Guid.NewGuid();

            var task = Task.Factory.StartNew(() => taskAction(token, taskId), token, 
                TaskCreationOptions.LongRunning, TaskScheduler.Current);

            _tasks.TryAdd(taskId, new TaskInfo()
            {
                CancellationTokenSource = cancellationTokenSource,
                Task = task
            });

            try
            {
                Debug.WriteLine($"Task {taskId} started");
                await task;
                Debug.WriteLine($"Task {taskId} completed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                _tasks.TryRemove(taskId, out TaskInfo removedTask);
                cancellationTokenSource.Dispose();
            }
        }

        public static void CancelRunningTasks(Guid exceptTaskId)
        {
            var tasksToCancel =_tasks.Where(t => t.Key != exceptTaskId).Select(t => t.Key).ToList();
            foreach(var taskId in tasksToCancel)
            {
                _tasks.TryRemove(taskId, out TaskInfo task);
                try
                {
                    task.CancellationTokenSource.Cancel();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        private class TaskInfo
        {
            public CancellationTokenSource CancellationTokenSource { get; set; }
            public Task Task { get; set; }
        }
    }
}
