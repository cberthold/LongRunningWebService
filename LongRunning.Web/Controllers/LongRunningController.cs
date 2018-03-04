using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LongRunning.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/LongRunning")]
    public class LongRunningController : Controller
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Start(CancellationToken token)
        {
            var taskId = await StartTaskRunningAsync(token);
            var uri = Url.Action("Check", new { taskId });
            return Accepted(uri);
        }

        [HttpGet("[action]")]
        public async Task Check(Guid taskId)
        {

        }

        private async Task<Guid> StartTaskRunningAsync(CancellationToken token)
        {
            var taskId = Guid.NewGuid();
            return await Task.FromResult(taskId);
        }
    }
}