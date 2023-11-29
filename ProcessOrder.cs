using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace DownloadService
{
    public class ProcessOrder : IInvocable
    {
        private readonly ILogger<ProcessOrder> logger;
        public ProcessOrder(ILogger<ProcessOrder> logger)
        {
            this.logger = logger;
        }
        public Task Invoke()
        {
            var jobID = Guid.NewGuid;
            logger.LogInformation($"Starting job ID {jobID}");
            return Task.FromResult(true);
        }
    }
}
