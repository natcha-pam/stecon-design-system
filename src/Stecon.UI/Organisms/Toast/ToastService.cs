namespace Stecon.UI;

/// <summary>Default <see cref="IToastService"/>. Flat list only - capacity/oldest-eviction policy lives in <see cref="SteconToastHost"/>, not here, so it can change without breaking this contract.</summary>
public sealed class ToastService : IToastService, IDisposable
{
    private readonly List<ToastMessage> _toasts = new();
    private readonly Dictionary<string, Timer> _timers = new();
    private readonly Lock _gate = new();
    private bool _disposed;

    public IReadOnlyList<ToastMessage> Toasts
    {
        get
        {
            lock (_gate)
            {
                return _toasts.ToArray();
            }
        }
    }

    public event Action? ToastsChanged;

    public string Show(string message, ToastVariant variant = ToastVariant.Info, ToastOptions? options = null)
    {
        options ??= new ToastOptions();
        var id = Guid.NewGuid().ToString("N");
        var toast = new ToastMessage(id, message, variant, options.Duration, options.Dismissible);

        lock (_gate)
        {
            if (_disposed)
            {
                return id;
            }

            _toasts.Add(toast);

            if (options.Duration is { } duration)
            {
                _timers[id] = new Timer(_ => Dismiss(id), null, duration, Timeout.InfiniteTimeSpan);
            }
        }

        ToastsChanged?.Invoke();
        return id;
    }

    public void Dismiss(string id)
    {
        Timer? timer;
        bool removed;

        lock (_gate)
        {
            removed = _toasts.RemoveAll(t => t.Id == id) > 0;
            _timers.Remove(id, out timer);
        }

        timer?.Dispose();

        if (removed)
        {
            ToastsChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            foreach (var timer in _timers.Values)
            {
                timer.Dispose();
            }

            _timers.Clear();
            _toasts.Clear();
        }
    }
}
