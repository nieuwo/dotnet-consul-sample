using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Sample.Api.HostedServices
{
    public class LeaderElectionService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;

        readonly string _key;
        IDistributedLock _distributedLock;
        Task _sessionRenewTask;
        CancellationTokenSource _sessionRenewCts;

        public LeaderElectionService(ILogger logger)
        {
            _logger = logger;
            _key = "org/dept/project/service"; // TODO: Get from config
        }

        protected async Task TryAccquireLockAsync(CancellationToken token)
        {
            if (!token.IsCancellationRequested)

            {
                try
                {
                    if (_distributedLock == null)
                    {
                        var client = new ConsulClient();
                        var se = new SessionEntry()
                        {
                            Checks = new List<string>()
                            {
                                // Default health check for the consul agent. It is very recommended to keep this.
                                "serfHealth",
                                // "myServiceHealthCheck" // Any additional health check.
                            },
                            Name = "myServicSession",
                            // Optional TTL check, to achieve sliding expiration. It is very recommended to use it.
                            TTL = TimeSpan.FromSeconds(30)
                        };

                        var sessionId = (await client.Session.Create(se).ConfigureAwait(false)).Response;
                        if (se.TTL.HasValue)
                        {
                            _sessionRenewCts = new CancellationTokenSource();
                            _sessionRenewTask = client.Session.RenewPeriodic(se.TTL.Value, sessionId,
                                WriteOptions.Default, _sessionRenewCts.Token);
                        }

                        _distributedLock = await client
                            .AcquireLock(
                                new LockOptions(_key)
                                    {Session = sessionId, LockTryOnce = true, LockWaitTime = TimeSpan.FromSeconds(3)},
                                token).ConfigureAwait(false);
                    }
                    else
                    {
                        if (!_distributedLock.IsHeld)
                        {
                            if (_sessionRenewTask.IsCompleted)
                            {
                                Task.WaitAny(
                                    _sessionRenewTask); //Awaits the task without throwing, cleaner than try catch.
                                _distributedLock = null;
                            }
                            else
                            {
                                await _distributedLock.Acquire(token).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (LockMaxAttemptsReachedException ex)
                {
                    _logger.Warning(ex, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                }
                finally
                {
                    if (_distributedLock == null && _sessionRenewCts != null)
                    {
                        _sessionRenewCts.Cancel();
                        _sessionRenewCts.Dispose();
                    }

                    HandleLockStatusChange(_distributedLock?.IsHeld == true);

                    // Retrigger the timer after an 10 seconds delay (in this example)
                    _timer.Change(10000, Timeout.Infinite); 
                }
            }
        }

        protected virtual void HandleLockStatusChange(bool isLeader)
        {
            Globals.IsLeader = isLeader;
            _logger.Information(Globals.IsLeader ? "Leader" : "Secondary");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Leader Election Service running.");
            _timer = new Timer(async (object state) => await TryAccquireLockAsync((CancellationToken) state),
                cancellationToken, 0, Timeout.Infinite);
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Leader Election Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}