using System.Collections.Concurrent;
using Sse_Poc.Models;

namespace Sse_Poc.Service;

public class PulseService
{
    private readonly ConcurrentDictionary<Guid, PulseRecord> _pulses = new();

    public Guid StorePulse(string value)
    {
        var id = Guid.NewGuid();
        var record = new PulseRecord
        {
            Value = value,
            Timestamp = DateTime.UtcNow
        };

        _pulses[id] = record;
        return id;
    }

    public List<Pulses> GetAllPulses()
    {
        var groupedPulses = _pulses
            .GroupBy(p => p.Value.Value)
            .Select(group => new
            {
                TabId = group.Key,
                Timestamps = group
                    .OrderByDescending(p => p.Value.Timestamp)
                    .Select(p => new
                    {
                        TabId = p.Value.Value,
                        Timestamp = p.Value.Timestamp
                    })
                    .ToList(),
                Count = group.Count()
            })
            .OrderByDescending(g => g.Timestamps.FirstOrDefault()?.Timestamp)
            .ToList();

        var pulses = groupedPulses
            .Select(p => new Pulses
            {
                TabId = p.TabId,
                Timestamps = p.Timestamps.Select(t => t.Timestamp).ToList()
            })
            .ToList();

        return pulses;
    }

    public PulseRecord? GetPulse(Guid id)
    {
        return _pulses.GetValueOrDefault(id);
    }

        public void DeletePulses(string prefix)
    {
        // Remove all pulses that start with the given prefix
        var keysToRemove = _pulses
            .Where(kvp => kvp.Value.Value.StartsWith(prefix))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _pulses.TryRemove(key, out _);
        }
    }
}
