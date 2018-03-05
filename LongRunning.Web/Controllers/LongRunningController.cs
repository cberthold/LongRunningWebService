using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LongRunning.Web.Background;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LongRunning.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/LongRunning")]
    public class LongRunningController : Controller
    {
        private static readonly ConcurrentDictionary<Guid, Task<LongRunningTaskResult>> concurrent = new ConcurrentDictionary<Guid, Task<LongRunningTaskResult>>();
        private readonly ILongRunningProcessService process;
        public LongRunningController(ILongRunningProcessService process)
        {
            this.process = process;
        }

        [HttpGet("[action]")]
        public IActionResult Start()
        {
            var taskId = StartTaskRunningAsync();
            var uri = Url.Action("Check", new { taskId });
            return Accepted(uri);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Check(Guid taskId)
        {
            if (concurrent.TryGetValue(taskId, out var task))
            {
                if (task.IsCompleted)
                {
                    return Ok(task.Result);
                }
            }

            await Task.Delay(50);
            
            var uri = Url.Action("Check", new { taskId });
            return Accepted(uri);
        }

        private Guid StartTaskRunningAsync()
        {
            var taskId = Guid.NewGuid();
            var task = process.ExecuteLongRunningTask(taskId);
            concurrent.TryAdd(taskId, task);
            return taskId;
        }
    }
}