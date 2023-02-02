using AirStack.Core.Service;

namespace AirStack.API.Service
{
    public class ProcessItemHistoryQueueService : BackgroundService
    {
        private readonly ILogger<ProcessItemHistoryQueueService> _logger;
        readonly IItemHistoryQueueProvider _qSvc;
        public ProcessItemHistoryQueueService(ILogger<ProcessItemHistoryQueueService> logger, IItemHistoryQueueProvider qProvider)
        {
            _logger = logger;
            _qSvc = qProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool hasError = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _qSvc.Process();
                    await Task.Delay(3000);

                    hasError = false;
                }
                catch (Exception ex)
                {
                    if (hasError != true)
                        _logger.LogError(ex.Message);

                    await Task.Delay(6000);
                    hasError = true;
                }
            }
        }
    }
}
