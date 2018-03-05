using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LongRunning.Web.Background
{
    public interface ILongRunningProcessService
    {
        Task<LongRunningTaskResult> ExecuteLongRunningTask(Guid taskId);
    }

    public class LongRunningProcessService : ILongRunningProcessService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        public LongRunningProcessService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        async Task<LongRunningTaskResult> Process(Guid taskId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                return await ProcessInScope(taskId, scope.ServiceProvider);
            }
        }

        async Task<LongRunningTaskResult> ProcessInScope(Guid taskId, IServiceProvider serviceProvider)
        {
            await Task.Delay(15000);
            return new LongRunningTaskResult
            {
                Result = $"Got {taskId.ToString("N")} at {DateTime.Now.ToShortTimeString()}",
            };
        }

        public Task<LongRunningTaskResult> ExecuteLongRunningTask(Guid taskId)
        {
            TaskCompletionSource<LongRunningTaskResult> tcs1 = new TaskCompletionSource<LongRunningTaskResult>();
            
            // Start a background task that will complete tcs1.Task
            Task.Factory.StartNew(async () =>
            {
                var result = await Process(taskId);
                tcs1.SetResult(result);
            });
            return tcs1.Task;

        }
    }

    public class LongRunningTaskResult
    {
        public string Result { get; set; }
    }
}
