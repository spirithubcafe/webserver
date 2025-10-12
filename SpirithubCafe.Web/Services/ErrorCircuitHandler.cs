using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Collections.Concurrent;

namespace SpirithubCafe.Web.Services
{
    public class ErrorCircuitHandler : CircuitHandler
    {
        private readonly ILogger<ErrorCircuitHandler> _logger;
        private readonly ConcurrentDictionary<string, DateTime> _circuits = new();

        public ErrorCircuitHandler(ILogger<ErrorCircuitHandler> logger)
        {
            _logger = logger;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _circuits.TryAdd(circuit.Id, DateTime.UtcNow);
            _logger.LogInformation("Circuit opened: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _circuits.TryRemove(circuit.Id, out _);
            _logger.LogInformation("Circuit closed: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Connection down for circuit: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Connection restored for circuit: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }

        public int ConnectedCircuits => _circuits.Count;

        public void CleanupStaleCircuits()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-5);
            var staleCircuits = _circuits.Where(kvp => kvp.Value < cutoff).ToList();
            
            foreach (var stale in staleCircuits)
            {
                _circuits.TryRemove(stale.Key, out _);
                _logger.LogInformation("Cleaned up stale circuit: {CircuitId}", stale.Key);
            }
        }
    }
}